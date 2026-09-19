using Microsoft.Extensions.Logging;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Abstractions.Models;

/// <summary>
/// Generates a property/estate index from selected OFB families.
/// Extracts Property-type facts (EFactType.Property) from all persons, groups by normalized label, and links to unique persons/families.
/// </summary>
public class PropertyIndexGenerator : IPropertyIndexGenerator
{
    private readonly ILogger<PropertyIndexGenerator>? _logger;

    public PropertyIndexGenerator(ILogger<PropertyIndexGenerator>? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<OFBIndexEntry>> GenerateAsync(
        IReadOnlyList<OFBFamilyModel> selectedFamilies,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(selectedFamilies);

        // Phase 1: Extract Property-type facts from all persons across selected families
        var propertyMap = new SortedDictionary<string, (string DisplayName, HashSet<string> FamilyRefs)>(StringComparer.Ordinal);

        foreach (var family in selectedFamilies)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var personsToCheck = new List<IGenPerson>();

            if (family.Husband != null)
                personsToCheck.Add(family.Husband);

            if (family.Wife != null)
                personsToCheck.Add(family.Wife);

            if (family.Children != null)
                personsToCheck.AddRange(family.Children.Where(c => c != null)!);

            foreach (var person in personsToCheck)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Get facts from the MainEntity/IHasOwner chain
                var facts = GetFacts(person);

                foreach (var fact in facts)
                {
                    if (fact == null || fact.eFactType != GenInterfaces.Data.EFactType.Property)
                        continue;

                    var data = !string.IsNullOrEmpty(fact.Data) ? fact.Data.Trim() : null;

                    if (string.IsNullOrWhiteSpace(data))
                        continue;

                    // Normalize: lowercase for sort key, preserve original display
                    var normalized = data.ToLowerInvariant();

                    if (!propertyMap.TryGetValue(normalized, out var entryData))
                    {
                        entryData = (DisplayName: data, new HashSet<string>(StringComparer.Ordinal));
                        propertyMap[normalized] = entryData;
                    }

                    // Add family reference (deduplicated per property entry)
                    entryData.FamilyRefs.Add(family.GlobalNumber);
                }
            }
        }

        // Phase 2: Build sorted entries from normalized map
        var entries = new List<OFBIndexEntry>(propertyMap.Count);

        foreach (var kvp in propertyMap.OrderBy(k => k.Key, StringComparer.Ordinal))
        {
            // Use first family reference as primary ref (sorted alphabetically)
            string primaryRef = kvp.Value.FamilyRefs.Any()
                ? kvp.Value.FamilyRefs.Min(StringComparer.Ordinal)
                : string.Empty;

            entries.Add(new OFBIndexEntry
            {
                SortKey = kvp.Key,
                Name = kvp.Value.DisplayName,
                Ref = primaryRef,
            });
        }

        _logger?.LogInformation("Generated {Count} property index entries from {FamilyCount} families.",
            entries.Count, selectedFamilies.Count);

        return entries.AsReadOnly();
    }

    /// <summary>
    /// Gets all facts from a person (IGenPerson inherits IGenEntity.Facts directly).
    /// Returns empty array if the Facts collection is null.
    /// </summary>
    private static IList<IGenFact?> GetFacts(IGenPerson person)
    {
        if (person == null || person.Facts == null)
            return Array.Empty<IGenFact?>();

        return person.Facts;
    }
}
