using RepoMigrator.Core;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class RepoTypeRecentValueHistoryTests
{
    [TestMethod]
    public void AddValue_StoresValuesPerRepoType()
    {
        IReadOnlyList<ProviderRecentValues> lstHistory = [];

        lstHistory = ProviderRecentValueHistory.AddValue(lstHistory, "git", "git-path");
        lstHistory = ProviderRecentValueHistory.AddValue(lstHistory, "svn", "svn-path");

        CollectionAssert.AreEqual(new[] { "git-path" }, ProviderRecentValueHistory.GetValues(lstHistory, "git").ToList());
        CollectionAssert.AreEqual(new[] { "svn-path" }, ProviderRecentValueHistory.GetValues(lstHistory, "svn").ToList());
    }

    [TestMethod]
    public void AddValue_PreservesOtherRepoTypeHistories()
    {
        IReadOnlyList<ProviderRecentValues> lstHistory =
        [
            new ProviderRecentValues { ProviderKey = "git", Values = ["git-old"] },
            new ProviderRecentValues { ProviderKey = "svn", Values = ["svn-old"] }
        ];

        lstHistory = ProviderRecentValueHistory.AddValue(lstHistory, "git", "git-new");

        CollectionAssert.AreEqual(new[] { "git-new", "git-old" }, ProviderRecentValueHistory.GetValues(lstHistory, "git").ToList());
        CollectionAssert.AreEqual(new[] { "svn-old" }, ProviderRecentValueHistory.GetValues(lstHistory, "svn").ToList());
    }
}
