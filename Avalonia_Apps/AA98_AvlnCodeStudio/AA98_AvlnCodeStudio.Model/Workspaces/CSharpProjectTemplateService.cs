using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Loads project templates from JSON and materializes them without overwriting existing files.
/// </summary>
public sealed class CSharpProjectTemplateService
{
#if NET10_0
    private const string DefaultTargetFramework = "net10.0";
#elif NET9_0
    private const string DefaultTargetFramework = "net9.0";
#else
    private const string DefaultTargetFramework = "net8.0";
#endif

    private const string TemplateResourcePrefix = "AA98_AvlnCodeStudio.Model.ProjectTemplates.";
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };
    private static readonly Regex s_targetFrameworkPattern = new(
        "^net\\d+\\.\\d+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex s_templateIdPattern = new(
        "^[a-zA-Z0-9][a-zA-Z0-9._-]*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly string[] _templateDirectories;
    private IReadOnlyList<CSharpProjectTemplateDefinition> _templates;

    /// <summary>
    /// Loads built-in templates and templates from the configured JSON directories.
    /// </summary>
    /// <param name="templateDirectories">
    /// Optional template directories. When omitted, application and per-user directories are used.
    /// </param>
    public CSharpProjectTemplateService(IEnumerable<string>? templateDirectories = null)
    {
        _templates = LoadBuiltInTemplates();
        _templateDirectories = (templateDirectories ?? GetDefaultTemplateDirectories())
            .Where(static path => !string.IsNullOrWhiteSpace(path))
            .Select(Path.GetFullPath)
            .Distinct(GetPathComparer())
            .ToArray();
    }

    /// <summary>
    /// Gets the currently available templates.
    /// </summary>
    public IReadOnlyList<CSharpProjectTemplateDefinition> Templates => _templates;

    /// <summary>
    /// Gets the directories searched for external JSON templates.
    /// </summary>
    public IReadOnlyList<string> TemplateDirectories => _templateDirectories;

    /// <summary>
    /// Reloads external JSON definitions, replacing built-ins when an external template has the same ID.
    /// The current catalog remains unchanged if a JSON file is invalid.
    /// </summary>
    public IReadOnlyList<CSharpProjectTemplateDefinition> ReloadTemplates()
    {
        Dictionary<string, CSharpProjectTemplateDefinition> templates = LoadBuiltInTemplates()
            .ToDictionary(static template => template.Id, StringComparer.OrdinalIgnoreCase);

        foreach (string directory in _templateDirectories)
        {
            if (!Directory.Exists(directory))
            {
                continue;
            }

            foreach (string filePath in Directory.EnumerateFiles(directory, "*.json", SearchOption.TopDirectoryOnly)
                         .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase))
            {
                CSharpProjectTemplateDefinition definition = ReadTemplate(filePath);
                templates[definition.Id] = definition;
            }
        }

        _templates = templates.Values
            .OrderBy(static template => template.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
        return _templates;
    }

    /// <summary>
    /// Saves a template definition as JSON. Existing template files at the explicit destination are replaced.
    /// </summary>
    public void SaveTemplate(CSharpProjectTemplateDefinition definition, string jsonFilePath)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentException.ThrowIfNullOrWhiteSpace(jsonFilePath);
        ValidateTemplate(definition);

        string normalizedPath = Path.GetFullPath(jsonFilePath);
        if (!string.Equals(Path.GetExtension(normalizedPath), ".json", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("A template definition path ending in .json is required.", nameof(jsonFilePath));
        }

        string directory = Path.GetDirectoryName(normalizedPath)
            ?? throw new ArgumentException("The template file must have a parent directory.", nameof(jsonFilePath));
        if (!Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException($"The template directory '{directory}' does not exist.");
        }

        string temporaryPath = Path.Combine(directory, $".{Guid.NewGuid():N}.tmp");
        try
        {
            File.WriteAllText(
                temporaryPath,
                JsonSerializer.Serialize(definition, s_jsonOptions),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            File.Move(temporaryPath, normalizedPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    /// <summary>
    /// Creates a project using the template currently loaded for <paramref name="templateId"/>.
    /// </summary>
    public string CreateProject(string templateId, string projectFilePath)
    {
        return CreateProject(templateId, projectFilePath, DefaultTargetFramework);
    }

    /// <summary>
    /// Creates a project using a loaded template and the requested target framework.
    /// </summary>
    public string CreateProject(string templateId, string projectFilePath, string targetFramework)
    {
        IReadOnlyList<GeneratedProjectFile> files = GetProjectFiles(
            templateId,
            projectFilePath,
            targetFramework);
        string normalizedProjectPath = files[0].Path;
        List<string> createdPaths = [];
        try
        {
            foreach (GeneratedProjectFile file in files)
            {
                string? outputDirectory = Path.GetDirectoryName(file.Path);
                if (outputDirectory is not null)
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                WriteNewFile(file.Path, file.Content, createdPaths);
            }

            return normalizedProjectPath;
        }
        catch
        {
            foreach (string createdPath in createdPaths)
            {
                File.Delete(createdPath);
            }

            throw;
        }
    }

    /// <summary>
    /// Prepares new-project file contents for review without writing them.
    /// </summary>
    public IReadOnlyList<WorkspaceFileChangeRequest> PrepareProjectChangeRequests(
        string templateId,
        string projectFilePath)
    {
        return PrepareProjectChangeRequests(templateId, projectFilePath, DefaultTargetFramework);
    }

    /// <summary>
    /// Prepares new-project file contents for review without writing them.
    /// </summary>
    public IReadOnlyList<WorkspaceFileChangeRequest> PrepareProjectChangeRequests(
        string templateId,
        string projectFilePath,
        string targetFramework)
    {
        IReadOnlyList<GeneratedProjectFile> files = GetProjectFiles(
            templateId,
            projectFilePath,
            targetFramework);
        string projectDirectory = Path.GetDirectoryName(files[0].Path)!;
        return files.Select(file => WorkspaceFileChangeRequest.CreateNew(
                Path.GetRelativePath(projectDirectory, file.Path),
                file.Content))
            .ToArray();
    }

    public string CreateConsoleProject(string projectFilePath)
    {
        return CreateProject("console", projectFilePath);
    }

    public string CreateConsoleProject(string projectFilePath, string targetFramework)
    {
        return CreateProject("console", projectFilePath, targetFramework);
    }

    public string CreateClassLibraryProject(string projectFilePath)
    {
        return CreateProject("class-library", projectFilePath);
    }

    public string CreateClassLibraryProject(string projectFilePath, string targetFramework)
    {
        return CreateProject("class-library", projectFilePath, targetFramework);
    }

    public string CreateTestProject(string projectFilePath)
    {
        return CreateProject("mstest", projectFilePath);
    }

    public string CreateTestProject(string projectFilePath, string targetFramework)
    {
        return CreateProject("mstest", projectFilePath, targetFramework);
    }

    private static IReadOnlyList<CSharpProjectTemplateDefinition> LoadBuiltInTemplates()
    {
        Assembly assembly = typeof(CSharpProjectTemplateService).Assembly;
        return assembly.GetManifestResourceNames()
            .Where(static name => name.StartsWith(TemplateResourcePrefix, StringComparison.Ordinal)
                && name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            .OrderBy(static name => name, StringComparer.Ordinal)
            .Select(name =>
            {
                using Stream stream = assembly.GetManifestResourceStream(name)
                    ?? throw new InvalidDataException($"Embedded template resource '{name}' could not be opened.");
                CSharpProjectTemplateDefinition definition = JsonSerializer.Deserialize<CSharpProjectTemplateDefinition>(
                    stream,
                    s_jsonOptions)
                    ?? throw new InvalidDataException($"Embedded template resource '{name}' is empty.");
                ValidateTemplate(definition);
                return definition;
            })
            .ToArray();
    }

    private static CSharpProjectTemplateDefinition ReadTemplate(string filePath)
    {
        try
        {
            CSharpProjectTemplateDefinition definition = JsonSerializer.Deserialize<CSharpProjectTemplateDefinition>(
                File.ReadAllText(filePath),
                s_jsonOptions)
                ?? throw new InvalidDataException("The JSON document does not contain a template definition.");
            ValidateTemplate(definition);
            return definition;
        }
        catch (Exception exception) when (exception is JsonException or InvalidDataException or ArgumentException)
        {
            throw new InvalidDataException($"Project template JSON '{filePath}' is invalid: {exception.Message}", exception);
        }
    }

    private IReadOnlyList<GeneratedProjectFile> GetProjectFiles(
        string templateId,
        string projectFilePath,
        string targetFramework)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templateId);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetFramework);
        if (!s_targetFrameworkPattern.IsMatch(targetFramework))
        {
            throw new ArgumentException("The target framework must use the netX.Y format.", nameof(targetFramework));
        }

        CSharpProjectTemplateDefinition template = _templates.FirstOrDefault(candidate =>
            string.Equals(candidate.Id, templateId, StringComparison.OrdinalIgnoreCase))
            ?? throw new KeyNotFoundException($"Project template '{templateId}' is not loaded.");

        string normalizedProjectPath = Path.GetFullPath(projectFilePath);
        if (!string.Equals(Path.GetExtension(normalizedProjectPath), ".csproj", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("A C# project file path ending in .csproj is required.", nameof(projectFilePath));
        }

        string projectDirectory = Path.GetDirectoryName(normalizedProjectPath)
            ?? throw new ArgumentException("The project file must have a parent directory.", nameof(projectFilePath));
        if (!Directory.Exists(projectDirectory))
        {
            throw new DirectoryNotFoundException($"The project directory '{projectDirectory}' does not exist.");
        }

        Dictionary<string, string> replacements = new(StringComparer.Ordinal)
        {
            ["{{ProjectName}}"] = Path.GetFileNameWithoutExtension(normalizedProjectPath),
            ["{{ProjectNameIdentifier}}"] = ToCSharpIdentifier(Path.GetFileNameWithoutExtension(normalizedProjectPath)),
            ["{{TargetFramework}}"] = targetFramework,
        };
        GeneratedProjectFile[] files =
        [
            new(
                normalizedProjectPath,
                ApplyReplacements(template.ProjectFileContent, replacements)),
            .. template.Files.Select(file => new GeneratedProjectFile(
                ResolveOutputPath(projectDirectory, ApplyReplacements(file.Path, replacements)),
                ApplyReplacements(file.Content, replacements))),
        ];
        if (files.Select(static file => file.Path).Distinct(GetPathComparer()).Count() != files.Length)
        {
            throw new InvalidDataException($"Template '{template.Id}' generates the same file more than once.");
        }

        if (files.Any(static file => File.Exists(file.Path)))
        {
            throw new IOException("The project file or a generated file already exists; no files were overwritten.");
        }

        return files;
    }

    private static void ValidateTemplate(CSharpProjectTemplateDefinition template)
    {
        if (!s_templateIdPattern.IsMatch(template.Id))
        {
            throw new InvalidDataException("A template ID containing only letters, digits, '.', '_' and '-' is required.");
        }

        if (string.IsNullOrWhiteSpace(template.Name))
        {
            throw new InvalidDataException($"Template '{template.Id}' must have a name.");
        }

        if (string.IsNullOrWhiteSpace(template.DefaultProjectFileName)
            || !string.Equals(
                Path.GetExtension(template.DefaultProjectFileName.Replace("{{ProjectName}}", "Project", StringComparison.Ordinal)),
                ".csproj",
                StringComparison.OrdinalIgnoreCase)
            || Path.GetFileName(template.DefaultProjectFileName) != template.DefaultProjectFileName)
        {
            throw new InvalidDataException($"Template '{template.Id}' must define a default .csproj file name.");
        }

        if (string.IsNullOrWhiteSpace(template.ProjectFileContent))
        {
            throw new InvalidDataException($"Template '{template.Id}' must define project file content.");
        }

        if (template.Files is null || template.Files.Any(static file =>
                file is null || string.IsNullOrWhiteSpace(file.Path) || file.Content is null))
        {
            throw new InvalidDataException($"Template '{template.Id}' contains an invalid generated file.");
        }

        HashSet<string> paths = new(GetPathComparer());
        foreach (CSharpProjectTemplateFile file in template.Files)
        {
            string normalizedPath = NormalizeRelativePath(file.Path);
            if (!paths.Add(normalizedPath))
            {
                throw new InvalidDataException($"Template '{template.Id}' defines '{file.Path}' more than once.");
            }
        }
    }

    private static string ResolveOutputPath(string projectDirectory, string relativePath)
    {
        string normalizedRelativePath = NormalizeRelativePath(relativePath);
        string fullPath = Path.GetFullPath(Path.Combine(projectDirectory, normalizedRelativePath));
        string directoryPrefix = Path.GetFullPath(projectDirectory)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        if (!fullPath.StartsWith(directoryPrefix, GetPathComparison()))
        {
            throw new InvalidDataException($"Generated file path '{relativePath}' escapes the project directory.");
        }

        return fullPath;
    }

    private static string NormalizeRelativePath(string path)
    {
        if (Path.IsPathRooted(path) || path.Contains(':'))
        {
            throw new InvalidDataException($"Generated file path '{path}' must be relative.");
        }

        string normalizedPath = path
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
        if (normalizedPath.Split(Path.DirectorySeparatorChar).Any(static segment => segment is "" or "." or ".."))
        {
            throw new InvalidDataException($"Generated file path '{path}' is not a safe relative path.");
        }

        return normalizedPath;
    }

    private static string ToCSharpIdentifier(string value)
    {
        StringBuilder identifier = new(value.Length + 1);
        foreach (char character in value)
        {
            identifier.Append(char.IsLetterOrDigit(character) || character == '_' ? character : '_');
        }

        if (identifier.Length == 0 || !char.IsLetter(identifier[0]) && identifier[0] != '_')
        {
            identifier.Insert(0, '_');
        }

        return identifier.ToString();
    }

    private static string ApplyReplacements(string value, IReadOnlyDictionary<string, string> replacements)
    {
        foreach ((string token, string replacement) in replacements)
        {
            value = value.Replace(token, replacement, StringComparison.Ordinal);
        }

        return value;
    }

    private static void WriteNewFile(string path, string content, ICollection<string> createdPaths)
    {
        using FileStream stream = new(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        createdPaths.Add(path);
        using StreamWriter writer = new(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        writer.Write(content);
    }

    private static IEnumerable<string> GetDefaultTemplateDirectories()
    {
        yield return Path.Combine(AppContext.BaseDirectory, "ProjectTemplates");
        string userTemplateDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AA98-CodeStudio",
            "ProjectTemplates");
        yield return userTemplateDirectory;
    }

    private static StringComparer GetPathComparer()
    {
        return OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
    }

    private static StringComparison GetPathComparison()
    {
        return OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
    }

    private sealed record GeneratedProjectFile(string Path, string Content);
}
