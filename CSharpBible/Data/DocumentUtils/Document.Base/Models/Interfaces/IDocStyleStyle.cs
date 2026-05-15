namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a named style definition with arbitrary string properties.
/// </summary>
/// <remarks>
/// This contract is deliberately flexible so that style information from different document
/// sources can be preserved even when the exact semantics are not yet normalized.
/// </remarks>
public interface IDocStyleStyle
{
    /// <summary>
    /// Gets the style name.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Gets the style properties.
    /// </summary>
    /// <remarks>
    /// The property bag can contain any renderer-neutral style values such as size, color,
    /// spacing, indentation, or other document-specific settings.
    /// </remarks>
    IDictionary<string, string> Properties { get; }
}
