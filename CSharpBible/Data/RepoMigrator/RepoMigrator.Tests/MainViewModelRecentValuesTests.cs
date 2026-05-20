using System.Collections.ObjectModel;
using System.Reflection;
using System.Collections.Specialized;
using RepoMigrator.App.Logic.Services;
using RepoMigrator.App.State.Services;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.App.Wpf.ViewModels;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class MainViewModelRecentValuesTests
{
    [TestMethod]
    public void SyncRecentValues_DoesNotReplaceOrRemoveExistingItems_AppendsOnlyMissingItems()
    {
        var recentValues = new ObservableCollection<string>
        {
            "svn://typed/source",
            "svn://typed/target"
        };

        var method = typeof(MainViewModel).GetMethod("SyncRecentValues", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);

        method.Invoke(null, new object[]
        {
            recentValues,
            new[] { "svn://history/one", "svn://typed/source", "svn://history/two" }
        });

        CollectionAssert.AreEqual(
            new[]
            {
                "svn://typed/source",
                "svn://typed/target",
                "svn://history/one",
                "svn://history/two"
            },
            recentValues.ToList());
    }

    [TestMethod]
    public void SyncRecentValues_KeepsSelectedSourceUrl_AndDoesNotRaiseSourceUrlPropertyChanged()
    {
        var vm = CreateMainViewModel();
        vm.SourceType = RepoType.Svn;
        vm.SourceUrl = "svn://typed/source";

        var sourceUrlPropertyChangedCount = 0;
        vm.PropertyChanged += (_, e) =>
        {
            if (string.Equals(e.PropertyName, nameof(MainViewModel.SourceUrl), StringComparison.Ordinal))
                sourceUrlPropertyChangedCount++;
        };

        var collectionActions = new List<NotifyCollectionChangedAction>();
        vm.RecentSourceUrls.CollectionChanged += (_, e) =>
        {
            if (e is not null)
                collectionActions.Add(e.Action);
        };

        var method = typeof(MainViewModel).GetMethod("SyncRecentValues", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);

        method.Invoke(null, new object[]
        {
            vm.RecentSourceUrls,
            new[] { "svn://history/one", "svn://typed/source", "svn://history/two" }
        });

        Assert.AreEqual("svn://typed/source", vm.SourceUrl, "Der aktuell ausgewählte Eingabestring darf durch Listensync nicht verändert werden.");
        Assert.AreEqual(0, sourceUrlPropertyChangedCount, "Für SourceUrl darf beim reinen Lookup-Listensync kein PropertyChanged ausgelöst werden.");
        Assert.IsFalse(collectionActions.Contains(NotifyCollectionChangedAction.Reset), "Lookup-Listensync darf kein Reset auslösen.");
    }

    private static MainViewModel CreateMainViewModel()
    {
        var migration = new NoOpMigrationService();
        var migrationEndpointFactory = new MigrationEndpointFactory();
        var migrationQueryService = new MigrationQueryService();
        var recentPathHistoryService = new RecentPathHistoryService();
        var providerFactory = new ThrowingProviderFactory();
        var repositorySelectionService = new RepositorySelectionService(providerFactory);
        var inputStateStore = new AppInputStateStore();

        return new MainViewModel(
            migration,
            migrationEndpointFactory,
            migrationQueryService,
            recentPathHistoryService,
            repositorySelectionService,
            providerFactory,
            inputStateStore);
    }

    private sealed class NoOpMigrationService : IMigrationService
    {
        public bool IsRunning => false;

        public Task MigrateAsync(
            RepositoryEndpoint source,
            RepositoryEndpoint target,
            ChangeSetQuery query,
            MigrationOptions options,
            IMigrationProgress progress,
            CancellationToken ct)
            => Task.CompletedTask;
    }

    private sealed class ThrowingProviderFactory : IProviderFactory
    {
        public IVersionControlProvider Create(RepoType type)
            => throw new InvalidOperationException("Provider access is not expected in this test.");
    }
}
