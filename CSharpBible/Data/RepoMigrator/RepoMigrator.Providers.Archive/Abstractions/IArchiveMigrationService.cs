using RepoMigrator.Core;

namespace RepoMigrator.Providers.Archive.Abstractions;

/// <summary>
/// Executes durable archive import plans and resumes interrupted archive migrations.
/// </summary>
public interface IArchiveMigrationService
{
    /// <summary>
    /// Prepares, persists, and returns an archive import plan for the supplied source and destination definitions.
    /// </summary>
    Task<ArchiveImportPlan> PreparePlanAsync(MigrationSourceDefinition source, MigrationDestinationDefinition destination, CancellationToken ct);

    /// <summary>
    /// Executes the supplied archive import plan identifier from the persisted DevOps manifests.
    /// </summary>
    Task<ArchiveImportState> ExecuteAsync(string planId, IMigrationProgress progress, CancellationToken ct);

    /// <summary>
    /// Resumes the supplied archive import plan identifier from persisted state.
    /// </summary>
    Task<ArchiveImportState> ResumeAsync(string planId, IMigrationProgress progress, CancellationToken ct);
}
