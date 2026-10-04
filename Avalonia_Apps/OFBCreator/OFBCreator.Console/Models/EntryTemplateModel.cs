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

    public required string VitalEventsGc { get; init; }

    public required string VitalEventsAk { get; init; }

    public string? Birth { get; init; }

    public string? Death { get; init; }

    public required string IndexAnchor { get; init; }

    public int? Ordinal { get; init; }

    public required IReadOnlyList<OccupationEntryTemplateModel> Occupations { get; init; }
}

/// <summary>
/// A repeatable occupation and its optional GEDCOM date.
/// </summary>
public sealed class OccupationEntryTemplateModel
{
    public required string Name { get; init; }

    public string? Date { get; init; }

    public required string IndexAnchor { get; init; }
}
