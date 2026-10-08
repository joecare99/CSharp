using Document.Base.Models.Interfaces;

namespace Document.Base.Models;

/// <summary>
/// Provides standard names for paragraph and character attributes.
/// </summary>
public static class DocAttributeNames
{
    /// <summary>Gets the paragraph indentation before its text.</summary>
    public const string IndentationBefore = "paragraph.indentationBefore";

    /// <summary>Gets the paragraph hanging indentation.</summary>
    public const string IndentationHanging = "paragraph.indentationHanging";

    /// <summary>Gets the paragraph first-line indentation.</summary>
    public const string IndentationFirstLine = "paragraph.indentationFirstLine";

    /// <summary>Gets the character font size in points.</summary>
    public const string FontSizePt = "character.fontSizePt";

    /// <summary>Gets the character font family.</summary>
    public const string FontFamily = "character.fontFamily";

    /// <summary>Gets the character bold setting.</summary>
    public const string Bold = "character.bold";

    /// <summary>Gets the character italic setting.</summary>
    public const string Italic = "character.italic";

    /// <summary>Gets the character underline setting.</summary>
    public const string Underline = "character.underline";

    /// <summary>Gets the character strike-through setting.</summary>
    public const string Strikeout = "character.strikeout";

    /// <summary>Gets the character color.</summary>
    public const string Color = "character.color";
}

/// <summary>
/// Represents a named document attribute and its value.
/// </summary>
public sealed record DocAttribute(string Name, object? Value) : IDocAttributes;
