using RepoMigrator.Core;

namespace RepoMigrator.Tools.PipelinedMigration;

/// <summary>
/// Writes migration progress to the console.
/// </summary>
public sealed class ConsoleMigrationProgress : IMigrationProgress
{
    /// <inheritdoc />
    public void Report(MigrationReportSeverity severity, MigrationReportMessage message, params object?[] arrAdditional)
        => Console.WriteLine(FormatProgressMessage(message, arrAdditional));

    private static string FormatProgressMessage(MigrationReportMessage message, object?[] arrAdditional)
        => message switch
        {
            MigrationReportMessage.SourceOpening => $"Öffne Quelle ({GetRequired<string>(arrAdditional, 0)}) …",
            MigrationReportMessage.NativeHistoryTransferStarting => $"Übertrage Historie nativ ({GetRequired<string>(arrAdditional, 0)} -> {GetRequired<RepoType>(arrAdditional, 1)}) …",
            MigrationReportMessage.ChangeSetsLoading => "Lese Changesets …",
            MigrationReportMessage.NoChangeSetsFound => "Keine Changesets gefunden.",
            MigrationReportMessage.TargetInitializing => $"Initialisiere Ziel ({GetRequired<string>(arrAdditional, 0)}) …",
            MigrationReportMessage.PipelineEnabled => $"Pipeline aktiviert: {GetRequired<int>(arrAdditional, 0)} Worker, Prefetch {GetRequired<int>(arrAdditional, 1)}, {GetRequired<int>(arrAdditional, 2)} Changesets.",
            MigrationReportMessage.PipelineCleanupStarting => "Pipeline: Abbruch oder Fehler erkannt, bereinige ausstehende Snapshots …",
            MigrationReportMessage.ChangeSetProcessingStarting => $"[{GetRequired<int>(arrAdditional, 1)}/{GetRequired<int>(arrAdditional, 2)}] {GetRequired<string>(arrAdditional, 0)}",
            MigrationReportMessage.CommitCompleted => $"Commit {GetRequired<int>(arrAdditional, 1)}/{GetRequired<int>(arrAdditional, 2)} übertragen: {Short(GetRequired<string>(arrAdditional, 0))}",
            MigrationReportMessage.FlushStarting => $"Starte Ziel-Synchronisierung ({GetRequired<string>(arrAdditional, 0)}) …",
            MigrationReportMessage.FlushCompleted => $"Ziel-Synchronisierung ({GetRequired<string>(arrAdditional, 0)}) abgeschlossen.",
            MigrationReportMessage.ExportWorkerSourceOpened => $"Export-Worker {GetRequired<int>(arrAdditional, 0)}: Quelle geöffnet.",
            MigrationReportMessage.ExportWorkerSnapshotExporting => $"Export-Worker {GetRequired<int>(arrAdditional, 0)}: exportiere {GetRequired<string>(arrAdditional, 1)} …",
            MigrationReportMessage.ExportWorkerSnapshotExported => $"Export-Worker {GetRequired<int>(arrAdditional, 0)}: Export {GetRequired<string>(arrAdditional, 1)} abgeschlossen.",
            MigrationReportMessage.ExportWorkerSnapshotHandedOff => $"Export-Worker {GetRequired<int>(arrAdditional, 0)}: Snapshot {GetRequired<string>(arrAdditional, 1)} an Commit-Stufe übergeben.",
            MigrationReportMessage.ExportWorkerCompleted => $"Export-Worker {GetRequired<int>(arrAdditional, 0)}: keine weiteren Export-Aufträge.",
            MigrationReportMessage.PipelineExportSlotRequested => $"Pipeline: fordere Export-Slot {GetRequired<int>(arrAdditional, 1)}/{GetRequired<int>(arrAdditional, 2)}: {GetRequired<string>(arrAdditional, 0)}",
            MigrationReportMessage.PipelineExportQueued => $"Pipeline: queue {GetRequired<int>(arrAdditional, 1)}/{GetRequired<int>(arrAdditional, 2)}: {GetRequired<string>(arrAdditional, 0)}",
            MigrationReportMessage.PipelineExportQueueCompleted => "Pipeline: alle Export-Aufträge wurden eingeplant.",
            MigrationReportMessage.PipelineSnapshotWaiting => $"Pipeline: warte auf Snapshot {GetRequired<int>(arrAdditional, 0)}/{GetRequired<int>(arrAdditional, 1)} …",
            MigrationReportMessage.PipelineSnapshotReady => $"Pipeline: Snapshot {GetRequired<string>(arrAdditional, 0)} ist in Commit-Reihenfolge bereit.",
            MigrationReportMessage.PipelineSnapshotBuffered => $"Pipeline: Snapshot {GetRequired<string>(arrAdditional, 0)} gepuffert, warte weiter auf Position {GetRequired<int>(arrAdditional, 1)}.",
            MigrationReportMessage.CommitToBranchStarting => $"Commit-Stufe: Revision {GetRequired<string>(arrAdditional, 0)} wird auf Branch '{GetRequired<string>(arrAdditional, 1)}' übertragen.",
            MigrationReportMessage.ProjectionPlanned => $"Commit-Stufe: Revision {GetRequired<string>(arrAdditional, 0)} wird in {GetRequired<int>(arrAdditional, 1)} Sub-Branches projiziert (Tiefe {GetRequired<int>(arrAdditional, 2)}).",
            MigrationReportMessage.ProjectedBranchPrepared => $"Commit-Stufe: Branch '{GetRequired<string>(arrAdditional, 1)}' erhält {GetRequired<int>(arrAdditional, 2)} Pfade aus Revision {GetRequired<string>(arrAdditional, 0)}.",
            MigrationReportMessage.ProjectedBranchEmpty => $"Commit-Stufe: Branch '{GetRequired<string>(arrAdditional, 1)}' wird für Revision {GetRequired<string>(arrAdditional, 0)} als leerer Snapshot fortgeführt.",
            MigrationReportMessage.ProjectedBranchCommitted => $"Commit-Stufe: Branch '{GetRequired<string>(arrAdditional, 1)}' für Revision {GetRequired<string>(arrAdditional, 0)} übertragen.",
            MigrationReportMessage.GitBranchTransferStarting => $"Übertrage Branch {GetRequired<string>(arrAdditional, 0)} -> {GetRequired<string>(arrAdditional, 1)} …",
            MigrationReportMessage.GitTagTransferStarting => $"Übertrage Tag {GetRequired<string>(arrAdditional, 0)} -> {GetRequired<string>(arrAdditional, 1)} …",
            MigrationReportMessage.MigrationCompleted => "Migration abgeschlossen.",
            _ => throw new ArgumentOutOfRangeException(nameof(message), message, null)
        };

    private static T GetRequired<T>(IReadOnlyList<object?> lstValues, int iIndex)
        => lstValues.Count > iIndex && lstValues[iIndex] is T value
            ? value
            : throw new InvalidOperationException($"Expected argument at index {iIndex} with type {typeof(T).Name}.");

    private static string Short(string? sId)
        => string.IsNullOrEmpty(sId) || sId.Length <= 8 ? sId ?? string.Empty : sId[..8];
}
