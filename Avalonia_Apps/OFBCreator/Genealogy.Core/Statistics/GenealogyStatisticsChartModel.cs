using System;
using System.Collections.Generic;
using System.Linq;

namespace Genealogy.Statistics;

/// <summary>
/// Contains normalized annual values for rendering a genealogy statistics chart.
/// </summary>
public sealed class GenealogyStatisticsChartModel
{
    private const double BaseEventSmoothingSigmaYears = 30.0;
    private const double EventSmoothingPopulationExponent = 0.6;
    private const double GaussianSupportSigma = 4.0;

    internal GenealogyStatisticsChartModel(
        IReadOnlyList<GenealogyStatisticsChartPoint> points,
        double maximumLivingPeople,
        double maximumEvents,
        double populationAxisMaximum,
        double eventAxisMaximum,
        IReadOnlyList<GenealogyStatisticsAxisTick> timeAxisTicks,
        IReadOnlyList<GenealogyStatisticsAxisTick> populationAxisTicks,
        IReadOnlyList<GenealogyStatisticsAxisTick> eventAxisTicks,
        int timeMajorStepYears,
        double populationMajorStep,
        double eventMajorStep,
        int firstYear,
        int lastYear)
    {
        Points = points;
        MaximumLivingPeople = maximumLivingPeople;
        MaximumEvents = maximumEvents;
        PopulationAxisMaximum = populationAxisMaximum;
        EventAxisMaximum = eventAxisMaximum;
        TimeAxisTicks = timeAxisTicks;
        PopulationAxisTicks = populationAxisTicks;
        EventAxisTicks = eventAxisTicks;
        TimeMajorStepYears = timeMajorStepYears;
        PopulationMajorStep = populationMajorStep;
        EventMajorStep = eventMajorStep;
        FirstYear = firstYear;
        LastYear = lastYear;
    }

    /// <summary>Gets the annual chart points in chronological order.</summary>
    public IReadOnlyList<GenealogyStatisticsChartPoint> Points { get; }

    /// <summary>Gets the primary-axis maximum for living people.</summary>
    public double MaximumLivingPeople { get; }

    /// <summary>Gets the secondary-axis maximum used to normalize births and deaths.</summary>
    public double MaximumEvents { get; }

    /// <summary>Gets the rounded upper bound of the primary population axis.</summary>
    public double PopulationAxisMaximum { get; }

    /// <summary>Gets the rounded upper bound of the secondary event axis.</summary>
    public double EventAxisMaximum { get; }

    /// <summary>Gets the labeled year-axis ticks.</summary>
    public IReadOnlyList<GenealogyStatisticsAxisTick> TimeAxisTicks { get; }

    /// <summary>Gets the labeled primary population-axis ticks.</summary>
    public IReadOnlyList<GenealogyStatisticsAxisTick> PopulationAxisTicks { get; }

    /// <summary>Gets the labeled secondary event-axis ticks.</summary>
    public IReadOnlyList<GenealogyStatisticsAxisTick> EventAxisTicks { get; }

    /// <summary>Gets the major horizontal grid interval in years.</summary>
    public int TimeMajorStepYears { get; }

    /// <summary>Gets the major population grid interval.</summary>
    public double PopulationMajorStep { get; }

    /// <summary>Gets the major event grid interval.</summary>
    public double EventMajorStep { get; }

    /// <summary>Gets the first displayed year.</summary>
    public int FirstYear { get; }

    /// <summary>Gets the last displayed year.</summary>
    public int LastYear { get; }

    /// <summary>
    /// Creates a chart model using a primary living-population axis and a shared normalized event axis.
    /// </summary>
    /// <param name="statistics">The yearly statistics to project.</param>
    /// <returns>A normalized chart model.</returns>
    public static GenealogyStatisticsChartModel Create(GenealogyStatisticsResult statistics)
    {
        ArgumentNullException.ThrowIfNull(statistics);
        var years = statistics.Years;
        var maximumLiving = years.Select(year => year.LivingPeople).DefaultIfEmpty().Max();
        var smoothedBirths = SmoothEvents(years, static year => year.Births);
        var smoothedDeaths = SmoothEvents(years, static year => year.Deaths);
        var smoothedFamilyFoundings = SmoothEvents(years, static year => year.FamilyFoundings);
        var maximumEvents = smoothedBirths
            .Concat(smoothedDeaths)
            .Concat(smoothedFamilyFoundings)
            .DefaultIfEmpty()
            .Max();
        var points = new GenealogyStatisticsChartPoint[years.Count];
        for (var index = 0; index < years.Count; index++)
        {
            points[index] = new GenealogyStatisticsChartPoint(
                years[index].Year,
                Normalize(years[index].LivingPeople, maximumLiving),
                Normalize(smoothedBirths[index], maximumEvents),
                Normalize(smoothedDeaths[index], maximumEvents),
                Normalize(smoothedFamilyFoundings[index], maximumEvents));
        }

        var firstYear = years.Count == 0 ? 0 : years[0].Year;
        var lastYear = years.Count == 0 ? 0 : years[^1].Year;
        var timeMajorStep = SelectTimeMajorStep(lastYear - firstYear);
        var populationMajorStep = SelectValueMajorStep(maximumLiving / 5.0);
        var eventMajorStep = SelectValueMajorStep(maximumEvents / 5.0);
        var populationAxisMaximum = RoundAxisMaximum(maximumLiving, populationMajorStep);
        var eventAxisMaximum = RoundAxisMaximum(maximumEvents, eventMajorStep);

        return new GenealogyStatisticsChartModel(
            points,
            maximumLiving,
            maximumEvents,
            populationAxisMaximum,
            eventAxisMaximum,
            CreateTimeTicks(firstYear, lastYear, timeMajorStep),
            CreateValueTicks(populationAxisMaximum, populationMajorStep),
            CreateValueTicks(eventAxisMaximum, eventMajorStep),
            timeMajorStep,
            populationMajorStep,
            eventMajorStep,
            firstYear,
            lastYear);
    }

    private static double RoundAxisMaximum(double maximum, double majorStep) =>
        maximum <= 0 ? majorStep : Math.Ceiling(maximum / majorStep) * majorStep;

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

    private static double SelectValueMajorStep(double target)
    {
        if (target <= 0)
            return 1;

        return new[] { 1.0, 2.5, 5.0 }
            .SelectMany(value => Enumerable.Range(-12, 25).Select(exponent => value * Math.Pow(10, exponent)))
            .Where(value => value <= target)
            .DefaultIfEmpty(1)
            .Max();
    }

    private static IReadOnlyList<GenealogyStatisticsAxisTick> CreateTimeTicks(int firstYear, int lastYear, int majorStep)
    {
        if (firstYear == 0 || lastYear < firstYear)
            return Array.Empty<GenealogyStatisticsAxisTick>();

        var firstTick = (int)Math.Ceiling(firstYear / (double)majorStep) * majorStep;
        var ticks = new List<GenealogyStatisticsAxisTick>();
        for (var year = firstTick; year <= lastYear; year += majorStep)
        {
            ticks.Add(new GenealogyStatisticsAxisTick(year, year.ToString(), true, year % 100 == 0));
            if (year > int.MaxValue - majorStep)
                break;
        }
        return ticks;
    }

    private static IReadOnlyList<GenealogyStatisticsAxisTick> CreateValueTicks(double maximum, double majorStep)
    {
        if (maximum <= 0)
            return [new GenealogyStatisticsAxisTick(0, "0", true, false)];

        var ticks = new List<GenealogyStatisticsAxisTick>();
        var minorStep = majorStep / 5.0;
        var lastTick = Math.Ceiling(maximum / minorStep) * minorStep;
        for (var value = 0.0; value <= lastTick + minorStep * 0.000001; value += minorStep)
        {
            var isMajor = Math.Abs(value / majorStep - Math.Round(value / majorStep)) < 0.000001;
            ticks.Add(new GenealogyStatisticsAxisTick(value, isMajor ? value.ToString("0.##") : string.Empty, isMajor, false));
            if (value > double.MaxValue - minorStep)
                break;
        }
        return ticks;
    }

    /// <summary>
    /// Distributes each annual event count with a population-sensitive Gaussian kernel.
    /// </summary>
    /// <param name="years">The original annual statistics.</param>
    /// <param name="useBirths"><see langword="true"/> to smooth births; otherwise, smooth deaths.</param>
    /// <returns>Smoothed event counts whose total is conserved within the chart interval.</returns>
    private static double[] SmoothEvents(IReadOnlyList<GenealogyYearStatistics> years, Func<GenealogyYearStatistics, double> eventSelector)
    {
        var smoothed = new double[years.Count];
        for (var sourceIndex = 0; sourceIndex < years.Count; sourceIndex++)
        {
            var sourceYear = years[sourceIndex];
            var eventCount = eventSelector(sourceYear);
            if (eventCount <= 0)
            {
                continue;
            }

            var localPopulation = Math.Max(
                sourceYear.LivingPeople,
                Math.Max(sourceYear.FamilyFoundings, Math.Max(sourceYear.Births, sourceYear.Deaths)));
            var sigma = BaseEventSmoothingSigmaYears *
                Math.Pow(Math.Max(1.0, localPopulation), -EventSmoothingPopulationExponent);
            var supportYears = (int)Math.Ceiling(GaussianSupportSigma * sigma);
            var firstTarget = Math.Max(0, sourceIndex - supportYears);
            var lastTarget = Math.Min(years.Count - 1, sourceIndex + supportYears);
            var weightTotal = 0.0;

            for (var targetIndex = firstTarget; targetIndex <= lastTarget; targetIndex++)
            {
                var offset = targetIndex - sourceIndex;
                weightTotal += Math.Exp(-0.5 * offset * offset / (sigma * sigma));
            }

            for (var targetIndex = firstTarget; targetIndex <= lastTarget; targetIndex++)
            {
                var offset = targetIndex - sourceIndex;
                var weight = Math.Exp(-0.5 * offset * offset / (sigma * sigma));
                smoothed[targetIndex] += eventCount * weight / weightTotal;
            }
        }

        return smoothed;
    }

    private static double Normalize(double value, double maximum) => maximum <= 0 ? 0 : value / maximum;
}

/// <summary>
/// Represents a labeled major grid tick on a chart axis.
/// </summary>
public sealed class GenealogyStatisticsAxisTick
{
    internal GenealogyStatisticsAxisTick(double value, string label, bool isMajor, bool isCentury)
    {
        Value = value;
        Label = label;
        IsMajor = isMajor;
        IsCentury = isCentury;
    }

    /// <summary>Gets the value on the corresponding axis.</summary>
    public double Value { get; }

    /// <summary>Gets the display label.</summary>
    public string Label { get; }

    /// <summary>Gets whether this is a major tick.</summary>
    public bool IsMajor { get; }

    /// <summary>Gets whether this time tick marks a century.</summary>
    public bool IsCentury { get; }
}

/// <summary>
/// Represents one year in the normalized chart coordinate space.
/// </summary>
public sealed class GenealogyStatisticsChartPoint
{
    internal GenealogyStatisticsChartPoint(int year, double livingPeople, double births, double deaths, double familyFoundings)
    {
        Year = year;
        LivingPeople = livingPeople;
        Births = births;
        Deaths = deaths;
        FamilyFoundings = familyFoundings;
    }

    /// <summary>Gets the calendar year on the horizontal axis.</summary>
    public int Year { get; }

    /// <summary>Gets the normalized living-population value on the primary axis.</summary>
    public double LivingPeople { get; }

    /// <summary>Gets the normalized births value on the secondary axis.</summary>
    public double Births { get; }

    /// <summary>Gets the normalized deaths value on the secondary axis.</summary>
    public double Deaths { get; }

    /// <summary>Gets the normalized family-founding value on the secondary axis.</summary>
    public double FamilyFoundings { get; }
}
