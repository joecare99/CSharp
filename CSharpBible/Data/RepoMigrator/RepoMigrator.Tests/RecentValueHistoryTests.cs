using RepoMigrator.Core;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class RecentValueHistoryTests
{
    [DataTestMethod]
    [DataRow("", "alpha|beta", "alpha|beta")]
    [DataRow("gamma", "alpha|beta", "gamma|alpha|beta")]
    [DataRow(" beta ", "alpha|beta|gamma", "beta|alpha|gamma")]
    public void AddValue_ReturnsExpectedOrdering(string sNewValue, string sExistingValues, string sExpectedValues)
    {
        var lstExistingValues = sExistingValues.Split('|', StringSplitOptions.RemoveEmptyEntries);
        var lstExpectedValues = sExpectedValues.Split('|', StringSplitOptions.RemoveEmptyEntries);

        var lstResolvedValues = RecentValueHistory.AddValue(lstExistingValues, sNewValue, 10);

        CollectionAssert.AreEqual(lstExpectedValues, lstResolvedValues.ToList());
    }

    [TestMethod]
    public void AddValue_LimitsResultToMaximumCount()
    {
        var lstExistingValues = Enumerable.Range(1, 10).Select(iValue => $"value-{iValue}").ToList();

        var lstResolvedValues = RecentValueHistory.AddValue(lstExistingValues, "value-11", 10);

        Assert.AreEqual(10, lstResolvedValues.Count);
        Assert.AreEqual("value-11", lstResolvedValues[0]);
        Assert.IsFalse(lstResolvedValues.Contains("value-10"));
    }
}
