using RepoMigrator.App.Logic.Services;
using RepoMigrator.App.State.Services;
using RepoMigrator.App.Wpf.ViewModels;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class MainViewModelStartWorkflowTests
{
    [TestMethod]
    public async Task StartCommand_DoesNotMutateEndpointUrls_WhenMigrationFailsBeforeRecentCapture()
    {
        var migration = new ThrowingMigrationService();
        var migrationEndpointFactory = new MigrationEndpointFactory();
        var migrationQueryService = new MigrationQueryService();
        var recentPathHistoryService = new RecentPathHistoryService();
        var repositorySelectionService = new RepositorySelectionService(new ThrowingProviderFactory());
        var providerFactory = new ThrowingProviderFactory();

        var tempStatePath = Path.Combine(Path.GetTempPath(), "RepoMigratorTests", $"inputs_{Guid.NewGuid():N}.json");
        var inputStateStore = new AppInputStateStore();

        var vm = new MainViewModel(
            migration,
            migrationEndpointFactory,
            migrationQueryService,
            recentPathHistoryService,
            repositorySelectionService,
            providerFactory,
            inputStateStore)
        {
            SourceType = RepoType.Svn,
            TargetType = RepoType.Svn,
            SourceUrl = "svn://source/repo",
            TargetUrl = "svn://target/repo"
        };

        var startCommand = vm.StartCommand;
        await startCommand.ExecuteAsync(null);

        Assert.AreEqual("svn://source/repo", vm.SourceUrl);
        Assert.AreEqual("svn://target/repo", vm.TargetUrl);
    }

    private sealed class ThrowingMigrationService : IMigrationService
    {
        public bool IsRunning => false;

        public Task MigrateAsync(
            RepositoryEndpoint source,
            RepositoryEndpoint target,
            ChangeSetQuery query,
            MigrationOptions options,
            IMigrationProgress progress,
            CancellationToken ct)
            => Task.FromException(new InvalidOperationException("expected test failure"));
    }

    private sealed class ThrowingProviderFactory : IProviderFactory
    {
        public IVersionControlProvider Create(RepoType type)
            => throw new InvalidOperationException("Provider access is not expected in this test.");
    }
}
