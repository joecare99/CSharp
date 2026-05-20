namespace RepoMigrator.Core;

/// <summary>
/// Defines the normalized kind of migration source that should be resolved by a source provider.
/// </summary>
public enum MigrationSourceKind
{
    Repository,
    ArchiveCollection
}
