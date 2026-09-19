namespace OFBCreator.Abstractions.Models;

/// <summary>
/// Defines the precision of a GEDCOM date for display and indexing purposes.
/// </summary>
public enum OFBGEDCOMPrecision
{
    /// <summary>
    /// Exact date (e.g., "15 JAN 1890").
    /// </summary>
    Exact,

    /// <summary>
    /// Approximately known date (e.g., "ABT 15 JAN 1890", "CAL 1890").
    /// </summary>
    Approximate,

    /// <summary>
    /// Date before the given value (e.g., "BEF 15 JAN 1890").
    /// </summary>
    Before,

    /// <summary>
    /// Date after the given value (e.g., "AFT 15 JAN 1890").
    /// </summary>
    After,

    /// <summary>
    /// Estimated date (e.g., "EST 1890").
    /// </summary>
    Estimated,

    /// <summary>
    /// Range between two dates (e.g., "BET 1 JAN 1880 AND 31 DEC 1890").
    /// </summary>
    Between,

    /// <summary>
    /// Just a year known without precision.
    /// </summary>
    YearOnly
}

/// <summary>
/// Represents the origin/source of a date value in GEDCOM data.
/// </summary>
public enum OFBGEDCOMOrigin
{
    /// <summary>
    /// Directly stated in the source GEDCOM (e.g., as a MARR event date).
    /// </summary>
    SourceStatement,

    /// <summary>
    /// Calculated from other GEDCOM data (e.g., inferred birth from age at marriage).
    /// </summary>
    Calculated,

    /// <summary>
    /// Estimated by external analysis (e.g., family formation date estimation).
    /// </summary>
    Estimated,

    /// <summary>
    /// Manually added or corrected data.
    /// </summary>
    Manual
}

/// <summary>
/// Immutable model of a GEDCOM-compatible date with full provenance tracking.
/// Preserves raw values, normalized sort value, precision and estimation origin.
/// </summary>
public sealed class OFBGEDCOMDate
{
    /// <summary>
    /// Raw GEDCOM text representation (e.g., "15 JAN 1890", "ABT 1890").
    /// May be null for calculated dates without a source statement.
    /// </summary>
    public string? RawText { get; init; }

    /// <summary>
    /// Normalized sort value used for collation (e.g., "YYYY-MM-DD" or "YYYY").
    /// Always set — derived from RawText, calculated date, or estimated range.
    /// </summary>
    public string SortKey { get; init; } = default!;

    /// <summary>
    /// The calendar date if it can be resolved to a DateTime (Exact dates only).
    /// Null for approximate, before, after, between, estimated or year-only dates.
    /// </summary>
    public DateTime? DateValue { get; init; }

    /// <summary>
    /// The start of a date range (for BETWEEN precision).
    /// Only populated when Precision is Between.
    /// </summary>
    public OFBGEDCOMDate? RangeStart { get; init; }

    /// <summary>
    /// The end of a date range (for BETWEEN precision).
    /// Only populated when Precision is Between.
    /// </summary>
    public OFBGEDCOMDate? RangeEnd { get; init; }

    /// <summary>
    /// The precision level of this date.
    /// </summary>
    public OFBGEDCOMPrecision Precision { get; init; }

    /// <summary>
    /// How this date was obtained (source statement, calculation, estimation).
    /// </summary>
    public OFBGEDCOMOrigin Origin { get; init; }

    /// <summary>
    /// Creates a GEDCOM date from a resolved DateTime value.
    /// </summary>
    public static OFBGEDCOMDate FromDateTime( DateTime date, string? rawText = null, OFBGEDCOMOrigin origin = OFBGEDCOMOrigin.SourceStatement )
    {
        return new()
        {
            RawText = rawText,
            SortKey = date.ToString( "yyyy-MM-dd" ),
            DateValue = date,
            Precision = OFBGEDCOMPrecision.Exact,
            Origin = origin,
        };
    }

    /// <summary>
    /// Creates a GEDCOM date from an approximate or estimated year only.
    /// </summary>
    public static OFBGEDCOMDate FromYear( int year, string? rawText, OFBGEDCOMOrigin origin )
    {
        return new()
        {
            RawText = rawText,
            SortKey = year.ToString( "D4" ),
            DateValue = null,
            Precision = OFBGEDCOMPrecision.Estimated,
            Origin = origin,
        };
    }

    /// <summary>
    /// Creates a date range (BETWEEN ... AND ...) from start and end dates.
    /// </summary>
    public static OFBGEDCOMDate FromRange( OFBGEDCOMDate rangeStart, OFBGEDCOMDate rangeEnd, string? rawText = null, OFBGEDCOMOrigin origin = OFBGEDCOMOrigin.Calculated )
    {
        return new()
        {
            RawText = rawText,
            SortKey = $"{rangeStart.SortKey}_to_{rangeEnd.SortKey}",
            DateValue = null,
            RangeStart = rangeStart ?? throw new ArgumentNullException( nameof( rangeStart ) ),
            RangeEnd = rangeEnd ?? throw new ArgumentNullException( nameof( rangeEnd ) ),
            Precision = OFBGEDCOMPrecision.Between,
            Origin = origin,
        };
    }

    /// <summary>
    /// Returns the formatted display string suitable for OFB output.
    /// Appends precision markers (e.g., "~" for approximate).
    /// </summary>
    public string GetDisplayString()
    {
        var baseDisplay = DateValue.HasValue
            ? DateValue.Value.ToString( "dd MMM yyyy" )
            : SortKey;

        return Precision switch
        {
            OFBGEDCOMPrecision.Exact => baseDisplay,
            OFBGEDCOMPrecision.Approximate => $"~{baseDisplay}",
            OFBGEDCOMPrecision.Before => $"< {baseDisplay}",
            OFBGEDCOMPrecision.After => $"> {baseDisplay}",
            OFBGEDCOMPrecision.Estimated => $"~{baseDisplay}",
            OFBGEDCOMPrecision.Between => $"{RangeStart?.GetDisplayString()} - {RangeEnd?.GetDisplayString()}",
            _ => baseDisplay,
        };
    }

    /// <summary>
    /// Returns a boolean indicating whether this date can be treated as exact.
    /// </summary>
    public bool IsApproximate => Precision != OFBGEDCOMPrecision.Exact;
}
