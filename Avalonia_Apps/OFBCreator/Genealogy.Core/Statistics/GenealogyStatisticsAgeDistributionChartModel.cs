using System;
using System.Collections.Generic;
using System.Linq;

namespace Genealogy.Statistics;

/// <summary>
/// Contains normalized yearly age values for rendering a genealogy timeline chart.
/// </summary>
public sealed class GenealogyStatisticsAgeDistributionChartModel
{
    internal GenealogyStatisticsAgeDistributionChartModel(
        IReadOnlyList<GenealogyStatisticsAgeDistributionChartPoint> points,
        double maximumValue,
        IReadOnlyList<GenealogyStatisticsAxisTick> timeAxisTicks,
        IReadOnlyList<GenealogyStatisticsAxisTick> yAxisTicks,
        double timeAxisMaximum,
        double yAxisMaximum,
        int firstYear,
        int lastYear)
    {
        Points = points;
        MaximumValue = maximumValue;
        TimeAxisTicks = timeAxisTicks;
        YAxisTicks = yAxisTicks;
        TimeAxisMaximum = timeAxisMaximum;
        YAxisMaximum = yAxisMaximum;
        FirstYear = firstYear;
        LastYear = lastYear;
    }

    /// <summary>
    /// Gets the yearly chart points in chronological order.
    /// </summary>
    public IReadOnlyList<GenealogyStatisticsAgeDistributionChartPoint> Points { get; }

    /// <summary>
    /// Gets the maximum raw value across all displayed series.
    /// </summary>
    public double MaximumValue { get; }

    /// <summary>
    /// Gets the labeled timeline ticks.
    /// </summary>
    public IReadOnlyList<GenealogyStatisticsAxisTick> TimeAxisTicks { get; }

    /// <summary>
    /// Gets the labeled value-axis ticks.
    /// </summary>
    public IReadOnlyList<GenealogyStatisticsAxisTick> YAxisTicks { get; }

    /// <summary>
    /// Gets the rounded upper bound of the time axis.
    /// </summary>
    public double TimeAxisMaximum { get; }

    /// <summary>
    /// Gets the rounded upper bound of the value axis.
    /// </summary>
    public double YAxisMaximum { get; }

    /// <summary>
    /// Gets the first displayed year.
    /// </summary>
    public int FirstYear { get; }

    /// <summary>
    /// Gets the last displayed year.
    /// </summary>
    public int LastYear { get; }

    /// <summary>
    /// Creates a chart model from yearly age statistics.
    /// </summary>
    /// <param name="statistics">The calculated statistics.</param>
    /// <returns>A normalized timeline chart model.</returns>
    public static GenealogyStatisticsAgeDistributionChartModel Create(GenealogyStatisticsResult statistics)
    {
        ArgumentNullException.ThrowIfNull(statistics);
        var years = statistics.Years;
        var windowedYears = BuildWindowedYears(years);
        var maximumValue = windowedYears
            .Select(year => Math.Max(year.AgeYears, Math.Max(year.GenerationGapYears, year.DeathAgeYears)))
            .DefaultIfEmpty()
            .Max();
        var firstYear = years.Count == 0 ? 0 : years[0].Year;
        var lastYear = years.Count == 0 ? 0 : years[^1].Year;
        var timeMajorStep = SelectTimeMajorStep(lastYear - firstYear);
        var valueMajorStep = SelectValueMajorStep(maximumValue / 6.0);
        var yAxisMaximum = RoundAxisMaximum(maximumValue, valueMajorStep);

        return new GenealogyStatisticsAgeDistributionChartModel(
            windowedYears.Select(year => new GenealogyStatisticsAgeDistributionChartPoint(
                year.Year,
                Normalize(year.AgeYears, maximumValue),
                Normalize(year.GenerationGapYears, maximumValue),
                Normalize(year.DeathAgeYears, maximumValue),
                year.AgeYears,
                year.GenerationGapYears,
                year.DeathAgeYears)).ToArray(),
            maximumValue,
            CreateTimeTicks(firstYear, lastYear, timeMajorStep),
            CreateValueTicks(yAxisMaximum, valueMajorStep),
            RoundAxisMaximum(lastYear - firstYear <= 0 ? 1 : lastYear, timeMajorStep),
            yAxisMaximum,
            firstYear,
            lastYear);
    }

    private static IReadOnlyList<WindowedYearStatistics> BuildWindowedYears(IReadOnlyList<GenealogyYearStatistics> years)
    {
        if (years.Count == 0)
        {
            return Array.Empty<WindowedYearStatistics>();
        }

        var windowedYears = new WindowedYearStatistics[years.Count];
        for (var index = 0; index < years.Count; index++)
        {
            var radius = SelectAdaptiveWindowRadiusYears(years[index].LivingPeople);
            var windowStart = Math.Max(0, index - radius);
            var windowEnd = Math.Min(years.Count - 1, index + radius);
            var ageWeight = 0.0;
            var ageSum = 0.0;
            var deathAgeWeight = 0.0;
            var deathAgeSum = 0.0;
            var generationGapWeight = 0.0;
            var generationGapSum = 0.0;

            for (var cursor = windowStart; cursor <= windowEnd; cursor++)
            {
                var current = years[cursor];
                if (current.AgeObservationCount > 0)
                {
                    ageWeight += current.AgeObservationCount;
                    ageSum += current.AverageAgeYears * current.AgeObservationCount;
                }

                if (current.DeathAgeObservationCount > 0)
                {
                    deathAgeWeight += current.DeathAgeObservationCount;
                    deathAgeSum += current.AverageDeathAgeYears * current.DeathAgeObservationCount;
                }

                if (current.GenerationGapObservationCount > 0)
                {
                    generationGapWeight += current.GenerationGapObservationCount;
                    generationGapSum += current.AverageGenerationGapYears * current.GenerationGapObservationCount;
                }
            }

            windowedYears[index] = new WindowedYearStatistics(
                years[index].Year,
                ageWeight <= 0 ? 0 : ageSum / ageWeight,
                generationGapWeight <= 0 ? 0 : generationGapSum / generationGapWeight,
                deathAgeWeight <= 0 ? 0 : deathAgeSum / deathAgeWeight);
        }

        return windowedYears;
    }

    private static int SelectAdaptiveWindowRadiusYears(double livingPeople)
    {
        if (livingPeople <= 0)
        {
            return 20;
        }

        var radius = (int)Math.Round(20.0 / Math.Sqrt(livingPeople), MidpointRounding.AwayFromZero);
        return Math.Clamp(radius, 1, 20);
    }

    private static double RoundAxisMaximum(double maximum, double majorStep) =>
        maximum <= 0 ? majorStep : Math.Ceiling(maximum / majorStep) * majorStep;

    private static double SelectValueMajorStep(double target)
    {
        if (target <= 0)
        {
            return 1;
        }

        return new[] { 1.0, 2.5, 5.0 }
            .SelectMany(value => Enumerable.Range(-12, 25).Select(exponent => value * Math.Pow(10, exponent)))
            .Where(value => value <= target)
            .DefaultIfEmpty(1)
            .Max();
    }

    private static int SelectTimeMajorStep(int yearSpan)
    {
        var target = Math.Max(10, yearSpan / 10.0);
        var candidates = new[] { 10.0, 25.0, 50.0, 100.0 }
            .SelectMany(value => Enumerable.Range(-1, 5).Select(exponent => value * Math.Pow(10, exponent)));
        return Math.Max(1, (int)candidates
            .Where(value => value <= target)
            .DefaultIfEmpty(10)
            .Max());
    }

    private static IReadOnlyList<GenealogyStatisticsAxisTick> CreateTimeTicks(int firstYear, int lastYear, int majorStep)
    {
        if (firstYear == 0 || lastYear < firstYear)
        {
            return Array.Empty<GenealogyStatisticsAxisTick>();
        }

        var firstTick = (int)Math.Ceiling(firstYear / (double)majorStep) * majorStep;
        var ticks = new List<GenealogyStatisticsAxisTick>();
        for (var year = firstTick; year <= lastYear; year += majorStep)
        {
            ticks.Add(new GenealogyStatisticsAxisTick(year, year.ToString(), true, year % 100 == 0));
            if (year > int.MaxValue - majorStep)
            {
                break;
            }
        }

        return ticks;
    }

    private static IReadOnlyList<GenealogyStatisticsAxisTick> CreateValueTicks(double maximum, double majorStep)
    {
        if (maximum <= 0)
        {
            return [new GenealogyStatisticsAxisTick(0, "0", true, false)];
        }

        var ticks = new List<GenealogyStatisticsAxisTick>();
        var minorStep = majorStep / 5.0;
        var lastTick = Math.Ceiling(maximum / minorStep) * minorStep;
        for (var value = 0.0; value <= lastTick + minorStep * 0.000001; value += minorStep)
        {
            var isMajor = Math.Abs(value / majorStep - Math.Round(value / majorStep)) < 0.000001;
            ticks.Add(new GenealogyStatisticsAxisTick(value, isMajor ? value.ToString("0.##") : string.Empty, isMajor, false));
            if (value > double.MaxValue - minorStep)
            {
                break;
            }
        }

        return ticks;
    }

    private static double Normalize(double value, double maximum) => maximum <= 0 ? 0 : value / maximum;

    private readonly struct WindowedYearStatistics
    {
        public WindowedYearStatistics(int year, double ageYears, double generationGapYears, double deathAgeYears)
        {
            Year = year;
            AgeYears = ageYears;
            GenerationGapYears = generationGapYears;
            DeathAgeYears = deathAgeYears;
        }

        public int Year { get; }

        public double AgeYears { get; }

        public double GenerationGapYears { get; }

        public double DeathAgeYears { get; }
    }
}

/// <summary>
/// Represents one yearly data point in the normalized chart coordinate space.
/// </summary>
public sealed class GenealogyStatisticsAgeDistributionChartPoint
{
    internal GenealogyStatisticsAgeDistributionChartPoint(int year, double age, double generationGap, double deathAge, double rawAge, double rawGenerationGap, double rawDeathAge)
    {
        Year = year;
        Age = age;
        GenerationGap = generationGap;
        DeathAge = deathAge;
        RawAge = rawAge;
        RawGenerationGap = rawGenerationGap;
        RawDeathAge = rawDeathAge;
    }

    /// <summary>Gets the calendar year on the horizontal axis.</summary>
    public int Year { get; }

    /// <summary>Gets the normalized yearly age value.</summary>
    public double Age { get; }

    /// <summary>Gets the normalized yearly generation-gap value.</summary>
    public double GenerationGap { get; }

    /// <summary>Gets the normalized yearly death-age value.</summary>
    public double DeathAge { get; }

    /// <summary>Gets the raw yearly age value.</summary>
    public double RawAge { get; }

    /// <summary>Gets the raw yearly generation-gap value.</summary>
    public double RawGenerationGap { get; }

    /// <summary>Gets the raw yearly death-age value.</summary>
    public double RawDeathAge { get; }
}
