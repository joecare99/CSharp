namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Represents the current resumable checkpoint of an archive import plan execution.
/// </summary>
public sealed class ArchiveImportCheckpoint
{
    /// <summary>
    /// Gets the checkpoint phase that has been recorded.
    /// </summary>
    public ArchiveImportCheckpointPhase Phase { get; init; } = ArchiveImportCheckpointPhase.None;

    /// <summary>
    /// Gets the logical snapshot identifier associated with the checkpoint when a snapshot-specific step was recorded.
    /// </summary>
    public string SnapshotId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional final order index associated with the checkpoint.
    /// </summary>
    public int? ItemOrderIndex { get; init; }

    /// <summary>
    /// Gets the optional human-readable checkpoint summary.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Gets the timestamp when the checkpoint was recorded.
    /// </summary>
    public DateTimeOffset RecordedAtUtc { get; init; }
}
