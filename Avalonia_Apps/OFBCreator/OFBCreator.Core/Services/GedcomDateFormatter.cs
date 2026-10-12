using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Core.Services;

/// <summary>
/// Formats GEDCOM dates for publication while preserving their source precision.
/// </summary>
public static partial class GedcomDateFormatter
{
    private static readonly string[] DateFormats =
    [
        "d MMM yyyy", "dd MMM yyyy", "d MMMM yyyy", "dd MMMM yyyy",
        "MMM yyyy", "MMMM yyyy", "yyyy-MM-dd", "yyyy-M-d", "M/d/yyyy", "d/M/yyyy"
    ];

    /// <summary>
    /// Formats a GEDCOM date using the requested publication culture.
    /// Unrecognized free-text dates are returned unchanged.
    /// </summary>
    public static string? Format(IGenDate? date, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);
        if (date is null)
            return null;

        return FormatDateText(
            date.DateText,
            date.Date1,
            date.eDateType1,
            date.Date2,
            date.eDateType2 ?? EDateType.Full,
            date.eDateModifier,
            culture);
    }

    /// <summary>
    /// Formats a raw GEDCOM date string when no typed date model is available.
    /// </summary>
    public static string? FormatText(string? rawText, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);
        return FormatDateText(rawText, default, EDateType.Full, null, EDateType.Full, EDateModifier.None, culture);
    }

    private static string? FormatDateText(
        string? sourceText,
        DateTime firstDate,
        EDateType firstPrecision,
        DateTime? secondDate,
        EDateType secondPrecision,
        EDateModifier dateModifier,
        CultureInfo culture)
    {
        var rawText = sourceText?.Trim();
        var modifier = ReadModifier(rawText, dateModifier);
        var dateText = RemoveModifier(rawText, modifier);
        var separator = modifier == "BET" ? "AND"
            : modifier == "FROM"
                && (dateModifier.ToString().Equals("FromTo", StringComparison.OrdinalIgnoreCase)
                    || rawText?.Contains(" TO ", StringComparison.OrdinalIgnoreCase) == true)
                    ? "TO"
                    : null;

        if (separator is not null)
        {
            var parts = SplitRange(dateText, separator);
            if (parts is null && firstDate != default && secondDate.HasValue)
                parts = (string.Empty, string.Empty);
            if (parts is null)
                return rawText;

            var start = FormatDatePart(parts.Value.Start, firstDate, firstPrecision, culture);
            var end = FormatDatePart(parts.Value.End, secondDate ?? default, secondPrecision, culture);
            if (start is null || end is null)
                return rawText;

            return modifier == "BET"
                ? $"{GetWord(culture, "zwischen", "between")} {start} {GetWord(culture, "und", "and")} {end}"
                : $"{GetWord(culture, "von", "from")} {start} {GetWord(culture, "bis", "to")} {end}";
        }

        var dateValue = FormatDatePart(dateText, firstDate, firstPrecision, culture);
        if (dateValue is null)
            return rawText;

        return modifier switch
        {
            "ABT" => $"{GetWord(culture, "um", "about")} {dateValue}",
            "CAL" => $"{GetWord(culture, "berechnet", "calculated")} {dateValue}",
            "EST" => $"{GetWord(culture, "geschätzt", "estimated")} {dateValue}",
            "BEF" => $"{GetWord(culture, "vor", "before")} {dateValue}",
            "AFT" => $"{GetWord(culture, "nach", "after")} {dateValue}",
            "FROM" => $"{GetWord(culture, "von", "from")} {dateValue}",
            "TO" => $"{GetWord(culture, "bis", "to")} {dateValue}",
            _ => dateValue
        };
    }

    private static string? ReadModifier(string? rawText, EDateModifier modifier)
    {
        var rawModifier = rawText?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.ToUpperInvariant();
        if (rawModifier is "ABT" or "CAL" or "EST" or "BEF" or "AFT" or "BET" or "FROM" or "TO")
            return rawModifier;

        return modifier.ToString() switch
        {
            "About" => "ABT",
            "Calculated" => "CAL",
            "Estimated" => "EST",
            "Before" => "BEF",
            "After" => "AFT",
            "Between" => "BET",
            "From" or "FromTo" => "FROM",
            "To" => "TO",
            _ => null
        };
    }

    private static string? RemoveModifier(string? rawText, string? modifier)
    {
        if (string.IsNullOrWhiteSpace(rawText))
            return null;
        if (modifier is null)
            return rawText;

        var firstTokenEnd = rawText.IndexOfAny([' ', '\t']);
        if (firstTokenEnd < 0)
            return string.Empty;
        var firstToken = rawText[..firstTokenEnd];
        if (!string.Equals(firstToken, modifier, StringComparison.OrdinalIgnoreCase))
            return rawText;
        return rawText[(firstTokenEnd + 1)..].Trim();
    }

    private static (string Start, string End)? SplitRange(string? text, string separator)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        var parts = Regex.Split(text, $@"\s+{Regex.Escape(separator)}\s+", RegexOptions.IgnoreCase);
        return parts.Length == 2 && parts.All(part => !string.IsNullOrWhiteSpace(part))
            ? (parts[0].Trim(), parts[1].Trim())
            : null;
    }

    private static string? FormatDatePart(
        string? rawPart,
        DateTime fallbackDate,
        EDateType precision,
        CultureInfo culture)
    {
        if (!string.IsNullOrWhiteSpace(rawPart))
        {
            var normalized = rawPart.Trim();
            if (YearPattern().IsMatch(normalized))
                return normalized;
            if (DateTime.TryParseExact(
                    normalized,
                    DateFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces,
                    out var parsed))
                return FormatParsedDate(parsed, DetectPrecision(normalized, precision), culture);

            return null;
        }

        return fallbackDate == default ? null : FormatParsedDate(fallbackDate, precision, culture);
    }

    private static EDateType DetectPrecision(string rawPart, EDateType fallback)
    {
        if (YearPattern().IsMatch(rawPart))
            return EDateType.Year;

        var tokenCount = rawPart.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        if (tokenCount == 2 && !rawPart.Contains('-', StringComparison.Ordinal) && !rawPart.Contains('/', StringComparison.Ordinal))
            return EDateType.MonthYear;
        return tokenCount >= 3 || rawPart.Contains('-', StringComparison.Ordinal) || rawPart.Contains('/', StringComparison.Ordinal)
            ? EDateType.Full
            : fallback;
    }

    private static string FormatParsedDate(DateTime date, EDateType precision, CultureInfo culture) =>
        precision switch
        {
            EDateType.Year => date.ToString("yyyy", CultureInfo.InvariantCulture),
            EDateType.MonthYear => date.ToString("MMMM yyyy", culture),
            _ => date.ToString(culture.DateTimeFormat.ShortDatePattern, culture)
        };

    private static string GetWord(CultureInfo culture, string german, string english) =>
        culture.TwoLetterISOLanguageName.Equals("de", StringComparison.OrdinalIgnoreCase) ? german : english;

    [GeneratedRegex(@"^\d{1,4}$", RegexOptions.CultureInvariant)]
    private static partial Regex YearPattern();
}
