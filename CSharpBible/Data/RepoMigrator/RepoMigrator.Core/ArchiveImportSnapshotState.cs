namespace RepoMigrator.Core;

/// <summary>
/// Represents the execution state of one snapshot item in an archive import run.
/// </summary>
public sealed class ArchiveImportSnapshotState
{
    /// <summary>
    /// Gets the stable logical snapshot identifier.
    /// </summary>
    public string SnapshotId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the last completed checkpoint phase for the snapshot.
    /// </summary>
    public ArchiveImportCheckpointPhase Phase { get; init; } = ArchiveImportCheckpointPhase.None;

    /// <summary>
    /// Gets a value indicating whether source acquisition completed for the snapshot.
    /// </summary>
    public bool AcquisitionCompleted { get; init; }

    /// <summary>
    /// Gets a value indicating whether extraction completed for the snapshot.
    /// </summary>
    public bool ExtractionCompleted { get; init; }

    /// <summary>
    /// Gets a value indicating whether destination commit or write creation completed for the snapshot.
    /// </summary>
    public bool CommitCompleted { get; init; }

    /// <summary>
    /// Gets the optional destination-side write identifier such as a commit id.
    /// </summary>
    public string? TargetWriteId { get; init; }

    /// <summary>
    /// Gets a value indicating whether the release tag creation step completed for the snapshot.
    /// </summary>
    public bool TagCreated { get; init; }

    /// <summary>
    /// Gets a value indicating whether the release branch creation step completed for the snapshot.
    /// </summary>
    public bool BranchCreated { get; init; }

    /// <summary>
    /// Gets the optional failure summary associated with the most recent processing attempt.
    /// </summary>
    public string? FailureSummary { get; init; }

    /// <summary>
    /// Gets the UTC timestamp of the most recent processing attempt.
    /// </summary>
    public DateTimeOffset? LastAttemptUtc { get; init; }

    /// <summary>
    /// Gets provider-specific extension data associated with the snapshot run state.
    /// </summary>
    public IReadOnlyDictionary<string, string> ExtensionData { get; init; } = new Dictionary<string, string>();
}
