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
    /// Creates a simple entry with a single-value sort key.
    /// </summary>
    public static OFBIndexEntry FromSingle( string value, string reference )
        => new() { SortKey = value, Name = value, Ref = reference };

    /// <summary>
    /// Creates an entry from primary/secondary components (e.g., surname/given name).
    /// Format: "{primary}, {secondary}".
    /// </summary>
    public static OFBIndexEntry FromCombined( string primary, string secondary, string reference )
        => new() { SortKey = $"{primary}, {secondary}", Name = primary, Ref = reference };

    /// <inheritdoc />
    public override string ToString() => $"{{ SortKey={SortKey}, Name={Name}, Ref={Ref} }}";
}
