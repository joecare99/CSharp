namespace RepoMigrator.Core;

/// <summary>
/// Represents the provider-agnostic inputs used to select an execution path for one migration flow.
/// </summary>
public sealed class MigrationExecutionPathRequest
{
    /// <summary>
    /// Gets a value indicating whether the source side explicitly supports direct transfer for this flow.
    /// </summary>
    public bool SourceSupportsDirectTransfer { get; init; }

    /// <summary>
    /// Gets a value indicating whether the destination side explicitly supports direct transfer for this flow.
    /// </summary>
    public bool DestinationSupportsDirectTransfer { get; init; }

    /// <summary>
    /// Gets the structured-change capabilities of the source side.
    /// </summary>
    public ChangeApplicationCapabilities SourceCapabilities { get; init; } = new();

    /// <summary>
    /// Gets the structured-change capabilities of the destination side.
    /// </summary>
    public ChangeApplicationCapabilities DestinationCapabilities { get; init; } = new();

    /// <summary>
    /// Gets a value indicating whether the flow requires explicit path rewriting.
    /// </summary>
    public bool RequiresPathRewrites { get; init; }

    /// <summary>
    /// Gets a value indicating whether the flow requires structured normalization such as patch parsing.
    /// </summary>
    public bool RequiresStructuredNormalization { get; init; }

    /// <summary>
    /// Gets a value indicating whether the legacy snapshot compatibility path is available.
    /// </summary>
    public bool SupportsSnapshotCompatibility { get; init; } = true;
}
