using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Abstractions.Models;

namespace OFBCreator.Core.Services;

/// <summary>
/// Generates an alphabetical place index from selected OFB families.
/// Extracts all unique places (marriage, birth, death) and maps them to family references.
/// </summary>
public class AlphabeticalPlaceIndexGenerator : IAlphabeticalPlaceIndexGenerator
{
    private readonly ILogger<AlphabeticalPlaceIndexGenerator>? _logger;

    public AlphabeticalPlaceIndexGenerator(ILogger<AlphabeticalPlaceIndexGenerator>? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<OFBIndexEntry>> GenerateAsync(
        IReadOnlyList<OFBFamilyModel> selectedFamilies,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(selectedFamilies);

        // Phase 1: Collect all unique places with their display names and family references
        var placeMap = new SortedDictionary<string, (string DisplayName, HashSet<string> FamilyRefs)>(StringComparer.Ordinal);

        foreach (var family in selectedFamilies)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Collect places from marriage, husband's birth, wife's birth, and children's births
            var placesToProcess = new List<IGenPlace?>();

            if (family.MarriagePlace != null)
                placesToProcess.Add(family.MarriagePlace);

            if (family.Husband?.BirthPlace != null)
                placesToProcess.Add(family.Husband.BirthPlace);

            if (family.Wife?.BirthPlace != null)
                placesToProcess.Add(family.Wife.BirthPlace);

            if (family.Children != null)
            {
                foreach (var child in family.Children)
                {
                    if (child != null && child.BirthPlace != null)
                        placesToProcess.Add(child.BirthPlace);
                }
            }

            // Process death places for adults
            if (family.Husband?.DeathPlace != null)
                placesToProcess.Add(family.Husband.DeathPlace);

            if (family.Wife?.DeathPlace != null)
                placesToProcess.Add(family.Wife.DeathPlace);

            foreach (var place in placesToProcess)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var placeName = !string.IsNullOrEmpty(place?.Name) ? place.Name.Trim() : null;

                if (string.IsNullOrWhiteSpace(placeName))
                    continue;

                // Normalize: lowercase for sort key, preserve original display name
                var normalized = placeName.ToLowerInvariant();

                if (!placeMap.TryGetValue(normalized, out var data))
                {
                    data = (DisplayName: placeName, new HashSet<string>(StringComparer.Ordinal));
                    placeMap[normalized] = data;
                }

                // Add family reference (deduplicated per place entry)
                data.FamilyRefs.Add(family.GlobalNumber);
            }
        }

        // Phase 2: Build sorted entries from normalized map
        var entries = new List<OFBIndexEntry>(placeMap.Count);

        foreach (var kvp in placeMap.OrderBy(k => k.Key, StringComparer.Ordinal))
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

        _logger?.LogInformation("Generated {Count} alphabetical place index entries from {FamilyCount} families.",
            entries.Count, selectedFamilies.Count);

        return entries.AsReadOnly();
    }
}
