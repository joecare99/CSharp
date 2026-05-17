using RepoMigrator.Core;

namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Represents one ordered snapshot item in a persisted archive import plan.
/// </summary>
public sealed class ArchiveImportPlanItem
{
    /// <summary>
    /// Gets the stable logical snapshot identifier.
    /// </summary>
    public string SnapshotId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the final deterministic processing order of the snapshot item.
    /// </summary>
    public int FinalOrderIndex { get; init; }

    /// <summary>
    /// Gets the normalized source item that produced this archive import plan item.
    /// </summary>
    public MigrationSourcePlanItem SourceItem { get; init; } = new();

    /// <summary>
    /// Gets the final release tag name to create for the snapshot.
    /// </summary>
    public string FinalTagName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional release branch name to create for the snapshot.
    /// </summary>
    public string? FinalBranchName { get; init; }

    /// <summary>
    /// Gets a value indicating whether a release branch should be created for the snapshot.
    /// </summary>
    public bool CreateBranch { get; init; }

    /// <summary>
    /// Gets provider-specific extension data associated with the plan item.
    /// </summary>
    public IReadOnlyDictionary<string, string> ExtensionData { get; init; } = new Dictionary<string, string>();
}
