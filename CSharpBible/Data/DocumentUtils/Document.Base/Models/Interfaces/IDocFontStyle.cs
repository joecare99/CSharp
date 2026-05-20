namespace Document.Base.Models.Interfaces;

/// <summary>
/// Describes a font style in a renderer-neutral form.
/// </summary>
/// <remarks>
/// The properties are intentionally simple so that implementers can map from PDF font data,
/// HTML styles, text-analysis metadata, or application-specific styling systems.
/// </remarks>
public interface IDocFontStyle
{
    /// <summary>
    /// Gets the style name.
    /// </summary>
    /// <remarks>
    /// The name may refer to a named style definition, a font face, or a user-defined style key.
    /// </remarks>
    string? Name { get; }

    /// <summary>
    /// Gets a value indicating whether the font is bold.
    /// </summary>
    bool Bold { get; }

    /// <summary>
    /// Gets a value indicating whether the font is italic.
    /// </summary>
    bool Italic { get; }

    /// <summary>
    /// Gets a value indicating whether the font is underlined.
    /// </summary>
    bool Underline { get; }

    /// <summary>
    /// Gets a value indicating whether the font is struck through.
    /// </summary>
    bool Strikeout { get; }

    /// <summary>
    /// Gets the font color.
    /// </summary>
    /// <remarks>
    /// The color representation is left open so implementations can use a string-based format
    /// appropriate for their source or renderer, for example a hex color value.
    /// </remarks>
    string? Color { get; }

    /// <summary>
    /// Gets the logical font family.
    /// </summary>
    string? FontFamily { get; }

    /// <summary>
    /// Gets the font size in points.
    /// </summary>
    /// <remarks>
    /// The value is optional because not all sources can provide a reliable font size.
    /// </remarks>
    double? FontSizePt { get; }
}
