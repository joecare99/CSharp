namespace RepoMigrator.Core;

/// <summary>
/// Defines the normalized kind of migration destination that should be resolved by a destination provider.
/// </summary>
public enum MigrationDestinationKind
{
    Repository,
    SequentialArchiveCollection
}
