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
    /// <summary>
    /// Returns the most frequent child surname, or the first available parent surname
    /// when no child surname is available.
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
                if (string.IsNullOrWhiteSpace(surname))
                    continue;

                childSurnameCounts.TryGetValue(surname, out var count);
                childSurnameCounts[surname] = count + 1;
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
        if (!string.IsNullOrWhiteSpace(husbandSurname))
            return husbandSurname;

        var wifeSurname = family.Wife?.Surname?.Trim();
        return string.IsNullOrWhiteSpace(wifeSurname) ? "Unbekannt" : wifeSurname;
    }
}
