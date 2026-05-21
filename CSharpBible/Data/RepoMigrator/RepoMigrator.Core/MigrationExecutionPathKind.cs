namespace RepoMigrator.Core;

/// <summary>
/// Defines the normalized execution path chosen for a migration.
/// </summary>
public enum MigrationExecutionPathKind
{
    /// <summary>
    /// No execution path has been selected yet.
    /// </summary>
    Unknown,

    /// <summary>
    /// A direct source-to-target provider path is used.
    /// </summary>
    DirectTransfer,

    /// <summary>
    /// Provider-agnostic structured changes are emitted and consumed directly.
    /// </summary>
    StructuredChange,

    /// <summary>
    /// Provider-agnostic structured changes are materialized through a compatibility work directory.
    /// </summary>
    StructuredChangeWithMaterializedWorkdir,

    /// <summary>
    /// A legacy snapshot-oriented compatibility path is used.
    /// </summary>
    SnapshotCompatibility
}
