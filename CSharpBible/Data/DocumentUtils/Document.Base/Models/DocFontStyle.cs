using Document.Base.Models.Interfaces;

namespace Document.Base.Models;

/// <summary>
/// Provides a provider-neutral font style value object.
/// </summary>
public sealed class DocFontStyle : IDocFontStyle
{
    /// <summary>Gets or initializes the style name.</summary>
    public string? Name { get; init; }

    /// <summary>Gets or initializes whether the text is bold.</summary>
    public bool Bold { get; init; }

    /// <summary>Gets or initializes whether the text is italic.</summary>
    public bool Italic { get; init; }

    /// <summary>Gets or initializes whether the text is underlined.</summary>
    public bool Underline { get; init; }

    /// <summary>Gets or initializes whether the text is struck through.</summary>
    public bool Strikeout { get; init; }

    /// <summary>Gets or initializes the text color.</summary>
    public string? Color { get; init; }

    /// <summary>Gets or initializes the font family.</summary>
    public string? FontFamily { get; init; }

    /// <summary>Gets or initializes the font size in points.</summary>
    public double? FontSizePt { get; init; }

    /// <summary>Gets an unformatted font style.</summary>
    public static DocFontStyle Default { get; } = new();
}
