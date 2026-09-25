using System;
using System.Collections.Generic;

namespace Osb.Core.Statistics;

public enum LegacyAgeBucketKind
{
    UnknownAge,
    Newborn,
    OneToTwoYears,
    ThreeToTenYears,
    ElevenToTwentyYears,
    TwentyOneToThirtyYears,
    ThirtyOneToFortyYears,
    FortyOneToFiftyYears,
    FiftyOneToSixtyYears,
    SixtyOneToSeventyYears,
    MoreThanSeventyYears
}

public sealed record LegacyAgeBucketResult(
    int BucketIndex,
    LegacyAgeBucketKind Kind,
    int StartAge,
    int EndAge,
    bool IsUnknown)
{
    public string Label => Kind switch
    {
        LegacyAgeBucketKind.UnknownAge => "unbekannt",
        LegacyAgeBucketKind.Newborn => "0 Jahre",
        LegacyAgeBucketKind.OneToTwoYears => "1-2 Jahre",
        LegacyAgeBucketKind.ThreeToTenYears => "3-10 Jahre",
        LegacyAgeBucketKind.ElevenToTwentyYears => "11-20 Jahre",
        LegacyAgeBucketKind.TwentyOneToThirtyYears => "21-30 Jahre",
        LegacyAgeBucketKind.ThirtyOneToFortyYears => "31-40 Jahre",
        LegacyAgeBucketKind.FortyOneToFiftyYears => "41-50 Jahre",
        LegacyAgeBucketKind.FiftyOneToSixtyYears => "51-60 Jahre",
        LegacyAgeBucketKind.SixtyOneToSeventyYears => "61-70 Jahre",
        LegacyAgeBucketKind.MoreThanSeventyYears => "> 70 Jahre",
        _ => throw new ArgumentOutOfRangeException(nameof(Kind))
    };
}

public static class LegacyAgeStatistics
{
    public static LegacyAgeBucketResult ClassifyAge(int age)
    {
        if (age < -1)
        {
            throw new ArgumentOutOfRangeException(nameof(age), "Age must be -1 for unknown or a non-negative integer.");
        }

        return age switch
        {
            -1 => new LegacyAgeBucketResult(0, LegacyAgeBucketKind.UnknownAge, -1, -1, true),
            0 => new LegacyAgeBucketResult(1, LegacyAgeBucketKind.Newborn, 0, 0, false),
            >= 1 and <= 2 => new LegacyAgeBucketResult(2, LegacyAgeBucketKind.OneToTwoYears, 1, 2, false),
            >= 3 and <= 10 => new LegacyAgeBucketResult(3, LegacyAgeBucketKind.ThreeToTenYears, 3, 10, false),
            >= 11 and <= 20 => new LegacyAgeBucketResult(4, LegacyAgeBucketKind.ElevenToTwentyYears, 11, 20, false),
            >= 21 and <= 30 => new LegacyAgeBucketResult(5, LegacyAgeBucketKind.TwentyOneToThirtyYears, 21, 30, false),
            >= 31 and <= 40 => new LegacyAgeBucketResult(6, LegacyAgeBucketKind.ThirtyOneToFortyYears, 31, 40, false),
            >= 41 and <= 50 => new LegacyAgeBucketResult(7, LegacyAgeBucketKind.FortyOneToFiftyYears, 41, 50, false),
            >= 51 and <= 60 => new LegacyAgeBucketResult(8, LegacyAgeBucketKind.FiftyOneToSixtyYears, 51, 60, false),
            >= 61 and <= 70 => new LegacyAgeBucketResult(9, LegacyAgeBucketKind.SixtyOneToSeventyYears, 61, 70, false),
            > 70 => new LegacyAgeBucketResult(10, LegacyAgeBucketKind.MoreThanSeventyYears, 71, int.MaxValue, false),
            _ => throw new InvalidOperationException("Age could not be mapped to a supported legacy bucket.")
        };
    }

    public static IReadOnlyDictionary<int, LegacyAgeBucketResult> AnalyzeAges(IEnumerable<int> ages)
    {
        if (ages == null)
        {
            throw new ArgumentNullException(nameof(ages));
        }

        var result = new Dictionary<int, LegacyAgeBucketResult>();
        foreach (var age in ages)
        {
            var bucket = ClassifyAge(age);
            result[age] = bucket;
        }

        return result;
    }
}
