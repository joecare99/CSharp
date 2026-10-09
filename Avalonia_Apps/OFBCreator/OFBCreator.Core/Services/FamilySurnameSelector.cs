using System;
using System.Collections.Generic;
using System.Linq;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Core.Services;

/// <summary>
/// Selects the single surname used as the OFB family name.
/// </summary>
internal static class FamilySurnameSelector
{
    internal static bool HasRealSurname(IGenFamily family)
    {
        ArgumentNullException.ThrowIfNull(family);
        return family.Children?.Any(child => !SurnamePlaceholderClassifier.IsPlaceholder(child?.Surname)) == true
            || !SurnamePlaceholderClassifier.IsPlaceholder(family.Husband?.Surname)
            || !SurnamePlaceholderClassifier.IsPlaceholder(family.Wife?.Surname);
    }

    /// <summary>
    /// Returns the most frequent non-placeholder child surname, or the first available real parent surname.
    /// Families without a real surname are assigned to the no-name section.
    /// </summary>
    internal static string Select(IGenFamily family)
    {
        ArgumentNullException.ThrowIfNull(family);

        var childSurnameCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (family.Children is not null)
        {
            foreach (var child in family.Children)
            {
                var surname = child?.Surname?.Trim();
                if (SurnamePlaceholderClassifier.IsPlaceholder(surname))
                    continue;

                childSurnameCounts.TryGetValue(surname!, out var count);
                childSurnameCounts[surname!] = count + 1;
            }
        }

        if (childSurnameCounts.Count > 0)
        {
            return childSurnameCounts
                .OrderByDescending(entry => entry.Value)
                .ThenBy(entry => entry.Key, StringComparer.OrdinalIgnoreCase)
                .ThenBy(entry => entry.Key, StringComparer.Ordinal)
                .First()
                .Key;
        }

        var husbandSurname = family.Husband?.Surname?.Trim();
        if (!SurnamePlaceholderClassifier.IsPlaceholder(husbandSurname))
            return husbandSurname!;

        var wifeSurname = family.Wife?.Surname?.Trim();
        return SurnamePlaceholderClassifier.IsPlaceholder(wifeSurname)
            ? SurnamePlaceholderClassifier.NoNameSectionName
            : wifeSurname!;
    }
}
