using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace OFBCreator.Publishing.Services;

internal static class GermanPlaceHierarchyCatalog
{
    private const string ResourceSuffix = "Data.Places.german-administrative-hierarchy.json";
    private static readonly Lazy<Catalog> CachedCatalog = new(LoadCatalog);

    public static string NormalizeComponent(string name)
    {
        var catalog = CachedCatalog.Value;
        return catalog.CanonicalNames.TryGetValue(NormalizeKey(name), out var canonical)
            ? canonical
            : name;
    }

    public static IReadOnlyList<string> CompleteHierarchy(IReadOnlyList<string> rootToLeaf)
    {
        ArgumentNullException.ThrowIfNull(rootToLeaf);
        if (rootToLeaf.Count == 0)
            return Array.Empty<string>();

        var catalog = CachedCatalog.Value;
        var result = rootToLeaf.Select(NormalizeComponent).ToList();
        var countryIndex = result.FindIndex(name => catalog.CountryNames.Contains(NormalizeKey(name)));
        if (countryIndex >= 0)
        {
            result[countryIndex] = catalog.CanonicalNames[NormalizeKey(result[countryIndex])];
            return result;
        }

        if (catalog.ParentByAdministrativeUnit.TryGetValue(NormalizeKey(result[0]), out var parentCountry))
            result.Insert(0, parentCountry);

        return result;
    }

    private static Catalog LoadCatalog()
    {
        var assembly = typeof(GermanPlaceHierarchyCatalog).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .SingleOrDefault(name => name.EndsWith(ResourceSuffix, StringComparison.Ordinal))
            ?? throw new InvalidDataException($"Embedded place hierarchy resource '{ResourceSuffix}' was not found.");
        using var resource = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidDataException($"Embedded place hierarchy resource '{resourceName}' could not be opened.");
        using var document = JsonDocument.Parse(resource);

        var canonicalNames = new Dictionary<string, string>(StringComparer.Ordinal);
        var parentByAdministrativeUnit = new Dictionary<string, string>(StringComparer.Ordinal);
        var countryNames = new HashSet<string>(StringComparer.Ordinal);
        var countries = document.RootElement.GetProperty("countries");
        foreach (var country in countries.EnumerateArray())
        {
            var countryName = GetRequiredString(country, "name");
            AddNames(canonicalNames, countryName, country.GetProperty("aliases"));
            countryNames.Add(NormalizeKey(countryName));

            foreach (var administrativeUnit in country.GetProperty("administrativeUnits").EnumerateArray())
            {
                var unitName = GetRequiredString(administrativeUnit, "name");
                AddNames(canonicalNames, unitName, administrativeUnit.GetProperty("aliases"));
                parentByAdministrativeUnit[NormalizeKey(unitName)] = countryName;
                foreach (var alias in administrativeUnit.GetProperty("aliases").EnumerateArray())
                    parentByAdministrativeUnit[NormalizeKey(alias.GetString()!)] = countryName;
            }
        }

        return new Catalog(canonicalNames, parentByAdministrativeUnit, countryNames);
    }

    private static void AddNames(
        IDictionary<string, string> canonicalNames,
        string canonicalName,
        JsonElement aliases)
    {
        canonicalNames[NormalizeKey(canonicalName)] = canonicalName;
        foreach (var alias in aliases.EnumerateArray())
        {
            var aliasName = alias.GetString()
                ?? throw new InvalidDataException("Place hierarchy aliases must be strings.");
            canonicalNames[NormalizeKey(aliasName)] = canonicalName;
        }
    }

    private static string GetRequiredString(JsonElement element, string propertyName) =>
        element.GetProperty(propertyName).GetString()
        ?? throw new InvalidDataException($"Place hierarchy property '{propertyName}' must be a string.");

    private static string NormalizeKey(string value)
    {
        var decomposed = value.Normalize(NormalizationForm.FormD);
        var key = new StringBuilder(decomposed.Length);
        foreach (var character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
                continue;
            if (char.IsLetterOrDigit(character))
                key.Append(char.ToLowerInvariant(character));
        }
        return key.ToString();
    }

    private sealed record Catalog(
        IReadOnlyDictionary<string, string> CanonicalNames,
        IReadOnlyDictionary<string, string> ParentByAdministrativeUnit,
        IReadOnlySet<string> CountryNames);
}
