using System;
using System.Collections.Generic;

namespace OFBCreator.Console.Models;

/// <summary>
/// Typed data exposed to family-entry templates.
/// </summary>
public sealed class FamilyEntryTemplateModel
{
    public required string Number { get; init; }

    public required string Anchor { get; init; }

    public required string Union { get; init; }

    public string MarriageMark { get; init; } = string.Empty;

    public string MarriageDate { get; init; } = string.Empty;

    public string MarriagePlace { get; init; } = string.Empty;

    public string MarriagePlaceAnchor { get; init; } = string.Empty;

    public string MarriagePlaceShort { get; init; } = string.Empty;

    public IReadOnlyList<PropertyEntryTemplateModel> Properties { get; init; } = Array.Empty<PropertyEntryTemplateModel>();

    public required IReadOnlyList<PersonEntryTemplateModel> Parents { get; init; }

    public required IReadOnlyList<PersonEntryTemplateModel> Children { get; init; }
}

/// <summary>
/// Typed person data exposed to family-entry templates.
/// </summary>
public sealed class PersonEntryTemplateModel
{
    public required string NameGc { get; init; }

    public required string NameAk { get; init; }

    public required string Anchor { get; init; }

    public required string Reference { get; init; }

    public string IndexLabel { get; init; } = string.Empty;

    public required string VitalEventsGc { get; init; }

    public string AdditionalLifeDataGc { get; init; } = string.Empty;

    public required string VitalEventsAk { get; init; }

    public string? Birth { get; init; }

    public string? Death { get; init; }

    public required string IndexAnchor { get; init; }

    public int? Ordinal { get; init; }

    public required IReadOnlyList<OccupationEntryTemplateModel> Occupations { get; init; }

    public IReadOnlyList<PropertyEntryTemplateModel> Properties { get; init; } = Array.Empty<PropertyEntryTemplateModel>();

    public string? Residence { get; init; }

    public string ResidenceAnchor { get; init; } = string.Empty;

    public FamilyReferenceEntryTemplateModel? ParentFamily { get; init; }

    public IReadOnlyList<FamilyReferenceEntryTemplateModel> ChildFamilies { get; init; } = Array.Empty<FamilyReferenceEntryTemplateModel>();

    public IReadOnlyList<FamilyReferenceEntryTemplateModel> ParentFamilies { get; init; } = Array.Empty<FamilyReferenceEntryTemplateModel>();

    public IReadOnlyList<FamilyReferenceTokenModel> ChildFamilyTokens { get; init; } = Array.Empty<FamilyReferenceTokenModel>();

    public IReadOnlyList<FamilyReferenceTokenModel> ParentFamilyTokens { get; init; } = Array.Empty<FamilyReferenceTokenModel>();
}

/// <summary>
/// A family relationship reference rendered as plain text or an internal document link.
/// </summary>
public sealed class FamilyReferenceEntryTemplateModel
{
    public required string Number { get; init; }

    public required string Anchor { get; init; }
}

/// <summary>
/// A repeatable occupation and its optional GEDCOM date.
/// </summary>
public sealed class OccupationEntryTemplateModel
{
    public required string Name { get; init; }

    public string? Date { get; init; }

    public required string IndexAnchor { get; init; }

    public string? Place { get; init; }

    public string PlaceAnchor { get; init; } = string.Empty;
}

/// <summary>
/// A property fact and its optional property-index bookmark.
/// </summary>
public sealed class PropertyEntryTemplateModel
{
    public required string Name { get; init; }

    public string? Date { get; init; }

    public string? Place { get; init; }

    public required string IndexAnchor { get; init; }

    public string PlaceAnchor { get; init; } = string.Empty;
}
