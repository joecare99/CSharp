// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
// ***********************************************************************
// <copyright file="VBCompatibilityHelper.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Bridge utilities for VB.NET compatibility in Modern C#</summary>
// ***********************************************************************

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GenFreeWin.ViewModels
{
    /// <summary>
    /// Helper class to bridge legacy VB.NET APIs with Modern C# equivalents.
    /// Provides wrappers for common VB functions used in NamenSuchViewModel.
    /// Simplifies migration of VB code patterns to idiomatic C#.
    /// </summary>
    public static class VBCompatibilityHelper
    {
        /// <summary>
        /// VB.NET Strings.Asc() equivalent: Get ASCII/Unicode value of first character.
        /// </summary>
        /// <param name="str">String to get character code from.</param>
        /// <returns>Integer ASCII/Unicode value, or 0 if empty.</returns>
        public static int AsciiValue(string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0;
            return Convert.ToInt32(str[0]);
        }

        /// <summary>
        /// VB.NET Strings.Chr() equivalent: Get character from ASCII/Unicode value.
        /// </summary>
        /// <param name="code">ASCII/Unicode value.</param>
        /// <returns>Single character string, or empty on error.</returns>
        public static string CharFromCode(int code)
        {
            try
            {
                return ((char)code).ToString();
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// VB.NET Strings.Len() equivalent: Get string length.
        /// </summary>
        public static int StringLength(string str) => str?.Length ?? 0;

        /// <summary>
        /// VB.NET Mid() equivalent with optional replacement (Mutate style).
        /// Returns substring starting at position.
        /// </summary>
        /// <param name="str">Source string.</param>
        /// <param name="start">1-based starting position (VB style).</param>
        /// <param name="length">Optional length; if omitted, returns to end.</param>
        /// <returns>Substring.</returns>
        public static string Mid(string str, int start, int? length = null)
        {
            if (string.IsNullOrEmpty(str))
                return "";

            // VB uses 1-based indexing
            var index = Math.Max(0, start - 1);
            if (index >= str.Length)
                return "";

            if (length.HasValue && length.Value > 0)
                return str.Substring(index, Math.Min(length.Value, str.Length - index));

            return str.Substring(index);
        }

        /// <summary>
        /// VB.NET Left() equivalent: Get first N characters.
        /// </summary>
        public static string Left(string str, int length)
        {
            if (string.IsNullOrEmpty(str) || length <= 0)
                return "";
            return str.Substring(0, Math.Min(length, str.Length));
        }

        /// <summary>
        /// VB.NET Right() equivalent: Get last N characters.
        /// </summary>
        public static string Right(string str, int length)
        {
            if (string.IsNullOrEmpty(str) || length <= 0)
                return "";
            var startIndex = Math.Max(0, str.Length - length);
            return str.Substring(startIndex);
        }

        /// <summary>
        /// Converts number to formatted string (VB Format() style).
        /// Simple format codes: "0" = no decimals, "0.00" = 2 decimals, "000" = zero-padded.
        /// </summary>
        public static string Format(double number, string formatCode)
        {
            try
            {
                return number.ToString(formatCode);
            }
            catch
            {
                return number.ToString();
            }
        }

        /// <summary>
        /// Parses date with VB-style format (handles MM/DD/YYYY, DD.MM.YYYY, etc.).
        /// </summary>
        public static DateTime? ParseDate(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
                return null;

            // Try common German format first (DD.MM.YYYY)
            if (DateTime.TryParseExact(dateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var result))
                return result;

            // Try US format (MM/DD/YYYY)
            if (DateTime.TryParseExact(dateStr, "MM/dd/yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out result))
                return result;

            // Try ISO format (YYYY-MM-DD)
            if (DateTime.TryParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out result))
                return result;

            // Fallback to TryParse
            if (DateTime.TryParse(dateStr, CultureInfo.CurrentCulture, DateTimeStyles.None, out result))
                return result;

            return null;
        }

        /// <summary>
        /// Escapes wildcards in search patterns for regex usage.
        /// Converts * → .* for regex, escapes other special chars.
        /// </summary>
        public static string EscapeWildcards(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return "";

            // Escape regex special chars except *
            var escaped = Regex.Escape(pattern);

            // Unescape * and convert to .*
            escaped = escaped.Replace("\\*", ".*");

            return escaped;
        }

        /// <summary>
        /// Checks if string matches wildcard pattern (case-insensitive).
        /// * = any characters, ? = single character.
        /// </summary>
        public static bool WildcardMatch(string text, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return true;
            if (string.IsNullOrEmpty(text))
                return pattern == "*";

            var regex = "^" + Regex.Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".")
                + "$";

            return Regex.IsMatch(text, regex, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Converts VB-style comparison operators to boolean result.
        /// Useful for porting comparison logic.
        /// </summary>
        public static int Compare(string str1, string str2)
        {
            return string.Compare(str1, str2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks whether string represents a number (Integer or Double).
        /// </summary>
        public static bool IsNumeric(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;
            return int.TryParse(str, out _) || double.TryParse(str, out _);
        }

        /// <summary>
        /// Returns "Y" or "N" based on boolean (VB Option string style).
        /// </summary>
        public static string BoolToOption(bool value) => value ? "Y" : "N";

        /// <summary>
        /// Returns boolean from "Y"/"N" string option.
        /// </summary>
        public static bool OptionToBool(string option) => option?.ToUpper() == "Y";
    }
}
