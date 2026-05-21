using System.Text.Json;

namespace RepoMigrator.Providers.Archive.Services;

/// <summary>
/// Loads optional archive extraction-root overrides from the archive source directory.
/// </summary>
public sealed class ArchiveExtractionRootConfigurationStore
{
    private const string FileName = "RepoMigrator.archive-roots.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Loads the extraction-root configuration that applies to the supplied archive source definition.
    /// </summary>
    /// <param name="source">The archive source definition.</param>
    /// <returns>The loaded configuration, or an empty configuration when no readable local configuration exists.</returns>
    public ArchiveExtractionRootConfiguration Load(ArchiveMigrationSourceDefinition source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (source.LocationKind != ArchiveSourceLocationKind.LocalDirectory)
            return new ArchiveExtractionRootConfiguration();

        var directoryPath = ResolveDirectoryPath(source.Location);
        var configFilePath = Path.Combine(directoryPath, FileName);
        if (!File.Exists(configFilePath))
            return new ArchiveExtractionRootConfiguration();

        using var stream = File.OpenRead(configFilePath);
        var configuration = JsonSerializer.Deserialize<ArchiveExtractionRootConfiguration>(stream, SerializerOptions);
        return configuration ?? new ArchiveExtractionRootConfiguration();
    }

    /// <summary>
    /// Gets the file path used for local extraction-root configuration.
    /// </summary>
    /// <param name="source">The archive source definition.</param>
    /// <returns>The configuration file path when the source is local; otherwise <see langword="null"/>.</returns>
    public string? GetConfigurationPath(ArchiveMigrationSourceDefinition source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (source.LocationKind != ArchiveSourceLocationKind.LocalDirectory)
            return null;

        return Path.Combine(ResolveDirectoryPath(source.Location), FileName);
    }

    private static string ResolveDirectoryPath(string location)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(location);
        return Path.GetFullPath(location);
    }
}
