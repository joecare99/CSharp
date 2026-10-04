using System.Text.Json;
using OFBCreator.Projects.Models;

namespace OFBCreator.Projects.Services;

/// <summary>
/// Reads and writes portable project documents and maintains a user-local recent-project catalog.
/// </summary>
public sealed class OFBProjectStore
{
    public const string ProjectExtension = ".ofbproject";
    private const int CatalogSchemaVersion = 1;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private readonly string _catalogDirectory;
    private readonly string _catalogPath;

    public OFBProjectStore()
        : this(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "OFBCreator",
            "Projects"))
    {
    }

    public OFBProjectStore(string catalogDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogDirectory);
        _catalogDirectory = Path.GetFullPath(catalogDirectory);
        _catalogPath = Path.Combine(_catalogDirectory, "recent.json");
    }

    /// <summary>
    /// Opens a portable project path, catalog name, or legacy version-1 project.
    /// Legacy projects are migrated in memory and are not rewritten until Save is called.
    /// </summary>
    public OFBProjectLoadResult Open(string projectPathOrName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPathOrName);
        var resolvedPath = ResolveExistingProjectPath(projectPathOrName);
        var result = LoadProjectFile(resolvedPath);
        AddRecentProject(result.ProjectPath);
        return result;
    }

    /// <summary>
    /// Opens an existing project or creates a new portable project at a path/name.
    /// </summary>
    public OFBProjectLoadResult LoadOrCreate(string projectPathOrName, string? templatePathOrName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPathOrName);
        if (File.Exists(projectPathOrName))
        {
            if (!string.IsNullOrWhiteSpace(templatePathOrName))
                throw new InvalidOperationException(
                    $"Project '{projectPathOrName}' already exists; a project template can only initialize a new project.");
            return Open(projectPathOrName);
        }
        var destinationPath = ResolveProjectPath(projectPathOrName);
        if (File.Exists(destinationPath))
        {
            if (!string.IsNullOrWhiteSpace(templatePathOrName))
                throw new InvalidOperationException(
                    $"Project '{destinationPath}' already exists; a project template can only initialize a new project.");
            return Open(destinationPath);
        }
        if (!LooksLikePath(projectPathOrName))
        {
            var legacyPath = Path.Combine(_catalogDirectory, projectPathOrName + ".json");
            if (File.Exists(legacyPath))
            {
                if (!string.IsNullOrWhiteSpace(templatePathOrName))
                    throw new InvalidOperationException(
                        $"Legacy project '{legacyPath}' already exists; a project template can only initialize a new project.");
                return Open(legacyPath);
            }
        }

        var project = string.IsNullOrWhiteSpace(templatePathOrName)
            ? CreateDefault(Path.GetFileNameWithoutExtension(destinationPath))
            : CloneTemplate(Path.GetFileNameWithoutExtension(destinationPath), templatePathOrName);
        Save(project, destinationPath);
        return new OFBProjectLoadResult(project, destinationPath, false, OFBProject.CurrentSchemaVersion);
    }

    /// <summary>
    /// Saves a project atomically to a portable project file.
    /// </summary>
    public string Save(OFBProject project, string projectPath)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        var fullPath = Path.GetFullPath(projectPath);
        if (!string.Equals(Path.GetExtension(fullPath), ProjectExtension, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"Portable project files must use the '{ProjectExtension}' extension.", nameof(projectPath));

        Validate(project);
        project.SchemaVersion = OFBProject.CurrentSchemaVersion;
        var directory = Path.GetDirectoryName(fullPath)
            ?? throw new InvalidOperationException($"Project path '{fullPath}' has no parent directory.");
        Directory.CreateDirectory(directory);

        var temporaryPath = fullPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                JsonSerializer.Serialize(stream, project, SerializerOptions);
            File.Move(temporaryPath, fullPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }

        AddRecentProject(fullPath);
        return fullPath;
    }

    /// <summary>
    /// Migrates an opened legacy project to a portable project file only when explicitly requested.
    /// </summary>
    public string SaveMigration(OFBProjectLoadResult loadedProject, string? destinationPath = null)
    {
        ArgumentNullException.ThrowIfNull(loadedProject);
        if (!loadedProject.RequiresMigrationSave)
            return loadedProject.ProjectPath;

        var path = destinationPath ?? GetPortablePathForLegacy(loadedProject.ProjectPath);
        return Save(loadedProject.Project, path);
    }

    /// <summary>
    /// Gets a catalog snapshot containing only project paths that still exist.
    /// </summary>
    public IReadOnlyList<string> GetRecentProjects()
    {
        if (!File.Exists(_catalogPath))
            return [];

        var catalog = ReadCatalog();
        return catalog.RecentProjects
            .Select(Path.GetFullPath)
            .Where(File.Exists)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    /// <summary>
    /// Resolves a stored relative path against its owning project file.
    /// </summary>
    public static string? ResolveProjectPath(string projectFile, string? storedPath)
    {
        if (string.IsNullOrWhiteSpace(storedPath))
            return null;
        if (Path.IsPathRooted(storedPath))
            return Path.GetFullPath(storedPath);

        var projectDirectory = Path.GetDirectoryName(Path.GetFullPath(projectFile))
            ?? throw new InvalidOperationException($"Project file '{projectFile}' has no parent directory.");
        return Path.GetFullPath(Path.Combine(projectDirectory, storedPath));
    }

    /// <summary>
    /// Resolves a project-selected external template relative to its project file.
    /// Built-in template names are returned unchanged.
    /// </summary>
    public static string ResolveEntryTemplate(string projectFile, string templateNameOrPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templateNameOrPath);
        if (string.Equals(templateNameOrPath, "gc", StringComparison.OrdinalIgnoreCase)
            || string.Equals(templateNameOrPath, "ak", StringComparison.OrdinalIgnoreCase))
            return templateNameOrPath.ToLowerInvariant();
        if (Path.IsPathRooted(templateNameOrPath))
            return Path.GetFullPath(templateNameOrPath);

        var projectDirectory = Path.GetDirectoryName(Path.GetFullPath(projectFile))
            ?? throw new InvalidOperationException($"Project file '{projectFile}' has no parent directory.");
        return Path.GetFullPath(Path.Combine(projectDirectory, templateNameOrPath));
    }

    private OFBProject CloneTemplate(string name, string templatePathOrName)
    {
        var sourceResult = Open(templatePathOrName);
        var source = sourceResult.Project;
        var entryTemplate = ResolveEntryTemplate(sourceResult.ProjectPath, source.EntryTemplate);
        return new OFBProject
        {
            Name = name,
            Title = name,
            EntryTemplate = entryTemplate,
            DataSource = source.DataSource,
            PlaceId = source.PlaceId,
            IncludeDescendants = source.IncludeDescendants,
            Preface = source.Preface,
            Legend = source.Legend,
            ExportRules = source.ExportRules.Select(rule => new OFBExportRule
            {
                Order = rule.Order,
                Enabled = rule.Enabled,
                TargetKind = rule.TargetKind,
                TargetId = rule.TargetId,
                Action = rule.Action,
                Field = rule.Field,
                Value = rule.Value,
                Occurrence = rule.Occurrence
            }).ToList(),
            GroupingPolicy = new OFBGroupingPolicy
            {
                AutoAcceptThreshold = source.GroupingPolicy.AutoAcceptThreshold
            },
            GroupingDecisions = source.GroupingDecisions.Select(decision => new OFBGroupingDecision
            {
                Order = decision.Order,
                LeftFamilyTargetId = decision.LeftFamilyTargetId,
                RightFamilyTargetId = decision.RightFamilyTargetId,
                Action = decision.Action,
                GroupName = decision.GroupName
            }).ToList()
        };
    }

    private OFBProjectLoadResult LoadProjectFile(string path)
    {
        using var stream = File.OpenRead(path);
        using var json = JsonDocument.Parse(stream);
        if (json.RootElement.ValueKind != JsonValueKind.Object)
            throw new InvalidDataException($"Project file '{path}' must contain a JSON object.");

        var schemaVersion = ReadSchemaVersion(json.RootElement, path);
        OFBProject project;
        var requiresMigration = schemaVersion < OFBProject.CurrentSchemaVersion;
        if (schemaVersion == 1)
        {
            project = JsonSerializer.Deserialize<OFBProject>(json.RootElement.GetRawText(), SerializerOptions)
                ?? throw new InvalidDataException($"Project file '{path}' does not contain a project.");
            project.Id = Guid.NewGuid().ToString("D");
            project.SchemaVersion = OFBProject.CurrentSchemaVersion;
        }
        else if (schemaVersion == 2)
        {
            project = JsonSerializer.Deserialize<OFBProject>(json.RootElement.GetRawText(), SerializerOptions)
                ?? throw new InvalidDataException($"Project file '{path}' does not contain a project.");
            project.SchemaVersion = OFBProject.CurrentSchemaVersion;
            project.ExportRules ??= [];
        }
        else if (schemaVersion == 3)
        {
            project = JsonSerializer.Deserialize<OFBProject>(json.RootElement.GetRawText(), SerializerOptions)
                ?? throw new InvalidDataException($"Project file '{path}' does not contain a project.");
            project.SchemaVersion = OFBProject.CurrentSchemaVersion;
            project.GroupingPolicy ??= new OFBGroupingPolicy();
            project.GroupingDecisions ??= [];
        }
        else if (schemaVersion == OFBProject.CurrentSchemaVersion)
        {
            project = JsonSerializer.Deserialize<OFBProject>(json.RootElement.GetRawText(), SerializerOptions)
                ?? throw new InvalidDataException($"Project file '{path}' does not contain a project.");
        }
        else
        {
            throw new InvalidDataException(
                $"Project '{path}' uses unsupported schema version {schemaVersion}; this application supports up to {OFBProject.CurrentSchemaVersion}.");
        }

        Validate(project);
        return new OFBProjectLoadResult(project, path, requiresMigration, schemaVersion);
    }

    private string ResolveExistingProjectPath(string projectPathOrName)
    {
        if (File.Exists(projectPathOrName))
            return Path.GetFullPath(projectPathOrName);

        if (LooksLikePath(projectPathOrName))
        {
            var fullPath = Path.GetFullPath(projectPathOrName);
            if (File.Exists(fullPath))
                return fullPath;
            throw new FileNotFoundException($"Project file '{projectPathOrName}' was not found.", fullPath);
        }

        ValidateProjectName(projectPathOrName);
        var portablePath = Path.Combine(_catalogDirectory, projectPathOrName + ProjectExtension);
        if (File.Exists(portablePath))
            return portablePath;

        var legacyPath = Path.Combine(_catalogDirectory, projectPathOrName + ".json");
        if (File.Exists(legacyPath))
            return legacyPath;

        throw new FileNotFoundException($"Project '{projectPathOrName}' was not found.");
    }

    private string ResolveProjectPath(string projectPathOrName)
    {
        if (LooksLikePath(projectPathOrName))
        {
            var fullPath = Path.GetFullPath(projectPathOrName);
            if (!string.Equals(Path.GetExtension(fullPath), ProjectExtension, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Project path must use the '{ProjectExtension}' extension.", nameof(projectPathOrName));
            return fullPath;
        }

        ValidateProjectName(projectPathOrName);
        return Path.Combine(_catalogDirectory, projectPathOrName + ProjectExtension);
    }

    private string GetPortablePathForLegacy(string legacyPath) =>
        Path.Combine(_catalogDirectory, Path.GetFileNameWithoutExtension(legacyPath) + ProjectExtension);

    private void AddRecentProject(string projectPath)
    {
        Directory.CreateDirectory(_catalogDirectory);
        var catalog = File.Exists(_catalogPath) ? ReadCatalog() : new OFBProjectCatalog();
        var fullPath = Path.GetFullPath(projectPath);
        catalog.RecentProjects.RemoveAll(path => string.Equals(
            Path.GetFullPath(path), fullPath, StringComparison.OrdinalIgnoreCase));
        catalog.RecentProjects.Insert(0, fullPath);
        if (catalog.RecentProjects.Count > 20)
            catalog.RecentProjects.RemoveRange(20, catalog.RecentProjects.Count - 20);
        WriteAtomic(_catalogPath, catalog);
    }

    private OFBProjectCatalog ReadCatalog()
    {
        using var stream = File.OpenRead(_catalogPath);
        var catalog = JsonSerializer.Deserialize<OFBProjectCatalog>(stream, SerializerOptions)
            ?? throw new InvalidDataException($"Project catalog '{_catalogPath}' is empty.");
        if (catalog.SchemaVersion != CatalogSchemaVersion)
            throw new InvalidDataException(
                $"Project catalog '{_catalogPath}' uses unsupported schema version {catalog.SchemaVersion}.");
        return catalog;
    }

    private static void WriteAtomic<T>(string path, T value)
    {
        var temporaryPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                JsonSerializer.Serialize(stream, value, SerializerOptions);
            File.Move(temporaryPath, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }
    }

    private static int ReadSchemaVersion(JsonElement root, string path)
    {
        if (!root.TryGetProperty("SchemaVersion", out var version)
            && !root.TryGetProperty("schemaVersion", out version))
            throw new InvalidDataException($"Project file '{path}' has no schemaVersion.");
        if (version.ValueKind != JsonValueKind.Number || !version.TryGetInt32(out var schemaVersion))
            throw new InvalidDataException($"Project file '{path}' has an invalid schemaVersion.");
        return schemaVersion;
    }

    private static OFBProject CreateDefault(string name) => new()
    {
        Name = name,
        Title = name,
        EntryTemplate = "gc"
    };

    private static void Validate(OFBProject project)
    {
        if (project.SchemaVersion is not (1 or OFBProject.CurrentSchemaVersion))
            throw new InvalidDataException($"Unsupported project schema version {project.SchemaVersion}.");
        if (!Guid.TryParse(project.Id, out _))
            throw new InvalidDataException("A project requires a valid stable identifier.");
        if (string.IsNullOrWhiteSpace(project.Name))
            throw new InvalidDataException("A project requires a name.");
        if (string.IsNullOrWhiteSpace(project.Title))
            throw new InvalidDataException($"Project '{project.Name}' has no document title.");
        if (string.IsNullOrWhiteSpace(project.EntryTemplate))
            throw new InvalidDataException($"Project '{project.Name}' has no selected entry template.");
        OFBExportRuleValidator.Validate(project.ExportRules);
        OFBGroupingPolicyValidator.Validate(project.GroupingPolicy, project.GroupingDecisions);
        if (string.Equals(project.EntryTemplate, "gc", StringComparison.OrdinalIgnoreCase)
            || string.Equals(project.EntryTemplate, "ak", StringComparison.OrdinalIgnoreCase))
            project.EntryTemplate = project.EntryTemplate.ToLowerInvariant();
    }

    private static bool LooksLikePath(string value) =>
        Path.IsPathRooted(value)
        || value.Contains(Path.DirectorySeparatorChar)
        || value.Contains(Path.AltDirectorySeparatorChar)
        || value.EndsWith(ProjectExtension, StringComparison.OrdinalIgnoreCase)
        || string.Equals(Path.GetExtension(value), ".json", StringComparison.OrdinalIgnoreCase);

    private static void ValidateProjectName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name is "." or ".."
            || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
            || name.IndexOf(Path.DirectorySeparatorChar) >= 0
            || name.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
            throw new ArgumentException("Project names must be a single valid file-name segment.", nameof(name));
    }
}
