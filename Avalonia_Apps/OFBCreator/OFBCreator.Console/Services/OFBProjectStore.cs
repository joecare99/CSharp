using System;
using System.IO;
using System.Text.Json;
using OFBCreator.Console.Models;

namespace OFBCreator.Console.Services;

/// <summary>
/// Loads and creates named family-book projects in the current user's application data directory.
/// </summary>
public sealed class OFBProjectStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _projectsDirectory;

    public OFBProjectStore()
        : this(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "OFBCreator",
            "Projects"))
    {
    }

    public OFBProjectStore(string projectsDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectsDirectory);
        _projectsDirectory = Path.GetFullPath(projectsDirectory);
    }

    public OFBProject LoadOrCreate(string name, string? templateName = null)
    {
        ValidateName(name, nameof(name));
        var projectPath = GetProjectPath(name);
        if (File.Exists(projectPath))
        {
            if (!string.IsNullOrWhiteSpace(templateName))
                throw new InvalidOperationException(
                    $"Project '{name}' already exists; --project-template can only initialize a new project.");

            return Load(projectPath, name);
        }

        var project = string.IsNullOrWhiteSpace(templateName)
            ? CreateDefault(name)
            : CloneTemplate(name, templateName);
        Save(projectPath, project);
        return project;
    }

    private OFBProject CloneTemplate(string newName, string templateName)
    {
        ValidateName(templateName, nameof(templateName));
        var templatePath = GetProjectPath(templateName);
        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Project template '{templateName}' was not found.", templatePath);

        var template = Load(templatePath, templateName);
        return new OFBProject
        {
            SchemaVersion = template.SchemaVersion,
            Name = newName,
            Title = newName,
            EntryTemplate = template.EntryTemplate
        };
    }

    private static OFBProject Load(string path, string expectedName)
    {
        using var stream = File.OpenRead(path);
        var project = JsonSerializer.Deserialize<OFBProject>(stream)
            ?? throw new InvalidDataException($"Project file '{path}' does not contain a project.");
        if (project.SchemaVersion != 1)
            throw new InvalidDataException(
                $"Project '{expectedName}' uses unsupported schema version {project.SchemaVersion}.");
        if (!string.Equals(project.Name, expectedName, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException(
                $"Project file '{path}' declares name '{project.Name}', but was loaded as '{expectedName}'.");
        if (string.IsNullOrWhiteSpace(project.Title))
            throw new InvalidDataException($"Project '{expectedName}' has no document title.");
        if (string.IsNullOrWhiteSpace(project.EntryTemplate))
            throw new InvalidDataException($"Project '{expectedName}' has no selected entry template.");
        if (string.Equals(project.EntryTemplate, "gc", StringComparison.OrdinalIgnoreCase)
            || string.Equals(project.EntryTemplate, "ak", StringComparison.OrdinalIgnoreCase))
            project.EntryTemplate = project.EntryTemplate.ToLowerInvariant();
        return project;
    }

    private void Save(string path, OFBProject project)
    {
        Directory.CreateDirectory(_projectsDirectory);
        var temporaryPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                JsonSerializer.Serialize(stream, project, SerializerOptions);
            File.Move(temporaryPath, path);
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }
    }

    private string GetProjectPath(string name) => Path.Combine(_projectsDirectory, name + ".json");

    private static OFBProject CreateDefault(string name) => new()
    {
        Name = name,
        Title = name,
        EntryTemplate = "gc"
    };

    private static void ValidateName(string name, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, parameterName);
        if (name is "." or ".."
            || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
            || name.IndexOf(Path.DirectorySeparatorChar) >= 0
            || name.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
            throw new ArgumentException("Project names must be a single valid file-name segment.", parameterName);
    }
}
