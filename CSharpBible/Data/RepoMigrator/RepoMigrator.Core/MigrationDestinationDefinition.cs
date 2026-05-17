namespace RepoMigrator.Core;

/// <summary>
/// Represents a normalized migration destination definition above provider-specific target implementations.
/// </summary>
public sealed class MigrationDestinationDefinition
{
    /// <summary>
    /// Gets the normalized destination kind.
    /// </summary>
    public MigrationDestinationKind Kind { get; init; } = MigrationDestinationKind.Repository;

    /// <summary>
    /// Gets the repository-specific destination endpoint when the destination kind is repository-backed.
    /// </summary>
    public RepositoryEndpoint? Repository { get; init; }

    /// <summary>
    /// Gets the future sequential-archive destination definition when archive output is selected.
    /// </summary>
    public SequentialArchiveDestinationDefinition? SequentialArchiveDestination { get; init; }
}
