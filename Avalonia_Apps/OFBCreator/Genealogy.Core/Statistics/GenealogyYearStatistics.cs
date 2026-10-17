namespace Genealogy.Statistics;

/// <summary>
/// Contains weighted demographic counts for one calendar year.
/// </summary>
public sealed class GenealogyYearStatistics
{
    /// <summary>
    /// Initializes annual statistics.
    /// </summary>
    /// <param name="year">The calendar year represented.</param>
    /// <param name="livingPeople">The expected number of people alive on July 1.</param>
    /// <param name="births">The expected number of births during the year.</param>
    /// <param name="deaths">The expected number of deaths during the year.</param>
    /// <param name="familyFoundings">The expected number of family founding events during the year.</param>
    /// <param name="averageAgeYears">The expected average age of living people in the year.</param>
    /// <param name="averageDeathAgeYears">The expected average age at death in the year.</param>
    /// <param name="averageGenerationGapYears">The expected average generation gap for births in the year.</param>
    /// <param name="ageObservationCount">The number of age observations contributing to the yearly age value.</param>
    /// <param name="deathAgeObservationCount">The number of death-age observations contributing to the yearly death-age value.</param>
    /// <param name="generationGapObservationCount">The number of generation-gap observations contributing to the yearly generation-gap value.</param>
    internal GenealogyYearStatistics(
        int year,
        double livingPeople,
        double births,
        double deaths,
        double familyFoundings,
        double averageAgeYears,
        double averageDeathAgeYears,
        double averageGenerationGapYears,
        double ageObservationCount,
        double deathAgeObservationCount,
        double generationGapObservationCount)
    {
        Year = year;
        LivingPeople = livingPeople;
        Births = births;
        Deaths = deaths;
        FamilyFoundings = familyFoundings;
        AverageAgeYears = averageAgeYears;
        AverageDeathAgeYears = averageDeathAgeYears;
        AverageGenerationGapYears = averageGenerationGapYears;
        AgeObservationCount = ageObservationCount;
        DeathAgeObservationCount = deathAgeObservationCount;
        GenerationGapObservationCount = generationGapObservationCount;
    }

    /// <summary>
    /// Gets the calendar year represented by this entry.
    /// </summary>
    public int Year { get; }

    /// <summary>
    /// Gets the expected number of people alive on July 1 of the year.
    /// </summary>
    public double LivingPeople { get; }

    /// <summary>
    /// Gets the expected number of births during the year.
    /// </summary>
    public double Births { get; }

    /// <summary>
    /// Gets the expected number of deaths during the year.
    /// </summary>
    public double Deaths { get; }

    /// <summary>Gets the expected number of family founding events during the year.</summary>
    public double FamilyFoundings { get; }

    /// <summary>
    /// Gets the expected average age of living people in the year.
    /// </summary>
    public double AverageAgeYears { get; }

    /// <summary>
    /// Gets the expected average age at death in the year.
    /// </summary>
    public double AverageDeathAgeYears { get; }

    /// <summary>
    /// Gets the expected average generation gap for births in the year.
    /// </summary>
    public double AverageGenerationGapYears { get; }

    /// <summary>
    /// Gets the number of age observations contributing to the yearly age value.
    /// </summary>
    public double AgeObservationCount { get; }

    /// <summary>
    /// Gets the number of death-age observations contributing to the yearly death-age value.
    /// </summary>
    public double DeathAgeObservationCount { get; }

    /// <summary>
    /// Gets the number of generation-gap observations contributing to the yearly generation-gap value.
    /// </summary>
    public double GenerationGapObservationCount { get; }
}
