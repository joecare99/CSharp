using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OFBCreator.Core.Models;

namespace OFBCreator.Core.Services;

/// <summary>
/// Splits common German lists of occupations while keeping trailing place/employer context.
/// </summary>
public static class OccupationDesignationParser
{
    private static readonly Regex DesignationSeparator = new(
        @"\s*(?:,|&|\bund\b)\s*",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly Regex ContextSeparator = new(
        @"\s+(?<preposition>in|bei)\s+(?<context>.+)$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    /// <summary>
    /// Parses comma/conjunction-separated designations. A trailing "in" value is place context;
    /// a trailing "bei" value is employer context, not an additional occupation.
    /// </summary>
    public static IReadOnlyList<OccupationDesignation> Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<OccupationDesignation>();

        var source = value.Trim();
        var contextMatch = ContextSeparator.Match(source);
        var designationText = contextMatch.Success ? source[..contextMatch.Index].Trim() : source;
        var context = contextMatch.Success ? contextMatch.Groups["context"].Value.Trim() : null;
        var place = contextMatch.Success
            && contextMatch.Groups["preposition"].Value.Equals("in", StringComparison.OrdinalIgnoreCase)
                ? context
                : null;
        var employer = contextMatch.Success
            && contextMatch.Groups["preposition"].Value.Equals("bei", StringComparison.OrdinalIgnoreCase)
                ? context
                : null;

        var designations = DesignationSeparator.Split(designationText)
            .Select(name => name.Trim())
            .Where(name => name.Length > 0)
            .Select(name => new OccupationDesignation(name, place, employer))
            .ToArray();
        return designations;
    }
}
