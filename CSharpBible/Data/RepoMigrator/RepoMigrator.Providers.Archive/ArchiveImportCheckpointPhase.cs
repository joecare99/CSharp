namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Defines the checkpoint phase reached by an archive import run or snapshot item.
/// </summary>
public enum ArchiveImportCheckpointPhase
{
    None,
    PlanPrepared,
    SnapshotReady,
    ExtractionCompleted,
    CommitCompleted,
    TagCreated,
    BranchCreated,
    SnapshotCompleted,
    RunCompleted
}
