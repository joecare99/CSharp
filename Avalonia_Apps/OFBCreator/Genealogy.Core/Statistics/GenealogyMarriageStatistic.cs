using System;
using GenInterfaces.Interfaces.Genealogic;

namespace Genealogy.Statistics;

/// <summary>
/// Describes a marriage selected by a genealogical statistic.
/// </summary>
public sealed class GenealogyMarriageStatistic
{
    /// <summary>
    /// Initializes a marriage statistic.
    /// </summary>
    /// <param name="family">The family containing the marriage.</param>
    /// <param name="possibleDateStart">The earliest possible marriage date.</param>
    /// <param name="possibleDateEnd">The latest possible marriage date.</param>
    internal GenealogyMarriageStatistic(IGenFamily family, DateTime possibleDateStart, DateTime possibleDateEnd)
    {
        Family = family;
        PossibleDateStart = possibleDateStart;
        PossibleDateEnd = possibleDateEnd;
    }

    /// <summary>
    /// Gets the family containing the marriage.
    /// </summary>
    public IGenFamily Family { get; }

    /// <summary>
    /// Gets the earliest possible marriage date.
    /// </summary>
    public DateTime PossibleDateStart { get; }

    /// <summary>
    /// Gets the latest possible marriage date.
    /// </summary>
    public DateTime PossibleDateEnd { get; }
}