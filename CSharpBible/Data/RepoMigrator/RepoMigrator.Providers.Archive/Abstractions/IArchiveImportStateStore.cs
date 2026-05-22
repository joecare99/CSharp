namespace RepoMigrator.Providers.Archive.Abstractions;

/// <summary>
/// Persists durable archive import plans and execution state under provider-defined runtime storage.
/// </summary>
public interface IArchiveImportStateStore
{
    /// <summary>
    /// Saves the supplied archive import plan.
    /// </summary>
    Task SavePlanAsync(ArchiveImportPlan plan, CancellationToken ct);

    /// <summary>
    /// Loads the archive import plan for the supplied plan identifier.
    /// </summary>
    Task<ArchiveImportPlan> LoadPlanAsync(string planId, CancellationToken ct);

    /// <summary>
    /// Saves the supplied archive import execution state.
    /// </summary>
    Task SaveStateAsync(ArchiveImportState state, CancellationToken ct);

    /// <summary>
    /// Loads the archive import execution state for the supplied plan identifier.
    /// </summary>
    Task<ArchiveImportState> LoadStateAsync(string planId, CancellationToken ct);

    /// <summary>
    /// Gets the deterministic storage directory path for the supplied plan identifier.
    /// </summary>
    string GetPlanDirectoryPath(string planId);
}
