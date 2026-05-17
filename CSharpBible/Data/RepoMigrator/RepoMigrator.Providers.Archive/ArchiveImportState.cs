namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Represents the persisted execution state of an archive import run.
/// </summary>
public sealed class ArchiveImportState
{
    /// <summary>
    /// Gets the persisted schema version of the archive import state document.
    /// </summary>
    public int SchemaVersion { get; init; } = ArchiveImportManifestVersion.Current;

    /// <summary>
    /// Gets the stable logical identifier of the associated archive import plan.
    /// </summary>
    public string PlanId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the lifecycle status of the archive import run.
    /// </summary>
    public ArchiveImportRunStatus Status { get; init; } = ArchiveImportRunStatus.NotStarted;

    /// <summary>
    /// Gets the UTC timestamp when the state was last updated.
    /// </summary>
    public DateTimeOffset LastUpdatedUtc { get; init; }

    /// <summary>
    /// Gets the optional machine name that last updated the state.
    /// </summary>
    public string? LastMachineName { get; init; }

    /// <summary>
    /// Gets the optional workspace root path that last updated the state.
    /// </summary>
    public string? LastWorkspaceRoot { get; init; }

    /// <summary>
    /// Gets the current resumable checkpoint of the archive import run.
    /// </summary>
    public ArchiveImportCheckpoint CurrentCheckpoint { get; init; } = new();

    /// <summary>
    /// Gets the per-snapshot execution states.
    /// </summary>
    public IReadOnlyList<ArchiveImportSnapshotState> Snapshots { get; init; } = Array.Empty<ArchiveImportSnapshotState>();

    /// <summary>
    /// Gets the optional summary of the most recent resume validation.
    /// </summary>
    public string? LastValidationSummary { get; init; }

    /// <summary>
    /// Gets provider-specific extension data associated with the run state.
    /// </summary>
    public IReadOnlyDictionary<string, string> ExtensionData { get; init; } = new Dictionary<string, string>();
}
