using Document.Base.Models.Interfaces;

namespace Document.Pdf.Model;

public sealed class PdfStyle : IDocStyleStyle
{
    public string? Name { get; }
    public IDictionary<string, string> Properties { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public PdfStyle(string? name = null, IDictionary<string, string>? props = null)
    {
        Name = name;
        if (props != null) foreach (var kv in props) Properties[kv.Key] = kv.Value;
    }
}

public sealed class PdfFontStyle : IDocFontStyle
{
    public string? Name { get; init; } = "Arial";
    public bool Bold { get; init; }
    public bool Italic { get; init; }
    public bool Underline { get; init; }
    public string? Color { get; init; }
    public string? FontFamily { get; init; } = "Arial";
    public double? FontSizePt { get; init; } = 12;
    public bool Strikeout { get; init; }

    public static readonly PdfFontStyle Default = new();
    public static readonly PdfFontStyle BoldStyle = new() { Name = "Bold", Bold = true };
    public static readonly PdfFontStyle ItalicStyle = new() { Name = "Italic", Italic = true };
    public static readonly PdfFontStyle BoldItalic = new() { Name = "BoldItalic", Bold = true, Italic = true };
    public static readonly PdfFontStyle UnderlineStyle = new() { Name = "Underline", Underline = true };
    public static readonly PdfFontStyle StrikeoutStyle = new() { Name = "Strikeout", Strikeout = true };
}