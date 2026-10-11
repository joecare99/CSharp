using System;
using System.Collections.Generic;
using System.Linq;

namespace OFBCreator.Core.Services;

/// <summary>Classifies source surname values that must not be presented as family names.</summary>
internal static class SurnamePlaceholderClassifier
{
    internal const string NoNameSectionName = "Familien ohne Namen";

    internal static bool IsPlaceholder(string? surname)
    {
        var value = surname?.Trim();
        if (string.IsNullOrEmpty(value))
            return true;

        return !value.Any(char.IsLetter)
            || IsEntirelyParenthesized(value)
            || string.Equals(value, "NN", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "NA", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsEntirelyParenthesized(string value)
    {
        if (value.Length < 2 || value[0] != '(' || value[^1] != ')')
            return false;

        var depth = 0;
        for (var index = 0; index < value.Length; index++)
        {
            if (value[index] == '(')
                depth++;
            else if (value[index] == ')' && --depth < 0)
                return false;

            if (depth == 0 && index < value.Length - 1)
                return false;
        }

        return depth == 0;
    }

    internal static string GetUniqueNoNameSectionName(IEnumerable<string> existingNames)
    {
        var names = new HashSet<string>(existingNames, StringComparer.OrdinalIgnoreCase);
        if (!names.Contains(NoNameSectionName))
            return NoNameSectionName;

        for (var suffix = 2; ; suffix++)
        {
            var candidate = $"{NoNameSectionName} ({suffix})";
            if (!names.Contains(candidate))
                return candidate;
        }
    }
}
