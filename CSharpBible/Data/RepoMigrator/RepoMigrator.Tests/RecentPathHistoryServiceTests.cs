using RepoMigrator.App.Logic.Services;
using RepoMigrator.Core;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class RecentPathHistoryServiceTests
{
    [TestMethod]
    public void AddPath_AddsValueForSelectedRepoType()
    {
        var service = new RecentPathHistoryService();
        IReadOnlyList<ProviderRecentValues> entries = [];

        var updated = service.AddPath(entries, "git", "C:/repo-a");

        var gitValues = service.GetPaths(updated, "git");
        Assert.AreEqual(1, gitValues.Count);
        Assert.AreEqual("C:/repo-a", gitValues[0]);
    }

    [TestMethod]
    public void AddPath_DoesNotMixValuesAcrossRepoTypes()
    {
        var service = new RecentPathHistoryService();
        IReadOnlyList<ProviderRecentValues> entries = [];
        entries = service.AddPath(entries, "git", "C:/git-repo");

        var updated = service.AddPath(entries, "svn", "svn://example/repo");

        var gitValues = service.GetPaths(updated, "git");
        var svnValues = service.GetPaths(updated, "svn");

        CollectionAssert.AreEqual(new List<string> { "C:/git-repo" }, gitValues.ToList());
        CollectionAssert.AreEqual(new List<string> { "svn://example/repo" }, svnValues.ToList());
    }
}
