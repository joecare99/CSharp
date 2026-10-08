using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathLibrary.Common;

namespace MathLibrary.Tests.Common;

[TestClass]
public class NumericComparisonsTests
{
    [TestMethod]
    [DataRow(null, null, 1f)]
    [DataRow(null, 0f, 0f)]
    [DataRow(0f, 0f, 1f)]
    [DataRow(1f, 1f, 1f)]
    [DataRow(-1f, -1f, 1f)]
    [DataRow(0.1f, 0.10001f, 0.9999f)]
    [DataRow(1f, -1.0001f, -0.9999f)]
    public void RelativeMatch_PreservesSimilaritySemantics(object? first, object? second, float expected)
    {
        Assert.AreEqual(expected, NumericComparisons.RelativeMatch(first, second), 1e-7f);
        Assert.AreEqual(expected, NumericComparisons.RelativeMatch(second, first), 1e-7f);
    }
}
