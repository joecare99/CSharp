namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Represents the final deterministic ordering outcome for one archive snapshot.
/// </summary>
public sealed class ArchiveOrderingDecision
{
    /// <summary>
    /// Gets the stable snapshot identifier.
    /// </summary>
    public string SnapshotId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the final zero-based order index.
    /// </summary>
    public int FinalOrderIndex { get; init; }

    /// <summary>
    /// Gets the evidence recorded for the ordering decision.
    /// </summary>
    public ArchiveOrderingEvidence Evidence { get; init; } = new();
}
