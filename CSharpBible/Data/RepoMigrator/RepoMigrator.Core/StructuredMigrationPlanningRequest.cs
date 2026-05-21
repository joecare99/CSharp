namespace RepoMigrator.Core;

/// <summary>
/// Represents the provider-agnostic inputs required to plan one structured migration flow.
/// </summary>
public sealed class StructuredMigrationPlanningRequest
{
    /// <summary>
    /// Gets the normalized migration source definition.
    /// </summary>
    public MigrationSourceDefinition Source { get; init; } = new();

    /// <summary>
    /// Gets the normalized migration destination definition.
    /// </summary>
    public MigrationDestinationDefinition Destination { get; init; } = new();

    /// <summary>
    /// Gets a value indicating whether the source side explicitly supports direct transfer for this flow.
    /// </summary>
    public bool SourceSupportsDirectTransfer { get; init; }

    /// <summary>
    /// Gets a value indicating whether the destination side explicitly supports direct transfer for this flow.
    /// </summary>
    public bool DestinationSupportsDirectTransfer { get; init; }

    /// <summary>
    /// Gets a value indicating whether explicit path rewrites are required for this flow.
    /// </summary>
    public bool RequiresPathRewrites { get; init; }

    /// <summary>
    /// Gets a value indicating whether structured normalization is required for this flow.
    /// </summary>
    public bool RequiresStructuredNormalization { get; init; }

    /// <summary>
    /// Gets a value indicating whether the legacy snapshot compatibility path remains available.
    /// </summary>
    public bool SupportsSnapshotCompatibility { get; init; } = true;
}
