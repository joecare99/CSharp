using RepoMigrator.App.Logic.Services;
using RepoMigrator.App.State.Services;
using RepoMigrator.App.Wpf;
using RepoMigrator.App.Wpf.ViewModels;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.Archive;
using RepoMigrator.Providers.Archive.Abstractions;
using RepoMigrator.Providers.Archive.Services;

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
        var archiveMigrationService = new ThrowingArchiveMigrationService();
        var migrationSourceProviderFactory = new ArchiveMigrationSourceProviderFactory(new DirectoryArchiveSnapshotSourceProvider(Path.GetTempPath()));

        var tempStatePath = Path.Combine(Path.GetTempPath(), "RepoMigratorTests", $"inputs_{Guid.NewGuid():N}.json");
        var inputStateStore = new AppInputStateStore();

        var vm = new MainViewModel(
            migration,
            migrationEndpointFactory,
            migrationQueryService,
            recentPathHistoryService,
            repositorySelectionService,
            providerFactory,
            inputStateStore,
            archiveMigrationService,
            migrationSourceProviderFactory)
        {
            SourceProviderKey = "svn",
            TargetProviderKey = "svn",
            SourceUrl = "svn://source/repo",
            TargetUrl = "svn://target/repo"
        };

        var startCommand = vm.StartCommand;
        await startCommand.ExecuteAsync(null);

        Assert.AreEqual("svn://source/repo", vm.SourceUrl);
        Assert.AreEqual("svn://target/repo", vm.TargetUrl);
    }

    [TestMethod]
    public async Task StartCommand_UsesArchiveWorkflow_ForLocalDirectorySourceAndGitTarget()
    {
        var migration = new ThrowingMigrationService();
        var migrationEndpointFactory = new MigrationEndpointFactory();
        var migrationQueryService = new MigrationQueryService();
        var recentPathHistoryService = new RecentPathHistoryService();
        var repositorySelectionService = new RepositorySelectionService(new ThrowingProviderFactory());
        var providerFactory = new ThrowingProviderFactory();
        var archiveMigrationService = new RecordingArchiveMigrationService();
        var inputStateStore = new AppInputStateStore();
        var migrationSourceProviderFactory = new ArchiveMigrationSourceProviderFactory(new DirectoryArchiveSnapshotSourceProvider(Path.GetTempPath()));
        var sourceDirectoryPath = Path.Combine(Path.GetTempPath(), "RepoMigratorTests", $"archive-source-{Guid.NewGuid():N}");
        Directory.CreateDirectory(sourceDirectoryPath);

        try
        {
            var vm = new MainViewModel(
                migration,
                migrationEndpointFactory,
                migrationQueryService,
                recentPathHistoryService,
                repositorySelectionService,
                providerFactory,
                inputStateStore,
                archiveMigrationService,
                migrationSourceProviderFactory)
            {
                SourceProviderKey = "archive",
                TargetProviderKey = "git",
                SourceUrl = sourceDirectoryPath,
                TargetUrl = @"C:\target\repo",
                TargetBranch = "main"
            };

            await vm.StartCommand.ExecuteAsync(null);

            Assert.AreEqual(1, archiveMigrationService.PrepareCalls);
            Assert.AreEqual(1, archiveMigrationService.ExecuteCalls);
            Assert.AreEqual(MigrationSourceKind.ArchiveCollection, archiveMigrationService.PreparedSource!.Kind);
            Assert.AreEqual(MigrationDestinationKind.Repository, archiveMigrationService.PreparedDestination!.Kind);
            Assert.AreEqual(sourceDirectoryPath, ArchiveMigrationSourceDefinition.FromMigrationSourceDefinition(archiveMigrationService.PreparedSource).Location);
            CollectionAssert.AreEquivalent(new[] { ".zip", ".tar.gz", ".tgz" }, ArchiveMigrationSourceDefinition.FromMigrationSourceDefinition(archiveMigrationService.PreparedSource).AllowedExtensions.ToArray());
            Assert.AreEqual(@"C:\target\repo", archiveMigrationService.PreparedDestination.Repository!.UrlOrPath);
            Assert.AreEqual("main", archiveMigrationService.PreparedDestination.Repository.BranchOrTrunk);
        }
        finally
        {
            if (Directory.Exists(sourceDirectoryPath))
                Directory.Delete(sourceDirectoryPath, recursive: true);
        }
    }

    [TestMethod]
    public async Task ContinueFromSetup_LoadsSvnSelectionDataAndAdvancesToOptions()
    {
        var sourceProvider = new RecordingVersionControlProvider
        {
            Capabilities = new RepositoryCapabilities { SupportsRevisionSelection = true },
            SelectionData = new RepositorySelectionData
            {
                Revisions =
                [
                    new RepositoryRevisionInfo { Id = "41", Message = "older", Timestamp = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero) },
                    new RepositoryRevisionInfo { Id = "42", Message = "newer", Timestamp = new DateTimeOffset(2024, 1, 2, 0, 0, 0, TimeSpan.Zero) }
                ],
                SuggestedFromRevisionId = "41"
            }
        };
        var providerFactory = new TestProviderFactory((providerKey, _createCount) =>
            string.Equals(providerKey, "svn", StringComparison.OrdinalIgnoreCase)
                ? sourceProvider
                : throw new InvalidOperationException($"Unexpected provider key '{providerKey}'."));

        var vm = CreateMainViewModel(providerFactory);
        vm.SourceProviderKey = "svn";
        vm.TargetProviderKey = "git";
        vm.SourceUrl = "svn://source/repo";
        vm.TargetUrl = @"C:\target\repo";

        await vm.ContinueFromSetupCommand.ExecuteAsync(null);

        Assert.AreEqual(WorkflowStage.Options, vm.WorkflowStage);
        Assert.AreEqual(2, vm.SvnRevisionSelections.Count);
        Assert.AreEqual("41", vm.SelectedSvnFromRevisionId);
    }

    [TestMethod]
    public async Task TestTargetCommand_WhenGitProbeSucceeds_LoadsTargetBranches()
    {
        var targetProvider = new RecordingVersionControlProvider
        {
            Capabilities = new RepositoryCapabilities { SupportsBranchSelection = true },
            ProbeResult = new RepositoryProbeResult
            {
                Success = true,
                Summary = "Git remote ok",
                Details = ["Branches loaded"]
            },
            SelectionData = new RepositorySelectionData
            {
                DefaultBranch = "main",
                Branches =
                [
                    new RepositoryReferenceInfo { Name = "release/1.0" },
                    new RepositoryReferenceInfo { Name = "main" }
                ]
            }
        };
        var providerFactory = new TestProviderFactory((providerKey, _createCount) =>
            string.Equals(providerKey, "git", StringComparison.OrdinalIgnoreCase)
                ? targetProvider
                : throw new InvalidOperationException($"Unexpected provider key '{providerKey}'."));

        var vm = CreateMainViewModel(providerFactory);
        vm.SourceProviderKey = "svn";
        vm.TargetProviderKey = "git";
        vm.SourceUrl = "svn://source/repo";
        vm.TargetUrl = @"C:\target\repo";

        await vm.TestTargetCommand.ExecuteAsync(null);

        CollectionAssert.AreEquivalent(new[] { "release/1.0", "main" }, vm.TargetGitBranchOptions.ToList());
        Assert.AreEqual("main", vm.TargetBranch);
        StringAssert.Contains(vm.Log, "Ziel erfolgreich getestet");
    }

    private static MainViewModel CreateMainViewModel(IProviderFactory providerFactory)
    {
        var migration = new ThrowingMigrationService();
        var migrationEndpointFactory = new MigrationEndpointFactory();
        var migrationQueryService = new MigrationQueryService();
        var recentPathHistoryService = new RecentPathHistoryService();
        var repositorySelectionService = new RepositorySelectionService(providerFactory);
        var inputStateStore = new AppInputStateStore();
        var archiveMigrationService = new ThrowingArchiveMigrationService();
        var migrationSourceProviderFactory = new ArchiveMigrationSourceProviderFactory(new DirectoryArchiveSnapshotSourceProvider(Path.GetTempPath()));

        return new MainViewModel(
            migration,
            migrationEndpointFactory,
            migrationQueryService,
            recentPathHistoryService,
            repositorySelectionService,
            providerFactory,
            inputStateStore,
            archiveMigrationService,
            migrationSourceProviderFactory);
    }

    [TestMethod]
    public async Task TestSourceCommand_ForArchiveSource_CountsTarGzAndTgzSnapshots()
    {
        var migration = new ThrowingMigrationService();
        var migrationEndpointFactory = new MigrationEndpointFactory();
        var migrationQueryService = new MigrationQueryService();
        var recentPathHistoryService = new RecentPathHistoryService();
        var repositorySelectionService = new RepositorySelectionService(new ThrowingProviderFactory());
        var providerFactory = new ThrowingProviderFactory();
        var archiveMigrationService = new RecordingArchiveMigrationService();
        var inputStateStore = new AppInputStateStore();
        var sourceDirectoryPath = Path.Combine(Path.GetTempPath(), "RepoMigratorTests", $"archive-source-{Guid.NewGuid():N}");
        Directory.CreateDirectory(sourceDirectoryPath);

        try
        {
            File.WriteAllText(Path.Combine(sourceDirectoryPath, "release-1.0.tar.gz"), "tar.gz");
            File.WriteAllText(Path.Combine(sourceDirectoryPath, "release-2.0.tgz"), "tgz");

            var migrationSourceProviderFactory = new ArchiveMigrationSourceProviderFactory(new DirectoryArchiveSnapshotSourceProvider(Path.GetTempPath()));
            var vm = new MainViewModel(
                migration,
                migrationEndpointFactory,
                migrationQueryService,
                recentPathHistoryService,
                repositorySelectionService,
                providerFactory,
                inputStateStore,
                archiveMigrationService,
                migrationSourceProviderFactory)
            {
                SourceProviderKey = "archive",
                TargetProviderKey = "git",
                SourceUrl = sourceDirectoryPath,
                TargetUrl = @"C:\target\repo"
            };

            await vm.TestSourceCommand.ExecuteAsync(null);

            StringAssert.Contains(vm.Log, "2 Archiv-Snapshot(s) erkannt.");
        }
        finally
        {
            if (Directory.Exists(sourceDirectoryPath))
                Directory.Delete(sourceDirectoryPath, recursive: true);
        }
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

    private sealed class ThrowingArchiveMigrationService : IArchiveMigrationService
    {
        public Task<ArchiveImportPlan> PreparePlanAsync(MigrationSourceDefinition source, MigrationDestinationDefinition destination, CancellationToken ct)
            => Task.FromException<ArchiveImportPlan>(new InvalidOperationException("Archive workflow access is not expected in this test."));

        public Task<ArchiveImportState> ExecuteAsync(string planId, IMigrationProgress progress, CancellationToken ct)
            => Task.FromException<ArchiveImportState>(new InvalidOperationException("Archive workflow access is not expected in this test."));

        public Task<ArchiveImportState> ResumeAsync(string planId, IMigrationProgress progress, CancellationToken ct)
            => Task.FromException<ArchiveImportState>(new InvalidOperationException("Archive workflow access is not expected in this test."));
    }

    private sealed class RecordingArchiveMigrationService : IArchiveMigrationService
    {
        public int PrepareCalls { get; private set; }
        public int ExecuteCalls { get; private set; }
        public MigrationSourceDefinition? PreparedSource { get; private set; }
        public MigrationDestinationDefinition? PreparedDestination { get; private set; }

        public Task<ArchiveImportPlan> PreparePlanAsync(MigrationSourceDefinition source, MigrationDestinationDefinition destination, CancellationToken ct)
        {
            PrepareCalls++;
            PreparedSource = source;
            PreparedDestination = destination;

            return Task.FromResult(new ArchiveImportPlan
            {
                PlanId = "plan-archive-test",
                Source = source,
                Destination = destination,
                Items = []
            });
        }

        public Task<ArchiveImportState> ExecuteAsync(string planId, IMigrationProgress progress, CancellationToken ct)
        {
            ExecuteCalls++;
            return Task.FromResult(new ArchiveImportState
            {
                PlanId = planId,
                Status = ArchiveImportRunStatus.Completed,
                CurrentCheckpoint = new ArchiveImportCheckpoint { Phase = ArchiveImportCheckpointPhase.RunCompleted }
            });
        }

        public Task<ArchiveImportState> ResumeAsync(string planId, IMigrationProgress progress, CancellationToken ct)
            => Task.FromResult(new ArchiveImportState
            {
                PlanId = planId,
                Status = ArchiveImportRunStatus.Completed,
                CurrentCheckpoint = new ArchiveImportCheckpoint { Phase = ArchiveImportCheckpointPhase.RunCompleted }
            });
    }

    private sealed class ThrowingProviderFactory : IProviderFactory
    {
        public IVersionControlProvider Create(string providerKey)
            => throw new InvalidOperationException("Provider access is not expected in this test.");
    }

    private sealed class TestProviderFactory(Func<string, int, IVersionControlProvider> providerFactory) : IProviderFactory
    {
        private readonly Dictionary<string, int> _createCounts = new(StringComparer.OrdinalIgnoreCase);

        public IVersionControlProvider Create(string providerKey)
        {
            _createCounts.TryGetValue(providerKey, out var count);
            count++;
            _createCounts[providerKey] = count;
            return providerFactory(providerKey, count);
        }
    }

    private sealed class RecordingVersionControlProvider : IVersionControlProvider
    {
        public string Name => "Test provider";
        public bool SupportsRead => true;
        public bool SupportsWrite => true;
        public RepositoryCapabilities Capabilities { get; init; } = new();
        public RepositorySelectionData SelectionData { get; init; } = new();
        public RepositoryProbeResult ProbeResult { get; init; } = new() { Success = true, Summary = "ok" };

        public Task OpenAsync(RepositoryEndpoint endpoint, CancellationToken ct) => Task.CompletedTask;

        public Task<RepositoryCapabilities> GetCapabilitiesAsync(RepositoryEndpoint endpoint, CancellationToken ct)
            => Task.FromResult(Capabilities);

        public Task<RepositorySelectionData> GetSelectionDataAsync(RepositoryEndpoint endpoint, CancellationToken ct)
            => Task.FromResult(SelectionData);

        public Task<RepositoryProbeResult> ProbeAsync(RepositoryEndpoint endpoint, RepositoryAccessMode accessMode, CancellationToken ct)
            => Task.FromResult(ProbeResult);

        public Task TransferAsync(RepositoryEndpoint source, RepositoryEndpoint target, MigrationOptions options, IMigrationProgress progress, CancellationToken ct)
            => Task.CompletedTask;

        public Task<IReadOnlyList<ChangeSetInfo>> GetChangeSetsAsync(ChangeSetQuery query, CancellationToken ct)
            => Task.FromResult<IReadOnlyList<ChangeSetInfo>>([]);

        public Task MaterializeSnapshotAsync(string workDir, string changeSetId, CancellationToken ct)
            => Task.CompletedTask;

        public Task InitializeTargetAsync(RepositoryEndpoint endpoint, bool emptyInit, CancellationToken ct)
            => Task.CompletedTask;

        public Task CommitSnapshotAsync(string workDir, CommitMetadata metadata, CancellationToken ct)
            => Task.CompletedTask;

        public Task CommitSnapshotAsync(string workDir, CommitMetadata metadata, IMigrationProgress progress, CancellationToken ct)
            => Task.CompletedTask;

        public Task FlushAsync(CancellationToken ct)
            => Task.CompletedTask;

        public ValueTask DisposeAsync()
            => ValueTask.CompletedTask;
    }
}
