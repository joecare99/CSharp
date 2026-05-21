namespace RepoMigrator.Core;

/// <summary>
/// Represents a prepared migration source plan that can be consumed by orchestration logic.
/// </summary>
public sealed class MigrationSourcePlan
{
    /// <summary>
    /// Gets the normalized source definition that produced this plan.
    /// </summary>
    public MigrationSourceDefinition Source { get; init; } = new();

    /// <summary>
    /// Gets the planned source items in deterministic processing order.
    /// </summary>
    public IReadOnlyList<MigrationSourcePlanItem> Items { get; init; } = Array.Empty<MigrationSourcePlanItem>();
}
