namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Represents one archive entry discovered during archive inspection.
/// </summary>
public sealed class ArchiveEntryMetadata
{
    /// <summary>
    /// Gets the full entry path inside the archive.
    /// </summary>
    public string EntryPath { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the entry represents a directory.
    /// </summary>
    public bool IsDirectory { get; init; }

    /// <summary>
    /// Gets the uncompressed entry size when available.
    /// </summary>
    public long? Size { get; init; }

    /// <summary>
    /// Gets the entry timestamp when available.
    /// </summary>
    public DateTimeOffset? Timestamp { get; init; }
}
