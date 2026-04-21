using RepoMigrator.Core;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class SubdirectoryBranchProjectionPlannerTests
{
    [TestMethod]
    public void BuildPlans_DepthOne_UsesDedicatedRootContentBranch_WhenChildBranchesExist()
    {
        var lstPlans = SubdirectoryBranchProjectionPlanner.BuildPlans(
            new[] { "README.md", "2001/test.txt", "2001/110209_Testprojekt/a.txt", "2002/other.txt" },
            "KG",
            1);

        var dctPlans = lstPlans.ToDictionary(plan => plan.BranchName, plan => plan.Paths, StringComparer.OrdinalIgnoreCase);
        CollectionAssert.AreEquivalent(new[] { "KG/_root", "KG/2001", "KG/2002" }, dctPlans.Keys.ToArray());
        CollectionAssert.AreEquivalent(new[] { "README.md" }, dctPlans["KG/_root"].ToArray());
        CollectionAssert.AreEquivalent(new[] { "2001/test.txt", "2001/110209_Testprojekt/a.txt" }, dctPlans["KG/2001"].ToArray());
    }

    [TestMethod]
    public void BuildPlans_DepthTwo_AppendsTwoDirectoryLevels()
    {
        var lstPlans = SubdirectoryBranchProjectionPlanner.BuildPlans(
            new[] { "README.md", "2001/test.txt", "2001/110209_Testprojekt/a.txt" },
            "KG",
            2);

        var dctPlans = lstPlans.ToDictionary(plan => plan.BranchName, plan => plan.Paths, StringComparer.OrdinalIgnoreCase);
        CollectionAssert.AreEquivalent(new[] { "KG/_root", "KG/2001", "KG/2001/110209_Testprojekt" }, dctPlans.Keys.ToArray());
        CollectionAssert.AreEquivalent(new[] { "2001/110209_Testprojekt/a.txt" }, dctPlans["KG/2001/110209_Testprojekt"].ToArray());
    }

    [TestMethod]
    public void BuildPlans_UsesRootBranch_WhenOnlyRootFilesExist()
    {
        var lstPlans = SubdirectoryBranchProjectionPlanner.BuildPlans(
            new[] { "README.md", "CHANGELOG.md" },
            "KG",
            1);

        var dctPlans = lstPlans.ToDictionary(plan => plan.BranchName, plan => plan.Paths, StringComparer.OrdinalIgnoreCase);
        CollectionAssert.AreEquivalent(new[] { "KG" }, dctPlans.Keys.ToArray());
        CollectionAssert.AreEquivalent(new[] { "README.md", "CHANGELOG.md" }, dctPlans["KG"].ToArray());
    }
}
