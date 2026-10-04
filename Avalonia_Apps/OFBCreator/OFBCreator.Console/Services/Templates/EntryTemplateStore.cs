using System;
using System.IO;
using System.Reflection;

namespace OFBCreator.Console.Services.Templates;

/// <summary>
/// Resolves built-in template names and loads external JSON templates.
/// </summary>
public sealed class EntryTemplateStore
{
    private const string ResourcePrefix = "OFBCreator.Console.Templates.";

    public EntryTemplateDefinition Load(string nameOrPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameOrPath);
        var resourceName = nameOrPath.Trim().ToLowerInvariant() switch
        {
            "gc" => ResourcePrefix + "gc.json",
            "ak" => ResourcePrefix + "ak.json",
            _ => null
        };

        if (resourceName is not null)
        {
            using var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Built-in entry template '{nameOrPath}' is unavailable.");
            using var reader = new StreamReader(resource);
            return EntryTemplateValidator.ParseAndValidate(reader.ReadToEnd());
        }

        var path = Path.GetFullPath(nameOrPath);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Entry template '{nameOrPath}' was not found.", path);
        if (!string.Equals(Path.GetExtension(path), ".json", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("External entry templates must be JSON files.");

        return EntryTemplateValidator.ParseAndValidate(File.ReadAllText(path));
    }
}
