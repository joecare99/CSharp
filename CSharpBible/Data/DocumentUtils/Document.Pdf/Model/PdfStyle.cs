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
    public static readonly PdfFontStyle Default = new();
}