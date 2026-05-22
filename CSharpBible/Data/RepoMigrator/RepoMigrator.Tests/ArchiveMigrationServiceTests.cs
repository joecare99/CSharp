using NSubstitute;
using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;
using RepoMigrator.Providers.Archive;
using RepoMigrator.Providers.Archive.Abstractions;
using RepoMigrator.Providers.Archive.Services;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class ArchiveMigrationServiceTests
{
    [TestMethod]
    public async Task PreparePlanAsync_WhenPlannerReturnsPlan_PersistsPlanAndInitialState()
    {
        var planner = Substitute.For<IArchiveImportPlanner>();
        var stateStore = Substitute.For<IArchiveImportStateStore>();
        var service = CreateService(planner, stateStore, Substitute.For<IArchiveDriverRegistry>(), Substitute.For<IMigrationDestinationProviderFactory>());
        var source = new ArchiveMigrationSourceDefinition { Location = @"C:\archives", AllowedExtensions = [".zip"] }.ToMigrationSourceDefinition();
        var destination = CreateDestination();
        var plan = CreatePlan(createBranch: true);
        ArchiveImportState? savedInitialState = null;

        planner.PrepareAsync(source, destination, Arg.Any<CancellationToken>()).Returns(plan);
        stateStore.SaveStateAsync(Arg.Do<ArchiveImportState>(state => savedInitialState = state), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        var result = await service.PreparePlanAsync(source, destination, CancellationToken.None);

        Assert.AreSame(plan, result);
        await stateStore.Received(1).SavePlanAsync(plan, CancellationToken.None);
        Assert.IsNotNull(savedInitialState);
        Assert.AreEqual(plan.PlanId, savedInitialState.PlanId);
        Assert.AreEqual(ArchiveImportRunStatus.NotStarted, savedInitialState.Status);
        Assert.AreEqual(ArchiveImportCheckpointPhase.PlanPrepared, savedInitialState.CurrentCheckpoint.Phase);
        Assert.AreEqual(1, savedInitialState.Snapshots.Count);
    }

    [TestMethod]
    public async Task ExecuteAsync_WhenPlanStartsFresh_WritesSnapshotCreatesRefsAndCompletesRun()
    {
        var plan = CreatePlan(createBranch: true);
        var state = CreateState(plan, ArchiveImportRunStatus.NotStarted, ArchiveImportCheckpointPhase.PlanPrepared);
        var savedStates = new List<ArchiveImportState>();
        var stateStore = Substitute.For<IArchiveImportStateStore>();
        var driverRegistry = Substitute.For<IArchiveDriverRegistry>();
        var driver = Substitute.For<IArchiveDriver>();
        var destinationProvider = Substitute.For<IMigrationDestinationProvider, IMigrationDestinationRefOperations>();
        var refOperations = (IMigrationDestinationRefOperations)destinationProvider;
        var destinationProviderFactory = Substitute.For<IMigrationDestinationProviderFactory>();
        var service = CreateService(Substitute.For<IArchiveImportPlanner>(), stateStore, driverRegistry, destinationProviderFactory);

        stateStore.LoadPlanAsync(plan.PlanId, Arg.Any<CancellationToken>()).Returns(plan);
        stateStore.LoadStateAsync(plan.PlanId, Arg.Any<CancellationToken>()).Returns(state);
        stateStore.SaveStateAsync(Arg.Do<ArchiveImportState>(savedStates.Add), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        driverRegistry.Resolve(plan.Items[0].SourceItem.SourceIdentifier).Returns(driver);
        driver.ExtractToAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(callInfo =>
        {
            var extractionDirectoryPath = callInfo.ArgAt<string>(1);
            Directory.CreateDirectory(Path.Combine(extractionDirectoryPath, "product-root", "src"));
            File.WriteAllText(Path.Combine(extractionDirectoryPath, "product-root", "src", "README.md"), "content");
            return Task.CompletedTask;
        });
        destinationProviderFactory.Create(plan.Destination).Returns(destinationProvider);
        refOperations.GetHeadCommitIdAsync(Arg.Any<CancellationToken>()).Returns("commit-001");

        var result = await service.ExecuteAsync(plan.PlanId, NullMigrationProgress.Instance, CancellationToken.None);

        await destinationProvider.Received(1).InitializeAsync(plan.Destination, CancellationToken.None);
        await driver.Received(1).ExtractToAsync(plan.Items[0].SourceItem.SourceIdentifier, Arg.Any<string>(), CancellationToken.None);
        await destinationProvider.Received(1).WriteSnapshotAsync(
            Arg.Is<string>(path => path.EndsWith(Path.Combine("archive", "product-root", "src"), StringComparison.OrdinalIgnoreCase)),
            Arg.Is<MigrationDestinationCommit>(commit => commit.SnapshotId == plan.Items[0].SnapshotId
                && commit.Timestamp == DateTimeOffset.Parse("2024-01-01T00:00:00.0000000+00:00")),
            NullMigrationProgress.Instance,
            CancellationToken.None);
        await refOperations.Received(1).EnsureTagAsync(plan.Items[0].FinalTagName, "commit-001", CancellationToken.None);
        await refOperations.Received(1).EnsureBranchAsync(plan.Items[0].FinalBranchName!, "commit-001", CancellationToken.None);
        await destinationProvider.Received(1).FinalizeAsync(CancellationToken.None);
        Assert.AreEqual(ArchiveImportRunStatus.Completed, result.Status);
        Assert.AreEqual(ArchiveImportCheckpointPhase.RunCompleted, result.CurrentCheckpoint.Phase);
        Assert.AreEqual(ArchiveImportCheckpointPhase.SnapshotCompleted, result.Snapshots[0].Phase);
        Assert.IsTrue(result.Snapshots[0].CommitCompleted);
        Assert.IsTrue(result.Snapshots[0].TagCreated);
        Assert.IsTrue(result.Snapshots[0].BranchCreated);
        Assert.IsTrue(savedStates.Count >= 4);
    }

    [TestMethod]
    public async Task ResumeAsync_WhenCommitAlreadyCompleted_CreatesMissingTagAndBranchWithoutRewritingSnapshot()
    {
        var plan = CreatePlan(createBranch: true);
        var state = CreateState(
            plan,
            ArchiveImportRunStatus.ReadyToResume,
            ArchiveImportCheckpointPhase.CommitCompleted,
            new ArchiveImportSnapshotState
            {
                SnapshotId = plan.Items[0].SnapshotId,
                Phase = ArchiveImportCheckpointPhase.CommitCompleted,
                AcquisitionCompleted = true,
                ExtractionCompleted = true,
                CommitCompleted = true,
                TargetWriteId = "commit-002",
                TagCreated = false,
                BranchCreated = false
            });
        var stateStore = Substitute.For<IArchiveImportStateStore>();
        var driverRegistry = Substitute.For<IArchiveDriverRegistry>();
        var destinationProvider = Substitute.For<IMigrationDestinationProvider, IMigrationDestinationRefOperations>();
        var refOperations = (IMigrationDestinationRefOperations)destinationProvider;
        var destinationProviderFactory = Substitute.For<IMigrationDestinationProviderFactory>();
        var service = CreateService(Substitute.For<IArchiveImportPlanner>(), stateStore, driverRegistry, destinationProviderFactory);

        stateStore.LoadPlanAsync(plan.PlanId, Arg.Any<CancellationToken>()).Returns(plan);
        stateStore.LoadStateAsync(plan.PlanId, Arg.Any<CancellationToken>()).Returns(state);
        stateStore.SaveStateAsync(Arg.Any<ArchiveImportState>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        destinationProviderFactory.Create(plan.Destination).Returns(destinationProvider);

        var result = await service.ResumeAsync(plan.PlanId, NullMigrationProgress.Instance, CancellationToken.None);

        await destinationProvider.DidNotReceive().WriteSnapshotAsync(Arg.Any<string>(), Arg.Any<MigrationDestinationCommit>(), Arg.Any<IMigrationProgress>(), Arg.Any<CancellationToken>());
        await refOperations.Received(1).EnsureTagAsync(plan.Items[0].FinalTagName, "commit-002", CancellationToken.None);
        await refOperations.Received(1).EnsureBranchAsync(plan.Items[0].FinalBranchName!, "commit-002", CancellationToken.None);
        Assert.AreEqual(ArchiveImportRunStatus.Completed, result.Status);
        Assert.IsTrue(result.Snapshots[0].TagCreated);
        Assert.IsTrue(result.Snapshots[0].BranchCreated);
    }

    [TestMethod]
    public async Task ResumeAsync_WhenTagAlreadyCreated_OnlyCreatesMissingBranch()
    {
        var plan = CreatePlan(createBranch: true);
        var state = CreateState(
            plan,
            ArchiveImportRunStatus.ReadyToResume,
            ArchiveImportCheckpointPhase.TagCreated,
            new ArchiveImportSnapshotState
            {
                SnapshotId = plan.Items[0].SnapshotId,
                Phase = ArchiveImportCheckpointPhase.TagCreated,
                AcquisitionCompleted = true,
                ExtractionCompleted = true,
                CommitCompleted = true,
                TargetWriteId = "commit-003",
                TagCreated = true,
                BranchCreated = false
            });
        var stateStore = Substitute.For<IArchiveImportStateStore>();
        var driverRegistry = Substitute.For<IArchiveDriverRegistry>();
        var destinationProvider = Substitute.For<IMigrationDestinationProvider, IMigrationDestinationRefOperations>();
        var refOperations = (IMigrationDestinationRefOperations)destinationProvider;
        var destinationProviderFactory = Substitute.For<IMigrationDestinationProviderFactory>();
        var service = CreateService(Substitute.For<IArchiveImportPlanner>(), stateStore, driverRegistry, destinationProviderFactory);

        stateStore.LoadPlanAsync(plan.PlanId, Arg.Any<CancellationToken>()).Returns(plan);
        stateStore.LoadStateAsync(plan.PlanId, Arg.Any<CancellationToken>()).Returns(state);
        stateStore.SaveStateAsync(Arg.Any<ArchiveImportState>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        destinationProviderFactory.Create(plan.Destination).Returns(destinationProvider);

        var result = await service.ResumeAsync(plan.PlanId, NullMigrationProgress.Instance, CancellationToken.None);

        await destinationProvider.DidNotReceive().WriteSnapshotAsync(Arg.Any<string>(), Arg.Any<MigrationDestinationCommit>(), Arg.Any<IMigrationProgress>(), Arg.Any<CancellationToken>());
        await refOperations.DidNotReceive().EnsureTagAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await refOperations.Received(1).EnsureBranchAsync(plan.Items[0].FinalBranchName!, "commit-003", CancellationToken.None);
        Assert.AreEqual(ArchiveImportRunStatus.Completed, result.Status);
        Assert.IsTrue(result.Snapshots[0].TagCreated);
        Assert.IsTrue(result.Snapshots[0].BranchCreated);
    }

    [TestMethod]
    public async Task ExecuteAsync_WhenTagCreationDetectsDivergence_PersistsFailedStateAndThrows()
    {
        var plan = CreatePlan(createBranch: false);
        var state = CreateState(plan, ArchiveImportRunStatus.NotStarted, ArchiveImportCheckpointPhase.PlanPrepared);
        var savedStates = new List<ArchiveImportState>();
        var stateStore = Substitute.For<IArchiveImportStateStore>();
        var driverRegistry = Substitute.For<IArchiveDriverRegistry>();
        var driver = Substitute.For<IArchiveDriver>();
        var destinationProvider = Substitute.For<IMigrationDestinationProvider, IMigrationDestinationRefOperations>();
        var refOperations = (IMigrationDestinationRefOperations)destinationProvider;
        var destinationProviderFactory = Substitute.For<IMigrationDestinationProviderFactory>();
        var service = CreateService(Substitute.For<IArchiveImportPlanner>(), stateStore, driverRegistry, destinationProviderFactory);

        stateStore.LoadPlanAsync(plan.PlanId, Arg.Any<CancellationToken>()).Returns(plan);
        stateStore.LoadStateAsync(plan.PlanId, Arg.Any<CancellationToken>()).Returns(state);
        stateStore.SaveStateAsync(Arg.Do<ArchiveImportState>(savedStates.Add), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        driverRegistry.Resolve(plan.Items[0].SourceItem.SourceIdentifier).Returns(driver);
        driver.ExtractToAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        destinationProviderFactory.Create(plan.Destination).Returns(destinationProvider);
        refOperations.GetHeadCommitIdAsync(Arg.Any<CancellationToken>()).Returns("commit-004");
        refOperations.EnsureTagAsync(plan.Items[0].FinalTagName, "commit-004", Arg.Any<CancellationToken>()).Returns<Task>(_ => throw new InvalidOperationException("unsafe divergence"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ExecuteAsync(plan.PlanId, NullMigrationProgress.Instance, CancellationToken.None));

        Assert.AreEqual("unsafe divergence", exception.Message);
        Assert.IsTrue(savedStates.Count > 0);
        var failedState = savedStates[^1];
        Assert.AreEqual(ArchiveImportRunStatus.Failed, failedState.Status);
        Assert.AreEqual("unsafe divergence", failedState.LastValidationSummary);
        Assert.AreEqual("unsafe divergence", failedState.Snapshots[0].FailureSummary);
    }

    private static ArchiveMigrationService CreateService(
        IArchiveImportPlanner planner,
        IArchiveImportStateStore stateStore,
        IArchiveDriverRegistry driverRegistry,
        IMigrationDestinationProviderFactory destinationProviderFactory)
        => new(planner, stateStore, driverRegistry, destinationProviderFactory);

    private static MigrationDestinationDefinition CreateDestination()
        => new()
        {
            Kind = MigrationDestinationKind.Repository,
            Repository = new RepositoryEndpoint { ProviderKey = "git", UrlOrPath = @"C:\target", BranchOrTrunk = "main" }
        };

    private static ArchiveImportPlan CreatePlan(bool createBranch)
    {
        var source = new ArchiveMigrationSourceDefinition
        {
            LocationKind = ArchiveSourceLocationKind.LocalDirectory,
            Location = @"C:\archives",
            AllowedExtensions = [".zip"]
        }.ToMigrationSourceDefinition();
        var destination = CreateDestination();
        var branchName = createBranch ? "releases/release-1.0" : null;

        return new ArchiveImportPlan
        {
            PlanId = "plan-001",
            CreatedUtc = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero),
            Source = source,
            Destination = destination,
            Items =
            [
                new ArchiveImportPlanItem
                {
                    SnapshotId = "snapshot-001",
                    FinalOrderIndex = 0,
                    SourceItem = new MigrationSourcePlanItem
                    {
                        ItemId = "release-1.0.zip",
                        SnapshotId = "snapshot-001",
                        SourceIdentifier = @"C:\archives\release-1.0.zip",
                        DisplayName = "release-1.0.zip"
                    },
                    FinalTagName = "release-1.0",
                    FinalBranchName = branchName,
                    CreateBranch = createBranch,
                    ExtensionData = new Dictionary<string, string>
                    {
                        [ArchiveImportPlanItemExtensionKeys.DriverId] = "zip",
                        [ArchiveImportPlanItemExtensionKeys.ExtractionRootPath] = "product-root/src",
                        [ArchiveImportPlanItemExtensionKeys.CommitTimestamp] = "2024-01-01T00:00:00.0000000+00:00",
                        [ArchiveImportPlanItemExtensionKeys.CommitTimestampSource] = nameof(ArchiveSnapshotDescriptor.NewestEntryTimestamp)
                    }
                }
            ],
            Status = ArchiveImportPlanStatus.Ready,
            SourceProviderData = source.ProviderData,
            DestinationProviderData = destination.ProviderData
        };
    }

    private static ArchiveImportState CreateState(
        ArchiveImportPlan plan,
        ArchiveImportRunStatus status,
        ArchiveImportCheckpointPhase checkpointPhase,
        ArchiveImportSnapshotState? snapshotState = null)
        => new()
        {
            PlanId = plan.PlanId,
            Status = status,
            LastUpdatedUtc = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero),
            CurrentCheckpoint = new ArchiveImportCheckpoint
            {
                Phase = checkpointPhase,
                SnapshotId = snapshotState?.SnapshotId ?? string.Empty,
                ItemOrderIndex = snapshotState is null ? null : 0,
                Summary = checkpointPhase.ToString(),
                RecordedAtUtc = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero)
            },
            Snapshots = [snapshotState ?? new ArchiveImportSnapshotState { SnapshotId = plan.Items[0].SnapshotId, Phase = checkpointPhase }]
        };
}
