using RepoMigrator.Core;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class RepositoryCoreModelsTests
{
    [TestMethod]
    public void RepositoryReferenceInfo_Defaults_AreInitialized()
    {
        var info = new RepositoryReferenceInfo();

        Assert.AreEqual(string.Empty, info.Name);
        Assert.IsNull(info.CommitId);
    }

    [TestMethod]
    public void RepositorySelectionData_DefaultCollections_AreInitialized()
    {
        var data = new RepositorySelectionData();

        Assert.IsNull(data.DefaultBranch);
        Assert.IsNull(data.SuggestedFromRevisionId);
        Assert.IsNull(data.SuggestedToRevisionId);
        Assert.AreEqual(0, data.Branches.Count);
        Assert.AreEqual(0, data.Tags.Count);
        Assert.AreEqual(0, data.Revisions.Count);
    }

    [TestMethod]
    public void RepositoryProbeResult_Defaults_AreInitialized()
    {
        var result = new RepositoryProbeResult();

        Assert.IsFalse(result.Success);
        Assert.AreEqual(string.Empty, result.Summary);
        Assert.AreEqual(0, result.Details.Count);
    }

    [TestMethod]
    public void RepoCredentials_AssignedValues_ArePreserved()
    {
        var credentials = new RepoCredentials
        {
            Username = "alice",
            Password = "secret",
            PrivateKeyPath = "/keys/id_rsa",
            PrivateKeyPassphrase = "passphrase"
        };

        Assert.AreEqual("alice", credentials.Username);
        Assert.AreEqual("secret", credentials.Password);
        Assert.AreEqual("/keys/id_rsa", credentials.PrivateKeyPath);
        Assert.AreEqual("passphrase", credentials.PrivateKeyPassphrase);
    }

    [TestMethod]
    public void MigrationOptions_Defaults_AreInitialized()
    {
        var options = new MigrationOptions();

        Assert.AreEqual(RepositoryTransferMode.LinearSnapshots, options.TransferMode);
        Assert.AreEqual(MigrationExecutionMode.Sequential, options.ExecutionMode);
        Assert.AreEqual(MigrationProjectionMode.None, options.ProjectionMode);
        Assert.AreEqual(1, options.BranchSplitDepth);
        Assert.AreEqual(3, options.PipelinePrefetchCount);
        Assert.AreEqual(2, options.PipelineExportWorkerCount);
        Assert.IsFalse(options.TransferBranches);
        Assert.IsFalse(options.TransferTags);
        Assert.AreEqual(0, options.SelectedBranches.Count);
        Assert.AreEqual(0, options.SelectedTags.Count);
    }
}
