// RepoMigrator.Core/Services/MigrationService.cs
using RepoMigrator.Core.Abstractions;
using System.Threading.Channels;

namespace RepoMigrator.Core;

public sealed class MigrationService : IMigrationService
{
    private readonly IProviderFactory _factory;

    public MigrationService(IProviderFactory factory) => _factory = factory;

    public async Task MigrateAsync(
        RepositoryEndpoint source,
        RepositoryEndpoint target,
        ChangeSetQuery query,
        MigrationOptions options,
        IMigrationProgress progress,
        CancellationToken ct)
    {
        if (options.TransferMode == RepositoryTransferMode.NativeHistory)
        {
            await using var src = _factory.Create(source.Type);
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.SourceOpening, src.Name);
            await src.OpenAsync(source, ct);
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.NativeHistoryTransferStarting, src.Name, target.Type);
            await src.TransferAsync(source, target, options, progress, ct);
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.MigrationCompleted);
            return;
        }

        if (options.ExecutionMode == MigrationExecutionMode.Pipelined && CanUsePipelinedExecution(source, target))
        {
            await MigrateSnapshotsPipelinedAsync(source, target, query, options, progress, ct);
            return;
        }

        await MigrateSnapshotsSequentialAsync(source, target, query, options, progress, ct);
    }

    private async Task MigrateSnapshotsSequentialAsync(
        RepositoryEndpoint source,
        RepositoryEndpoint target,
        ChangeSetQuery query,
        MigrationOptions options,
        IMigrationProgress progress,
        CancellationToken ct)
    {
        await using var src = _factory.Create(source.Type);
        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.SourceOpening, src.Name);
        await src.OpenAsync(source, ct);

        await using var dst = _factory.Create(target.Type);

        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ChangeSetsLoading);
        var lstChanges = await src.GetChangeSetsAsync(query, ct);
        if (lstChanges.Count == 0)
        {
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.NoChangeSetsFound);
            return;
        }

        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.TargetInitializing, dst.Name);
        await dst.InitializeTargetAsync(target, emptyInit: true, ct);

        var hsKnownProjectedBranches = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var iTotal = lstChanges.Count;
        for (var iIndex = 0; iIndex < iTotal; iIndex++)
        {
            ct.ThrowIfCancellationRequested();
            var changeSet = lstChanges[iIndex];
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ChangeSetProcessingStarting, changeSet.Id, iIndex + 1, iTotal);

            var sTempDirectory = CreateTempDirectory("linear");
            try
            {
                await src.MaterializeSnapshotAsync(sTempDirectory, changeSet.Id, ct);
                await CommitProjectedSnapshotAsync(dst, target, changeSet, sTempDirectory, options, hsKnownProjectedBranches, progress, ct);
                progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.CommitCompleted, changeSet.Id, iIndex + 1, iTotal);
            }
            finally
            {
                TryDeleteDirectory(sTempDirectory);
            }
        }

        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.FlushStarting, dst.Name);
        await dst.FlushAsync(ct);
        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.FlushCompleted, dst.Name);

        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.MigrationCompleted);
    }

    private async Task MigrateSnapshotsPipelinedAsync(
        RepositoryEndpoint source,
        RepositoryEndpoint target,
        ChangeSetQuery query,
        MigrationOptions options,
        IMigrationProgress progress,
        CancellationToken ct)
    {
        await using var enumerateProvider = _factory.Create(source.Type);
        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.SourceOpening, enumerateProvider.Name);
        await enumerateProvider.OpenAsync(source, ct);

        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ChangeSetsLoading);
        var lstChanges = await enumerateProvider.GetChangeSetsAsync(query, ct);
        if (lstChanges.Count == 0)
        {
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.NoChangeSetsFound);
            return;
        }

        progress.Report(
            MigrationReportSeverity.Information,
            MigrationReportMessage.PipelineEnabled,
            Math.Min(options.PipelineExportWorkerCount, lstChanges.Count),
            Math.Max(options.PipelinePrefetchCount, 1),
            lstChanges.Count);

        await using var dst = _factory.Create(target.Type);
        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.TargetInitializing, dst.Name);
        await dst.InitializeTargetAsync(target, emptyInit: true, ct);

        using var ctsLinked = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var exportRequests = Channel.CreateBounded<(int Index, ChangeSetInfo ChangeSet)>(new BoundedChannelOptions(Math.Max(options.PipelinePrefetchCount, 1))
        {
            SingleWriter = true,
            SingleReader = false,
            FullMode = BoundedChannelFullMode.Wait
        });
        var exportedSnapshots = Channel.CreateBounded<(int Index, ChangeSetInfo ChangeSet, string TempDirectory)>(new BoundedChannelOptions(Math.Max(options.PipelinePrefetchCount, 1))
        {
            SingleWriter = false,
            SingleReader = true,
            FullMode = BoundedChannelFullMode.Wait
        });

        var iWorkerCount = Math.Min(options.PipelineExportWorkerCount, lstChanges.Count);
        var arrExportTasks = Enumerable.Range(0, iWorkerCount)
            .Select(iWorkerIndex => ExportSnapshotsAsync(iWorkerIndex, source, exportRequests.Reader, exportedSnapshots.Writer, progress, ctsLinked.Token))
            .ToArray();

        var completeExportsTask = CompleteExportChannelAsync(arrExportTasks, exportedSnapshots.Writer, ctsLinked.Token);
        var commitTask = CommitSnapshotsInOrderAsync(dst, target, exportedSnapshots.Reader, lstChanges.Count, options, progress, ctsLinked.Token);
        try
        {
            await QueueExportRequestsAsync(lstChanges, exportRequests.Writer, progress, ctsLinked.Token);
            await completeExportsTask;
            await commitTask;
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.FlushStarting, dst.Name);
            await dst.FlushAsync(ct);
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.FlushCompleted, dst.Name);
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.MigrationCompleted);
        }
        catch
        {
            progress.Report(MigrationReportSeverity.Warning, MigrationReportMessage.PipelineCleanupStarting);
            ctsLinked.Cancel();
            exportRequests.Writer.TryComplete();
            await DrainExportedSnapshotsAsync(exportedSnapshots.Reader);
            throw;
        }
    }

    private async Task ExportSnapshotsAsync(
        int iWorkerIndex,
        RepositoryEndpoint source,
        ChannelReader<(int Index, ChangeSetInfo ChangeSet)> exportReader,
        ChannelWriter<(int Index, ChangeSetInfo ChangeSet, string TempDirectory)> snapshotWriter,
        IMigrationProgress progress,
        CancellationToken ct)
    {
        await using var src = _factory.Create(source.Type);
        await src.OpenAsync(source, ct);
        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ExportWorkerSourceOpened, iWorkerIndex + 1);

        await foreach (var exportRequest in exportReader.ReadAllAsync(ct))
        {
            var sTempDirectory = CreateTempDirectory("pipeline");
            try
            {
                progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ExportWorkerSnapshotExporting, iWorkerIndex + 1, exportRequest.ChangeSet.Id);
                await src.MaterializeSnapshotAsync(sTempDirectory, exportRequest.ChangeSet.Id, ct);
                progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ExportWorkerSnapshotExported, iWorkerIndex + 1, exportRequest.ChangeSet.Id);
                await snapshotWriter.WriteAsync((exportRequest.Index, exportRequest.ChangeSet, sTempDirectory), ct);
                progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ExportWorkerSnapshotHandedOff, iWorkerIndex + 1, exportRequest.ChangeSet.Id);
            }
            catch
            {
                TryDeleteDirectory(sTempDirectory);
                throw;
            }
        }

        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ExportWorkerCompleted, iWorkerIndex + 1);
    }

    private static async Task QueueExportRequestsAsync(
        IReadOnlyList<ChangeSetInfo> lstChanges,
        ChannelWriter<(int Index, ChangeSetInfo ChangeSet)> exportWriter,
        IMigrationProgress progress,
        CancellationToken ct)
    {
        try
        {
            for (var iIndex = 0; iIndex < lstChanges.Count; iIndex++)
            {
                progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.PipelineExportSlotRequested, lstChanges[iIndex].Id, iIndex + 1, lstChanges.Count);
                await exportWriter.WriteAsync((iIndex, lstChanges[iIndex]), ct);
                progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.PipelineExportQueued, lstChanges[iIndex].Id, iIndex + 1, lstChanges.Count);
            }

            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.PipelineExportQueueCompleted);
            exportWriter.TryComplete();
        }
        catch (Exception ex)
        {
            exportWriter.TryComplete(ex);
            throw;
        }
    }

    private static async Task CompleteExportChannelAsync(
        Task[] arrExportTasks,
        ChannelWriter<(int Index, ChangeSetInfo ChangeSet, string TempDirectory)> snapshotWriter,
        CancellationToken ct)
    {
        try
        {
            await Task.WhenAll(arrExportTasks).WaitAsync(ct);
            snapshotWriter.TryComplete();
        }
        catch (Exception ex)
        {
            snapshotWriter.TryComplete(ex);
            throw;
        }
    }

    private async Task CommitSnapshotsInOrderAsync(
        IVersionControlProvider dst,
        RepositoryEndpoint target,
        ChannelReader<(int Index, ChangeSetInfo ChangeSet, string TempDirectory)> snapshotReader,
        int iExpectedItemCount,
        MigrationOptions options,
        IMigrationProgress progress,
        CancellationToken ct)
    {
        var dctBufferedItems = new SortedDictionary<int, (ChangeSetInfo ChangeSet, string TempDirectory)>();
        var hsKnownProjectedBranches = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var iExpectedIndex = 0; iExpectedIndex < iExpectedItemCount; iExpectedIndex++)
        {
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.PipelineSnapshotWaiting, iExpectedIndex + 1, iExpectedItemCount);
            (ChangeSetInfo ChangeSet, string TempDirectory) bufferedItem;
            while (!dctBufferedItems.Remove(iExpectedIndex, out bufferedItem))
            {
                var nextItem = await snapshotReader.ReadAsync(ct);
                if (nextItem.Index == iExpectedIndex)
                {
                    progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.PipelineSnapshotReady, nextItem.ChangeSet.Id);
                    bufferedItem = (nextItem.ChangeSet, nextItem.TempDirectory);
                    break;
                }

                dctBufferedItems[nextItem.Index] = (nextItem.ChangeSet, nextItem.TempDirectory);
                progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.PipelineSnapshotBuffered, nextItem.ChangeSet.Id, iExpectedIndex + 1);
            }

            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ChangeSetProcessingStarting, bufferedItem.ChangeSet.Id, iExpectedIndex + 1, iExpectedItemCount);
            try
            {
                await CommitProjectedSnapshotAsync(dst, target, bufferedItem.ChangeSet, bufferedItem.TempDirectory, options, hsKnownProjectedBranches, progress, ct);
                progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.CommitCompleted, bufferedItem.ChangeSet.Id, iExpectedIndex + 1, iExpectedItemCount);
            }
            finally
            {
                TryDeleteDirectory(bufferedItem.TempDirectory);
            }
        }

        foreach (var bufferedItem in dctBufferedItems.Values)
            TryDeleteDirectory(bufferedItem.TempDirectory);
    }

    private async Task DrainExportedSnapshotsAsync(ChannelReader<(int Index, ChangeSetInfo ChangeSet, string TempDirectory)> snapshotReader)
    {
        while (await snapshotReader.WaitToReadAsync())
        {
            while (snapshotReader.TryRead(out var workItem))
                TryDeleteDirectory(workItem.TempDirectory);
        }
    }

    private async Task CommitProjectedSnapshotAsync(
        IVersionControlProvider dst,
        RepositoryEndpoint target,
        ChangeSetInfo changeSet,
        string sTempDirectory,
        MigrationOptions options,
        HashSet<string> hsKnownProjectedBranches,
        IMigrationProgress progress,
        CancellationToken ct)
    {
        if (options.ProjectionMode != MigrationProjectionMode.SubdirectoryBranches || target.Type != RepoType.Git)
        {
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.CommitToBranchStarting, changeSet.Id, target.BranchOrTrunk ?? "(default)");
            await dst.CommitSnapshotAsync(sTempDirectory, BuildCommitMetadata(changeSet, target.BranchOrTrunk), ct);
            return;
        }

        var sRootBranchName = target.BranchOrTrunk;
        if (string.IsNullOrWhiteSpace(sRootBranchName))
            throw new InvalidOperationException("Für die Aufteilung in Sub-Branches muss ein Ziel-Branch angegeben sein.");

        var lstTrackedPaths = EnumerateTrackedPaths(sTempDirectory).ToList();
        var lstProjectionPlans = SubdirectoryBranchProjectionPlanner.BuildPlans(lstTrackedPaths, sRootBranchName, options.BranchSplitDepth);
        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ProjectionPlanned, changeSet.Id, lstProjectionPlans.Count, options.BranchSplitDepth);
        var dctCurrentPlans = lstProjectionPlans.ToDictionary(plan => plan.BranchName, plan => plan.Paths, StringComparer.OrdinalIgnoreCase);
        foreach (var sBranchName in dctCurrentPlans.Keys)
            hsKnownProjectedBranches.Add(sBranchName);

        foreach (var sBranchName in hsKnownProjectedBranches.OrderBy(static sValue => sValue, StringComparer.OrdinalIgnoreCase))
        {
            var sProjectionDirectory = CreateTempDirectory("projection");
            try
            {
                if (dctCurrentPlans.TryGetValue(sBranchName, out var hsPaths))
                {
                    progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ProjectedBranchPrepared, changeSet.Id, sBranchName, hsPaths.Count);
                    CopyProjectedPaths(sTempDirectory, sProjectionDirectory, hsPaths);
                }
                else
                {
                    progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ProjectedBranchEmpty, changeSet.Id, sBranchName);
                }

                await dst.CommitSnapshotAsync(sProjectionDirectory, BuildCommitMetadata(changeSet, sBranchName), ct);
                progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ProjectedBranchCommitted, changeSet.Id, sBranchName);
            }
            finally
            {
                TryDeleteDirectory(sProjectionDirectory);
            }
        }
    }

    private static CommitMetadata BuildCommitMetadata(ChangeSetInfo changeSet, string? sTargetBranch)
        => new()
        {
            Message = string.IsNullOrWhiteSpace(changeSet.Message) ? $"Imported revision {changeSet.Id}" : changeSet.Message,
            AuthorName = changeSet.AuthorName,
            AuthorEmail = changeSet.AuthorEmail,
            Timestamp = changeSet.Timestamp,
            TargetBranch = sTargetBranch
        };

    private static IEnumerable<string> EnumerateTrackedPaths(string sRootDirectory)
        => Directory.EnumerateFiles(sRootDirectory, "*", SearchOption.AllDirectories)
            .Select(sFilePath => Path.GetRelativePath(sRootDirectory, sFilePath).Replace('\\', '/'));

    private static void CopyProjectedPaths(string sSourceDirectory, string sTargetDirectory, IReadOnlySet<string> hsPaths)
    {
        Directory.CreateDirectory(sTargetDirectory);
        foreach (var sPath in hsPaths)
        {
            var sSourcePath = Path.Combine(sSourceDirectory, sPath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(sSourcePath))
                continue;

            var sTargetPath = Path.Combine(sTargetDirectory, sPath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(sTargetPath)!);
            File.Copy(sSourcePath, sTargetPath, overwrite: true);
        }
    }

    private static string CreateTempDirectory(string sCategory)
    {
        var sTempDirectory = Path.Combine(Path.GetTempPath(), "RepoMigrator", sCategory, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sTempDirectory);
        return sTempDirectory;
    }

    private static void TryDeleteDirectory(string sDirectory)
    {
        try
        {
            if (Directory.Exists(sDirectory))
                Directory.Delete(sDirectory, recursive: true);
        }
        catch
        {
        }
    }

    private static bool CanUsePipelinedExecution(RepositoryEndpoint source, RepositoryEndpoint target)
        => source.Type == RepoType.Svn && target.Type == RepoType.Git;
}
