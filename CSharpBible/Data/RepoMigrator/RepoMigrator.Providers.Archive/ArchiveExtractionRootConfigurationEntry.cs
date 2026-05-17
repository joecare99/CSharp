namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Represents one configured extraction-root path for a concrete archive item.
/// </summary>
public sealed class ArchiveExtractionRootConfigurationEntry
{
    /// <summary>
    /// Gets the stable archive item identifier, typically the relative archive path inside the source directory.
    /// </summary>
    public string ArchiveItemId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the relative extraction-root path inside the archive.
    /// An empty value means the actual archive root should be used.
    /// </summary>
    public string RootPath { get; init; } = string.Empty;
}
