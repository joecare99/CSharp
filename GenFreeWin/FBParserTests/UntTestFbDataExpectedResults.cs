using System.Collections.Generic;

namespace FBParserTests;

/// <summary>
/// Provides compact expected result prefixes derived from the original Pascal parser tests.
/// </summary>
internal static class UntTestFbDataExpectedResults
{
    public static readonly IReadOnlyList<ParseResult> Gc5065Prefix =
    [
        new("ParserStartFamily", "5065", string.Empty, 0),
        new("ParserFamilyType", string.Empty, "5065", 1),
        new("ParserFamilyDate", "20.09.1855", "5065", 3),
        new("ParserFamilyPlace", "Mörtelstein", "5065", 3),
    ];

    public static readonly IReadOnlyList<ParseResult> Ak2421Prefix =
    [
        new("ParserStartFamily", "2421", string.Empty, 0),
        new("ParserFamilyType", string.Empty, "2421", 1),
        new("ParserFamilyDate", "28.12.1823", "2421", 3),
    ];
}
