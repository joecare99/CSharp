using Document.Base.Models.Interfaces;

namespace Document.Demo.Gui.Models;

public sealed class DemoFontStyle : IDocFontStyle
{
    public DemoFontStyle(
        string? name = null,
        bool bold = false,
        bool italic = false,
        bool underline = false,
        bool strikeout = false,
        string? color = null,
        string? fontFamily = null,
        double? fontSizePt = null)
    {
        Name = name;
        Bold = bold;
        Italic = italic;
        Underline = underline;
        Strikeout = strikeout;
        Color = color;
        FontFamily = fontFamily;
        FontSizePt = fontSizePt;
    }

    public string? Name { get; }

    public bool Bold { get; }

    public bool Italic { get; }

    public bool Underline { get; }

    public bool Strikeout { get; }

    public string? Color { get; }

    public string? FontFamily { get; }

    public double? FontSizePt { get; }
}
