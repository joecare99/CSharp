using System;
using System.Collections.Generic;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Abstractions.Models;

/// <summary>
/// Model representing a single person entry in the OFB person index.
/// Contains structured data for cross-referencing and life event documentation.
/// </summary>
public sealed class PersonIndexEntry
{
    /// <summary>
    /// Primary surname of the person (for alphabetical sorting).
    /// </summary>
    public string SortKey { get; init; } = default!;

    /// <summary>
    /// Full name display string (e.g., "Müller, Hans").
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// Birth date of the person. May be null if unknown.
    /// </summary>
    public IGenDate? BirthDate { get; init; }

    /// <summary>
    /// Birth place of the person. May be null if unknown.
    /// </summary>
    public IGenPlace? BirthPlace { get; init; }

    /// <summary>
    /// Death date of the person. May be null if unknown.
    /// </summary>
    public IGenDate? DeathDate { get; init; }

    /// <summary>
    /// Death place of the person. May be null if unknown.
    /// </summary>
    public IGenPlace? DeathPlace { get; init; }

    private readonly List<string> _parentFamilyNumbers = new();
    private readonly List<string> _spouseFamilyNumbers = new();

    /// <summary>
    /// Family numbers of parent families this person belongs to.
    /// Typically contains a single family number (the family of birth).
    /// </summary>
    public IReadOnlyList<string> ParentFamilyNumbers => _parentFamilyNumbers.AsReadOnly();

    /// <summary>
    /// Family numbers of spouse families (marriage families) this person belongs to.
    /// May contain multiple entries for remarriages.
    /// </summary>
    public IReadOnlyList<string> SpouseFamilyNumbers => _spouseFamilyNumbers.AsReadOnly();

    /// <summary>
    /// Adds a parent family number reference. Used internally during construction.
    /// Skips duplicate assignments automatically.
    /// </summary>
    /// <remarks>This method is for use only by internal OFB pipeline components.</remarks>
    public void AddParentFamilyNumber(string familyNumber)
    {
        if (!_parentFamilyNumbers.Contains(familyNumber))
        {
            _parentFamilyNumbers.Add(familyNumber);
            _parentFamilyNumbers.Sort(StringComparer.Ordinal);
        }
    }

    /// <summary>
    /// Adds a spouse family number reference. Used internally during construction.
    /// Skips duplicate assignments automatically.
    /// </summary>
    /// <remarks>This method is for use only by internal OFB pipeline components.</remarks>
    public void AddSpouseFamilyNumber(string familyNumber)
    {
        if (!_spouseFamilyNumbers.Contains(familyNumber))
        {
            _spouseFamilyNumbers.Add(familyNumber);
            _spouseFamilyNumbers.Sort(StringComparer.Ordinal);
        }
    }

    /// <summary>
    /// Occupations of this person. May be empty if no occupation data is available.
    /// </summary>
    public IReadOnlyList<string> Occupations { get; init; } = new List<string>().AsReadOnly();

    /// <summary>
    /// Properties/estate items associated with this person. May be empty.
    /// </summary>
    public IReadOnlyList<string> Properties { get; init; } = new List<string>().AsReadOnly();

    /// <summary>
    /// Original source reference ID (GEDCOM individual pointer).
    /// </summary>
    public string SourceRefId { get; init; } = default!;

    /// <summary>
    /// Returns a string representation for display in the person index.
    /// </summary>
    public override string ToString() => Name;
}
