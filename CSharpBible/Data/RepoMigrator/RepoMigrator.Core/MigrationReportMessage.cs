namespace RepoMigrator.Core;

/// <summary>
/// Identifies a migration report message.
/// </summary>
public enum MigrationReportMessage
{
    SourceOpening,
    NativeHistoryTransferStarting,
    ChangeSetsLoading,
    NoChangeSetsFound,
    TargetInitializing,
    PipelineEnabled,
    PipelineCleanupStarting,
    ChangeSetProcessingStarting,
    CommitCompleted,
    FlushStarting,
    FlushCompleted,
    ExportWorkerSourceOpened,
    ExportWorkerSnapshotExporting,
    ExportWorkerSnapshotExported,
    ExportWorkerSnapshotHandedOff,
    ExportWorkerCompleted,
    PipelineExportSlotRequested,
    PipelineExportQueued,
    PipelineExportQueueCompleted,
    PipelineSnapshotWaiting,
    PipelineSnapshotReady,
    PipelineSnapshotBuffered,
    CommitToBranchStarting,
    ProjectionPlanned,
    ProjectedBranchPrepared,
    ProjectedBranchEmpty,
    ProjectedBranchCommitted,
    GitBranchTransferStarting,
    GitTagTransferStarting,
    MigrationCompleted
}
