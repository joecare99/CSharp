using System;
using System.Globalization;

namespace OFBCreator.Core.Services;

/// <summary>
/// Parses dates expressed with a GEDCOM calendar escape into Gregorian dates.
/// </summary>
internal static class GedcomCalendarDateParser
{
    private const string FrenchRepublicanEscape = "@#DFRENCH R@";
    private static readonly string[] GregorianFormats =
    [
        "d MMM yyyy", "dd MMM yyyy", "d.M.yyyy", "dd.MM.yyyy", "d/M/yyyy", "dd/MM/yyyy",
        "MMM yyyy", "MMMM yyyy", "yyyy"
    ];
    private static readonly DateTime[] FrenchRepublicanYearStarts = CreateFrenchRepublicanYearStarts();

    /// <summary>
    /// Parses a GEDCOM date using supported calendar escapes and Gregorian formats.
    /// </summary>
    /// <param name="value">The GEDCOM date text without its modifier.</param>
    /// <param name="date">The Gregorian date when parsing succeeds.</param>
    /// <returns><see langword="true"/> when the date is supported and valid.</returns>
    public static bool TryParseDate(string value, out DateTime date)
    {
        return TryParseFrenchRepublicanDate(value, out date) ||
            DateTime.TryParseExact(
                value.Trim(),
                GregorianFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces,
                out date);
    }

    /// <summary>
    /// Converts a French Republican calendar date to its Gregorian equivalent.
    /// </summary>
    /// <param name="value">The GEDCOM date text, including its calendar escape.</param>
    /// <param name="date">The converted Gregorian date when parsing succeeds.</param>
    /// <returns><see langword="true"/> when the value is a supported French Republican date.</returns>
    public static bool TryParseFrenchRepublicanDate(string value, out DateTime date)
    {
        date = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim();
        if (!normalized.StartsWith(FrenchRepublicanEscape, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var dateParts = normalized[FrenchRepublicanEscape.Length..]
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (dateParts.Length != 3 ||
            !int.TryParse(dateParts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var day) ||
            !TryParseFrenchRepublicanMonth(dateParts[1], out var month) ||
            !TryParseFrenchRepublicanYear(dateParts[2], out var year))
        {
            return false;
        }

        if (year < 1 || year >= FrenchRepublicanYearStarts.Length || day < 1)
        {
            return false;
        }

        var daysInMonth = month == 13
            ? (IsFrenchRepublicanLeapYear(year) ? 6 : 5)
            : 30;
        if (day > daysInMonth)
        {
            return false;
        }

        date = FrenchRepublicanYearStarts[year - 1].AddDays((month - 1) * 30 + day - 1);
        return true;
    }

    /// <summary>
    /// Creates the known Gregorian start dates for Republican years I through XIV.
    /// </summary>
    /// <returns>The start date for each supported Republican year.</returns>
    private static DateTime[] CreateFrenchRepublicanYearStarts()
    {
        var starts = new DateTime[15];
        starts[0] = new DateTime(1792, 9, 22);
        for (var year = 1; year < starts.Length - 1; year++)
        {
            starts[year] = starts[year - 1].AddDays(IsFrenchRepublicanLeapYear(year) ? 366 : 365);
        }

        return starts;
    }

    /// <summary>
    /// Determines whether a Republican year has six complementary days.
    /// </summary>
    /// <param name="year">The Republican year number.</param>
    /// <returns><see langword="true"/> for a known leap year.</returns>
    private static bool IsFrenchRepublicanLeapYear(int year) => year is 3 or 7 or 11;

    /// <summary>
    /// Parses a Republican month abbreviation or its French month name.
    /// </summary>
    /// <param name="value">The month token.</param>
    /// <param name="month">The one-based month number when parsing succeeds.</param>
    /// <returns><see langword="true"/> when the month is recognized.</returns>
    private static bool TryParseFrenchRepublicanMonth(string value, out int month)
    {
        var normalized = value.TrimEnd('.').ToUpperInvariant()
            .Replace("É", "E", StringComparison.Ordinal)
            .Replace("È", "E", StringComparison.Ordinal)
            .Replace("Û", "U", StringComparison.Ordinal)
            .Replace("Ô", "O", StringComparison.Ordinal);
        month = normalized switch
        {
            "VEND" or "VENDEMIAIRE" => 1,
            "BRUM" or "BRUMAIRE" => 2,
            "FRIM" or "FRIMAIRE" => 3,
            "NIVO" or "NIV" or "NIVOSE" => 4,
            "PLUV" or "PLUVIOSE" => 5,
            "VENT" or "VENTOSE" => 6,
            "GERM" or "GERMINAL" => 7,
            "FLOR" or "FLOREAL" => 8,
            "PRAI" or "PRAIRIAL" => 9,
            "MESS" or "MESSIDOR" => 10,
            "THER" or "THERMIDOR" => 11,
            "FRUC" or "FRUCTIDOR" => 12,
            "COMP" or "COMPLEMENTAIRE" => 13,
            _ => 0
        };
        return month != 0;
    }

    /// <summary>
    /// Parses an Arabic or Roman Republican year number.
    /// </summary>
    /// <param name="value">The year token.</param>
    /// <param name="year">The one-based year number when parsing succeeds.</param>
    /// <returns><see langword="true"/> when the year is recognized.</returns>
    private static bool TryParseFrenchRepublicanYear(string value, out int year)
    {
        if (int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out year))
        {
            return true;
        }

        year = value.ToUpperInvariant() switch
        {
            "I" => 1,
            "II" => 2,
            "III" => 3,
            "IV" => 4,
            "V" => 5,
            "VI" => 6,
            "VII" => 7,
            "VIII" => 8,
            "IX" => 9,
            "X" => 10,
            "XI" => 11,
            "XII" => 12,
            "XIII" => 13,
            "XIV" => 14,
            _ => 0
        };
        return year != 0;
    }
}
