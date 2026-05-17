using RepoMigrator.Core;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class ArchiveImportModelsTests
{
    [TestMethod]
    public void ArchiveImportPlan_Defaults_AreInitialized()
    {
        var plan = new ArchiveImportPlan();

        Assert.AreEqual(ArchiveImportManifestVersion.Current, plan.SchemaVersion);
        Assert.AreEqual(string.Empty, plan.PlanId);
        Assert.AreEqual(default, plan.CreatedUtc);
        Assert.IsNotNull(plan.Source);
        Assert.IsNotNull(plan.Destination);
        Assert.AreEqual(0, plan.Items.Count);
        Assert.AreEqual(ArchiveImportPlanStatus.Draft, plan.Status);
        Assert.IsNull(plan.Notes);
        Assert.AreEqual(0, plan.SourceProviderData.Count);
        Assert.AreEqual(0, plan.DestinationProviderData.Count);
    }

    [TestMethod]
    public void ArchiveImportPlanItem_Defaults_AreInitialized()
    {
        var item = new ArchiveImportPlanItem();

        Assert.AreEqual(string.Empty, item.SnapshotId);
        Assert.AreEqual(0, item.FinalOrderIndex);
        Assert.IsNotNull(item.SourceItem);
        Assert.AreEqual(string.Empty, item.FinalTagName);
        Assert.IsNull(item.FinalBranchName);
        Assert.IsFalse(item.CreateBranch);
        Assert.AreEqual(0, item.ExtensionData.Count);
    }

    [TestMethod]
    public void ArchiveImportState_Defaults_AreInitialized()
    {
        var state = new ArchiveImportState();

        Assert.AreEqual(ArchiveImportManifestVersion.Current, state.SchemaVersion);
        Assert.AreEqual(string.Empty, state.PlanId);
        Assert.AreEqual(ArchiveImportRunStatus.NotStarted, state.Status);
        Assert.AreEqual(default, state.LastUpdatedUtc);
        Assert.IsNull(state.LastMachineName);
        Assert.IsNull(state.LastWorkspaceRoot);
        Assert.IsNotNull(state.CurrentCheckpoint);
        Assert.AreEqual(ArchiveImportCheckpointPhase.None, state.CurrentCheckpoint.Phase);
        Assert.AreEqual(0, state.Snapshots.Count);
        Assert.IsNull(state.LastValidationSummary);
        Assert.AreEqual(0, state.ExtensionData.Count);
    }

    [TestMethod]
    public void ArchiveImportSnapshotState_Defaults_AreInitialized()
    {
        var state = new ArchiveImportSnapshotState();

        Assert.AreEqual(string.Empty, state.SnapshotId);
        Assert.AreEqual(ArchiveImportCheckpointPhase.None, state.Phase);
        Assert.IsFalse(state.AcquisitionCompleted);
        Assert.IsFalse(state.ExtractionCompleted);
        Assert.IsFalse(state.CommitCompleted);
        Assert.IsNull(state.TargetWriteId);
        Assert.IsFalse(state.TagCreated);
        Assert.IsFalse(state.BranchCreated);
        Assert.IsNull(state.FailureSummary);
        Assert.IsNull(state.LastAttemptUtc);
        Assert.AreEqual(0, state.ExtensionData.Count);
    }

    [TestMethod]
    public void ArchiveImportCheckpoint_Defaults_AreInitialized()
    {
        var checkpoint = new ArchiveImportCheckpoint();

        Assert.AreEqual(ArchiveImportCheckpointPhase.None, checkpoint.Phase);
        Assert.AreEqual(string.Empty, checkpoint.SnapshotId);
        Assert.IsNull(checkpoint.ItemOrderIndex);
        Assert.IsNull(checkpoint.Summary);
        Assert.AreEqual(default, checkpoint.RecordedAtUtc);
    }

    [TestMethod]
    public void ArchiveImportModels_AssignedValues_ArePreserved()
    {
        var source = new MigrationSourceDefinition
        {
            Kind = MigrationSourceKind.ArchiveCollection,
            ArchiveSource = new ArchiveMigrationSourceDefinition
            {
                Location = @"C:\archives",
                AllowedExtensions = [".zip"]
            }
        };
        var destination = new MigrationDestinationDefinition
        {
            Kind = MigrationDestinationKind.Repository,
            Repository = new RepositoryEndpoint { Type = RepoType.Git, UrlOrPath = @"C:\target", BranchOrTrunk = "main" }
        };
        var sourceItem = new MigrationSourcePlanItem
        {
            ItemId = "item-001",
            SnapshotId = "snapshot-001",
            SourceIdentifier = @"C:\archives\product-1.0.0.zip",
            DisplayName = "Product 1.0.0"
        };
        var planItem = new ArchiveImportPlanItem
        {
            SnapshotId = "snapshot-001",
            FinalOrderIndex = 1,
            SourceItem = sourceItem,
            FinalTagName = "Product_1.0.0",
            FinalBranchName = "releases/Product_1.0.0",
            CreateBranch = true,
            ExtensionData = new Dictionary<string, string> { ["DriverId"] = "zip" }
        };
        var checkpoint = new ArchiveImportCheckpoint
        {
            Phase = ArchiveImportCheckpointPhase.TagCreated,
            SnapshotId = "snapshot-001",
            ItemOrderIndex = 1,
            Summary = "Tag created.",
            RecordedAtUtc = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero)
        };
        var snapshotState = new ArchiveImportSnapshotState
        {
            SnapshotId = "snapshot-001",
            Phase = ArchiveImportCheckpointPhase.TagCreated,
            AcquisitionCompleted = true,
            ExtractionCompleted = true,
            CommitCompleted = true,
            TargetWriteId = "abc123",
            TagCreated = true,
            BranchCreated = false,
            FailureSummary = null,
            LastAttemptUtc = checkpoint.RecordedAtUtc,
            ExtensionData = new Dictionary<string, string> { ["TargetBranch"] = "main" }
        };
        var plan = new ArchiveImportPlan
        {
            PlanId = "plan-001",
            CreatedUtc = checkpoint.RecordedAtUtc,
            Source = source,
            Destination = destination,
            Items = [planItem],
            Status = ArchiveImportPlanStatus.Ready,
            Notes = "Ready to run.",
            SourceProviderData = new Dictionary<string, string> { ["LocationKind"] = "LocalDirectory" },
            DestinationProviderData = new Dictionary<string, string> { ["TargetKind"] = "Repository" }
        };
        var state = new ArchiveImportState
        {
            PlanId = plan.PlanId,
            Status = ArchiveImportRunStatus.ReadyToResume,
            LastUpdatedUtc = checkpoint.RecordedAtUtc,
            LastMachineName = "builder-01",
            LastWorkspaceRoot = @"C:\workspace",
            CurrentCheckpoint = checkpoint,
            Snapshots = [snapshotState],
            LastValidationSummary = "Validated.",
            ExtensionData = new Dictionary<string, string> { ["ResumeMode"] = "Safe" }
        };

        Assert.AreEqual(MigrationSourceKind.ArchiveCollection, plan.Source.Kind);
        Assert.AreEqual(MigrationDestinationKind.Repository, plan.Destination.Kind);
        Assert.AreEqual(1, plan.Items.Count);
        Assert.AreEqual("zip", plan.Items[0].ExtensionData["DriverId"]);
        Assert.AreEqual(ArchiveImportPlanStatus.Ready, plan.Status);
        Assert.AreEqual(ArchiveImportRunStatus.ReadyToResume, state.Status);
        Assert.AreEqual(ArchiveImportCheckpointPhase.TagCreated, state.CurrentCheckpoint.Phase);
        Assert.AreEqual("abc123", state.Snapshots[0].TargetWriteId);
        Assert.AreEqual("main", state.Snapshots[0].ExtensionData["TargetBranch"]);
        Assert.AreEqual("Safe", state.ExtensionData["ResumeMode"]);
    }
}
