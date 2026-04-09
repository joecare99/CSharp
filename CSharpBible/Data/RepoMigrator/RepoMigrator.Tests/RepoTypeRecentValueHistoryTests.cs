using RepoMigrator.Core;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class RepoTypeRecentValueHistoryTests
{
    [TestMethod]
    public void AddValue_StoresValuesPerRepoType()
    {
        IReadOnlyList<RepoTypeRecentValues> lstHistory = [];

        lstHistory = RepoTypeRecentValueHistory.AddValue(lstHistory, RepoType.Git, "git-path");
        lstHistory = RepoTypeRecentValueHistory.AddValue(lstHistory, RepoType.Svn, "svn-path");

        CollectionAssert.AreEqual(new[] { "git-path" }, RepoTypeRecentValueHistory.GetValues(lstHistory, RepoType.Git).ToList());
        CollectionAssert.AreEqual(new[] { "svn-path" }, RepoTypeRecentValueHistory.GetValues(lstHistory, RepoType.Svn).ToList());
    }

    [TestMethod]
    public void AddValue_PreservesOtherRepoTypeHistories()
    {
        IReadOnlyList<RepoTypeRecentValues> lstHistory =
        [
            new RepoTypeRecentValues { RepoType = RepoType.Git, Values = ["git-old"] },
            new RepoTypeRecentValues { RepoType = RepoType.Svn, Values = ["svn-old"] }
        ];

        lstHistory = RepoTypeRecentValueHistory.AddValue(lstHistory, RepoType.Git, "git-new");

        CollectionAssert.AreEqual(new[] { "git-new", "git-old" }, RepoTypeRecentValueHistory.GetValues(lstHistory, RepoType.Git).ToList());
        CollectionAssert.AreEqual(new[] { "svn-old" }, RepoTypeRecentValueHistory.GetValues(lstHistory, RepoType.Svn).ToList());
    }
}
