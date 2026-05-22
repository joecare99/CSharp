namespace RepoMigrator.Core.Abstractions;

/// <summary>
/// Plans one provider-agnostic structured migration flow by resolving structured endpoints and selecting an execution path.
/// </summary>
public interface IStructuredMigrationPlanner
{
    /// <summary>
    /// Plans the structured migration flow for the supplied request.
    /// </summary>
    Task<StructuredMigrationPlanningResult> PlanAsync(StructuredMigrationPlanningRequest request, CancellationToken ct);
}
