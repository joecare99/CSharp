using RepoMigrator.Core;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class SubdirectoryBranchProjectionPlannerTests
{
    [TestMethod]
    public void BuildPlans_DepthOne_UsesRootBranchAsBase()
    {
        var lstPlans = SubdirectoryBranchProjectionPlanner.BuildPlans(
            new[] { "README.md", "2001/test.txt", "2001/110209_Testprojekt/a.txt", "2002/other.txt" },
            "KG",
            1);

        var dctPlans = lstPlans.ToDictionary(plan => plan.BranchName, plan => plan.Paths, StringComparer.OrdinalIgnoreCase);
        CollectionAssert.AreEquivalent(new[] { "KG", "KG/2001", "KG/2002" }, dctPlans.Keys.ToArray());
        CollectionAssert.AreEquivalent(new[] { "README.md" }, dctPlans["KG"].ToArray());
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
        CollectionAssert.AreEquivalent(new[] { "KG", "KG/2001", "KG/2001/110209_Testprojekt" }, dctPlans.Keys.ToArray());
        CollectionAssert.AreEquivalent(new[] { "2001/110209_Testprojekt/a.txt" }, dctPlans["KG/2001/110209_Testprojekt"].ToArray());
    }
}
