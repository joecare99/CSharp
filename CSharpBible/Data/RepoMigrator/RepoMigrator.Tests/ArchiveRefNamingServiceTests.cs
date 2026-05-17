using RepoMigrator.Providers.Archive;
using RepoMigrator.Providers.Archive.Services;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class ArchiveRefNamingServiceTests
{
    [TestMethod]
    public void CreateTagName_WhenArchiveUsesSimpleExtension_RemovesExtension()
    {
        var service = new ArchiveRefNamingService();
        var snapshot = new ArchiveSnapshotDescriptor
        {
            SnapshotId = "snapshot-1",
            ArchivePathOrUrl = @"C:\archives\release-1.0.zip",
            ArchiveFileName = "release-1.0.zip"
        };

        var tagName = service.CreateTagName(snapshot);

        Assert.AreEqual("release-1.0", tagName);
    }

    [TestMethod]
    public void CreateTagName_WhenArchiveUsesCompoundExtension_RemovesCompoundExtension()
    {
        var service = new ArchiveRefNamingService();
        var snapshot = new ArchiveSnapshotDescriptor
        {
            SnapshotId = "snapshot-1",
            ArchivePathOrUrl = @"C:\archives\release-1.0.tar.gz",
            ArchiveFileName = "release-1.0.tar.gz"
        };

        var tagName = service.CreateTagName(snapshot, new ArchiveRefNamingOptions { TagPrefix = "tags/" });

        Assert.AreEqual("tags/release-1.0", tagName);
    }

    [TestMethod]
    public void CreateBranchName_WhenBranchCreationEnabled_ReturnsPrefixedBranchName()
    {
        var service = new ArchiveRefNamingService();
        var snapshot = new ArchiveSnapshotDescriptor
        {
            SnapshotId = "snapshot-1",
            ArchivePathOrUrl = @"C:\archives\release-2.0.zip",
            ArchiveFileName = "release-2.0.zip"
        };

        var branchName = service.CreateBranchName(snapshot, new ArchiveBranchOptions { CreateBranches = true, BranchPrefix = "releases/" });

        Assert.AreEqual("releases/release-2.0", branchName);
    }

    [TestMethod]
    public void CreateBranchName_WhenBranchCreationDisabled_ReturnsNull()
    {
        var service = new ArchiveRefNamingService();
        var snapshot = new ArchiveSnapshotDescriptor
        {
            SnapshotId = "snapshot-1",
            ArchivePathOrUrl = @"C:\archives\release-2.0.zip",
            ArchiveFileName = "release-2.0.zip"
        };

        var branchName = service.CreateBranchName(snapshot, new ArchiveBranchOptions { CreateBranches = false });

        Assert.IsNull(branchName);
    }
}
