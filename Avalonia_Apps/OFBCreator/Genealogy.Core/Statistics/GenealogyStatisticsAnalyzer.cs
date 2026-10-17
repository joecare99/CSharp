using System;
using System.Collections.Generic;
using System.Linq;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;

namespace Genealogy.Statistics;

/// <summary>
/// Calculates provider-independent statistics from public genealogy interfaces.
/// </summary>
public static class GenealogyStatisticsAnalyzer
{
    private const int DefaultFirstYear = 1500;
    private const double DaysPerYear = 365.2425;
    private const double UnknownDeathFadeDurationYears = 15.0;
    private const double UnknownDeathFadeStartAgeYears = 75.0;

    /// <summary>
    /// Calculates statistics over the genealogy's dated-event range with a rounded ten-year margin.
    /// </summary>
    /// <param name="people">The people to analyze.</param>
    /// <param name="families">The families to include.</param>
    /// <param name="firstYearOverride">An optional inclusive first-year override.</param>
    /// <param name="lastYearOverride">An optional inclusive last-year override.</param>
    /// <returns>Statistics over the selected or automatically inferred year range.</returns>
    public static GenealogyStatisticsResult AnalyzeForChart(
        IEnumerable<IGenPerson> people,
        IEnumerable<IGenFamily> families,
        int? firstYearOverride = null,
        int? lastYearOverride = null)
    {
        ArgumentNullException.ThrowIfNull(people);
        ArgumentNullException.ThrowIfNull(families);
        var peopleList = people.Where(person => person is not null).ToArray();
        var familyList = families.Where(family => family is not null).ToArray();
        var datedRanges = GetDatedEventRanges(peopleList, familyList).ToArray();
        var firstEventYear = datedRanges.Select(range => range.Start.Year).DefaultIfEmpty(DefaultFirstYear).Min();
        var lastEventYear = datedRanges.Select(range => range.End.Year).DefaultIfEmpty(DateTime.Today.Year).Max();
        var firstYear = firstYearOverride ?? Math.Max(1, (int)(Math.Floor((firstEventYear - 10) / 10.0) * 10));
        var lastYear = lastYearOverride ?? Math.Min(9999, (int)(Math.Ceiling((lastEventYear + 10) / 10.0) * 10));
        return Analyze(peopleList, familyList, firstYear, lastYear);
    }

    /// <summary>
    /// Calculates extreme people and marriages together with an annual demographic time series.
    /// </summary>
    /// <param name="people">The people to analyze.</param>
    /// <param name="firstYear">The first year in the time series.</param>
    /// <param name="lastYear">The last year in the time series.</param>
    /// <returns>The calculated extreme statistics.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="people"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The year interval is invalid.</exception>
    public static GenealogyStatisticsResult Analyze(IEnumerable<IGenPerson> people, int firstYear, int lastYear)
    {
        ArgumentNullException.ThrowIfNull(people);
        var peopleList = people.Where(person => person is not null).ToArray();
        var families = new List<IGenFamily>();
        foreach (var person in peopleList)
        {
            foreach (var family in person.Marriages)
            {
                if (family is not null)
                {
                    families.Add(family);
                }
            }
        }
        return Analyze(peopleList, families, firstYear, lastYear);
    }

    /// <summary>
    /// Calculates statistics from an explicit person and family set.
    /// </summary>
    /// <param name="people">The people to analyze.</param>
    /// <param name="families">The families to include in marriage statistics.</param>
    /// <param name="firstYear">The first year in the time series.</param>
    /// <param name="lastYear">The last year in the time series.</param>
    /// <returns>The calculated extreme statistics and annual time series.</returns>
    /// <exception cref="ArgumentNullException">An input sequence is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The year interval is invalid.</exception>
    public static GenealogyStatisticsResult Analyze(
        IEnumerable<IGenPerson> people,
        IEnumerable<IGenFamily> families,
        int firstYear,
        int lastYear)
    {
        ArgumentNullException.ThrowIfNull(people);
        ArgumentNullException.ThrowIfNull(families);
        if (firstYear < 1 || firstYear > 9999)
        {
            throw new ArgumentOutOfRangeException(nameof(firstYear));
        }
        if (lastYear < firstYear || lastYear > 9999)
        {
            throw new ArgumentOutOfRangeException(nameof(lastYear));
        }

        var peopleList = people.Where(person => person is not null).ToList();
        var familyList = families.Where(family => family is not null).ToList();

        GenealogyPersonStatistic? earliestPerson = null;
        GenealogyPersonStatistic? latestPerson = null;
        GenealogyPersonStatistic? oldestWoman = null;
        GenealogyPersonStatistic? oldestMan = null;
        GenealogyMarriageStatistic? earliestMarriage = null;
        GenealogyMarriageStatistic? latestMarriage = null;
        var knownFamilies = new HashSet<Guid>();
        var unkeyedFamilies = new HashSet<IGenFamily>(ReferenceEqualityComparer.Instance);

        foreach (var person in peopleList)
        {
            var birthDate = GetBirthDate(person);
            if (GenealogyDateRange.TryCreate(birthDate, out var birthRange))
            {
                var age = GetAgeYears(person, birthRange);
                var personStatistic = new GenealogyPersonStatistic(person, birthRange.Start, birthRange.End, age);
                if (earliestPerson is null || birthRange.Start < earliestPerson.PossibleBirthStart)
                {
                    earliestPerson = personStatistic;
                }
                if (latestPerson is null || birthRange.End > latestPerson.PossibleBirthEnd)
                {
                    latestPerson = personStatistic;
                }

                if (age.HasValue && IsSex(person.Sex, 'F') &&
                    (oldestWoman?.EstimatedAgeYears is null || age.Value > oldestWoman.EstimatedAgeYears.Value))
                {
                    oldestWoman = personStatistic;
                }
                if (age.HasValue && IsSex(person.Sex, 'M') &&
                    (oldestMan?.EstimatedAgeYears is null || age.Value > oldestMan.EstimatedAgeYears.Value))
                {
                    oldestMan = personStatistic;
                }
            }
        }

        foreach (var family in familyList)
        {
            if (!IsNewFamily(family, knownFamilies, unkeyedFamilies))
            {
                continue;
            }

            var marriageDate = family.MarriageDate ?? family.Marriage?.Date;
            if (!GenealogyDateRange.TryCreate(marriageDate, out var marriageRange))
            {
                continue;
            }

            var marriageStatistic = new GenealogyMarriageStatistic(family, marriageRange.Start, marriageRange.End);
            if (earliestMarriage is null || marriageRange.Start < earliestMarriage.PossibleDateStart)
            {
                earliestMarriage = marriageStatistic;
            }
            if (latestMarriage is null || marriageRange.End > latestMarriage.PossibleDateEnd)
            {
                latestMarriage = marriageStatistic;
            }
        }

        var ageSummary = BuildAgeSummary(peopleList);
        return new GenealogyStatisticsResult(
            earliestPerson,
            latestPerson,
            oldestWoman,
            oldestMan,
            earliestMarriage,
            latestMarriage,
            BuildYearStatistics(peopleList, familyList, firstYear, lastYear),
            BuildTopEntries(EnumeratePlaces(peopleList, familyList), 2),
            BuildTopEntries(EnumerateOccupations(peopleList), 2),
            BuildTopEntries(EnumerateGivenNames(peopleList), 2),
            BuildTopEntries(EnumerateSurnames(peopleList), 2),
            ageSummary.AverageAgeYears,
            ageSummary.AverageDeathAgeYears,
            ageSummary.AverageGenerationGapMaleYears,
            ageSummary.AverageGenerationGapFemaleYears,
            ageSummary.AverageGenerationGapGeneralYears,
            ageSummary.AgeDistribution);
    }

    /// <summary>
    /// Aggregates the age-related summary data for the separate distribution chart.
    /// </summary>
    /// <param name="people">The people to inspect.</param>
    /// <returns>The calculated age summary.</returns>
    private static AgeSummary BuildAgeSummary(IReadOnlyList<IGenPerson> people)
    {
        var totalAge = 0.0;
        var ageCount = 0;
        var totalDeathAge = 0.0;
        var deathAgeCount = 0;
        var totalMaleGenerationGap = 0.0;
        var maleGenerationGapCount = 0;
        var totalFemaleGenerationGap = 0.0;
        var femaleGenerationGapCount = 0;
        var totalGeneralGenerationGap = 0.0;
        var generalGenerationGapCount = 0;
        var buckets = new Dictionary<int, AgeDistributionBucket>();

        foreach (var person in people)
        {
            var birthDate = GetBirthDate(person);
            if (!GenealogyDateRange.TryCreate(birthDate, out var birthRange))
            {
                continue;
            }

            var age = GetAgeYears(person, birthRange);
            if (age.HasValue)
            {
                totalAge += age.Value;
                ageCount++;
                AddDistributionValue(buckets, age.Value, DistributionSeries.PersonAge);
            }

            if (GenealogyDateRange.TryCreate(GetActualDeathDate(person), out var actualDeathRange))
            {
                var deathAge = Math.Max(0, (actualDeathRange.End - birthRange.Start).TotalDays / DaysPerYear);
                totalDeathAge += deathAge;
                deathAgeCount++;
                AddDistributionValue(buckets, deathAge, DistributionSeries.DeathAge);
            }

            if (TryGetBirthRange(person.Father, out var fatherBirthRange))
            {
                var gap = GetGenerationGapYears(fatherBirthRange, birthRange);
                if (gap.HasValue)
                {
                    totalMaleGenerationGap += gap.Value;
                    maleGenerationGapCount++;
                    totalGeneralGenerationGap += gap.Value;
                    generalGenerationGapCount++;
                    AddDistributionValue(buckets, gap.Value, DistributionSeries.GenerationGap);
                }
            }

            if (TryGetBirthRange(person.Mother, out var motherBirthRange))
            {
                var gap = GetGenerationGapYears(motherBirthRange, birthRange);
                if (gap.HasValue)
                {
                    totalFemaleGenerationGap += gap.Value;
                    femaleGenerationGapCount++;
                    totalGeneralGenerationGap += gap.Value;
                    generalGenerationGapCount++;
                    AddDistributionValue(buckets, gap.Value, DistributionSeries.GenerationGap);
                }
            }
        }

        return new AgeSummary(
            ageCount == 0 ? 0 : totalAge / ageCount,
            deathAgeCount == 0 ? 0 : totalDeathAge / deathAgeCount,
            maleGenerationGapCount == 0 ? 0 : totalMaleGenerationGap / maleGenerationGapCount,
            femaleGenerationGapCount == 0 ? 0 : totalFemaleGenerationGap / femaleGenerationGapCount,
            generalGenerationGapCount == 0 ? 0 : totalGeneralGenerationGap / generalGenerationGapCount,
            buckets
                .OrderBy(entry => entry.Key)
                .Select(entry => new GenealogyStatisticsAgeDistributionPoint(
                    entry.Key,
                    entry.Value.PersonAge,
                    entry.Value.GenerationGap,
                    entry.Value.DeathAge))
                .ToArray());
    }

    /// <summary>
    /// Gets the primary birth date, falling back to a birth fact when needed.
    /// </summary>
    /// <param name="person">The person to inspect.</param>
    /// <returns>The first available birth date.</returns>
    private static IGenDate? GetBirthDate(IGenPerson person)
    {
        return FirstUsableDate(
            person.BirthDate,
            person.Birth?.Date,
            GetFactDate(person, EFactType.Birth),
            person.BaptDate,
            person.Baptism?.Date,
            GetFactDate(person, EFactType.Baptism));
    }

    /// <summary>
    /// Estimates age using a known death range or the configured unknown-death fade rule.
    /// </summary>
    /// <param name="person">The person whose age is estimated.</param>
    /// <param name="birthRange">The person's possible birth dates.</param>
    /// <returns>The estimated age in years, or <see langword="null"/> when no death or birth information is usable.</returns>
    private static double? GetAgeYears(IGenPerson person, GenealogyDateRange birthRange)
    {
        var deathDate = GetDeathDate(person);

        if (GenealogyDateRange.TryCreate(deathDate, out var deathRange))
        {
            return Math.Max(0, (deathRange.End - birthRange.Start).TotalDays / DaysPerYear);
        }

        var lastEventDate = GetLatestPersonEventDate(person);
        var lastEvidenceAge = lastEventDate.HasValue
            ? Math.Max(0, (lastEventDate.Value - birthRange.Start).TotalDays / DaysPerYear)
            : 0;
        var fadeStartAge = Math.Max(UnknownDeathFadeStartAgeYears, lastEvidenceAge);
        return fadeStartAge + UnknownDeathFadeDurationYears / 2;
    }

    /// <summary>
    /// Finds the latest usable date across all dated facts and vital events for a person.
    /// </summary>
    /// <param name="person">The person whose events are inspected.</param>
    /// <returns>The latest possible event date, when any dated event is available.</returns>
    private static DateTime? GetLatestPersonEventDate(IGenPerson person)
    {
        DateTime? latestDate = null;

        void Include(IGenDate? date)
        {
            if (GenealogyDateRange.TryCreate(date, out var range) && (!latestDate.HasValue || range.End > latestDate.Value))
            {
                latestDate = range.End;
            }
        }

        Include(person.BirthDate);
        Include(person.BaptDate);
        Include(person.DeathDate);
        Include(person.BurialDate);
        Include(person.Birth?.Date);
        Include(person.Baptism?.Date);
        Include(person.Death?.Date);
        Include(person.Burial?.Date);
        foreach (var fact in person.Facts)
        {
            Include(fact?.Date);
        }
        foreach (var family in person.Marriages)
        {
            if (family is null)
            {
                continue;
            }
            Include(family.MarriageDate);
            Include(family.Marriage?.Date);
            foreach (var fact in family.Facts)
            {
                Include(fact?.Date);
            }
        }
        return latestDate;
    }

    /// <summary>
    /// Determines whether a family is not already included in the marriage analysis.
    /// </summary>
    /// <param name="family">The family to test.</param>
    /// <param name="knownFamilies">The set of non-empty family identifiers already seen.</param>
    /// <param name="unkeyedFamilies">The set of identifier-less family instances already seen.</param>
    /// <returns><see langword="true"/> when the family has not been included before.</returns>
    private static bool IsNewFamily(IGenFamily family, HashSet<Guid> knownFamilies, HashSet<IGenFamily> unkeyedFamilies)
    {
        return family.UId == Guid.Empty ? unkeyedFamilies.Add(family) : knownFamilies.Add(family.UId);
    }

    /// <summary>
    /// Checks whether a sex value represents the requested GEDCOM sex code.
    /// </summary>
    /// <param name="sex">The recorded sex value.</param>
    /// <param name="code">The requested one-letter sex code.</param>
    /// <returns><see langword="true"/> when the value matches the requested code.</returns>
    private static bool IsSex(string sex, char code)
    {
        return !string.IsNullOrEmpty(sex) && sex.Length == 1 && char.ToUpperInvariant(sex[0]) == code;
    }

    private static IReadOnlyList<GenealogyStatisticsCountEntry> BuildTopEntries(IEnumerable<string?> values, int limit)
    {
        return values
            .Select(NormalizeLabel)
            .Where(value => value is not null)
            .GroupBy(value => value!, StringComparer.OrdinalIgnoreCase)
            .Select(group => new GenealogyStatisticsCountEntry(group.First(), group.Count()))
            .OrderByDescending(entry => entry.Count)
            .ThenBy(entry => entry.Label, StringComparer.OrdinalIgnoreCase)
            .ThenBy(entry => entry.Label, StringComparer.Ordinal)
            .Take(limit)
            .ToArray();
    }

    private static IEnumerable<string?> EnumeratePlaces(IEnumerable<IGenPerson> people, IEnumerable<IGenFamily> families)
    {
        foreach (var person in people)
        {
            yield return GetPlaceLabel(person.BirthPlace);
            yield return GetPlaceLabel(person.BaptPlace);
            yield return GetPlaceLabel(person.DeathPlace);
            yield return GetPlaceLabel(person.BurialPlace);
            yield return GetPlaceLabel(person.OccuPlace);
            yield return GetPlaceLabel(person.Residence);
            foreach (var fact in person.Facts)
            {
                yield return GetPlaceLabel(fact?.Place);
            }
        }

        foreach (var family in families)
        {
            yield return GetPlaceLabel(family.MarriagePlace);
            yield return GetPlaceLabel(family.Marriage?.Place);
            foreach (var fact in family.Facts)
            {
                yield return GetPlaceLabel(fact?.Place);
            }
        }
    }

    private static IEnumerable<string?> EnumerateOccupations(IEnumerable<IGenPerson> people)
    {
        foreach (var person in people)
        {
            var occupation = NormalizeLabel(person.Occupation);
            if (occupation is not null)
            {
                yield return occupation;
                continue;
            }

            foreach (var fact in person.Facts)
            {
                if (fact?.eFactType != EFactType.Occupation)
                {
                    continue;
                }

                yield return NormalizeLabel(fact.Data);
            }
        }
    }

    private static IEnumerable<string?> EnumerateGivenNames(IEnumerable<IGenPerson> people)
    {
        foreach (var person in people)
        {
            yield return NormalizeLabel(person.GivenName) ?? ExtractGivenName(person.Name);
        }
    }

    private static IEnumerable<string?> EnumerateSurnames(IEnumerable<IGenPerson> people)
    {
        foreach (var person in people)
        {
            yield return NormalizeLabel(person.Surname) ?? ExtractSurname(person.Name);
        }
    }

    private static string? GetPlaceLabel(IGenPlace? place) => NormalizeLabel(place?.Name);

    private static string? ExtractGivenName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var slashParts = name.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (slashParts.Length >= 1)
        {
            return NormalizeLabel(slashParts[0]);
        }

        return NormalizeLabel(name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault());
    }

    private static string? ExtractSurname(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var slashParts = name.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (slashParts.Length >= 2)
        {
            return NormalizeLabel(slashParts[1]);
        }

        return NormalizeLabel(name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).LastOrDefault());
    }

    private static string? NormalizeLabel(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = string.Join(" ", value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    private static void AddDistributionValue(Dictionary<int, AgeDistributionBucket> buckets, double value, DistributionSeries series)
    {
        var bucket = (int)Math.Floor(Math.Max(0, value));
        if (!buckets.TryGetValue(bucket, out var accumulator))
        {
            accumulator = default;
        }

        switch (series)
        {
            case DistributionSeries.PersonAge:
                accumulator.PersonAge++;
                break;
            case DistributionSeries.GenerationGap:
                accumulator.GenerationGap++;
                break;
            case DistributionSeries.DeathAge:
                accumulator.DeathAge++;
                break;
        }

        buckets[bucket] = accumulator;
    }

    /// <summary>
    /// Calculates the annual event and living-person counts for the requested interval.
    /// </summary>
    /// <param name="people">The people to include.</param>
    /// <param name="firstYear">The first year to calculate.</param>
    /// <param name="lastYear">The final year to calculate.</param>
    /// <returns>The annual statistics in chronological order.</returns>
    private static IReadOnlyList<GenealogyYearStatistics> BuildYearStatistics(
        IReadOnlyList<IGenPerson> people,
        IEnumerable<IGenFamily> families,
        int firstYear,
        int lastYear)
    {
        var profiles = people
            .Select(person => (Person: person, Profile: CreateTemporalProfile(person)))
            .Where(entry => entry.Profile.HasBirth)
            .ToArray();
        var knownFamilyIds = new HashSet<Guid>();
        var unkeyedFamilies = new HashSet<IGenFamily>(ReferenceEqualityComparer.Instance);
        var familyMarriageRanges = families
            .Where(family => family is not null && IsNewFamily(family, knownFamilyIds, unkeyedFamilies))
            .Select(family => family.MarriageDate ?? family.Marriage?.Date)
            .Select(date => GenealogyDateRange.TryCreate(date, out var range) ? (GenealogyDateRange?)range : null)
            .Where(range => range.HasValue)
            .Select(range => range!.Value)
            .ToArray();
        var years = new List<GenealogyYearStatistics>(lastYear - firstYear + 1);

        for (var year = firstYear; year <= lastYear; year++)
        {
            var checkpoint = new DateTime(year, 7, 1);
            var living = 0.0;
            var births = 0.0;
            var deaths = 0.0;
            var familyFoundings = 0.0;
            var totalAgeYears = 0.0;
            var totalDeathAgeYears = 0.0;
            var totalGenerationGapYears = 0.0;
            var ageObservationCount = 0.0;
            var deathAgeObservationCount = 0.0;
            var generationGapObservationCount = 0.0;

            foreach (var entry in profiles)
            {
                var profile = entry.Profile;
                var birthProbability = GetEventProbability(profile.BirthRange, year);
                births += birthProbability;

                var birthMidpoint = GetRangeMidpoint(profile.BirthRange);
                var ageAtCheckpoint = Math.Max(0, (checkpoint - birthMidpoint).TotalDays / DaysPerYear);

                double livingProbability;
                if (profile.DeathRange.HasValue)
                {
                    var deathProbability = GetEventProbability(profile.DeathRange.Value, year);
                    deaths += deathProbability;
                    livingProbability = GetProbabilityBornBy(profile.BirthRange, checkpoint) *
                        GetProbabilityAliveAfter(profile.DeathRange.Value, checkpoint);

                    var deathAge = Math.Max(0, (GetRangeMidpoint(profile.DeathRange.Value) - birthMidpoint).TotalDays / DaysPerYear);
                    totalDeathAgeYears += deathAge * deathProbability;
                    deathAgeObservationCount += deathProbability;
                }
                else
                {
                    livingProbability = GetEstimatedUnknownDeathSurvival(profile, checkpoint);
                    var estimatedDeathAge = profile.FadeStartAgeYears + UnknownDeathFadeDurationYears / 2.0;
                    var estimatedDeathYear = (int)Math.Round(birthMidpoint.Year + estimatedDeathAge, MidpointRounding.AwayFromZero);

                    if (estimatedDeathYear == year)
                    {
                        totalDeathAgeYears += estimatedDeathAge;
                        deathAgeObservationCount += 1;
                    }
                }

                living += livingProbability;
                totalAgeYears += ageAtCheckpoint * livingProbability;
                ageObservationCount += livingProbability;

                if (birthProbability <= 0)
                {
                    continue;
                }

                if (TryGetBirthRange(entry.Person.Father, out var fatherBirthRange))
                {
                    var gap = GetGenerationGapYears(fatherBirthRange, profile.BirthRange);
                    if (gap.HasValue)
                    {
                        totalGenerationGapYears += gap.Value * birthProbability;
                        generationGapObservationCount += birthProbability;
                    }
                }

                if (TryGetBirthRange(entry.Person.Mother, out var motherBirthRange))
                {
                    var gap = GetGenerationGapYears(motherBirthRange, profile.BirthRange);
                    if (gap.HasValue)
                    {
                        totalGenerationGapYears += gap.Value * birthProbability;
                        generationGapObservationCount += birthProbability;
                    }
                }
            }

            foreach (var marriageRange in familyMarriageRanges)
            {
                familyFoundings += GetEventProbability(marriageRange, year);
            }

            years.Add(new GenealogyYearStatistics(
                year,
                living,
                births,
                deaths,
                familyFoundings,
                ageObservationCount == 0 ? 0 : totalAgeYears / ageObservationCount,
                deathAgeObservationCount == 0 ? 0 : totalDeathAgeYears / deathAgeObservationCount,
                generationGapObservationCount == 0 ? 0 : totalGenerationGapYears / generationGapObservationCount,
                ageObservationCount,
                deathAgeObservationCount,
                generationGapObservationCount));
        }
        return years;
    }

    private static IEnumerable<GenealogyDateRange> GetDatedEventRanges(
        IReadOnlyList<IGenPerson> people,
        IReadOnlyList<IGenFamily> families)
    {
        foreach (var person in people)
        {
            foreach (var date in GetPersonEventDates(person))
            {
                if (GenealogyDateRange.TryCreate(date, out var range))
                    yield return range;
            }
        }

        var knownFamilyIds = new HashSet<Guid>();
        var unkeyedFamilies = new HashSet<IGenFamily>(ReferenceEqualityComparer.Instance);
        foreach (var family in families)
        {
            if (!IsNewFamily(family, knownFamilyIds, unkeyedFamilies))
                continue;
            foreach (var date in GetFamilyEventDates(family))
            {
                if (GenealogyDateRange.TryCreate(date, out var range))
                    yield return range;
            }
        }
    }

    private static IEnumerable<IGenDate?> GetPersonEventDates(IGenPerson person)
    {
        yield return person.BirthDate;
        yield return person.BaptDate;
        yield return person.DeathDate;
        yield return person.BurialDate;
        yield return person.Birth?.Date;
        yield return person.Baptism?.Date;
        yield return person.Death?.Date;
        yield return person.Burial?.Date;
        foreach (var fact in person.Facts)
            yield return fact?.Date;
    }

    private static IEnumerable<IGenDate?> GetFamilyEventDates(IGenFamily family)
    {
        yield return family.MarriageDate;
        yield return family.Marriage?.Date;
        foreach (var fact in family.Facts)
            yield return fact?.Date;
    }

    /// <summary>
    /// Creates the date profile used to calculate a person's annual contribution.
    /// </summary>
    /// <param name="person">The person to inspect.</param>
    /// <returns>The person's birth, death, and evidence profile.</returns>
    private static TemporalProfile CreateTemporalProfile(IGenPerson person)
    {
        if (!GenealogyDateRange.TryCreate(GetBirthDate(person), out var birthRange))
        {
            return default;
        }

        GenealogyDateRange? deathRange = null;
        if (GenealogyDateRange.TryCreate(GetDeathDate(person), out var knownDeathRange))
        {
            deathRange = knownDeathRange;
        }

        var lastEventDate = GetLatestPersonEventDate(person);
        var lastEvidenceAge = lastEventDate.HasValue
            ? Math.Max(0, (lastEventDate.Value - birthRange.Start).TotalDays / DaysPerYear)
            : 0;
        var fadeStartAge = Math.Max(UnknownDeathFadeStartAgeYears, lastEvidenceAge);
        return new TemporalProfile(birthRange, deathRange, fadeStartAge);
    }

    /// <summary>
    /// Gets the primary death date, falling back to a death fact when needed.
    /// </summary>
    /// <param name="person">The person to inspect.</param>
    /// <returns>The first available death date.</returns>
    private static IGenDate? GetDeathDate(IGenPerson person)
    {
        return FirstUsableDate(
            person.DeathDate,
            person.Death?.Date,
            GetFactDate(person, EFactType.Death),
            person.BurialDate,
            person.Burial?.Date,
            GetFactDate(person, EFactType.Burial));
    }

    private static IGenDate? FirstUsableDate(params IGenDate?[] dates)
    {
        foreach (var date in dates)
        {
            if (GenealogyDateRange.TryCreate(date, out _))
            {
                return date;
            }
        }
        return null;
    }

    private static IGenDate? GetFactDate(IGenPerson person, EFactType factType)
    {
        foreach (var fact in person.Facts)
        {
            if (fact?.eFactType == factType && GenealogyDateRange.TryCreate(fact.Date, out _))
            {
                return fact.Date;
            }
        }
        return null;
    }

    private static IGenDate? GetActualDeathDate(IGenPerson person)
    {
        return FirstUsableDate(
            person.DeathDate,
            person.Death?.Date,
            GetFactDate(person, EFactType.Death));
    }

    private static bool TryGetBirthRange(IGenPerson? person, out GenealogyDateRange birthRange)
    {
        birthRange = default;
        if (person is null)
        {
            return false;
        }

        return GenealogyDateRange.TryCreate(GetBirthDate(person), out birthRange);
    }

    private static double? GetGenerationGapYears(GenealogyDateRange parentBirthRange, GenealogyDateRange childBirthRange)
    {
        var parentMidpoint = GetRangeMidpoint(parentBirthRange);
        var childMidpoint = GetRangeMidpoint(childBirthRange);
        var gap = (childMidpoint - parentMidpoint).TotalDays / DaysPerYear;
        return gap < 0 ? null : gap;
    }

    private static DateTime GetRangeMidpoint(GenealogyDateRange range)
    {
        var midpointTicks = range.Start.Ticks + (range.End.Ticks - range.Start.Ticks) / 2;
        return new DateTime(midpointTicks, DateTimeKind.Unspecified);
    }

    /// <summary>
    /// Gets the fraction of a possible date interval that falls within a calendar year.
    /// </summary>
    /// <param name="range">The possible event dates.</param>
    /// <param name="year">The calendar year to inspect.</param>
    /// <returns>A value between zero and one.</returns>
    private static double GetEventProbability(GenealogyDateRange range, int year)
    {
        var yearStart = new DateTime(year, 1, 1);
        var yearEnd = new DateTime(year, 12, 31);
        var start = range.Start > yearStart ? range.Start : yearStart;
        var end = range.End < yearEnd ? range.End : yearEnd;
        if (start > end)
        {
            return 0;
        }

        var possibleDays = (range.End - range.Start).Days + 1.0;
        return ((end - start).Days + 1.0) / possibleDays;
    }

    /// <summary>
    /// Gets the probability that an event in the date range has occurred by the checkpoint.
    /// </summary>
    /// <param name="range">The possible event dates.</param>
    /// <param name="checkpoint">The date at which the probability is evaluated.</param>
    /// <returns>A value between zero and one.</returns>
    private static double GetProbabilityBornBy(GenealogyDateRange range, DateTime checkpoint)
    {
        if (checkpoint < range.Start)
        {
            return 0;
        }
        if (checkpoint >= range.End)
        {
            return 1;
        }
        return ((checkpoint - range.Start).Days + 1.0) / ((range.End - range.Start).Days + 1.0);
    }

    /// <summary>
    /// Gets the probability that an event in the date range occurs after the checkpoint.
    /// </summary>
    /// <param name="range">The possible event dates.</param>
    /// <param name="checkpoint">The date at which the probability is evaluated.</param>
    /// <returns>A value between zero and one.</returns>
    private static double GetProbabilityAliveAfter(GenealogyDateRange range, DateTime checkpoint)
    {
        if (checkpoint < range.Start)
        {
            return 1;
        }
        if (checkpoint >= range.End)
        {
            return 0;
        }
        return (range.End - checkpoint).Days / ((range.End - range.Start).Days + 1.0);
    }

    /// <summary>
    /// Calculates expected survival when the person's death date is unknown.
    /// </summary>
    /// <param name="profile">The person's temporal profile.</param>
    /// <param name="checkpoint">The date at which survival is evaluated.</param>
    /// <returns>The expected contribution between zero and one.</returns>
    private static double GetEstimatedUnknownDeathSurvival(TemporalProfile profile, DateTime checkpoint)
    {
        var firstPossibleBirthDay = profile.BirthRange.Start.Ticks / TimeSpan.TicksPerDay;
        var lastPossibleBirthDay = profile.BirthRange.End.Ticks / TimeSpan.TicksPerDay;
        var checkpointDay = checkpoint.Ticks / TimeSpan.TicksPerDay;
        var totalPossibleBirthDays = lastPossibleBirthDay - firstPossibleBirthDay + 1.0;
        var fadeDurationDays = UnknownDeathFadeDurationYears * DaysPerYear;
        var fadeEndAge = profile.FadeStartAgeYears + UnknownDeathFadeDurationYears;

        var fullSurvivalStart = (long)Math.Ceiling(checkpointDay - profile.FadeStartAgeYears * DaysPerYear);
        var fullSurvivalEnd = Math.Min(lastPossibleBirthDay, checkpointDay);
        var fullSurvivalCount = Math.Max(0, fullSurvivalEnd - Math.Max(firstPossibleBirthDay, fullSurvivalStart) + 1);

        var fadeStart = (long)Math.Floor(checkpointDay - fadeEndAge * DaysPerYear) + 1;
        var fadeEnd = Math.Min(lastPossibleBirthDay, fullSurvivalStart - 1);
        fadeStart = Math.Max(firstPossibleBirthDay, fadeStart);
        fadeEnd = Math.Min(checkpointDay, fadeEnd);

        var fadingContribution = 0.0;
        if (fadeStart <= fadeEnd)
        {
            var fadeCount = fadeEnd - fadeStart + 1.0;
            var birthDaySum = fadeCount * (fadeStart + fadeEnd) / 2.0;
            var zeroSurvivalBoundary = checkpointDay - fadeEndAge * DaysPerYear;
            fadingContribution = (birthDaySum - fadeCount * zeroSurvivalBoundary) / fadeDurationDays;
        }

        return Math.Clamp((fullSurvivalCount + fadingContribution) / totalPossibleBirthDays, 0, 1);
    }

    /// <summary>
    /// Holds the date ranges and unknown-death fade threshold for one person.
    /// </summary>
    private readonly struct TemporalProfile
    {
        /// <summary>
        /// Initializes a temporal profile.
        /// </summary>
        /// <param name="birthRange">The possible birth dates.</param>
        /// <param name="deathRange">The possible death dates, when known.</param>
        /// <param name="fadeStartAgeYears">The age at which unknown-death survival begins to fade.</param>
        public TemporalProfile(GenealogyDateRange birthRange, GenealogyDateRange? deathRange, double fadeStartAgeYears)
        {
            HasBirth = true;
            BirthRange = birthRange;
            DeathRange = deathRange;
            FadeStartAgeYears = fadeStartAgeYears;
        }

        /// <summary>
        /// Gets whether the profile has usable birth information.
        /// </summary>
        public bool HasBirth { get; }

        /// <summary>
        /// Gets the possible birth dates.
        /// </summary>
        public GenealogyDateRange BirthRange { get; }

        /// <summary>
        /// Gets the possible death dates when death is known.
        /// </summary>
        public GenealogyDateRange? DeathRange { get; }

        /// <summary>
        /// Gets the age at which unknown-death survival begins to fade.
        /// </summary>
        public double FadeStartAgeYears { get; }
    }

    private readonly struct AgeSummary
    {
        public AgeSummary(
            double averageAgeYears,
            double averageDeathAgeYears,
            double averageGenerationGapMaleYears,
            double averageGenerationGapFemaleYears,
            double averageGenerationGapGeneralYears,
            IReadOnlyList<GenealogyStatisticsAgeDistributionPoint> ageDistribution)
        {
            AverageAgeYears = averageAgeYears;
            AverageDeathAgeYears = averageDeathAgeYears;
            AverageGenerationGapMaleYears = averageGenerationGapMaleYears;
            AverageGenerationGapFemaleYears = averageGenerationGapFemaleYears;
            AverageGenerationGapGeneralYears = averageGenerationGapGeneralYears;
            AgeDistribution = ageDistribution;
        }

        public double AverageAgeYears { get; }
        public double AverageDeathAgeYears { get; }
        public double AverageGenerationGapMaleYears { get; }
        public double AverageGenerationGapFemaleYears { get; }
        public double AverageGenerationGapGeneralYears { get; }
        public IReadOnlyList<GenealogyStatisticsAgeDistributionPoint> AgeDistribution { get; }
    }

    private struct AgeDistributionBucket
    {
        public double PersonAge;
        public double GenerationGap;
        public double DeathAge;
    }

    private enum DistributionSeries
    {
        PersonAge,
        GenerationGap,
        DeathAge
    }
}
