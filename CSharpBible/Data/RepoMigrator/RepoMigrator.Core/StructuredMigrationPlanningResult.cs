using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.Core;

/// <summary>
/// Represents the structured migration planning result including resolved endpoints and selected execution path.
/// </summary>
public sealed class StructuredMigrationPlanningResult
{
    /// <summary>
    /// Gets the resolved structured change-set source.
    /// </summary>
    public required IMigrationChangeSetSource Source { get; init; }

    /// <summary>
    /// Gets the resolved structured change-set sink.
    /// </summary>
    public required IMigrationChangeSetSink Sink { get; init; }

    /// <summary>
    /// Gets the selected execution path.
    /// </summary>
    public MigrationExecutionPathSelection ExecutionPath { get; init; } = new();

    /// <summary>
    /// Gets the structured capabilities resolved for the source side.
    /// </summary>
    public ChangeApplicationCapabilities SourceCapabilities { get; init; } = new();

    /// <summary>
    /// Gets the structured capabilities resolved for the destination side.
    /// </summary>
    public ChangeApplicationCapabilities DestinationCapabilities { get; init; } = new();
}
