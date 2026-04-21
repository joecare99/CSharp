using RepoMigrator.Tools.GitBranchSplitter;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class GitPathBranchSplitPlannerTests
{
    [TestMethod]
    public void BuildPlans_CreatesRootFirstLevelAndSecondLevelBranches()
    {
        var lstTrackedPaths = new[]
        {
            "README.md",
            "src/api/appsettings.json",
            "src/api/Program.cs",
            "src/ui/MainWindow.xaml",
            "docs/intro.md"
        };

        var lstPlans = GitPathBranchSplitPlanner.BuildPlans(lstTrackedPaths, "split");

        var dicPlans = lstPlans.ToDictionary(plan => plan.BranchName, plan => plan.Paths, StringComparer.OrdinalIgnoreCase);
        CollectionAssert.AreEquivalent(new[] { "split/_root", "split/docs", "split/src", "split/src/api", "split/src/ui" }, dicPlans.Keys.ToArray());
        CollectionAssert.AreEquivalent(new[] { "README.md" }, dicPlans["split/_root"].ToArray());
        CollectionAssert.AreEquivalent(new[] { "docs/intro.md" }, dicPlans["split/docs"].ToArray());
        CollectionAssert.AreEquivalent(new[] { "src/api/appsettings.json", "src/api/Program.cs", "src/ui/MainWindow.xaml" }, dicPlans["split/src"].ToArray());
        CollectionAssert.AreEquivalent(new[] { "src/api/appsettings.json", "src/api/Program.cs" }, dicPlans["split/src/api"].ToArray());
        CollectionAssert.AreEquivalent(new[] { "src/ui/MainWindow.xaml" }, dicPlans["split/src/ui"].ToArray());
    }

    [TestMethod]
    [DataRow("split prefix", "folder/file.txt", "split-prefix/folder")]
    [DataRow("split", "folder/sub folder/file.txt", "split/folder/sub-folder")]
    public void BuildPlans_SanitizesGeneratedBranchNames(string sPrefix, string sTrackedPath, string sExpectedBranch)
    {
        var lstPlans = GitPathBranchSplitPlanner.BuildPlans(new[] { sTrackedPath }, sPrefix);

        CollectionAssert.Contains(lstPlans.Select(plan => plan.BranchName).ToList(), sExpectedBranch);
    }
}
