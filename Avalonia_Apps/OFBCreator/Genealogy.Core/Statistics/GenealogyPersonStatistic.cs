using System;
using GenInterfaces.Interfaces.Genealogic;

namespace Genealogy.Statistics;

/// <summary>
/// Describes a person selected by a genealogical statistic.
/// </summary>
public sealed class GenealogyPersonStatistic
{
    /// <summary>
    /// Initializes a person statistic.
    /// </summary>
    /// <param name="person">The selected person.</param>
    /// <param name="possibleBirthStart">The earliest possible birth date.</param>
    /// <param name="possibleBirthEnd">The latest possible birth date.</param>
    /// <param name="estimatedAgeYears">The known maximum or estimated age in years.</param>
    internal GenealogyPersonStatistic(IGenPerson person, DateTime possibleBirthStart, DateTime possibleBirthEnd, double? estimatedAgeYears = null)
    {
        Person = person;
        PossibleBirthStart = possibleBirthStart;
        PossibleBirthEnd = possibleBirthEnd;
        EstimatedAgeYears = estimatedAgeYears;
    }

    /// <summary>
    /// Gets the selected person through the public genealogy contract.
    /// </summary>
    public IGenPerson Person { get; }

    /// <summary>
    /// Gets the earliest possible birth date.
    /// </summary>
    public DateTime PossibleBirthStart { get; }

    /// <summary>
    /// Gets the latest possible birth date.
    /// </summary>
    public DateTime PossibleBirthEnd { get; }

    /// <summary>
    /// Gets the known maximum age or estimated age in years, when calculable.
    /// </summary>
    public double? EstimatedAgeYears { get; }
}