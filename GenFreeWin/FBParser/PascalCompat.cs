using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBParser;

/// <summary>
/// Provides small Pascal-compatible helper functions that simplify the direct port of the original parser.
/// </summary>
public static class PascalCompat
{
    /// <summary>
    /// Represents the platform newline used by the original Pascal code.
    /// </summary>
    public static readonly string VbNewLine = Environment.NewLine;

    /// <summary>
    /// Represents the set of decimal digits.
    /// </summary>
    public static readonly HashSet<char> Ziffern = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

    /// <summary>
    /// Represents the whitespace characters used by the original helper unit.
    /// </summary>
    public static readonly HashSet<char> WhiteSpaceChars = [' ', '\0', '\r', '\n', '\t', (char)255, (char)8239];

    /// <summary>
    /// Represents common punctuation characters.
    /// </summary>
    public static readonly HashSet<char> SatzZeichen = [' ', ',', ';', '.', ':', '?', '!', '\t'];

    /// <summary>
    /// Represents lower-case Latin letters.
    /// </summary>
    public static readonly HashSet<char> LowerCharset = Enumerable.Range('a', 26).Select(static c => (char)c).ToHashSet();

    /// <summary>
    /// Represents upper-case Latin letters.
    /// </summary>
    public static readonly HashSet<char> UpperCharset = Enumerable.Range('A', 26).Select(static c => (char)c).ToHashSet();

    /// <summary>
    /// Represents lower-case Latin letters including common German umlauts.
    /// </summary>
    public static readonly HashSet<char> LowerCharsetErw = Enumerable.Range('a', 26).Select(static c => (char)c).Concat(['ä', 'ö', 'ü', 'ß']).ToHashSet();

    /// <summary>
    /// Represents upper-case Latin letters including common German umlauts.
    /// </summary>
    public static readonly HashSet<char> UpperCharsetErw = Enumerable.Range('A', 26).Select(static c => (char)c).Concat(['Ä', 'Ö', 'Ü']).ToHashSet();

    /// <summary>
    /// Represents all letters used by the original parser.
    /// </summary>
    public static readonly HashSet<char> Charset = Enumerable.Range('a', 26).Select(static c => (char)c)
        .Concat(Enumerable.Range('A', 26).Select(static c => (char)c))
        .Concat(['ä', 'ö', 'ü', 'Ä', 'Ö', 'Ü', 'ß', 'é'])
        .ToHashSet();

    /// <summary>
    /// Represents all alphanumeric characters used by the parser.
    /// </summary>
    public static readonly HashSet<char> AlphaNum = Ziffern.Concat(Charset).ToHashSet();

    /// <summary>
    /// Returns the one-based character at the specified position.
    /// </summary>
    public static char CharAt(string text, int positionOneBased) => text[positionOneBased - 1];

    /// <summary>
    /// Determines whether the specified one-based position exists.
    /// </summary>
    public static bool HasCharAt(string text, int positionOneBased) => positionOneBased >= 1 && positionOneBased <= text.Length;

    /// <summary>
    /// Returns a Pascal-like substring using a one-based start index.
    /// </summary>
    public static string Copy(string text, int startOneBased, int length = int.MaxValue)
    {
        if (string.IsNullOrEmpty(text) || startOneBased > text.Length || length <= 0)
        {
            return string.Empty;
        }

        var startIndex = Math.Max(0, startOneBased - 1);
        var maxLength = Math.Min(length, text.Length - startIndex);
        return text.Substring(startIndex, maxLength);
    }

    /// <summary>
    /// Returns the leftmost portion of a string.
    /// </summary>
    public static string Left(string text, int length)
    {
        if (string.IsNullOrEmpty(text) || length <= 0)
        {
            return string.Empty;
        }

        if (length >= text.Length)
        {
            return text;
        }

        return text[..length];
    }

    /// <summary>
    /// Returns the rightmost portion of a string.
    /// </summary>
    public static string Right(string text, int length)
    {
        if (string.IsNullOrEmpty(text) || length <= 0)
        {
            return string.Empty;
        }

        if (length >= text.Length)
        {
            return text;
        }

        return text[^length..];
    }

    /// <summary>
    /// Performs a Pascal-like <c>Pos</c> search and returns a one-based index, or zero when not found.
    /// </summary>
    public static int Pos(string value, string text, int startOneBased = 1)
    {
        var index = text.IndexOf(value, Math.Max(0, startOneBased - 1), StringComparison.Ordinal);
        return index < 0 ? 0 : index + 1;
    }

    /// <summary>
    /// Removes characters from the start of the string.
    /// </summary>
    public static string RemoveStart(string text, int count)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        if (count <= 0)
        {
            return text;
        }

        if (count >= text.Length)
        {
            return string.Empty;
        }

        return text[count..];
    }

    /// <summary>
    /// Returns the last zero-based index of any matching character from the supplied set.
    /// </summary>
    public static int LastIndexOfAny(string text, IEnumerable<char> chars)
    {
        var set = chars is HashSet<char> hashSet ? hashSet : chars.ToHashSet();
        for (var index = text.Length - 1; index >= 0; index--)
        {
            if (set.Contains(text[index]))
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns the first zero-based index of any matching character from the supplied set.
    /// </summary>
    public static int IndexOfAny(string text, IEnumerable<char> chars)
    {
        var set = chars is HashSet<char> hashSet ? hashSet : chars.ToHashSet();
        for (var index = 0; index < text.Length; index++)
        {
            if (set.Contains(text[index]))
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns the first zero-based index of any matching substring from the supplied set.
    /// </summary>
    public static int IndexOfAny(string text, IEnumerable<string> values)
    {
        var result = -1;
        foreach (var value in values)
        {
            var index = text.IndexOf(value, StringComparison.Ordinal);
            if (index >= 0 && (result < 0 || index < result))
            {
                result = index;
            }
        }

        return result;
    }

    /// <summary>
    /// Counts occurrences of a character.
    /// </summary>
    public static int CountChar(string text, char value) => text.Count(ch => ch == value);

    /// <summary>
    /// Returns a single-quoted representation of the string.
    /// </summary>
    public static string QuotedString(string text) => $"'{text}'";

    /// <summary>
    /// Determines whether a character belongs to the provided set.
    /// </summary>
    public static bool In(char value, IEnumerable<char> chars) => chars.Contains(value);
}
