using RepoMigrator.Core;

namespace RepoMigrator.Providers.Archive.Abstractions;

/// <summary>
/// Prepares durable archive import plans from normalized source and destination definitions.
/// </summary>
public interface IArchiveImportPlanner
{
    /// <summary>
    /// Prepares a durable archive import plan for the supplied source and destination definitions.
    /// </summary>
    /// <param name="source">The normalized source definition.</param>
    /// <param name="destination">The normalized destination definition.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The prepared archive import plan.</returns>
    Task<ArchiveImportPlan> PrepareAsync(MigrationSourceDefinition source, MigrationDestinationDefinition destination, CancellationToken ct);
}
