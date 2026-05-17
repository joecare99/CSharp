namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Represents the persisted manual fallback configuration for archive extraction root paths.
/// </summary>
public sealed class ArchiveExtractionRootConfiguration
{
    /// <summary>
    /// Gets the schema version of the extraction-root configuration document.
    /// </summary>
    public int SchemaVersion { get; init; } = 1;

    /// <summary>
    /// Gets the configured per-archive extraction-root entries.
    /// </summary>
    public IReadOnlyList<ArchiveExtractionRootConfigurationEntry> Entries { get; init; } = Array.Empty<ArchiveExtractionRootConfigurationEntry>();
}
