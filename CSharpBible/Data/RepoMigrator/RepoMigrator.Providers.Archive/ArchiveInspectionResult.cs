namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Represents the metadata collected when inspecting one archive file.
/// </summary>
public sealed class ArchiveInspectionResult
{
    /// <summary>
    /// Gets the inspected archive path.
    /// </summary>
    public string ArchiveFilePath { get; init; } = string.Empty;

    /// <summary>
    /// Gets the driver identifier that produced the inspection result.
    /// </summary>
    public string DriverId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the newest entry timestamp found in the archive when available.
    /// </summary>
    public DateTimeOffset? NewestEntryTimestamp { get; init; }

    /// <summary>
    /// Gets the inspected archive entries.
    /// </summary>
    public IReadOnlyList<ArchiveEntryMetadata> Entries { get; init; } = Array.Empty<ArchiveEntryMetadata>();
}
