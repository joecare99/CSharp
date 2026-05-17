namespace RepoMigrator.Core;

/// <summary>
/// Represents a durable archive import plan that can be persisted under the DevOps workspace.
/// </summary>
public sealed class ArchiveImportPlan
{
    /// <summary>
    /// Gets the persisted schema version of the archive import plan.
    /// </summary>
    public int SchemaVersion { get; init; } = ArchiveImportManifestVersion.Current;

    /// <summary>
    /// Gets the stable logical identifier of the archive import plan.
    /// </summary>
    public string PlanId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the UTC timestamp when the plan was created.
    /// </summary>
    public DateTimeOffset CreatedUtc { get; init; }

    /// <summary>
    /// Gets the normalized migration source definition used to prepare the plan.
    /// </summary>
    public MigrationSourceDefinition Source { get; init; } = new();

    /// <summary>
    /// Gets the normalized migration destination definition used to execute the plan.
    /// </summary>
    public MigrationDestinationDefinition Destination { get; init; } = new();

    /// <summary>
    /// Gets the ordered archive import plan items.
    /// </summary>
    public IReadOnlyList<ArchiveImportPlanItem> Items { get; init; } = Array.Empty<ArchiveImportPlanItem>();

    /// <summary>
    /// Gets the lifecycle status of the plan.
    /// </summary>
    public ArchiveImportPlanStatus Status { get; init; } = ArchiveImportPlanStatus.Draft;

    /// <summary>
    /// Gets optional free-form notes associated with the plan.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Gets provider-specific extension data associated with the normalized source definition.
    /// </summary>
    public IReadOnlyDictionary<string, string> SourceProviderData { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets provider-specific extension data associated with the normalized destination definition.
    /// </summary>
    public IReadOnlyDictionary<string, string> DestinationProviderData { get; init; } = new Dictionary<string, string>();
}
