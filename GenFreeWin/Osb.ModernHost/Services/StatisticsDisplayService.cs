using System;
using System.Collections.Generic;
using System.Linq;
using Osb.Core.Statistics;
using Osb.ModernHost.Models;

namespace Osb.ModernHost.Services;

public sealed class StatisticsDisplayService
{
    private static readonly int[] SampleAges =
    [
        -1, 0, 1, 2, 5, 12, 18, 25, 35, 45, 55, 65, 80
    ];

    public IReadOnlyList<StatisticsBucketDisplayItem> CreateSampleBuckets()
    {
        var counts = SampleAges
            .Select(LegacyAgeStatistics.ClassifyAge)
            .GroupBy(bucket => bucket.BucketIndex)
            .ToDictionary(group => group.Key, group => group.Count());

        return Enumerable.Range(0, 11)
            .Select(index =>
            {
                var bucket = LegacyAgeStatistics.ClassifyAge(index switch
                {
                    0 => -1,
                    1 => 0,
                    2 => 1,
                    3 => 3,
                    4 => 11,
                    5 => 21,
                    6 => 31,
                    7 => 41,
                    8 => 51,
                    9 => 61,
                    _ => 71
                });
                return new StatisticsBucketDisplayItem(
                    bucket.Label,
                    counts.GetValueOrDefault(bucket.BucketIndex));
            })
            .ToArray();
    }
}
