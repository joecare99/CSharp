using RepoMigrator.Core;
using RepoMigrator.Tools.PipelinedMigration;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class PipelineWorkItemTests
{
    [TestMethod]
    public void PipelineWorkItem_Defaults_AreInitialized()
    {
        var item = new PipelineWorkItem();

        Assert.AreEqual(0, item.Index);
        Assert.IsNotNull(item.ChangeSet);
        Assert.AreEqual(string.Empty, item.ChangeSet.Id);
        Assert.AreEqual(string.Empty, item.TempDirectory);
    }

    [TestMethod]
    public void PipelineWorkItem_AssignedValues_ArePreserved()
    {
        var timestamp = new DateTimeOffset(2025, 1, 10, 8, 30, 0, TimeSpan.Zero);
        var changeSet = new ChangeSetInfo
        {
            Id = "123",
            Message = "Snapshot",
            AuthorName = "alice",
            Timestamp = timestamp
        };

        var item = new PipelineWorkItem
        {
            Index = 3,
            ChangeSet = changeSet,
            TempDirectory = "C:/temp/snapshot-123"
        };

        Assert.AreEqual(3, item.Index);
        Assert.AreSame(changeSet, item.ChangeSet);
        Assert.AreEqual("C:/temp/snapshot-123", item.TempDirectory);
    }
}
