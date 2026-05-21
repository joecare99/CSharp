namespace RepoMigrator.Providers.Archive;

/// <summary>
/// Defines where archive-backed migration source snapshots are discovered.
/// </summary>
public enum ArchiveSourceLocationKind
{
    HttpIndex,
    LocalDirectory
}
