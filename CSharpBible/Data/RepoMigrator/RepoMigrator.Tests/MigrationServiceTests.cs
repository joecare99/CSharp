using NSubstitute;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class MigrationServiceTests
{
    [TestMethod]
    public async Task MigrateAsync_UsesNativeTransfer_WhenRequested()
    {
        var providerFactory = Substitute.For<IProviderFactory>();
        var sourceProvider = Substitute.For<IVersionControlProvider>();
        var progress = Substitute.For<IMigrationProgress>();
        var service = new MigrationService(providerFactory);
        var sourceEndpoint = new RepositoryEndpoint { Type = RepoType.Git, UrlOrPath = @"C:\source" };
        var targetEndpoint = new RepositoryEndpoint { Type = RepoType.Git, UrlOrPath = @"C:\target" };
        var query = new ChangeSetQuery();
        var options = new MigrationOptions { TransferMode = RepositoryTransferMode.NativeHistory };

        providerFactory.Create(RepoType.Git).Returns(sourceProvider);
        sourceProvider.OpenAsync(sourceEndpoint, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        sourceProvider.TransferAsync(sourceEndpoint, targetEndpoint, options, progress, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        sourceProvider.DisposeAsync().Returns(ValueTask.CompletedTask);

        await service.MigrateAsync(sourceEndpoint, targetEndpoint, query, options, progress, CancellationToken.None);

        await sourceProvider.Received(1).OpenAsync(sourceEndpoint, Arg.Any<CancellationToken>());
        await sourceProvider.Received(1).TransferAsync(sourceEndpoint, targetEndpoint, options, progress, Arg.Any<CancellationToken>());
        await sourceProvider.DidNotReceive().GetChangeSetsAsync(Arg.Any<ChangeSetQuery>(), Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task MigrateAsync_ProjectsSnapshotsIntoConfiguredSubdirectoryBranches()
    {
        var providerFactory = Substitute.For<IProviderFactory>();
        var sourceProvider = Substitute.For<IVersionControlProvider>();
        var targetProvider = Substitute.For<IVersionControlProvider>();
        var progress = Substitute.For<IMigrationProgress>();
        var service = new MigrationService(providerFactory);
        var sourceEndpoint = new RepositoryEndpoint { Type = RepoType.Svn, UrlOrPath = "svn://source/repo" };
        var targetEndpoint = new RepositoryEndpoint { Type = RepoType.Git, UrlOrPath = @"C:\target", BranchOrTrunk = "KG" };
        var query = new ChangeSetQuery();
        var options = new MigrationOptions { ProjectionMode = MigrationProjectionMode.SubdirectoryBranches, BranchSplitDepth = 2 };
        var changeSet = new ChangeSetInfo
        {
            Id = "100",
            Message = "Import",
            AuthorName = "alice",
            Timestamp = new DateTimeOffset(2024, 1, 1, 8, 0, 0, TimeSpan.Zero)
        };

        providerFactory.Create(RepoType.Svn).Returns(sourceProvider);
        providerFactory.Create(RepoType.Git).Returns(targetProvider);
        sourceProvider.Name.Returns("SVN");
        targetProvider.Name.Returns("Git");
        sourceProvider.OpenAsync(sourceEndpoint, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        sourceProvider.GetChangeSetsAsync(query, Arg.Any<CancellationToken>()).Returns(Task.FromResult<IReadOnlyList<ChangeSetInfo>>([changeSet]));
        sourceProvider.MaterializeSnapshotAsync(Arg.Any<string>(), changeSet.Id, Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var sTempDirectory = callInfo.ArgAt<string>(0);
                Directory.CreateDirectory(Path.Combine(sTempDirectory, "2001", "110209_Testprojekt"));
                File.WriteAllText(Path.Combine(sTempDirectory, "README.md"), "root");
                File.WriteAllText(Path.Combine(sTempDirectory, "2001", "summary.txt"), "level-1");
                File.WriteAllText(Path.Combine(sTempDirectory, "2001", "110209_Testprojekt", "detail.txt"), "level-2");
                return Task.CompletedTask;
            });
        sourceProvider.DisposeAsync().Returns(ValueTask.CompletedTask);
        targetProvider.InitializeTargetAsync(targetEndpoint, true, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        targetProvider.CommitSnapshotAsync(Arg.Any<string>(), Arg.Any<CommitMetadata>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        targetProvider.FlushAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        targetProvider.DisposeAsync().Returns(ValueTask.CompletedTask);

        await service.MigrateAsync(sourceEndpoint, targetEndpoint, query, options, progress, CancellationToken.None);

        await targetProvider.Received(1).CommitSnapshotAsync(Arg.Any<string>(), Arg.Is<CommitMetadata>(metadata => metadata.TargetBranch == "KG"), Arg.Any<CancellationToken>());
        await targetProvider.Received(1).CommitSnapshotAsync(Arg.Any<string>(), Arg.Is<CommitMetadata>(metadata => metadata.TargetBranch == "KG/2001"), Arg.Any<CancellationToken>());
        await targetProvider.Received(1).CommitSnapshotAsync(Arg.Any<string>(), Arg.Is<CommitMetadata>(metadata => metadata.TargetBranch == "KG/2001/110209_Testprojekt"), Arg.Any<CancellationToken>());
        await targetProvider.Received(1).FlushAsync(Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task MigrateAsync_PipelinedExecution_ReportsDetailedDiagnostics()
    {
        var providerFactory = Substitute.For<IProviderFactory>();
        var enumerateProvider = Substitute.For<IVersionControlProvider>();
        var exportProvider = Substitute.For<IVersionControlProvider>();
        var targetProvider = Substitute.For<IVersionControlProvider>();
        var progress = Substitute.For<IMigrationProgress>();
        var service = new MigrationService(providerFactory);
        var sourceEndpoint = new RepositoryEndpoint { Type = RepoType.Svn, UrlOrPath = "svn://source/repo" };
        var targetEndpoint = new RepositoryEndpoint { Type = RepoType.Git, UrlOrPath = @"C:\target", BranchOrTrunk = "main" };
        var query = new ChangeSetQuery();
        var options = new MigrationOptions
        {
            ExecutionMode = MigrationExecutionMode.Pipelined,
            PipelinePrefetchCount = 2,
            PipelineExportWorkerCount = 1
        };
        var changeSet = new ChangeSetInfo
        {
            Id = "100",
            Message = "Import",
            AuthorName = "alice",
            Timestamp = new DateTimeOffset(2024, 1, 1, 8, 0, 0, TimeSpan.Zero)
        };

        providerFactory.Create(RepoType.Svn).Returns(enumerateProvider, exportProvider);
        providerFactory.Create(RepoType.Git).Returns(targetProvider);
        enumerateProvider.Name.Returns("SVN");
        exportProvider.Name.Returns("SVN");
        targetProvider.Name.Returns("Git");
        enumerateProvider.OpenAsync(sourceEndpoint, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        enumerateProvider.GetChangeSetsAsync(query, Arg.Any<CancellationToken>()).Returns(Task.FromResult<IReadOnlyList<ChangeSetInfo>>([changeSet]));
        enumerateProvider.DisposeAsync().Returns(ValueTask.CompletedTask);
        exportProvider.OpenAsync(sourceEndpoint, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        exportProvider.MaterializeSnapshotAsync(Arg.Any<string>(), changeSet.Id, Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var sTempDirectory = callInfo.ArgAt<string>(0);
                File.WriteAllText(Path.Combine(sTempDirectory, "README.md"), "root");
                return Task.CompletedTask;
            });
        exportProvider.DisposeAsync().Returns(ValueTask.CompletedTask);
        targetProvider.InitializeTargetAsync(targetEndpoint, true, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        targetProvider.CommitSnapshotAsync(Arg.Any<string>(), Arg.Any<CommitMetadata>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        targetProvider.FlushAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        targetProvider.DisposeAsync().Returns(ValueTask.CompletedTask);

        await service.MigrateAsync(sourceEndpoint, targetEndpoint, query, options, progress, CancellationToken.None);

        progress.Received().Report(
            MigrationReportSeverity.Information,
            MigrationReportMessage.PipelineEnabled,
            Arg.Is<object?[]>(arrAdditional => arrAdditional.Length == 3
                && Equals(arrAdditional[0], 1)
                && Equals(arrAdditional[1], 2)
                && Equals(arrAdditional[2], 1)));
        progress.Received().Report(
            MigrationReportSeverity.Information,
            MigrationReportMessage.PipelineExportQueued,
            Arg.Is<object?[]>(arrAdditional => arrAdditional.Length == 3
                && arrAdditional[0] as string == "100"
                && Equals(arrAdditional[1], 1)
                && Equals(arrAdditional[2], 1)));
        progress.Received().Report(
            MigrationReportSeverity.Information,
            MigrationReportMessage.ExportWorkerSnapshotExported,
            Arg.Is<object?[]>(arrAdditional => arrAdditional.Length == 2
                && Equals(arrAdditional[0], 1)
                && arrAdditional[1] as string == "100"));
        progress.Received().Report(
            MigrationReportSeverity.Information,
            MigrationReportMessage.PipelineSnapshotWaiting,
            Arg.Is<object?[]>(arrAdditional => arrAdditional.Length == 2
                && Equals(arrAdditional[0], 1)
                && Equals(arrAdditional[1], 1)));
        progress.Received().Report(
            MigrationReportSeverity.Information,
            MigrationReportMessage.FlushStarting,
            Arg.Is<object?[]>(arrAdditional => arrAdditional.Length == 1
                && arrAdditional[0] as string == "Git"));
        progress.Received().Report(
            MigrationReportSeverity.Information,
            MigrationReportMessage.FlushCompleted,
            Arg.Is<object?[]>(arrAdditional => arrAdditional.Length == 1
                && arrAdditional[0] as string == "Git"));
    }

    [TestMethod]
    public async Task MigrateAsync_PipelinedExecution_StartsCommitingBeforeLaterExportsFinish()
    {
        var providerFactory = Substitute.For<IProviderFactory>();
        var enumerateProvider = Substitute.For<IVersionControlProvider>();
        var exportProvider = Substitute.For<IVersionControlProvider>();
        var targetProvider = Substitute.For<IVersionControlProvider>();
        var progress = Substitute.For<IMigrationProgress>();
        var service = new MigrationService(providerFactory);
        var sourceEndpoint = new RepositoryEndpoint { Type = RepoType.Svn, UrlOrPath = "svn://source/repo" };
        var targetEndpoint = new RepositoryEndpoint { Type = RepoType.Git, UrlOrPath = @"C:\target", BranchOrTrunk = "main" };
        var query = new ChangeSetQuery();
        var options = new MigrationOptions
        {
            ExecutionMode = MigrationExecutionMode.Pipelined,
            PipelinePrefetchCount = 1,
            PipelineExportWorkerCount = 1
        };
        var lstChanges = new[]
        {
            new ChangeSetInfo { Id = "100", Message = "Import 100", AuthorName = "alice", Timestamp = new DateTimeOffset(2024, 1, 1, 8, 0, 0, TimeSpan.Zero) },
            new ChangeSetInfo { Id = "101", Message = "Import 101", AuthorName = "alice", Timestamp = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero) }
        };
        var tcsSecondExportCanFinish = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var tcsFirstCommitReached = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        providerFactory.Create(RepoType.Svn).Returns(enumerateProvider, exportProvider);
        providerFactory.Create(RepoType.Git).Returns(targetProvider);
        enumerateProvider.Name.Returns("SVN");
        exportProvider.Name.Returns("SVN");
        targetProvider.Name.Returns("Git");
        enumerateProvider.OpenAsync(sourceEndpoint, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        enumerateProvider.GetChangeSetsAsync(query, Arg.Any<CancellationToken>()).Returns(Task.FromResult<IReadOnlyList<ChangeSetInfo>>(lstChanges));
        enumerateProvider.DisposeAsync().Returns(ValueTask.CompletedTask);
        exportProvider.OpenAsync(sourceEndpoint, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        exportProvider.MaterializeSnapshotAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(async callInfo =>
            {
                var sTempDirectory = callInfo.ArgAt<string>(0);
                var sChangeSetId = callInfo.ArgAt<string>(1);
                File.WriteAllText(Path.Combine(sTempDirectory, $"{sChangeSetId}.txt"), sChangeSetId);

                if (string.Equals(sChangeSetId, "101", StringComparison.Ordinal))
                    await tcsSecondExportCanFinish.Task;
            });
        exportProvider.DisposeAsync().Returns(ValueTask.CompletedTask);
        targetProvider.InitializeTargetAsync(targetEndpoint, true, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        targetProvider.CommitSnapshotAsync(Arg.Any<string>(), Arg.Any<CommitMetadata>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var metadata = callInfo.ArgAt<CommitMetadata>(1);
                if (string.Equals(metadata.Message, "Import 100", StringComparison.Ordinal))
                    tcsFirstCommitReached.TrySetResult();

                return Task.CompletedTask;
            });
        targetProvider.FlushAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        targetProvider.DisposeAsync().Returns(ValueTask.CompletedTask);

        var migrationTask = service.MigrateAsync(sourceEndpoint, targetEndpoint, query, options, progress, CancellationToken.None);

        await tcsFirstCommitReached.Task.WaitAsync(TimeSpan.FromSeconds(2));

        tcsSecondExportCanFinish.TrySetResult();
        await migrationTask;

        await targetProvider.Received().CommitSnapshotAsync(Arg.Any<string>(), Arg.Is<CommitMetadata>(metadata => metadata.Message == "Import 100"), Arg.Any<CancellationToken>());
        await targetProvider.Received().CommitSnapshotAsync(Arg.Any<string>(), Arg.Is<CommitMetadata>(metadata => metadata.Message == "Import 101"), Arg.Any<CancellationToken>());
    }
}
