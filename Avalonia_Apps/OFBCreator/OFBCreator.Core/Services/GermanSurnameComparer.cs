using System;
using System.Collections.Generic;
using System.Globalization;

namespace OFBCreator.Core.Services;

/// <summary>
/// Compares surnames using German linguistic collation with an ordinal tie-break.
/// </summary>
internal sealed class GermanSurnameComparer : IComparer<string>
{
    private static readonly CompareInfo GermanCompareInfo = CultureInfo.GetCultureInfo("de-DE").CompareInfo;
    private const CompareOptions Options = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;

    /// <summary>
    /// Compares names in German alphabetic order, then ordinally to make ties deterministic.
    /// </summary>
    public int Compare(string? left, string? right)
    {
        var germanComparison = GermanCompareInfo.Compare(left, right, Options);
        return germanComparison != 0
            ? germanComparison
            : StringComparer.Ordinal.Compare(left, right);
    }
}
