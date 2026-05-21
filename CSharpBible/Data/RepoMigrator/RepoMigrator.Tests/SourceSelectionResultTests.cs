using RepoMigrator.App.Logic.Models;
using RepoMigrator.Core;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class SourceSelectionResultTests
{
    [TestMethod]
    public void SourceSelectionResult_Defaults_AreInitialized()
    {
        var result = new SourceSelectionResult();

        Assert.IsNotNull(result.Capabilities);
        Assert.AreEqual(0, result.Branches.Count);
        Assert.AreEqual(0, result.Tags.Count);
        Assert.AreEqual(0, result.Revisions.Count);
        Assert.IsNull(result.DefaultBranch);
        Assert.IsNull(result.SuggestedFromRevisionId);
        Assert.IsNull(result.SuggestedToRevisionId);
    }

    [TestMethod]
    public void SourceSelectionResult_AssignedValues_ArePreserved()
    {
        var revisions = new[] { new RepositoryRevisionInfo { Id = "10", AuthorName = "alice", Message = "msg", Timestamp = DateTimeOffset.UtcNow } };

        var result = new SourceSelectionResult
        {
            Capabilities = new RepositoryCapabilities { SupportsBranchSelection = true, SupportsTagSelection = true, SupportsRevisionSelection = true },
            Branches = [new RepositoryReferenceInfo { Name = "main" }],
            Tags = [new RepositoryReferenceInfo { Name = "v1" }],
            Revisions = revisions,
            DefaultBranch = "main",
            SuggestedFromRevisionId = "1",
            SuggestedToRevisionId = "10"
        };

        Assert.IsTrue(result.Capabilities.SupportsBranchSelection);
        Assert.IsTrue(result.Capabilities.SupportsTagSelection);
        Assert.IsTrue(result.Capabilities.SupportsRevisionSelection);
        Assert.AreEqual(1, result.Branches.Count);
        Assert.AreEqual("main", result.Branches[0].Name);
        Assert.AreEqual(1, result.Tags.Count);
        Assert.AreEqual("v1", result.Tags[0].Name);
        Assert.AreEqual(1, result.Revisions.Count);
        Assert.AreEqual("10", result.Revisions[0].Id);
        Assert.AreEqual("main", result.DefaultBranch);
        Assert.AreEqual("1", result.SuggestedFromRevisionId);
        Assert.AreEqual("10", result.SuggestedToRevisionId);
    }
}
