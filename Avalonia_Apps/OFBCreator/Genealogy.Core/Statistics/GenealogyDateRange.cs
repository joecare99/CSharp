using System;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;

namespace Genealogy.Statistics;

/// <summary>
/// Represents the inclusive possible dates of a genealogical date.
/// </summary>
internal readonly struct GenealogyDateRange
{
    /// <summary>
    /// Initializes a date range with inclusive boundaries.
    /// </summary>
    /// <param name="start">The earliest possible date.</param>
    /// <param name="end">The latest possible date.</param>
    private GenealogyDateRange(DateTime start, DateTime end)
    {
        Start = start.Date;
        End = end.Date;
    }

    /// <summary>
    /// Gets the earliest possible date.
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// Gets the latest possible date.
    /// </summary>
    public DateTime End { get; }

    /// <summary>
    /// Creates a finite date range from a genealogical date value.
    /// </summary>
    /// <param name="date">The date value to normalize.</param>
    /// <param name="range">The normalized range when the value is usable.</param>
    /// <returns><see langword="true"/> when a finite possible range can be determined.</returns>
    public static bool TryCreate(IGenDate? date, out GenealogyDateRange range)
    {
        range = default;
        if (date is null || date.eDateModifier == EDateModifier.Text)
        {
            return false;
        }

        var firstStart = GetStart(date.Date1, date.eDateType1);
        var firstEnd = GetEnd(date.Date1, date.eDateType1);
        var hasSecondDate = date.Date2.HasValue;
        var secondType = date.eDateType2 ?? date.eDateType1;
        var secondEnd = hasSecondDate ? GetEnd(date.Date2!.Value, secondType) : firstEnd;

        DateTime start;
        DateTime end;
        switch (date.eDateModifier)
        {
            case EDateModifier.Before:
            case EDateModifier.To:
            case EDateModifier.After:
            case EDateModifier.From:
                return false;
            case EDateModifier.Between:
            case EDateModifier.FromTo:
                if (!hasSecondDate)
                {
                    return false;
                }
                start = firstStart;
                end = secondEnd;
                break;
            default:
                start = firstStart;
                end = hasSecondDate ? secondEnd : firstEnd;
                break;
        }

        if (start > end)
        {
            (start, end) = (end, start);
        }

        range = new GenealogyDateRange(start, end);
        return true;
    }

    /// <summary>
    /// Gets the earliest possible date at the specified precision.
    /// </summary>
    /// <param name="date">The date value.</param>
    /// <param name="type">The precision of the date.</param>
    /// <returns>The inclusive lower boundary.</returns>
    private static DateTime GetStart(DateTime date, EDateType type)
    {
        return type switch
        {
            EDateType.Year => new DateTime(date.Year, 1, 1),
            EDateType.MonthYear => new DateTime(date.Year, date.Month, 1),
            _ => date.Date
        };
    }

    /// <summary>
    /// Gets the latest possible date at the specified precision.
    /// </summary>
    /// <param name="date">The date value.</param>
    /// <param name="type">The precision of the date.</param>
    /// <returns>The inclusive upper boundary.</returns>
    private static DateTime GetEnd(DateTime date, EDateType type)
    {
        return type switch
        {
            EDateType.Year => new DateTime(date.Year, 12, 31),
            EDateType.MonthYear => new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)),
            _ => date.Date
        };
    }
}