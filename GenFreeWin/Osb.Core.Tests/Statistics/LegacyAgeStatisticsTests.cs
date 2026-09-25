using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.Core.Statistics;

namespace Osb.Core.Tests.Statistics;

[TestClass]
public sealed class LegacyAgeStatisticsTests
{
    [TestMethod]
    [DataRow(-1, 0, "UnknownAge", "unbekannt")]
    [DataRow(0, 1, "Newborn", "0 Jahre")]
    [DataRow(1, 2, "OneToTwoYears", "1-2 Jahre")]
    [DataRow(3, 3, "ThreeToTenYears", "3-10 Jahre")]
    [DataRow(11, 4, "ElevenToTwentyYears", "11-20 Jahre")]
    [DataRow(21, 5, "TwentyOneToThirtyYears", "21-30 Jahre")]
    [DataRow(31, 6, "ThirtyOneToFortyYears", "31-40 Jahre")]
    [DataRow(41, 7, "FortyOneToFiftyYears", "41-50 Jahre")]
    [DataRow(51, 8, "FiftyOneToSixtyYears", "51-60 Jahre")]
    [DataRow(61, 9, "SixtyOneToSeventyYears", "61-70 Jahre")]
    [DataRow(71, 10, "MoreThanSeventyYears", "> 70 Jahre")]
    public void ClassifyAge_MapsLegacyBucketRanges(int age, int expectedBucket, string expectedKind, string expectedLabel)
    {
        var result = LegacyAgeStatistics.ClassifyAge(age);

        Assert.AreEqual(expectedBucket, result.BucketIndex);
        Assert.AreEqual(expectedKind, result.Kind.ToString());
        Assert.AreEqual(expectedLabel, result.Label);
    }

    [TestMethod]
    public void ClassifyAge_RejectsInvalidNegativeValuesBelowMinusOne()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => LegacyAgeStatistics.ClassifyAge(-2));
    }

    [TestMethod]
    public void AnalyzeAges_UsesDistinctAgeBucketsForEachValue()
    {
        var actual = LegacyAgeStatistics.AnalyzeAges(new[] { -1, 0, 1, 12, 21, 31, 41, 51, 61, 85 });

        Assert.AreEqual(10, actual.Count);
        Assert.AreEqual(LegacyAgeBucketKind.UnknownAge, actual[-1].Kind);
        Assert.AreEqual(LegacyAgeBucketKind.Newborn, actual[0].Kind);
        Assert.AreEqual(LegacyAgeBucketKind.ElevenToTwentyYears, actual[12].Kind);
        Assert.AreEqual(LegacyAgeBucketKind.MoreThanSeventyYears, actual[85].Kind);
    }
}
