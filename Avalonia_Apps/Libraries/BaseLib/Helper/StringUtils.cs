// ***********************************************************************
// Assembly         : BaseLib
// Author           : Mir
// Created          : 03-27-2023
//
// Last Modified By : Mir
// Last Modified On : 03-27-2023
// ***********************************************************************
// <copyright file="StringUtils.cs" company="JC-Soft">
//     Copyright � JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Linq;
using BaseLib.Interfaces;

namespace BaseLib.Helper;

/// <summary>A static class with useful string-routines.</summary>
public static class StringUtils
{
    public const string AlphaUpper="ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string AlphaLower="abcdefghijklmnopqrstuvwxyz";
    public const string Alpha=AlphaUpper+AlphaLower;
    public const string Numeric="0123456789";
    public const string AlphaNumeric=Alpha+Numeric;

    /// <summary>
    ///   <para>
    /// Makes the specified string quotable.<br />-&gt; by escaping special Characters (Linefeed, NewLine, Tab ...)</para>
    ///   <para>
    ///     <br />
    ///   </para>
    /// </summary>
    /// <param name="aStr">the string .</param>
    /// <returns>Quoted/escaped string</returns>
    /// <remarks>Does the opposite of <see cref="StringUtils.UnQuote()" /><br />In other words: Puts a given text into one line of text.</remarks>
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
    /// <summary>Un-quotes the given string.<br />by un-escaping special characters (Linefeed, Newline, Tab ...)</summary>
    /// <param name="aStr">a string.</param>
    /// <returns>the unquoted/un-escaped string</returns>
    /// <remarks>Does the opposite of <see cref="StringUtils.Quote()" /><br />In other words: Takes a given line of text and extracts the (original) text.</remarks>
    public static string UnQuote(this string? aStr) 
        => (aStr ?? "")
              .Replace("\\\\", "\\\u0001")
              .Replace("\\t", "\t")
              .Replace("\\r", "\r")
              .Replace("\\n", "\n")
             .Replace("\\\u0001", "\\");

    /// <summary>
    /// Formats the specified string with par.
    /// </summary>
    /// <param name="aStr">a string.</param>
    /// <param name="par">The par.</param>
    /// <returns>System.String.</returns>
    /// <autogeneratedoc />
    public static string Format(this string aStr, params object[] par)
        => string.Format(aStr, par);

    /// <summary>
    /// Gets the first part of the string separated by the separator.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <param name="sep">The sep.</param>
    /// <returns>System.String.</returns>
    /// <autogeneratedoc />
    public static string SFirst(this string s, string sep = " ")
    {
        if (!s.Contains(sep)) return s;
        return s.Substring(0, s.IndexOf(sep));
    }

    /// <summary>
    /// Gets the other part of the string separated by the separator.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <param name="sep">The sep.</param>
    /// <returns>System.String.</returns>
    /// <autogeneratedoc />
    public static string SRest(this string s, string sep = " ")
    {
        if (!s.Contains(sep)) return "";
        return s.Substring(s.IndexOf(sep) + 1);
    }


    /// <summary>Pads the tab of the string with spaces.</summary>
    /// <param name="s">The string.</param>
    /// <param name="offs">The offset from the start of the line.</param>
    /// <returns>System.String.</returns>
    /// <autogeneratedoc />
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

    /// <summary>Converts to "<em>Normal</em>" case. (first letter to upper- rest to lowercase) </summary>
    /// <param name="s">The string.</param>
    /// <returns>System.String.</returns>
    ///   <example>e.G: <strong>pEtEr</strong> will be converted to <strong>Peter.</strong></example>
    public static string ToNormal(this string s)
        => string.IsNullOrEmpty(s) ? s : s.Substring(0, 1).ToUpper() + s.Remove(0, 1).ToLower();

    /// <summary>Does a quoted split of the given string.</summary>
    /// <param name="Data">The data.</param>
    /// <param name="Separator">The separator.</param>
    /// <param name="QuoteMark">The quotation mark.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    /// <autogeneratedoc />
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

    public static bool EndswithAny(this string s,params string[] strings)
    {
        foreach (var item in strings)
            if ((" "+s).EndsWith(" "+item))
                return true;
        return false;
    }

    public static bool ContainsAny(this string s, params string[] strings)
    {
        foreach (var item in strings)
            if (s.Contains(item))
                return true;
        return false;
    }
    public static bool StartswithAny(this string s, params string[] strings)
    {
        foreach (var item in strings)
            if ((s+" ").StartsWith(item+" "))
                return true;
        return false;
    }

    public static bool IsValidIdentifyer(this string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return false;
        var _s = s!.ToUpper();
        if (!AlphaUpper.Contains(_s[0])) return false;
        foreach (var c in _s)
            if (!(AlphaNumeric+"_").Contains(c)) return false;
        return true;
    }

    public static string Left(this string data, int iCnt)
    => iCnt >= 0
    ? data.Substring(0, Math.Min(data.Length, iCnt))
    : data.Substring(0, Math.Max(0, data.Length + iCnt));

    public static string Right(this string data, int iCnt)
        => iCnt >= 0
        ? data.Substring(Math.Max(0, data.Length - iCnt))
        : data.Substring(Math.Min(data.Length, -iCnt));

    public static string AsString(this object? data,string? format =null)
    => data switch
    {
        string s => s,
        IHasValue f => f.Value.AsString(),
        null => "",
        object o => o.ToString() ?? "",
    };

    public static IList<string> IntoString(this string[] asData, IList<string> asKont = null, int offs = 0)
    {
        asKont ??= new string[Math.Max(0, asData.Length + offs)];
        for (var i = 0; i < asData.Length; i++)
            if (i + offs >= 0 && i + offs < asKont.Count)
                asKont[i + offs] = asData[i];
        return asKont;
    }

}
