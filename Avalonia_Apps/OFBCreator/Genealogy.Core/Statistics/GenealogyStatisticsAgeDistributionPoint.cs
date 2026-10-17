namespace Genealogy.Statistics;

/// <summary>
/// Represents one age bucket for the separate distribution chart.
/// </summary>
public sealed class GenealogyStatisticsAgeDistributionPoint
{
    /// <summary>
    /// Initializes an age-distribution bucket.
    /// </summary>
    /// <param name="age">The age bucket in years.</param>
    /// <param name="personAge">The count of person ages in this bucket.</param>
    /// <param name="generationGap">The count of generation gaps in this bucket.</param>
    /// <param name="deathAge">The count of death ages in this bucket.</param>
    internal GenealogyStatisticsAgeDistributionPoint(int age, double personAge, double generationGap, double deathAge)
    {
        Age = age;
        PersonAge = personAge;
        GenerationGap = generationGap;
        DeathAge = deathAge;
    }

    /// <summary>
    /// Gets the age bucket in years.
    /// </summary>
    public int Age { get; }

    /// <summary>
    /// Gets the count of person ages in this bucket.
    /// </summary>
    public double PersonAge { get; }

    /// <summary>
    /// Gets the count of generation gaps in this bucket.
    /// </summary>
    public double GenerationGap { get; }

    /// <summary>
    /// Gets the count of death ages in this bucket.
    /// </summary>
    public double DeathAge { get; }
}
