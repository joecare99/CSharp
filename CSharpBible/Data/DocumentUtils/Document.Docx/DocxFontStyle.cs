using Document.Base.Models.Interfaces;

namespace Document.Docx;

public sealed class DocxFontStyle : IDocFontStyle
{
    public string? Name { get; init; }
    public bool Bold { get; init; }
    public bool Italic { get; init; }
    public bool Underline { get; init; }
    public bool Strikeout { get; init; }
    public string? Color { get; init; }
    public string? FontFamily { get; init; }
    public double? FontSizePt { get; init; }

    public static DocxFontStyle Default => new() { Name = "Default" };
    public static DocxFontStyle BoldStyle => new() { Bold = true, Name = "Bold" };
    public static DocxFontStyle ItalicStyle => new() { Italic = true, Name = "Italic" };
    public static DocxFontStyle UnderlineStyle => new() { Underline = true, Name = "Underline" };
    public static DocxFontStyle StrikeoutStyle => new() { Strikeout = true, Name = "Strike" };
}
