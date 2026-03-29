using Document.Base.Models;
using Document.Base.Models.Interfaces;

namespace Document.Odf.Models;

public sealed class OdfStyle : IDocStyleStyle
{
    public string? Name { get; }
    public IDictionary<string, string> Properties { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public OdfStyle(string? name = null, IDictionary<string, string>? properties = null)
    {
        Name = name;
        if (properties != null)
        {
            foreach (var kv in properties) Properties[kv.Key] = kv.Value;
        }
    }
}

public sealed class OdfFontStyle : IDocFontStyle
{
    public string? Name { get; init; }
    public bool Bold { get; init; }
    public bool Italic { get; init; }
    public bool Underline { get; init; }
    public bool Strikeout { get; init; }
    public string? Color { get; init; }
    public string? FontFamily { get; init; }
    public double? FontSizePt { get; init; }

    public static readonly OdfFontStyle Default = new();
    public static readonly OdfFontStyle BoldStyle = new() { Name = "Bold", Bold = true };
    public static readonly OdfFontStyle ItalicStyle = new() { Name = "Italic", Italic = true };
    public static readonly OdfFontStyle UnderlineStyle = new() { Name = "Underline", Underline = true };
    public static readonly OdfFontStyle StrikeoutStyle = new() { Name = "Strikeout", Strikeout= true };

    /// <summary>
    /// Creates a font style from a list of EFontStyle values.
    /// </summary>
    public static OdfFontStyle FromStyles(IList<object> styles)
    {
        bool bold = false, italic = false, underline = false, strikeout = false;

        foreach (var style in styles)
        {
            if (style is EFontStyle fs)
            {
                // Handle individual flags
                if (fs == EFontStyle.Bold || fs == EFontStyle.BoldItalic ||
                    fs == EFontStyle.UnderlineBold || fs == EFontStyle.UnderlineBoldItalic)
                    bold = true;

                if (fs == EFontStyle.Italic || fs == EFontStyle.BoldItalic ||
                    fs == EFontStyle.UnderlineItalic || fs == EFontStyle.UnderlineBoldItalic)
                    italic = true;

                if (fs == EFontStyle.Underline || fs == EFontStyle.UnderlineItalic ||
                    fs == EFontStyle.UnderlineBold || fs == EFontStyle.UnderlineBoldItalic)
                    underline = true;

                if (fs == EFontStyle.Strikeout)
                    strikeout = true;
            }
        }

        return new OdfFontStyle
        {
            Bold = bold,
            Italic = italic,
            Underline = underline,
            Strikeout = strikeout
        };
    }
}
