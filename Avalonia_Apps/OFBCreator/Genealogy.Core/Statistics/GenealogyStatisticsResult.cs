using System.Collections.Generic;

namespace Genealogy.Statistics;

/// <summary>
/// Contains the extreme-person and marriage results of a genealogy analysis.
/// </summary>
public sealed class GenealogyStatisticsResult
{
    /// <summary>
    /// Initializes an immutable statistics result.
    /// </summary>
    /// <param name="earliestPerson">The person with the earliest possible birth date.</param>
    /// <param name="latestPerson">The person with the latest possible birth date.</param>
    /// <param name="oldestWoman">The woman with the greatest known or estimated age.</param>
    /// <param name="oldestMan">The man with the greatest known or estimated age.</param>
    /// <param name="earliestMarriage">The family with the earliest possible marriage date.</param>
    /// <param name="latestMarriage">The family with the latest possible marriage date.</param>
    /// <param name="years">The annual time series for the requested period.</param>
    /// <param name="topPlaces">The most common places.</param>
    /// <param name="topOccupations">The most common occupations.</param>
    /// <param name="topGivenNames">The most common given names.</param>
    /// <param name="topSurnames">The most common surnames.</param>
    /// <param name="averageAgeYears">The average estimated age over all people.</param>
    /// <param name="averageDeathAgeYears">The average known death age.</param>
    /// <param name="averageGenerationGapMaleYears">The average father-to-child generation gap.</param>
    /// <param name="averageGenerationGapFemaleYears">The average mother-to-child generation gap.</param>
    /// <param name="averageGenerationGapGeneralYears">The average parent-to-child generation gap.</param>
    /// <param name="ageDistribution">The age distribution used for the separate chart.</param>
    internal GenealogyStatisticsResult(
        GenealogyPersonStatistic? earliestPerson,
        GenealogyPersonStatistic? latestPerson,
        GenealogyPersonStatistic? oldestWoman,
        GenealogyPersonStatistic? oldestMan,
        GenealogyMarriageStatistic? earliestMarriage,
        GenealogyMarriageStatistic? latestMarriage,
        IReadOnlyList<GenealogyYearStatistics> years,
        IReadOnlyList<GenealogyStatisticsCountEntry>? topPlaces = null,
        IReadOnlyList<GenealogyStatisticsCountEntry>? topOccupations = null,
        IReadOnlyList<GenealogyStatisticsCountEntry>? topGivenNames = null,
        IReadOnlyList<GenealogyStatisticsCountEntry>? topSurnames = null,
        double averageAgeYears = 0,
        double averageDeathAgeYears = 0,
        double averageGenerationGapMaleYears = 0,
        double averageGenerationGapFemaleYears = 0,
        double averageGenerationGapGeneralYears = 0,
        IReadOnlyList<GenealogyStatisticsAgeDistributionPoint>? ageDistribution = null)
    {
        EarliestPerson = earliestPerson;
        LatestPerson = latestPerson;
        OldestWoman = oldestWoman;
        OldestMan = oldestMan;
        EarliestMarriage = earliestMarriage;
        LatestMarriage = latestMarriage;
        Years = years;
        TopPlaces = topPlaces ?? [];
        TopOccupations = topOccupations ?? [];
        TopGivenNames = topGivenNames ?? [];
        TopSurnames = topSurnames ?? [];
        AverageAgeYears = averageAgeYears;
        AverageDeathAgeYears = averageDeathAgeYears;
        AverageGenerationGapMaleYears = averageGenerationGapMaleYears;
        AverageGenerationGapFemaleYears = averageGenerationGapFemaleYears;
        AverageGenerationGapGeneralYears = averageGenerationGapGeneralYears;
        AgeDistribution = ageDistribution ?? [];
    }

    /// <summary>
    /// Gets the person with the earliest possible birth date, or <see langword="null"/> when none is known.
    /// </summary>
    public GenealogyPersonStatistic? EarliestPerson { get; }

    /// <summary>
    /// Gets the person with the latest possible birth date, or <see langword="null"/> when none is known.
    /// </summary>
    public GenealogyPersonStatistic? LatestPerson { get; }

    /// <summary>
    /// Gets the oldest person recorded as female, or <see langword="null"/> when none is known.
    /// </summary>
    public GenealogyPersonStatistic? OldestWoman { get; }

    /// <summary>
    /// Gets the oldest person recorded as male, or <see langword="null"/> when none is known.
    /// </summary>
    public GenealogyPersonStatistic? OldestMan { get; }

    /// <summary>
    /// Gets the family with the earliest possible marriage date, or <see langword="null"/> when none is known.
    /// </summary>
    public GenealogyMarriageStatistic? EarliestMarriage { get; }

    /// <summary>
    /// Gets the family with the latest possible marriage date, or <see langword="null"/> when none is known.
    /// </summary>
    public GenealogyMarriageStatistic? LatestMarriage { get; }

    /// <summary>
    /// Gets the annual demographic statistics for the requested period.
    /// </summary>
    public IReadOnlyList<GenealogyYearStatistics> Years { get; }

    /// <summary>
    /// Gets the most common places.
    /// </summary>
    public IReadOnlyList<GenealogyStatisticsCountEntry> TopPlaces { get; }

    /// <summary>
    /// Gets the most common occupations.
    /// </summary>
    public IReadOnlyList<GenealogyStatisticsCountEntry> TopOccupations { get; }

    /// <summary>
    /// Gets the most common given names.
    /// </summary>
    public IReadOnlyList<GenealogyStatisticsCountEntry> TopGivenNames { get; }

    /// <summary>
    /// Gets the most common surnames.
    /// </summary>
    public IReadOnlyList<GenealogyStatisticsCountEntry> TopSurnames { get; }

    /// <summary>
    /// Gets the average estimated age over all people.
    /// </summary>
    public double AverageAgeYears { get; }

    /// <summary>
    /// Gets the average known death age.
    /// </summary>
    public double AverageDeathAgeYears { get; }

    /// <summary>
    /// Gets the average father-to-child generation gap.
    /// </summary>
    public double AverageGenerationGapMaleYears { get; }

    /// <summary>
    /// Gets the average mother-to-child generation gap.
    /// </summary>
    public double AverageGenerationGapFemaleYears { get; }

    /// <summary>
    /// Gets the average parent-to-child generation gap.
    /// </summary>
    public double AverageGenerationGapGeneralYears { get; }

    /// <summary>
    /// Gets the age distribution used for the separate chart.
    /// </summary>
    public IReadOnlyList<GenealogyStatisticsAgeDistributionPoint> AgeDistribution { get; }
}
