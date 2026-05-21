using RepoMigrator.Core;
using RepoMigrator.Providers.Archive;
using RepoMigrator.Providers.Archive.Services;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class FileSystemArchiveImportStateStoreTests
{
    [TestMethod]
    public async Task SaveAndLoadPlanAsync_WhenPlanIsPersisted_RoundTripsDeterministically()
    {
        var storageRootPath = CreateTempDirectory();
        try
        {
            var store = new FileSystemArchiveImportStateStore(storageRootPath);
            var plan = new ArchiveImportPlan
            {
                PlanId = "sample-plan",
                CreatedUtc = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero),
                Source = new ArchiveMigrationSourceDefinition
                {
                    Location = @"C:\archives",
                    AllowedExtensions = [".zip"]
                }.ToMigrationSourceDefinition(),
                Destination = new MigrationDestinationDefinition
                {
                    Kind = MigrationDestinationKind.Repository,
                    Repository = new RepositoryEndpoint { ProviderKey = "git", UrlOrPath = @"C:\target", BranchOrTrunk = "main" }
                },
                Items =
                [
                    new ArchiveImportPlanItem
                    {
                        SnapshotId = "release-1.0",
                        FinalOrderIndex = 0,
                        SourceItem = new MigrationSourcePlanItem { ItemId = "release-1.0.zip", SnapshotId = "release-1.0", SourceIdentifier = @"C:\archives\release-1.0.zip" },
                        FinalTagName = "release-1.0",
                        FinalBranchName = "releases/release-1.0",
                        CreateBranch = true,
                        ExtensionData = new Dictionary<string, string> { ["DriverId"] = "zip" }
                    }
                ],
                Status = ArchiveImportPlanStatus.Ready,
                SourceProviderData = new Dictionary<string, string> { ["Archive.Location"] = @"C:\archives" }
            };

            await store.SavePlanAsync(plan, CancellationToken.None);
            var loadedPlan = await store.LoadPlanAsync(plan.PlanId, CancellationToken.None);
            var planDirectoryPath = store.GetPlanDirectoryPath(plan.PlanId);

            Assert.AreEqual(Path.Combine(storageRootPath, "RepoMigrator", "ArchiveImports", "sample-plan"), planDirectoryPath);
            Assert.AreEqual(plan.PlanId, loadedPlan.PlanId);
            Assert.AreEqual(1, loadedPlan.Items.Count);
            Assert.AreEqual("zip", loadedPlan.Items[0].ExtensionData["DriverId"]);
            Assert.IsTrue(File.Exists(Path.Combine(planDirectoryPath, "plan.json")));
        }
        finally
        {
            Directory.Delete(storageRootPath, recursive: true);
        }
    }

    [TestMethod]
    public async Task SaveAndLoadStateAsync_WhenStateIsPersisted_RoundTripsDeterministically()
    {
        var storageRootPath = CreateTempDirectory();
        try
        {
            var store = new FileSystemArchiveImportStateStore(storageRootPath);
            var state = new ArchiveImportState
            {
                PlanId = "sample-plan",
                Status = ArchiveImportRunStatus.ReadyToResume,
                LastUpdatedUtc = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero),
                LastMachineName = "builder-01",
                LastWorkspaceRoot = @"C:\workspace",
                CurrentCheckpoint = new ArchiveImportCheckpoint
                {
                    Phase = ArchiveImportCheckpointPhase.TagCreated,
                    SnapshotId = "release-1.0",
                    ItemOrderIndex = 0,
                    Summary = "Tag created.",
                    RecordedAtUtc = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero)
                },
                Snapshots =
                [
                    new ArchiveImportSnapshotState
                    {
                        SnapshotId = "release-1.0",
                        Phase = ArchiveImportCheckpointPhase.TagCreated,
                        AcquisitionCompleted = true,
                        ExtractionCompleted = true,
                        CommitCompleted = true,
                        TagCreated = true,
                        BranchCreated = false,
                        TargetWriteId = "abc123"
                    }
                ]
            };

            await store.SaveStateAsync(state, CancellationToken.None);
            var loadedState = await store.LoadStateAsync(state.PlanId, CancellationToken.None);
            var planDirectoryPath = store.GetPlanDirectoryPath(state.PlanId);

            Assert.AreEqual(state.PlanId, loadedState.PlanId);
            Assert.AreEqual(ArchiveImportRunStatus.ReadyToResume, loadedState.Status);
            Assert.AreEqual("abc123", loadedState.Snapshots[0].TargetWriteId);
            Assert.IsTrue(File.Exists(Path.Combine(planDirectoryPath, "state.json")));
        }
        finally
        {
            Directory.Delete(storageRootPath, recursive: true);
        }
    }

    private static string CreateTempDirectory()
    {
        var tempDirectoryPath = Path.Combine(Path.GetTempPath(), $"RepoMigrator-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDirectoryPath);
        return tempDirectoryPath;
    }
}
