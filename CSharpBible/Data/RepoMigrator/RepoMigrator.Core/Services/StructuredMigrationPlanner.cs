using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.Core.Services;

/// <summary>
/// Orchestrates structured source resolution, structured sink resolution, capability lookup, and execution-path selection.
/// </summary>
public sealed class StructuredMigrationPlanner : IStructuredMigrationPlanner
{
    private readonly IMigrationChangeSetSourceFactory _sourceFactory;
    private readonly IMigrationChangeSetSinkFactory _sinkFactory;
    private readonly MigrationExecutionPathSelector _selector;

    /// <summary>
    /// Initializes a new instance of the <see cref="StructuredMigrationPlanner"/> class.
    /// </summary>
    public StructuredMigrationPlanner(
        IMigrationChangeSetSourceFactory sourceFactory,
        IMigrationChangeSetSinkFactory sinkFactory,
        MigrationExecutionPathSelector selector)
    {
        _sourceFactory = sourceFactory ?? throw new ArgumentNullException(nameof(sourceFactory));
        _sinkFactory = sinkFactory ?? throw new ArgumentNullException(nameof(sinkFactory));
        _selector = selector ?? throw new ArgumentNullException(nameof(selector));
    }

    /// <inheritdoc />
    public async Task<StructuredMigrationPlanningResult> PlanAsync(StructuredMigrationPlanningRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        ct.ThrowIfCancellationRequested();

        var source = _sourceFactory.Create(request.Source);
        var sink = _sinkFactory.Create(request.Destination);
        var sourceCapabilities = await source.GetCapabilitiesAsync(request.Source, ct).ConfigureAwait(false);
        var destinationCapabilities = await sink.GetCapabilitiesAsync(request.Destination, ct).ConfigureAwait(false);
        var selection = _selector.Select(new MigrationExecutionPathRequest
        {
            SourceSupportsDirectTransfer = request.SourceSupportsDirectTransfer,
            DestinationSupportsDirectTransfer = request.DestinationSupportsDirectTransfer,
            SourceCapabilities = sourceCapabilities,
            DestinationCapabilities = destinationCapabilities,
            RequiresPathRewrites = request.RequiresPathRewrites,
            RequiresStructuredNormalization = request.RequiresStructuredNormalization,
            SupportsSnapshotCompatibility = request.SupportsSnapshotCompatibility
        });

        return new StructuredMigrationPlanningResult
        {
            Source = source,
            Sink = sink,
            ExecutionPath = selection,
            SourceCapabilities = sourceCapabilities,
            DestinationCapabilities = destinationCapabilities
        };
    }
}
