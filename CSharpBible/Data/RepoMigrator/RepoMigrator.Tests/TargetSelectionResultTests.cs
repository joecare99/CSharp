using RepoMigrator.App.Logic.Models;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class TargetSelectionResultTests
{
    [TestMethod]
    public void TargetSelectionResult_Defaults_AreInitialized()
    {
        var result = new TargetSelectionResult();

        Assert.IsNotNull(result.Capabilities);
        Assert.AreEqual(0, result.Branches.Count);
        Assert.IsNull(result.DefaultBranch);
    }

    [TestMethod]
    public void TargetSelectionResult_AssignedValues_ArePreserved()
    {
        var result = new TargetSelectionResult
        {
            Capabilities = new RepoMigrator.Core.RepositoryCapabilities { SupportsBranchSelection = true },
            Branches = ["main", "release"],
            DefaultBranch = "main"
        };

        Assert.IsTrue(result.Capabilities.SupportsBranchSelection);
        CollectionAssert.AreEqual(new[] { "main", "release" }, result.Branches.ToArray());
        Assert.AreEqual("main", result.DefaultBranch);
    }
}
