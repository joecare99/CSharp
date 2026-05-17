namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Defines the lifecycle state of an archive import execution run.
/// </summary>
public enum ArchiveImportRunStatus
{
    NotStarted,
    ReadyToResume,
    Running,
    Completed,
    Failed,
    Abandoned
}
