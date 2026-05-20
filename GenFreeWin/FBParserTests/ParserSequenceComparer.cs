using System;
using System.Collections.Generic;
using System.Linq;

namespace FBParserTests;

/// <summary>
/// Provides helpers to compare parser callback sequences for regression analysis.
/// </summary>
internal static class ParserSequenceComparer
{
    /// <summary>
    /// Finds the first mismatch between the expected and actual callback sequences.
    /// </summary>
    /// <param name="expected">The expected parser results.</param>
    /// <param name="actual">The actual parser results.</param>
    /// <returns>The first mismatch, or <see langword="null"/> when both sequences are equal.</returns>
    public static ParserSequenceMismatch? FindFirstMismatch(IReadOnlyList<ParseResult> expected, IReadOnlyList<ParseResult> actual)
    {
        var length = Math.Max(expected.Count, actual.Count);
        for (var index = 0; index < length; index++)
        {
            var hasExpected = index < expected.Count;
            var hasActual = index < actual.Count;
            ParseResult? expectedItem = hasExpected ? expected[index] : null;
            ParseResult? actualItem = hasActual ? actual[index] : null;
            if (!hasExpected || !hasActual || !Equals(expectedItem, actualItem))
            {
                return new ParserSequenceMismatch(index, expectedItem, actualItem);
            }
        }

        return null;
    }

    /// <summary>
    /// Filters debug trace entries from the supplied parser result sequence.
    /// </summary>
    /// <param name="results">The parser results.</param>
    /// <returns>The filtered result list.</returns>
    public static IReadOnlyList<ParseResult> WithoutDebugMessages(IReadOnlyList<ParseResult> results)
        => results.Where(static result => result.EventType != "ParserDebugMsg").ToList();
}
