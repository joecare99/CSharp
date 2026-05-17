namespace RepoMigrator.Core;

/// <summary>
/// Defines the lifecycle state of an archive import plan.
/// </summary>
public enum ArchiveImportPlanStatus
{
    Draft,
    Ready,
    InProgress,
    Completed,
    Failed,
    Abandoned
}
