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
/// Generates the person index from selected, numbered OFB families.
/// Extracts all unique persons (husband, wife, children) with their life events and family links.
/// Implements deterministic sorting by surname then given name.
/// </summary>
public class PersonIndexGenerator : IPersonIndexGenerator
{
    private readonly ILogger<PersonIndexGenerator>? _logger;

    /// <summary>
    /// Creates a new instance of PersonIndexGenerator with optional logging.
    /// </summary>
    public PersonIndexGenerator(ILogger<PersonIndexGenerator>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generates person index entries from selected OFB families.
    /// </summary>
    /// <param name="selectedFamilies">Numbered families to extract persons from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Sorted list of person index entries.</returns>
    public async Task<IReadOnlyList<PersonIndexEntry>> GenerateAsync(
        IReadOnlyList<OFBFamilyModel> selectedFamilies,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(selectedFamilies);

        // Phase 1: Collect all unique persons with their context (parent/spouse family numbers)
        var personMap = new Dictionary<string, PersonIndexEntry>(StringComparer.Ordinal);

        foreach (var family in selectedFamilies)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Process husband (spouse family = formation family)
            if (family.Husband != null)
            {
                var husbandId = GetPersonId(family.Husband);
                if (!personMap.TryGetValue(husbandId, out var entry))
                {
                    entry = CreatePersonEntry(family.Husband, husbandId);
                    personMap[husbandId] = entry;
                }
                entry.AddSpouseFamilyNumber(family.GlobalNumber);
            }

            // Process wife (spouse family = formation family)
            if (family.Wife != null)
            {
                var wifeId = GetPersonId(family.Wife);
                if (!personMap.TryGetValue(wifeId, out var entry))
                {
                    entry = CreatePersonEntry(family.Wife, wifeId);
                    personMap[wifeId] = entry;
                }
                entry.AddSpouseFamilyNumber(family.GlobalNumber);
            }

            // Process children (parent family = birth family)
            if (family.Children != null)
            {
                foreach (var child in family.Children)
                {
                    if (child == null)
                        continue;

                    cancellationToken.ThrowIfCancellationRequested();

                    var childId = GetPersonId(child);
                    if (!personMap.TryGetValue(childId, out var entry))
                    {
                        entry = CreatePersonEntry(child, childId);
                        personMap[childId] = entry;
                    }
                    entry.AddParentFamilyNumber(family.GlobalNumber);
                }
            }
        }

        // Phase 2: Sort deterministically by surname (SortKey) then given name
        var sortedEntries = personMap.Values
            .OrderBy(e => e.SortKey, StringComparer.Ordinal)
            .ThenBy(e => e.Name, StringComparer.Ordinal)
            .ToList();

        _logger?.LogInformation("Generated {Count} person index entries from {FamilyCount} families.",
            sortedEntries.Count, selectedFamilies.Count);

        return sortedEntries.AsReadOnly();
    }

    /// <summary>
    /// Creates a new PersonIndexEntry from an IGenPerson.
    /// </summary>
    private static PersonIndexEntry CreatePersonEntry(IGenPerson person, string personId)
    {
        var surname = !string.IsNullOrEmpty(person.Surname) ? person.Surname : "";
        var givenName = !string.IsNullOrEmpty(person.GivenName) ? person.GivenName : "";

        // Sort key: surname for alphabetical ordering
        var sortKey = string.IsNullOrWhiteSpace(surname)
            ? (string.IsNullOrEmpty(givenName) ? "Unknown" : givenName)
            : surname;

        // Name: "Surname, GivenName" or just "GivenName" if no surname
        var name = string.IsNullOrWhiteSpace(surname)
            ? (string.IsNullOrEmpty(givenName) ? (!string.IsNullOrWhiteSpace(person.Name) ? person.Name! : "Unknown") : givenName)
            : (string.IsNullOrEmpty(givenName) ? surname : $"{surname}, {givenName}");

        return new PersonIndexEntry
        {
            SortKey = sortKey,
            Name = name,
            BirthDate = person.BirthDate,
            DeathDate = person.DeathDate,
            SourceRefId = personId,
        };
    }

    /// <summary>
    /// Extracts a unique identifier for a person (GEDCOM individual pointer like I1, I2).
    /// </summary>
    private static string GetPersonId(IGenPerson person)
    {
        // Try to get the IndRefID (GEDCOM individual pointer like "I1", "I2")
        if (!string.IsNullOrEmpty(person.IndRefID))
            return person.IndRefID!;

        // Fallback: use the Name as ID
        return !string.IsNullOrEmpty(person.Name) ? person.Name : $"UNKNOWN_{Guid.NewGuid():N}";
    }
}
