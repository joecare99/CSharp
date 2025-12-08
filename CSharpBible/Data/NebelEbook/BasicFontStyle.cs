using Document.Base.Models.Interfaces;

namespace NebelEbook;

internal sealed class BasicFontStyle : IDocFontStyle
{
    public static readonly BasicFontStyle Instance = new();

    public string? Name => null;
    public bool Bold => false;
    public bool Italic => false;
    public bool Underline => false;
    public string? Color => null;
    public string? FontFamily => null;
    public double? FontSizePt => null;
    public bool Strikeout => false;
}