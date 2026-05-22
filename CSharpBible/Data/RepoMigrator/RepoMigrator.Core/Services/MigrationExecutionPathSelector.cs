namespace RepoMigrator.Core.Services;

/// <summary>
/// Selects a provider-agnostic execution path from direct-transfer and structured-change capabilities.
/// </summary>
public sealed class MigrationExecutionPathSelector
{
    /// <summary>
    /// Selects the most appropriate execution path for the supplied request.
    /// </summary>
    public MigrationExecutionPathSelection Select(MigrationExecutionPathRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var rejectedAlternatives = new List<MigrationExecutionPathKind>();
        var metadata = new Dictionary<string, string>();

        var canUseDirectTransfer = request.SourceSupportsDirectTransfer
            && request.DestinationSupportsDirectTransfer
            && !request.RequiresPathRewrites
            && !request.RequiresStructuredNormalization;

        if (canUseDirectTransfer)
        {
            metadata[StructuredChangeMetadataKeys.ExecutionPathDecision] = nameof(MigrationExecutionPathKind.DirectTransfer);
            return new MigrationExecutionPathSelection
            {
                Kind = MigrationExecutionPathKind.DirectTransfer,
                Rationale = "Both sides explicitly support direct transfer and no structured normalization constraints are active.",
                RejectedAlternatives = Array.Empty<MigrationExecutionPathKind>(),
                Metadata = metadata
            };
        }

        rejectedAlternatives.Add(MigrationExecutionPathKind.DirectTransfer);

        var sourceSupportsStructuredChanges = request.SourceCapabilities.SupportsStructuredChanges;
        var destinationSupportsStructuredChanges = request.DestinationCapabilities.SupportsStructuredChanges;

        if (sourceSupportsStructuredChanges && destinationSupportsStructuredChanges)
        {
            metadata[StructuredChangeMetadataKeys.ExecutionPathDecision] = nameof(MigrationExecutionPathKind.StructuredChange);
            return new MigrationExecutionPathSelection
            {
                Kind = MigrationExecutionPathKind.StructuredChange,
                Rationale = "Both sides support provider-agnostic structured changes directly.",
                RejectedAlternatives = rejectedAlternatives,
                Metadata = metadata
            };
        }

        rejectedAlternatives.Add(MigrationExecutionPathKind.StructuredChange);

        if (sourceSupportsStructuredChanges && request.DestinationCapabilities.SupportsMaterializedWorkdirFallback)
        {
            metadata[StructuredChangeMetadataKeys.ExecutionPathDecision] = nameof(MigrationExecutionPathKind.StructuredChangeWithMaterializedWorkdir);
            return new MigrationExecutionPathSelection
            {
                Kind = MigrationExecutionPathKind.StructuredChangeWithMaterializedWorkdir,
                Rationale = "The source can emit structured changes and the destination supports compatibility materialization.",
                RejectedAlternatives = rejectedAlternatives,
                Metadata = metadata
            };
        }

        rejectedAlternatives.Add(MigrationExecutionPathKind.StructuredChangeWithMaterializedWorkdir);

        if (request.SupportsSnapshotCompatibility)
        {
            metadata[StructuredChangeMetadataKeys.ExecutionPathDecision] = nameof(MigrationExecutionPathKind.SnapshotCompatibility);
            return new MigrationExecutionPathSelection
            {
                Kind = MigrationExecutionPathKind.SnapshotCompatibility,
                Rationale = "Structured-change execution is unavailable, so the legacy snapshot compatibility path remains the fallback.",
                RejectedAlternatives = rejectedAlternatives,
                Metadata = metadata
            };
        }

        rejectedAlternatives.Add(MigrationExecutionPathKind.SnapshotCompatibility);
        metadata[StructuredChangeMetadataKeys.ExecutionPathDecision] = nameof(MigrationExecutionPathKind.Unknown);
        return new MigrationExecutionPathSelection
        {
            Kind = MigrationExecutionPathKind.Unknown,
            Rationale = "No compatible execution path could be selected from the supplied capabilities.",
            RejectedAlternatives = rejectedAlternatives,
            Metadata = metadata
        };
    }
}
