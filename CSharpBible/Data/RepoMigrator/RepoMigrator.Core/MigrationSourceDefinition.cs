namespace RepoMigrator.Core;

/// <summary>
/// Represents a normalized migration source definition above provider-specific source implementations.
/// </summary>
public sealed class MigrationSourceDefinition
{
    /// <summary>
    /// Gets the normalized source kind.
    /// </summary>
    public MigrationSourceKind Kind { get; init; } = MigrationSourceKind.Repository;

    /// <summary>
    /// Gets the repository-specific source endpoint when the source kind is repository-backed.
    /// </summary>
    public RepositoryEndpoint? Repository { get; init; }

    /// <summary>
    /// Gets the archive-specific source definition when the source kind is archive-backed.
    /// </summary>
    public ArchiveMigrationSourceDefinition? ArchiveSource { get; init; }
}
