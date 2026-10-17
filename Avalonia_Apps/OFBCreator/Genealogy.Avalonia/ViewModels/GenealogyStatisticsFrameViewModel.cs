using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using GenInterfaces.Interfaces.Genealogic;
using Genealogy.Statistics;

namespace Genealogy.Avalonia.ViewModels;

/// <summary>
/// Presents a provider-independent genealogy statistics result to the Avalonia frame.
/// </summary>
public sealed class GenealogyStatisticsFrameViewModel : INotifyPropertyChanged
{
    private GenealogyStatisticsResult? _result;

    /// <summary>
    /// Occurs when a presentation property changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the current analysis result, or <see langword="null"/> before a result is loaded.
    /// </summary>
    public GenealogyStatisticsResult? Result => _result;

    /// <summary>
    /// Gets a value indicating whether the frame has a result to present.
    /// </summary>
    public bool HasResult => _result is not null;

    /// <summary>
    /// Gets the person with the earliest possible birth date.
    /// </summary>
    public string EarliestPersonText => DescribePerson(_result?.EarliestPerson);

    /// <summary>
    /// Gets the person with the latest possible birth date.
    /// </summary>
    public string LatestPersonText => DescribePerson(_result?.LatestPerson);

    /// <summary>
    /// Gets the oldest person recorded as female.
    /// </summary>
    public string OldestWomanText => DescribePerson(_result?.OldestWoman, includeAge: true);

    /// <summary>
    /// Gets the oldest person recorded as male.
    /// </summary>
    public string OldestManText => DescribePerson(_result?.OldestMan, includeAge: true);

    /// <summary>
    /// Gets the family with the earliest possible marriage date.
    /// </summary>
    public string EarliestMarriageText => DescribeMarriage(_result?.EarliestMarriage);

    /// <summary>
    /// Gets the family with the latest possible marriage date.
    /// </summary>
    public string LatestMarriageText => DescribeMarriage(_result?.LatestMarriage);

    /// <summary>
    /// Gets the most common places.
    /// </summary>
    public string TopPlacesText => DescribeTopEntries(_result?.TopPlaces ?? Array.Empty<GenealogyStatisticsCountEntry>());

    /// <summary>
    /// Gets the most common occupations.
    /// </summary>
    public string TopOccupationsText => DescribeTopEntries(_result?.TopOccupations ?? Array.Empty<GenealogyStatisticsCountEntry>());

    /// <summary>
    /// Gets the most common given names.
    /// </summary>
    public string TopGivenNamesText => DescribeTopEntries(_result?.TopGivenNames ?? Array.Empty<GenealogyStatisticsCountEntry>());

    /// <summary>
    /// Gets the most common surnames.
    /// </summary>
    public string TopSurnamesText => DescribeTopEntries(_result?.TopSurnames ?? Array.Empty<GenealogyStatisticsCountEntry>());

    /// <summary>
    /// Gets the average estimated age.
    /// </summary>
    public string AverageAgeText => DescribeAverage(_result?.AverageAgeYears);

    /// <summary>
    /// Gets the average death age.
    /// </summary>
    public string AverageDeathAgeText => DescribeAverage(_result?.AverageDeathAgeYears);

    /// <summary>
    /// Gets the average male generation gap.
    /// </summary>
    public string AverageGenerationGapMaleText => DescribeAverage(_result?.AverageGenerationGapMaleYears);

    /// <summary>
    /// Gets the average female generation gap.
    /// </summary>
    public string AverageGenerationGapFemaleText => DescribeAverage(_result?.AverageGenerationGapFemaleYears);

    /// <summary>
    /// Gets the average general generation gap.
    /// </summary>
    public string AverageGenerationGapGeneralText => DescribeAverage(_result?.AverageGenerationGapGeneralYears);

    /// <summary>
    /// Gets the chronological annual values used to draw the timeline.
    /// </summary>
    public ObservableCollection<GenealogyStatisticsYearViewModel> Years { get; } = [];

    /// <summary>
    /// Gets the normalized chart data for the time-series graph.
    /// </summary>
    public GenealogyStatisticsChartModel? Chart { get; private set; }

    /// <summary>
    /// Gets the normalized chart data for the age-distribution graph.
    /// </summary>
    public GenealogyStatisticsAgeDistributionChartModel? AgeDistributionChart { get; private set; }

    /// <summary>
    /// Loads a result and rebuilds the normalized annual display values.
    /// </summary>
    /// <param name="result">The statistics result to display.</param>
    public void Load(GenealogyStatisticsResult? result)
    {
        _result = result;
        OnPropertyChanged(nameof(Result));
        OnPropertyChanged(nameof(HasResult));
        OnPropertyChanged(nameof(EarliestPersonText));
        OnPropertyChanged(nameof(LatestPersonText));
        OnPropertyChanged(nameof(OldestWomanText));
        OnPropertyChanged(nameof(OldestManText));
        OnPropertyChanged(nameof(EarliestMarriageText));
        OnPropertyChanged(nameof(LatestMarriageText));
        OnPropertyChanged(nameof(TopPlacesText));
        OnPropertyChanged(nameof(TopOccupationsText));
        OnPropertyChanged(nameof(TopGivenNamesText));
        OnPropertyChanged(nameof(TopSurnamesText));
        OnPropertyChanged(nameof(AverageAgeText));
        OnPropertyChanged(nameof(AverageDeathAgeText));
        OnPropertyChanged(nameof(AverageGenerationGapMaleText));
        OnPropertyChanged(nameof(AverageGenerationGapFemaleText));
        OnPropertyChanged(nameof(AverageGenerationGapGeneralText));
        Chart = result is null ? null : GenealogyStatisticsChartModel.Create(result);
        AgeDistributionChart = result is null ? null : GenealogyStatisticsAgeDistributionChartModel.Create(result);
        OnPropertyChanged(nameof(Chart));
        OnPropertyChanged(nameof(AgeDistributionChart));

        Years.Clear();
        if (result is null || result.Years.Count == 0)
        {
            return;
        }

        foreach (var year in result.Years)
        {
            Years.Add(new GenealogyStatisticsYearViewModel(year, Chart!.MaximumLivingPeople, Chart.MaximumEvents));
        }
    }

    /// <summary>
    /// Raises the property-changed event for a presentation property.
    /// </summary>
    /// <param name="propertyName">The name of the changed property.</param>
    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Creates a readable label for a selected person statistic.
    /// </summary>
    /// <param name="statistic">The person statistic to describe.</param>
    /// <param name="includeAge">Whether to append its age.</param>
    /// <returns>A display label or a not-available label.</returns>
    private static string DescribePerson(GenealogyPersonStatistic? statistic, bool includeAge = false)
    {
        if (statistic is null)
        {
            return "Not available";
        }

        var person = statistic.Person;
        var name = GetPersonName(person);
        var firstYear = statistic.PossibleBirthStart.Year;
        var lastYear = statistic.PossibleBirthEnd.Year;
        var birthYear = firstYear == lastYear ? firstYear.ToString(CultureInfo.CurrentCulture) : $"{firstYear}–{lastYear}";
        var age = includeAge && statistic.EstimatedAgeYears.HasValue
            ? $"; {statistic.EstimatedAgeYears.Value.ToString("0.0", CultureInfo.CurrentCulture)} years"
            : string.Empty;
        return $"{name} ({birthYear}{age})";
    }

    /// <summary>
    /// Creates a readable label for a selected family marriage statistic.
    /// </summary>
    /// <param name="statistic">The marriage statistic to describe.</param>
    /// <returns>A display label or a not-available label.</returns>
    private static string DescribeMarriage(GenealogyMarriageStatistic? statistic)
    {
        if (statistic is null)
        {
            return "Not available";
        }

        var family = statistic.Family;
        var partners = new[] { family.Husband, family.Wife }
            .Where(person => person is not null)
            .Select(person => GetPersonName(person!))
            .Where(name => !string.IsNullOrWhiteSpace(name));
        var label = string.Join(" & ", partners);
        if (label.Length == 0)
        {
            label = string.IsNullOrWhiteSpace(family.FamilyName) ? "Marriage" : family.FamilyName;
        }
        var firstYear = statistic.PossibleDateStart.Year;
        var lastYear = statistic.PossibleDateEnd.Year;
        return firstYear == lastYear ? $"{label} ({firstYear})" : $"{label} ({firstYear}–{lastYear})";
    }

    /// <summary>
    /// Creates a readable summary for the top-ranked entries.
    /// </summary>
    private static string DescribeTopEntries(System.Collections.Generic.IReadOnlyList<GenealogyStatisticsCountEntry> entries)
    {
        if (entries.Count == 0)
        {
            return "Not available";
        }

        return string.Join(", ", entries.Select(static entry => $"{entry.Label} ({entry.Count})"));
    }

    /// <summary>
    /// Formats one average value.
    /// </summary>
    private static string DescribeAverage(double? value) => value.HasValue ? $"{value.Value:0.0} years" : "Not available";

    /// <summary>
    /// Gets a readable name without depending on a provider-specific person implementation.
    /// </summary>
    /// <param name="person">The person to name.</param>
    /// <returns>The best available name.</returns>
    private static string GetPersonName(IGenPerson person)
    {
        var nameParts = new[] { person.GivenName, person.Surname }
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .Select(part => part!.Trim())
            .ToArray();
        if (nameParts.Length > 0)
        {
            return string.Join(" ", nameParts);
        }

        var name = person.Name?.Replace("/", " ", StringComparison.Ordinal).Trim();
        return string.IsNullOrWhiteSpace(name) ? "Unnamed person" : string.Join(" ", name.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}

/// <summary>
/// Contains one annual series point and its normalized bar widths.
/// </summary>
public sealed class GenealogyStatisticsYearViewModel
{
    /// <summary>
    /// Initializes a normalized annual series point.
    /// </summary>
    /// <param name="statistics">The original annual statistics.</param>
    /// <param name="maximum">The largest value across all displayed annual series.</param>
    internal GenealogyStatisticsYearViewModel(GenealogyYearStatistics statistics, double maximumLiving, double maximumEvents)
    {
        Year = statistics.Year;
        LivingPeople = statistics.LivingPeople;
        Births = statistics.Births;
        Deaths = statistics.Deaths;
        LivingPeoplePercent = GetPercent(statistics.LivingPeople, maximumLiving);
        BirthsPercent = GetPercent(statistics.Births, maximumEvents);
        DeathsPercent = GetPercent(statistics.Deaths, maximumEvents);
        LivingPeopleText = Format(statistics.LivingPeople);
        BirthsText = Format(statistics.Births);
        DeathsText = Format(statistics.Deaths);
    }

    /// <summary>Gets the calendar year.</summary>
    public int Year { get; }

    /// <summary>Gets the expected living population.</summary>
    public double LivingPeople { get; }

    /// <summary>Gets the expected births.</summary>
    public double Births { get; }

    /// <summary>Gets the expected deaths.</summary>
    public double Deaths { get; }

    /// <summary>Gets the normalized living population bar value.</summary>
    public double LivingPeoplePercent { get; }

    /// <summary>Gets the normalized births bar value.</summary>
    public double BirthsPercent { get; }

    /// <summary>Gets the normalized deaths bar value.</summary>
    public double DeathsPercent { get; }

    /// <summary>Gets the formatted living population.</summary>
    public string LivingPeopleText { get; }

    /// <summary>Gets the formatted birth count.</summary>
    public string BirthsText { get; }

    /// <summary>Gets the formatted death count.</summary>
    public string DeathsText { get; }

    /// <summary>
    /// Formats a weighted count for display.
    /// </summary>
    /// <param name="value">The count value.</param>
    /// <returns>The culture-aware count string.</returns>
    private static string Format(double value) => value.ToString("0.##", CultureInfo.CurrentCulture);

    /// <summary>
    /// Scales a value to the progress bar's zero-to-one-hundred range.
    /// </summary>
    /// <param name="value">The count to scale.</param>
    /// <param name="maximum">The common maximum count.</param>
    /// <returns>A value from zero through one hundred.</returns>
    private static double GetPercent(double value, double maximum) =>
        maximum <= 0 ? 0 : Math.Clamp(value / maximum * 100, 0, 100);
}
