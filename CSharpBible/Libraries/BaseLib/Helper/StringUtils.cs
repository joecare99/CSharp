// ***********************************************************************
// Assembly         : BaseLib
// Author           : Mir
// Created          : 03-27-2023
//
// Last Modified By : Mir
// Last Modified On : 03-27-2023
// ***********************************************************************
// <copyright file="StringUtils.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary>
// Provides a collection of utility extension methods for string manipulation,
// including quoting/unquoting, splitting, formatting, validation, and substring operations.
// </summary>
// ***********************************************************************
using BaseLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace BaseLib.Helper;

/// <summary>
/// A static class providing a comprehensive set of utility extension methods for string manipulation.
/// </summary>
/// <remarks>
/// <para>
/// This class contains various helper methods for common string operations such as:
/// </para>
/// <list type="bullet">
///   <item><description>Quoting and unquoting strings (escaping/unescaping special characters)</description></item>
///   <item><description>String formatting with parameters</description></item>
///   <item><description>Splitting strings by separators with support for quoted sections</description></item>
///   <item><description>Tab padding and normalization</description></item>
///   <item><description>Substring extraction (Left, Right)</description></item>
///   <item><description>Identifier validation</description></item>
///   <item><description>Pattern matching (StartswithAny, EndswithAny, ContainsAny)</description></item>
/// </list>
/// <para>
/// All methods are implemented as extension methods on <see cref="string"/> or related types,
/// allowing for fluent method chaining syntax.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Quoting a multi-line string
/// string quoted = "Hello\nWorld".Quote(); // Returns "Hello\\nWorld"
/// 
/// // Getting the first part of a string
/// string first = "Hello World".SFirst(); // Returns "Hello"
/// 
/// // Checking if a string is a valid identifier
/// bool valid = "MyVariable".IsValidIdentifyer(); // Returns true
/// </code>
/// </example>
public static class StringUtils
{
    /// <summary>
    /// Contains all uppercase letters of the English alphabet (A-Z).
    /// </summary>
    /// <value>The string "ABCDEFGHIJKLMNOPQRSTUVWXYZ".</value>
    public const string AlphaUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// Contains all lowercase letters of the English alphabet (a-z).
    /// </summary>
    /// <value>The string "abcdefghijklmnopqrstuvwxyz".</value>
    public const string AlphaLower = "abcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// Contains all letters of the English alphabet, both uppercase and lowercase.
    /// </summary>
    /// <value>The concatenation of <see cref="AlphaUpper"/> and <see cref="AlphaLower"/>.</value>
    public const string Alpha = AlphaUpper + AlphaLower;

    /// <summary>
    /// Contains all numeric digits (0-9).
    /// </summary>
    /// <value>The string "0123456789".</value>
    public const string Numeric = "0123456789";

    /// <summary>
    /// Contains all alphanumeric characters (letters and digits).
    /// </summary>
    /// <value>The concatenation of <see cref="Alpha"/> and <see cref="Numeric"/>.</value>
    public const string AlphaNumeric = Alpha + Numeric;

    /// <summary>
    /// Escapes special characters in a string to make it suitable for single-line representation.
    /// </summary>
    /// <param name="aStr">The input string to quote. Can be <c>null</c>.</param>
    /// <returns>
    /// A string with special characters escaped:
    /// <list type="bullet">
    ///   <item><description>Backslash (\) becomes \\</description></item>
    ///   <item><description>Tab (\t) becomes \t</description></item>
    ///   <item><description>Carriage return (\r) becomes \r</description></item>
    ///   <item><description>Line feed (\n) becomes \n</description></item>
    /// </list>
    /// Returns an empty string if the input is <c>null</c> or if an exception occurs.
    /// </returns>
    /// <remarks>
    /// This method is the inverse of <see cref="UnQuote(string?)"/>.
    /// It converts a multi-line text into a single-line representation by escaping control characters.
    /// </remarks>
    /// <example>
    /// <code>
    /// string input = "Line1\nLine2\tTabbed";
    /// string quoted = input.Quote(); // Returns "Line1\\nLine2\\tTabbed"
    /// </code>
    /// </example>
    /// <seealso cref="UnQuote(string?)"/>
    public static string Quote(this string? aStr)
    {
        try
        {
            return aStr!
                .Replace("\\", "\\\\")
                .Replace("\t", "\\t")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
        }
        catch { return ""; }
    }

    /// <summary>
    /// Unescapes special character sequences in a string, restoring the original control characters.
    /// </summary>
    /// <param name="aStr">The input string to unquote. Can be <c>null</c>.</param>
    /// <returns>
    /// A string with escape sequences converted back to their original characters:
    /// <list type="bullet">
    ///   <item><description>\\ becomes backslash (\)</description></item>
    ///   <item><description>\t becomes tab character</description></item>
    ///   <item><description>\r becomes carriage return</description></item>
    ///   <item><description>\n becomes line feed</description></item>
    /// </list>
    /// Returns an empty string if the input is <c>null</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method is the inverse of <see cref="Quote(string?)"/>.
    /// It converts a single-line escaped representation back to its original multi-line form.
    /// </para>
    /// <para>
    /// The method uses a temporary placeholder character (U+0001) to handle nested backslashes correctly.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// string escaped = "Line1\\nLine2\\tTabbed";
    /// string unquoted = escaped.UnQuote(); // Returns "Line1\nLine2\tTabbed"
    /// </code>
    /// </example>
    /// <seealso cref="Quote(string?)"/>
    public static string UnQuote(this string? aStr)
        => (aStr ?? "")
              .Replace("\\\\", "\\\u0001")
              .Replace("\\t", "\t")
              .Replace("\\r", "\r")
              .Replace("\\n", "\n")
             .Replace("\\\u0001", "\\");

    /// <summary>
    /// Formats a string using the specified parameters, similar to <see cref="string.Format(string, object[])"/>.
    /// </summary>
    /// <param name="aStr">The format string containing placeholders like {0}, {1}, etc.</param>
    /// <param name="par">An array of objects to format into the string.</param>
    /// <returns>The formatted string with placeholders replaced by the corresponding parameter values.</returns>
    /// <remarks>
    /// This is a convenience extension method that wraps <see cref="string.Format(string, object[])"/>,
    /// allowing for a more fluent syntax when formatting strings.
    /// </remarks>
    /// <example>
    /// <code>
    /// string template = "Hello, {0}! You have {1} messages.";
    /// string result = template.Format("John", 5); // Returns "Hello, John! You have 5 messages."
    /// </code>
    /// </example>
    /// <exception cref="FormatException">
    /// Thrown when the format string is invalid or when there are more placeholders than parameters.
    /// </exception>
    public static string Format(this string aStr, params object[] par)
        => string.Format(aStr, par);

    /// <summary>
    /// Extracts the first part of a string before the specified separator.
    /// </summary>
    /// <param name="s">The input string to split.</param>
    /// <param name="sep">The separator string to search for. Defaults to a single space.</param>
    /// <returns>
    /// The portion of the string before the first occurrence of the separator.
    /// If the separator is not found, returns the entire original string.
    /// </returns>
    /// <remarks>
    /// This method is useful for parsing delimited strings when only the first element is needed.
    /// For the remaining part after the separator, use <see cref="SRest(string, string)"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// string input = "key=value";
    /// string key = input.SFirst("="); // Returns "key"
    /// 
    /// string words = "Hello World Today";
    /// string firstWord = words.SFirst(); // Returns "Hello"
    /// </code>
    /// </example>
    /// <seealso cref="SRest(string, string)"/>
    public static string SFirst(this string s, string sep = " ")
    {
        if (!s.Contains(sep)) return s;
        return s.Substring(0, s.IndexOf(sep));
    }

    /// <summary>
    /// Extracts the remaining part of a string after the first occurrence of the specified separator.
    /// </summary>
    /// <param name="s">The input string to split.</param>
    /// <param name="sep">The separator string to search for. Defaults to a single space.</param>
    /// <returns>
    /// The portion of the string after the first occurrence of the separator.
    /// If the separator is not found, returns an empty string.
    /// </returns>
    /// <remarks>
    /// This method complements <see cref="SFirst(string, string)"/> for parsing delimited strings.
    /// Together, they can be used to iterate through delimited elements.
    /// </remarks>
    /// <example>
    /// <code>
    /// string input = "key=value=extra";
    /// string rest = input.SRest("="); // Returns "value=extra"
    /// 
    /// string words = "Hello World Today";
    /// string remaining = words.SRest(); // Returns "World Today"
    /// </code>
    /// </example>
    /// <seealso cref="SFirst(string, string)"/>
    public static string SRest(this string s, string sep = " ")
    {
        if (!s.Contains(sep)) return "";
        return s.Substring(s.IndexOf(sep) + 1);
    }

    /// <summary>
    /// Replaces tab characters in a string with the appropriate number of spaces to align to tab stops.
    /// </summary>
    /// <param name="s">The input string containing tab characters.</param>
    /// <param name="offs">
    /// The offset from the start of the line to use for tab stop calculation. 
    /// Defaults to 0.
    /// </param>
    /// <returns>
    /// A string with all tab characters replaced by spaces, aligned to 8-character tab stops.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Tab stops are calculated at every 8th character position, taking into account the current 
    /// position in the line plus the specified offset.
    /// </para>
    /// <para>
    /// This is useful for converting tab-formatted text to fixed-width spacing for display 
    /// in environments that don't support tab characters.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// string input = "Name\tAge\tCity";
    /// string padded = input.PadTab(); // Returns "Name    Age     City" (aligned to 8-char tabs)
    /// </code>
    /// </example>
    public static string PadTab(this string s, int offs = 0)
    {
        var _s = s.Split('\t');
        var _result = "";
        for (var i = 0; i < _s.Length; i++)
        {
            var sL = _s[i];
            _result += sL;
            if (i < _s.Length - 1)
                _result = _result.PadRight(TabLen(_result.Length, offs));
        }
        return _result;

        static int TabLen(int l, int o) => l + o + (8 - (l + o) % 8) - o;
    }

    /// <summary>
    /// Converts a string to "normal" case, where the first letter is uppercase and all subsequent letters are lowercase.
    /// </summary>
    /// <param name="s">The input string to convert.</param>
    /// <returns>
    /// The string with the first character in uppercase and all remaining characters in lowercase.
    /// Returns the original string unchanged if it is <c>null</c> or empty.
    /// </returns>
    /// <remarks>
    /// This method is useful for normalizing names or titles that may have inconsistent casing.
    /// Only the first character is capitalized; all other characters are converted to lowercase.
    /// </remarks>
    /// <example>
    /// <code>
    /// string name = "pEtEr";
    /// string normalized = name.ToNormal(); // Returns "Peter"
    /// 
    /// string allCaps = "HELLO";
    /// string result = allCaps.ToNormal(); // Returns "Hello"
    /// </code>
    /// </example>
    public static string ToNormal(this string s)
        => string.IsNullOrEmpty(s) ? s : s.Substring(0, 1).ToUpper() + s.Remove(0, 1).ToLower();

    /// <summary>
    /// Splits a string by a separator while respecting quoted sections that may contain the separator.
    /// </summary>
    /// <param name="Data">The input string to split.</param>
    /// <param name="Separator">The separator string to split by. Defaults to comma (",").</param>
    /// <param name="QuoteMark">The quotation mark string that defines quoted sections. Defaults to double quote (").</param>
    /// <returns>
    /// A <see cref="List{T}"/> of strings representing the split elements.
    /// Quoted sections are preserved as single elements even if they contain the separator.
    /// Leading and trailing whitespace is trimmed from each element.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method is particularly useful for parsing CSV data where fields may contain commas 
    /// enclosed in quotes. The quote marks themselves are removed from the resulting elements.
    /// </para>
    /// <para>
    /// If a quoted section is not properly closed, the remaining content is added as a final element.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// string csv = "John,\"Doe, Jr.\",25";
    /// List&lt;string&gt; fields = csv.QuotedSplit(); 
    /// // Returns: ["John", "Doe, Jr.", "25"]
    /// 
    /// string data = "name='John Smith';age='30'";
    /// List&lt;string&gt; parts = data.QuotedSplit(";", "'");
    /// // Returns: ["name=John Smith", "age=30"]
    /// </code>
    /// </example>
    public static List<string> QuotedSplit(this string Data, string Separator = ",", string QuoteMark = "\"")
    {
        var arPreSplit = Data.Split(new string[] { Separator }, StringSplitOptions.None);
        bool quoteMode = false;
        string quotedStr = "";
        List<string> result = new();
        foreach (var s in arPreSplit)
            if (!quoteMode)
                if (!s.TrimStart(' ').StartsWith(QuoteMark))
                    result.Add(s.Trim());
                else
                {
                    if (s.Trim().EndsWith(QuoteMark))
                        result.Add(s.Trim().Substring(0, s.Trim().Length - 1).Substring(1));
                    else
                    {
                        quoteMode = true;
                        quotedStr = s.TrimStart(' ').Substring(1);
                    }
                }
            else if (s.TrimEnd(' ').EndsWith(QuoteMark))
            {
                result.Add(quotedStr + Separator[0] + s.TrimEnd(' ').Substring(0, s.TrimEnd(' ').Length - 1));
                quoteMode = false;
                quotedStr = "";
            }
            else
            {
                quotedStr += Separator[0] + s;
            }
        if (quoteMode && !String.IsNullOrEmpty(quotedStr))
        {
            result.Add(quotedStr);
        }

        return result;
    }

    /// <summary>
    /// Determines whether the string ends with any of the specified suffixes.
    /// </summary>
    /// <param name="s">The string to check.</param>
    /// <param name="strings">An array of suffix strings to test against.</param>
    /// <returns>
    /// <c>true</c> if the string ends with any of the specified suffixes as a complete word boundary; 
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method checks for word-boundary endings by prepending a space to both the input string 
    /// and each suffix before comparison. This ensures that "test" does not match "contest" when 
    /// checking for "test" as an ending.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// bool result1 = "Hello World".EndswithAny("World", "Test"); // Returns true
    /// bool result2 = "contest".EndswithAny("test"); // Returns false (word boundary check)
    /// </code>
    /// </example>
    /// <seealso cref="StartswithAny(string, string[])"/>
    /// <seealso cref="ContainsAny(string, string[])"/>
    public static bool EndswithAny(this string s, params string[] strings)
    {
        foreach (var item in strings)
            if ((" " + s).EndsWith(" " + item))
                return true;
        return false;
    }

    /// <summary>
    /// Determines whether the string contains any of the specified substrings.
    /// </summary>
    /// <param name="s">The string to search in.</param>
    /// <param name="strings">An array of substrings to search for.</param>
    /// <returns>
    /// <c>true</c> if the string contains any of the specified substrings; 
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The search is case-sensitive. The method returns <c>true</c> as soon as 
    /// the first matching substring is found.
    /// </remarks>
    /// <example>
    /// <code>
    /// bool hasKeyword = "The quick brown fox".ContainsAny("cat", "dog", "fox"); // Returns true
    /// bool noMatch = "Hello World".ContainsAny("xyz", "123"); // Returns false
    /// </code>
    /// </example>
    /// <seealso cref="StartswithAny(string, string[])"/>
    /// <seealso cref="EndswithAny(string, string[])"/>
    public static bool ContainsAny(this string s, params string[] strings)
    {
        foreach (var item in strings)
            if (s.Contains(item))
                return true;
        return false;
    }

    /// <summary>
    /// Determines whether the string starts with any of the specified prefixes.
    /// </summary>
    /// <param name="s">The string to check.</param>
    /// <param name="strings">An array of prefix strings to test against.</param>
    /// <returns>
    /// <c>true</c> if the string starts with any of the specified prefixes as a complete word boundary; 
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method checks for word-boundary beginnings by appending a space to both the input string 
    /// and each prefix before comparison. This ensures that "test" does not match "testing" when 
    /// checking for "test" as a beginning.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// bool result1 = "Hello World".StartswithAny("Hello", "Hi"); // Returns true
    /// bool result2 = "testing".StartswithAny("test"); // Returns false (word boundary check)
    /// </code>
    /// </example>
    /// <seealso cref="EndswithAny(string, string[])"/>
    /// <seealso cref="ContainsAny(string, string[])"/>
    public static bool StartswithAny(this string s, params string[] strings)
    {
        foreach (var item in strings)
            if ((s + " ").StartsWith(item + " "))
                return true;
        return false;
    }

    /// <summary>
    /// Determines whether the specified string is a valid identifier according to specific naming rules.
    /// </summary>
    /// <param name="s">The string to validate. Can be <c>null</c>.</param>
    /// <returns>
    /// <c>true</c> if the string is a valid identifier; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// A valid identifier must meet the following criteria:
    /// </para>
    /// <list type="bullet">
    ///   <item><description>Cannot be <c>null</c>, empty, or contain only whitespace</description></item>
    ///   <item><description>Must begin with an uppercase letter (A-Z)</description></item>
    ///   <item><description>Can only contain letters (A-Z, a-z), digits (0-9), or underscores (_)</description></item>
    /// </list>
    /// <para>
    /// Note: The validation is case-insensitive for subsequent characters, but the first character 
    /// must be an alphabetic character (converted to uppercase for checking).
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// bool valid1 = "MyVariable".IsValidIdentifyer();     // Returns true
    /// bool valid2 = "my_variable_123".IsValidIdentifyer(); // Returns true
    /// bool invalid1 = "123Variable".IsValidIdentifyer();   // Returns false (starts with digit)
    /// bool invalid2 = "my-variable".IsValidIdentifyer();   // Returns false (contains hyphen)
    /// bool invalid3 = "".IsValidIdentifyer();              // Returns false (empty)
    /// </code>
    /// </example>
    public static bool IsValidIdentifyer(this string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return false;
        var _s = s!.ToUpper();
        if (!AlphaUpper.Contains(_s[0])) return false;
        foreach (var c in _s)
            if (!(AlphaNumeric + "_").Contains(c)) return false;
        return true;
    }

    /// <summary>
    /// Returns the leftmost characters of a string up to the specified count.
    /// </summary>
    /// <param name="data">The input string.</param>
    /// <param name="iCnt">
    /// The number of characters to return.
    /// <list type="bullet">
    ///   <item><description>Positive value: Returns the first <paramref name="iCnt"/> characters from the left</description></item>
    ///   <item><description>Negative value: Returns all characters except the last |<paramref name="iCnt"/>| characters</description></item>
    /// </list>
    /// </param>
    /// <returns>
    /// A substring containing the specified number of leftmost characters.
    /// If <paramref name="iCnt"/> exceeds the string length, the entire string is returned.
    /// If the result would be negative length, an empty string is returned.
    /// </returns>
    /// <remarks>
    /// This method provides Python-like string slicing behavior for the beginning of strings.
    /// </remarks>
    /// <example>
    /// <code>
    /// string text = "Hello World";
    /// 
    /// // Positive count - get first N characters
    /// string first5 = text.Left(5);    // Returns "Hello"
    /// string first20 = text.Left(20);  // Returns "Hello World" (entire string)
    /// 
    /// // Negative count - exclude last N characters
    /// string allButLast3 = text.Left(-3);  // Returns "Hello Wo" (excludes "rld")
    /// </code>
    /// </example>
    /// <seealso cref="Right(string, int)"/>
    public static string Left(this string data, int iCnt)
    => iCnt >= 0
    ? data.Substring(0, Math.Min(data.Length, iCnt))
    : data.Substring(0, Math.Max(0, data.Length + iCnt));

    /// <summary>
    /// Returns the rightmost characters of a string up to the specified count.
    /// </summary>
    /// <param name="data">The input string.</param>
    /// <param name="iCnt">
    /// The number of characters to return.
    /// <list type="bullet">
    ///   <item><description>Positive value: Returns the last <paramref name="iCnt"/> characters from the right</description></item>
    ///   <item><description>Negative value: Returns all characters starting from the |<paramref name="iCnt"/>|th position</description></item>
    /// </list>
    /// </param>
    /// <returns>
    /// A substring containing the specified number of rightmost characters.
    /// If <paramref name="iCnt"/> exceeds the string length, the entire string is returned.
    /// </returns>
    /// <remarks>
    /// This method provides Python-like string slicing behavior for the end of strings.
    /// </remarks>
    /// <example>
    /// <code>
    /// string text = "Hello World";
    /// 
    /// // Positive count - get last N characters
    /// string last5 = text.Right(5);    // Returns "World"
    /// string last20 = text.Right(20);  // Returns "Hello World" (entire string)
    /// 
    /// // Negative count - skip first N characters
    /// string skipFirst3 = text.Right(-3);  // Returns "lo World" (skips "Hel")
    /// </code>
    /// </example>
    /// <seealso cref="Left(string, int)"/>
    public static string Right(this string data, int iCnt)
        => iCnt >= 0
        ? data.Substring(Math.Max(0, data.Length - iCnt))
        : data.Substring(Math.Min(data.Length, -iCnt));

    /// <summary>
    /// Converts an object to its string representation with special handling for certain types.
    /// </summary>
    /// <param name="data">The object to convert. Can be <c>null</c>.</param>
    /// <param name="format">
    /// An optional format string. Currently unused but reserved for future implementation.
    /// </param>
    /// <returns>
    /// A string representation of the object according to the following rules:
    /// <list type="bullet">
    ///   <item><description>If <paramref name="data"/> is a <see cref="string"/>, it is returned directly</description></item>
    ///   <item><description>If <paramref name="data"/> implements <see cref="IHasValue"/>, the Value property's string representation is returned</description></item>
    ///   <item><description>If <paramref name="data"/> is <c>null</c>, an empty string is returned</description></item>
    ///   <item><description>Otherwise, <see cref="object.ToString()"/> is called, with <c>null</c> results converted to empty string</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method provides a null-safe way to convert objects to strings, with special handling 
    /// for value wrapper types that implement <see cref="IHasValue"/>.
    /// </para>
    /// <para>
    /// The <paramref name="format"/> parameter is included for API consistency but is not 
    /// currently used in the implementation.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // String passthrough
    /// string s = "Hello".AsString(); // Returns "Hello"
    /// 
    /// // Null handling
    /// object? nullObj = null;
    /// string empty = nullObj.AsString(); // Returns ""
    /// 
    /// // Object conversion
    /// int number = 42;
    /// string numStr = number.AsString(); // Returns "42"
    /// </code>
    /// </example>
    public static string AsString(this object? data, string? format = null)
    => data switch
    {
        string s => s,
        IHasValue f => f.Value.AsString(),
        null => "",
        object o => o.ToString() ?? "",
    };

    /// <summary>
    /// Copies elements from a string array into a target list of strings at a specified offset.
    /// </summary>
    /// <param name="asData">The source array of strings to copy from.</param>
    /// <param name="asKont">
    /// The target list to copy into. If <c>null</c>, a new array is created with 
    /// sufficient capacity to hold all source elements considering the offset.
    /// </param>
    /// <param name="offs">
    /// The zero-based index in the target list at which copying begins.
    /// Can be negative, which will skip the first |<paramref name="offs"/>| elements of the source array.
    /// </param>
    /// <returns>
    /// The target list with the copied elements. If <paramref name="asKont"/> was <c>null</c>, 
    /// returns the newly created array.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Only elements that fit within the bounds of the target list are copied. Elements that would 
    /// fall outside the target list boundaries (either negative indices or beyond the list count) 
    /// are silently ignored.
    /// </para>
    /// <para>
    /// When <paramref name="asKont"/> is <c>null</c>, a new string array is created with size 
    /// <c>Math.Max(0, asData.Length + offs)</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Copy into a new array
    /// string[] source = { "a", "b", "c" };
    /// IList&lt;string&gt; result = source.IntoString(); // Creates ["a", "b", "c"]
    /// 
    /// // Copy with offset into existing array
    /// string[] target = new string[5];
    /// source.IntoString(target, 1); // target becomes [null, "a", "b", "c", null]
    /// 
    /// // Copy with negative offset (skips first element of source)
    /// string[] target2 = new string[2];
    /// source.IntoString(target2, -1); // target2 becomes ["b", "c"]
    /// </code>
    /// </example>
    public static IList<string> IntoString(this string[] asData, IList<string>? asKont = null, int offs = 0)
    {
        asKont ??= new string[Math.Max(0, asData.Length + offs)];
        for (var i = 0; i < asData.Length; i++)
            if (i + offs >= 0 && i + offs < asKont.Count)
                asKont[i + offs] = asData[i];
        return asKont;
    }

    /// <summary>
    /// Finds the index of the next placeholder opening bracket ('&lt;') in the specified line that is followed by an alphabetical character.
    /// </summary>
    /// <param name="line">The source text that may contain placeholder tokens.</param>
    /// <param name="offset">The starting position in the line from which the search begins.</param>
    /// <returns>
    /// The zero-based index of the next placeholder start if one exists; otherwise, the length of the line plus one.
    /// </returns>
    public static int GetNextPlaceHolder(this string line, int offset=0)
    {
        int result = line.Length + 1;
        int i = offset;
        while (i < line.Length)
        {
            var p = line.IndexOf('<', i);
            if (p > -1 && p < line.Length - 2)
            {
                if (Alpha.Contains(line[p + 1]))
                {
                    result = p;
                    i = line.Length;
                }
                else
                    i = p + 1;
            }
            else
                i = line.Length;
        }
        return result;
    }


    /// <summary>
    /// Attempts to match a probe string against a mask containing named placeholders while collecting the values that fill those placeholders.
    /// </summary>
    /// <param name="Probe">The input text that should conform to the placeholder mask.</param>
    /// <param name="Mask">The pattern that may contain literal text and placeholder tokens of the form "&lt;Name&gt;".</param>
    /// <param name="WilldCardFill">A list that receives the resolved placeholder/value pairs when the match succeeds.</param>
    /// <param name="checkPlaceholderCharset">
    /// An optional callback that can validate whether a candidate value is acceptable for a given placeholder token; returning <c>false</c> rejects the candidate.
    /// </param>
    /// <returns>
    /// <c>true</c> if the probe string can be fully matched to the mask with consistent placeholder assignments; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The algorithm walks through the mask, matching literal segments case-insensitively and treating placeholders as flexible sections that can absorb
    /// varying amounts of text. When placeholders are bounded by literal anchors, the method tests each possible split until a consistent assignment is found.
    /// </para>
    /// <para>
    /// Any assignments that fail deeper in the recursion are rolled back to ensure <paramref name="WilldCardFill"/> only contains results from successful matches.
    /// </para>
    /// </remarks>
    public static bool TryPlaceHolderMatching(string Probe, string Mask, List<KeyValuePair<string, string>> WilldCardFill, Func<string, string, bool>? checkPlaceholderCharset = null)
    {
        if (WilldCardFill == null)
            throw new ArgumentNullException(nameof(WilldCardFill));

        Probe ??= string.Empty;
        Mask ??= string.Empty;

        return Match(Mask, Probe);


        bool Match(string currentMask, string currentProbe)
        {
            var placeholderIndex = FindPlaceholderIndex(currentMask);
            if (placeholderIndex >= currentMask.Length)
                return currentMask.Equals(currentProbe, StringComparison.OrdinalIgnoreCase);

            var literalPrefix = currentMask.Substring(0, placeholderIndex);
            if (!currentProbe.StartsWith(literalPrefix, StringComparison.OrdinalIgnoreCase))
                return false;

            var placeholderEnd = currentMask.IndexOf('>', placeholderIndex);
            if (placeholderEnd < 0)
                return false;

            var placeholderToken = currentMask.Substring(placeholderIndex, placeholderEnd - placeholderIndex + 1);
            var suffix = currentMask.Substring(placeholderEnd + 1);
            var probeRemainder = currentProbe.Substring(literalPrefix.Length);

            if (suffix.Length == 0)
                return TryAssignCandidate(probeRemainder, placeholderToken, suffix, string.Empty);

            return TryMatchWithAnchors(placeholderToken, suffix, probeRemainder);
        }

        bool TryMatchWithAnchors(string placeholderToken, string suffix, string probeRemainder)
        {
            var nextPlaceholder = FindPlaceholderIndex(suffix);
            var literalAnchor = nextPlaceholder >= suffix.Length ? suffix : suffix.Substring(0, nextPlaceholder);

            if (!string.IsNullOrEmpty(literalAnchor))
            {
                var searchIndex = 0;
                while (true)
                {
                    var anchorPos = probeRemainder.IndexOf(literalAnchor, searchIndex, StringComparison.OrdinalIgnoreCase);
                    if (anchorPos < 0)
                        break;

                    if (TryAssignCandidate(probeRemainder.Substring(0, anchorPos), placeholderToken, suffix, probeRemainder.Substring(anchorPos)))
                        return true;

                    searchIndex = anchorPos + 1;
                }

                return false;
            }

            for (var split = 0; split <= probeRemainder.Length; split++)
            {
                if (TryAssignCandidate(probeRemainder.Substring(0, split), placeholderToken, suffix, probeRemainder.Substring(split)))
                    return true;
            }

            return false;
        }

        bool TryAssignCandidate(string value, string placeholderToken, string suffix, string remainingProbe)
        {
            var trimmed = value.Trim();
            if (checkPlaceholderCharset?.Invoke(placeholderToken, trimmed) == false)
                return false;

            WilldCardFill.Add(new KeyValuePair<string, string>(placeholderToken, trimmed));
            if (Match(suffix, remainingProbe))
                return true;

            WilldCardFill.RemoveAt(WilldCardFill.Count - 1);
            return false;
        }

        int FindPlaceholderIndex(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return pattern.Length;

            var next = pattern.GetNextPlaceHolder();
            return next >= pattern.Length ? pattern.Length : next;
        }
    }

    public static string MTSpaceTrim(this string MT)
    {
        IList<char> result = [];
        char _last = ' ';
        for (var I = 0; I < MT.Length; I++)
        {
            if (MT[I] == ' ')
            {
                if (I == 0 || I == MT.Length - 1 || _last == ' ')
                    continue;

                if ((AlphaNumeric.Contains(_last) == AlphaNumeric.Contains(MT[I + 1]) && MT[I + 1] != ' ')
                   || (_last is '>' or '<')
                   || (MT[I + 1] is '>' or '<' or ':'))
                    result.Add(_last = MT[I]);
            }
            else
                result.Add(_last = MT[I]);
        }
        return string.Join("", result);
    }


}
