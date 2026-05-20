using Document.Base.Models.Interfaces;

namespace Document.Markdown.Model;

public sealed class MarkdownFontStyle : IDocFontStyle
{
    public static MarkdownFontStyle Default { get; } = new("Default");
    public static MarkdownFontStyle BoldStyle { get; } = new("Bold") { Bold = true };
    public static MarkdownFontStyle ItalicStyle { get; } = new("Italic") { Italic = true };
    public static MarkdownFontStyle UnderlineStyle { get; } = new("Underline") { Underline = true };
    public static MarkdownFontStyle StrikeoutStyle { get; } = new("Strikeout") { Strikeout = true };

    private MarkdownFontStyle(string? name)
    {
        Name = name;
    }

    public string? Name { get; }
    public bool Bold { get; private init; }
    public bool Italic { get; private init; }
    public bool Underline { get; private init; }
    public bool Strikeout { get; private init; }
    public string? Color { get; private init; }
    public string? FontFamily { get; private init; }
    public double? FontSizePt { get; private init; }
}
