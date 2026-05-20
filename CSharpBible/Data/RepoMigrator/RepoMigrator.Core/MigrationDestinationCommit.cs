namespace RepoMigrator.Core;

/// <summary>
/// Represents normalized snapshot-write metadata that destination providers can consume.
/// </summary>
public sealed class MigrationDestinationCommit
{
    /// <summary>
    /// Gets the logical snapshot identifier associated with the destination write.
    /// </summary>
    public string SnapshotId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the commit or write message associated with the snapshot.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets the author display name associated with the snapshot write.
    /// </summary>
    public string AuthorName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional author e-mail address associated with the snapshot write.
    /// </summary>
    public string? AuthorEmail { get; init; }

    /// <summary>
    /// Gets the timestamp associated with the snapshot write.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; }

    /// <summary>
    /// Gets the optional logical destination reference such as a branch, trunk, or target stream name.
    /// </summary>
    public string? DestinationReference { get; init; }
}
