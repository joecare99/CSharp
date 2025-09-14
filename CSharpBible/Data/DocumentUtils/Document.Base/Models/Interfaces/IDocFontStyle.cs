namespace Document.Base.Models.Interfaces;

public interface IDocFontStyle
{
    string? Name { get; }
    bool Bold { get; }
    bool Italic { get; }
    bool Underline { get; }
    string? Color { get; }
    string? FontFamily { get; }
    double? FontSizePt { get; }
}