using System.Collections.Generic;

namespace FBParserTests;

/// <summary>
/// Provides the current callback prefixes emitted by the in-progress C# port for representative
/// original Pascal sample entries.
/// </summary>
internal static class UntTestFbDataRegressionResults
{
    public static readonly IReadOnlyList<ParseResult> Gc5065CurrentPrefix =
    [
        new("ParserStartFamily", "5065", string.Empty, 0),
        new("ParserFamilyType", string.Empty, "5065", 1),
        new("ParserFamilyDate", "20.09.1855", "5065", 3),
        new("ParserFamilyPlace", "Mörtelstein", "5065", 3),
    ];

    public static readonly IReadOnlyList<ParseResult> Ak2421CurrentPrefix =
    [
        new("ParserStartFamily", "2421", string.Empty, 0),
        new("ParserFamilyType", string.Empty, "2421", 1),
        new("ParserFamilyDate", "28.12.1823", "2421", 3),
        new("ParserFamilyPlace", "Meißenheim", "2421", 3),
        new("ParserIndiName", "Andreas Rosewich", "I2421M", 0),
    ];
}
