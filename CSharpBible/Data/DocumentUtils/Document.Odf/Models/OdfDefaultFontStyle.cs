using Document.Base.Models.Interfaces;
namespace Document.Odf.Models;
// Minimaler Default-Style für Parser-Operationen (Tabs, Links, Spans)
internal sealed class OdfDefaultFontStyle : IDocFontStyle
{
    public static readonly OdfDefaultFontStyle Instance = new();
    private OdfDefaultFontStyle() { }

    public string? Name => null;
    public bool Bold => false;
    public bool Italic => false;
    public bool Underline => false;
    public bool Strikeout => false;
    public string? Color => null;
    public string? FontFamily => null;
    public double? FontSizePt => null;
}