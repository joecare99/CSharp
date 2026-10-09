using System;
using System.Collections.Generic;

/// <summary>
/// Abstract index entry for alphabetical cross-reference indices (Person, Occupation, Property).
/// Shared contract between Abstractions and Core — both layers use this type directly.
/// Immutable value object for index data.
/// </summary>
namespace OFBCreator.Abstractions.Models;

public sealed class OFBIndexEntry
{
    /// <summary>
    /// Alphabetical sort key derived from the index field.
    /// </summary>
    public string SortKey { get; init; } = default!;

    /// <summary>
    /// Display name shown in the index.
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// Reference to the related entity (e.g., family number).
    /// </summary>
    public string Ref { get; init; } = default!;

    /// <summary>
    /// Additional family numbers associated with this index entry.
    /// </summary>
    public IReadOnlyList<string> FamilyReferences { get; init; } = Array.Empty<string>();

    /// <summary>Life span rendered beside a person name in the person index.</summary>
    public string LifeSpan { get; init; } = string.Empty;

    /// <summary>Date associated with a historical name entry, if known.</summary>
    public string NameEventDate { get; init; } = string.Empty;

    /// <summary>
    /// References to families where the indexed person appears as a child.
    /// </summary>
    public IReadOnlyList<string> ChildFamilyReferences { get; init; } = Array.Empty<string>();

    /// <summary>
    /// References to families where the indexed person appears as a parent.
    /// </summary>
    public IReadOnlyList<string> ParentFamilyReferences { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Optional role associated with the entry (e.g., Bride, Groom, Parent).
    /// Enables role-separated index data without external side dictionaries.
    /// </summary>
    public string? Role { get; init; }

    /// <summary>
    /// Creates a simple entry with a single-value sort key.
    /// </summary>
    public static OFBIndexEntry FromSingle( string value, string reference, string? role = null )
        => new() { SortKey = value, Name = value, Ref = reference, Role = role };

    /// <summary>
    /// Creates an entry from primary/secondary components (e.g., surname/given name).
    /// Format: "{primary}, {secondary}".
    /// </summary>
    public static OFBIndexEntry FromCombined( string primary, string secondary, string reference, string? role = null )
        => new() { SortKey = $"{primary}, {secondary}", Name = primary, Ref = reference, Role = role };

    /// <inheritdoc />
    public override string ToString() => $"{{ SortKey={SortKey}, Name={Name}, Ref={Ref}, Role={Role} }}";
}
