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
        IReadOnlyList<RepoTypeRecentValues> entries = [];

        var updated = service.AddPath(entries, RepoType.Git, "C:/repo-a");

        var gitValues = service.GetPaths(updated, RepoType.Git);
        Assert.AreEqual(1, gitValues.Count);
        Assert.AreEqual("C:/repo-a", gitValues[0]);
    }

    [TestMethod]
    public void AddPath_DoesNotMixValuesAcrossRepoTypes()
    {
        var service = new RecentPathHistoryService();
        IReadOnlyList<RepoTypeRecentValues> entries = [];
        entries = service.AddPath(entries, RepoType.Git, "C:/git-repo");

        var updated = service.AddPath(entries, RepoType.Svn, "svn://example/repo");

        var gitValues = service.GetPaths(updated, RepoType.Git);
        var svnValues = service.GetPaths(updated, RepoType.Svn);

        CollectionAssert.AreEqual(new List<string> { "C:/git-repo" }, gitValues.ToList());
        CollectionAssert.AreEqual(new List<string> { "svn://example/repo" }, svnValues.ToList());
    }
}
