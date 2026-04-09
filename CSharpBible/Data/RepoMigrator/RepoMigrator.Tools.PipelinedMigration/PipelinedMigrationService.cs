using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using System.Threading.Channels;

namespace RepoMigrator.Tools.PipelinedMigration;

/// <summary>
/// Provides a prototype SVN-to-Git migration flow that pipelines snapshot export while preserving ordered commits.
/// </summary>
public sealed class PipelinedMigrationService
{
    private readonly IProviderFactory _providerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelinedMigrationService" /> class.
    /// </summary>
    /// <param name="providerFactory">The provider factory.</param>
    public PipelinedMigrationService(IProviderFactory providerFactory)
        => _providerFactory = providerFactory;

    /// <summary>
    /// Runs the pipelined SVN-to-Git migration.
    /// </summary>
    /// <param name="options">The migration options.</param>
    /// <param name="progress">The progress sink.</param>
    /// <param name="ct">The cancellation token.</param>
    public async Task RunAsync(PipelinedMigrationOptions options, IMigrationProgress progress, CancellationToken ct)
    {
        options.Validate();

        var sourceEndpoint = options.CreateSourceEndpoint();
        var targetEndpoint = options.CreateTargetEndpoint();
        var query = options.CreateQuery();

        await using var enumerateProvider = _providerFactory.Create(RepoType.Svn);
        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.SourceOpening, enumerateProvider.Name);
        await enumerateProvider.OpenAsync(sourceEndpoint, ct);

        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ChangeSetsLoading);
        var lstChanges = await enumerateProvider.GetChangeSetsAsync(query, ct);
        if (lstChanges.Count == 0)
        {
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.NoChangeSetsFound);
            return;
        }

        await using var targetProvider = _providerFactory.Create(RepoType.Git);
        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.PipelineEnabled, Math.Min(options.MaxExportWorkers, lstChanges.Count), Math.Max(options.PrefetchCount, 1), lstChanges.Count);
        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.TargetInitializing, targetProvider.Name);
        await targetProvider.InitializeTargetAsync(targetEndpoint, emptyInit: true, ct);

        using var ctsLinked = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var exportRequests = Channel.CreateBounded<(int Index, ChangeSetInfo ChangeSet)>(new BoundedChannelOptions(Math.Max(options.PrefetchCount, 1))
        {
            SingleWriter = true,
            SingleReader = false,
            FullMode = BoundedChannelFullMode.Wait
        });
        var exportedSnapshots = Channel.CreateBounded<PipelineWorkItem>(new BoundedChannelOptions(Math.Max(options.PrefetchCount, 1))
        {
            SingleWriter = false,
            SingleReader = true,
            FullMode = BoundedChannelFullMode.Wait
        });

        var iWorkerCount = Math.Min(options.MaxExportWorkers, lstChanges.Count);
        var arrExportTasks = Enumerable.Range(0, iWorkerCount)
            .Select(iWorkerIndex => ExportSnapshotsAsync(iWorkerIndex, options, sourceEndpoint, exportRequests.Reader, exportedSnapshots.Writer, progress, ctsLinked.Token))
            .ToArray();

        var completeExportChannelTask = CompleteExportChannelAsync(arrExportTasks, exportedSnapshots.Writer, ctsLinked.Token);

        try
        {
            for (var iIndex = 0; iIndex < lstChanges.Count; iIndex++)
                await exportRequests.Writer.WriteAsync((iIndex, lstChanges[iIndex]), ctsLinked.Token);

            exportRequests.Writer.Complete();

            await CommitSnapshotsInOrderAsync(targetProvider, exportedSnapshots.Reader, lstChanges.Count, progress, ctsLinked.Token);
            await completeExportChannelTask;

            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.MigrationCompleted);
        }
        catch
        {
            ctsLinked.Cancel();
            exportRequests.Writer.TryComplete();
            await DrainExportedSnapshotsAsync(exportedSnapshots.Reader);
            throw;
        }
    }

    private async Task ExportSnapshotsAsync(
        int iWorkerIndex,
        PipelinedMigrationOptions options,
        RepositoryEndpoint sourceEndpoint,
        ChannelReader<(int Index, ChangeSetInfo ChangeSet)> exportReader,
        ChannelWriter<PipelineWorkItem> snapshotWriter,
        IMigrationProgress progress,
        CancellationToken ct)
    {
        await using var sourceProvider = _providerFactory.Create(RepoType.Svn);
        await sourceProvider.OpenAsync(sourceEndpoint, ct);
        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ExportWorkerSourceOpened, iWorkerIndex + 1);

        await foreach (var exportRequest in exportReader.ReadAllAsync(ct))
        {
            var sTempDirectory = CreateTempDirectory(options.TempRoot);
            try
            {
                progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ExportWorkerSnapshotExporting, iWorkerIndex + 1, exportRequest.ChangeSet.Id);
                await sourceProvider.MaterializeSnapshotAsync(sTempDirectory, exportRequest.ChangeSet.Id, ct);
                progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ExportWorkerSnapshotExported, iWorkerIndex + 1, exportRequest.ChangeSet.Id);

                await snapshotWriter.WriteAsync(new PipelineWorkItem
                {
                    Index = exportRequest.Index,
                    ChangeSet = exportRequest.ChangeSet,
                    TempDirectory = sTempDirectory
                }, ct);
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

    private static async Task CompleteExportChannelAsync(Task[] arrExportTasks, ChannelWriter<PipelineWorkItem> snapshotWriter, CancellationToken ct)
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

    private static async Task CommitSnapshotsInOrderAsync(
        IVersionControlProvider targetProvider,
        ChannelReader<PipelineWorkItem> snapshotReader,
        int iExpectedItemCount,
        IMigrationProgress progress,
        CancellationToken ct)
    {
        var orderedBuffer = new OrderedWorkItemBuffer<PipelineWorkItem>();

        for (var iExpectedIndex = 0; iExpectedIndex < iExpectedItemCount; iExpectedIndex++)
        {
            PipelineWorkItem? workItem;
            while (!orderedBuffer.TryTakeNext(iExpectedIndex, out workItem))
            {
                var nextItem = await snapshotReader.ReadAsync(ct);
                if (nextItem.Index == iExpectedIndex)
                {
                    workItem = nextItem;
                    break;
                }

                orderedBuffer.Add(nextItem.Index, nextItem);
            }

            await CommitSnapshotAsync(targetProvider, workItem!, iExpectedIndex + 1, iExpectedItemCount, progress, ct);
        }

        foreach (var bufferedItem in orderedBuffer.GetBufferedItems().OfType<PipelineWorkItem>())
            TryDeleteDirectory(bufferedItem.TempDirectory);
    }

    private static async Task CommitSnapshotAsync(
        IVersionControlProvider targetProvider,
        PipelineWorkItem workItem,
        int iIndex,
        int iTotal,
        IMigrationProgress progress,
        CancellationToken ct)
    {
        progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ChangeSetProcessingStarting, workItem.ChangeSet.Id, iIndex, iTotal);

        var commitMetadata = new CommitMetadata
        {
            Message = string.IsNullOrWhiteSpace(workItem.ChangeSet.Message) ? $"Imported revision {workItem.ChangeSet.Id}" : workItem.ChangeSet.Message,
            AuthorName = workItem.ChangeSet.AuthorName,
            AuthorEmail = workItem.ChangeSet.AuthorEmail,
            Timestamp = workItem.ChangeSet.Timestamp
        };

        try
        {
            await targetProvider.CommitSnapshotAsync(workItem.TempDirectory, commitMetadata, ct);
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.CommitCompleted, workItem.ChangeSet.Id, iIndex, iTotal);
        }
        finally
        {
            TryDeleteDirectory(workItem.TempDirectory);
        }
    }

    private static async Task DrainExportedSnapshotsAsync(ChannelReader<PipelineWorkItem> snapshotReader)
    {
        while (await snapshotReader.WaitToReadAsync())
        {
            while (snapshotReader.TryRead(out var workItem))
                TryDeleteDirectory(workItem.TempDirectory);
        }
    }

    private static string CreateTempDirectory(string? sTempRoot)
    {
        var sRootDirectory = string.IsNullOrWhiteSpace(sTempRoot)
            ? Path.Combine(Path.GetTempPath(), "RepoMigrator", "pipeline")
            : Path.GetFullPath(sTempRoot);

        var sTempDirectory = Path.Combine(sRootDirectory, Guid.NewGuid().ToString("N"));
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
}
