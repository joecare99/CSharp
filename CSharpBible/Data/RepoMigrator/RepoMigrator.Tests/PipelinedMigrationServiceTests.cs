using NSubstitute;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Tools.PipelinedMigration;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class PipelinedMigrationServiceTests
{
    [TestMethod]
    public async Task RunAsync_ReportsNoChangeSetsFound_AndReturnsEarly()
    {
        var providerFactory = Substitute.For<IProviderFactory>();
        var enumerateProvider = Substitute.For<IVersionControlProvider>();
        providerFactory.Create(RepoType.Svn).Returns(enumerateProvider);

        enumerateProvider.Name.Returns("SVN");
        enumerateProvider.OpenAsync(Arg.Any<RepositoryEndpoint>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        enumerateProvider.GetChangeSetsAsync(Arg.Any<ChangeSetQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ChangeSetInfo>>([]));
        enumerateProvider.DisposeAsync().Returns(ValueTask.CompletedTask);

        var progress = Substitute.For<IMigrationProgress>();
        var service = new PipelinedMigrationService(providerFactory);
        var options = new PipelinedMigrationOptions
        {
            SourceUrl = "svn://example/source",
            TargetUrl = "C:/git-target"
        };

        await service.RunAsync(options, progress, CancellationToken.None);

        progress.Received().Report(MigrationReportSeverity.Information, MigrationReportMessage.NoChangeSetsFound);
        providerFactory.DidNotReceive().Create(RepoType.Git);
    }

    [TestMethod]
    public async Task RunAsync_ProcessesSingleChangeSet_AndReportsCompleted()
    {
        var providerFactory = Substitute.For<IProviderFactory>();
        var enumerateProvider = Substitute.For<IVersionControlProvider>();
        var exportProvider = Substitute.For<IVersionControlProvider>();
        var targetProvider = Substitute.For<IVersionControlProvider>();

        providerFactory.Create(RepoType.Svn).Returns(enumerateProvider, exportProvider);
        providerFactory.Create(RepoType.Git).Returns(targetProvider);

        enumerateProvider.Name.Returns("SVN");
        enumerateProvider.OpenAsync(Arg.Any<RepositoryEndpoint>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        enumerateProvider.GetChangeSetsAsync(Arg.Any<ChangeSetQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ChangeSetInfo>>([
                new ChangeSetInfo
                {
                    Id = "100",
                    Message = "Import",
                    AuthorName = "alice",
                    Timestamp = new DateTimeOffset(2024, 1, 1, 8, 0, 0, TimeSpan.Zero)
                }
            ]));
        enumerateProvider.DisposeAsync().Returns(ValueTask.CompletedTask);

        exportProvider.OpenAsync(Arg.Any<RepositoryEndpoint>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        exportProvider.MaterializeSnapshotAsync(Arg.Any<string>(), "100", Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var sTempDirectory = callInfo.ArgAt<string>(0);
                Directory.CreateDirectory(sTempDirectory);
                File.WriteAllText(Path.Combine(sTempDirectory, "README.md"), "snapshot");
                return Task.CompletedTask;
            });
        exportProvider.DisposeAsync().Returns(ValueTask.CompletedTask);

        targetProvider.Name.Returns("Git");
        targetProvider.InitializeTargetAsync(Arg.Any<RepositoryEndpoint>(), true, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        targetProvider.CommitSnapshotAsync(Arg.Any<string>(), Arg.Any<CommitMetadata>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        targetProvider.DisposeAsync().Returns(ValueTask.CompletedTask);

        var progress = Substitute.For<IMigrationProgress>();
        var service = new PipelinedMigrationService(providerFactory);
        var options = new PipelinedMigrationOptions
        {
            SourceUrl = "svn://example/source",
            TargetUrl = "C:/git-target",
            PrefetchCount = 1,
            MaxExportWorkers = 1
        };

        await service.RunAsync(options, progress, CancellationToken.None);

        await targetProvider.Received(1).InitializeTargetAsync(Arg.Any<RepositoryEndpoint>(), true, Arg.Any<CancellationToken>());
        await targetProvider.Received(1).CommitSnapshotAsync(Arg.Any<string>(), Arg.Is<CommitMetadata>(m => m.Message == "Import"), Arg.Any<CancellationToken>());
        await targetProvider.DidNotReceive().FlushAsync(Arg.Any<CancellationToken>());
        progress.Received().Report(MigrationReportSeverity.Information, MigrationReportMessage.MigrationCompleted);
    }
}
