using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Abstractions.Models;

/// <summary>
/// Generates a normalized occupation index from selected OFB families.
/// Groups persons by their occupation, excluding empty labels.
/// </summary>
public class OccupationIndexGenerator : IOccupationIndexGenerator
{
    private readonly ILogger<OccupationIndexGenerator>? _logger;

    public OccupationIndexGenerator(ILogger<OccupationIndexGenerator>? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<OFBIndexEntry>> GenerateAsync(
        IReadOnlyList<OFBFamilyModel> selectedFamilies,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(selectedFamilies);

        // Phase 1: Collect occupations from all persons across selected families
        var occupationMap = new SortedDictionary<string, (string DisplayName, HashSet<string> FamilyRefs)>(StringComparer.Ordinal);

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

                var occupation = !string.IsNullOrEmpty(person.Occupation) ? person.Occupation!.Trim() : null;

                if (string.IsNullOrWhiteSpace(occupation))
                    continue;

                // Normalize: lowercase for sort key, preserve original display
                var normalized = occupation.ToLowerInvariant();

                if (!occupationMap.TryGetValue(normalized, out var data))
                {
                    data = (DisplayName: occupation, new HashSet<string>(StringComparer.Ordinal));
                    occupationMap[normalized] = data;
                }

                // Add family reference (deduplicated per occupation entry)
                data.FamilyRefs.Add(family.GlobalNumber);
            }
        }

        // Phase 2: Build sorted entries from normalized map
        var entries = new List<OFBIndexEntry>(occupationMap.Count);

        foreach (var kvp in occupationMap.OrderBy(k => k.Key, StringComparer.Ordinal))
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

        _logger?.LogInformation("Generated {Count} occupation index entries from {FamilyCount} families.",
            entries.Count, selectedFamilies.Count);

        return entries.AsReadOnly();
    }
}
