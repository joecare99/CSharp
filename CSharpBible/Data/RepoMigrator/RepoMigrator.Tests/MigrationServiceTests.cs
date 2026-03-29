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
}
