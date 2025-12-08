using Document.Base.Models.Interfaces;

namespace Document.Html.Model;

public sealed class HtmlStyle : IDocStyleStyle
{
    public string? Name { get; }
    public IDictionary<string, string> Properties { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public HtmlStyle(string? name = null, IDictionary<string, string>? properties = null)
    {
        Name = name;
        if (properties != null)
        {
            foreach (var kv in properties) Properties[kv.Key] = kv.Value;
        }
    }
}

public sealed class HtmlFontStyle : IDocFontStyle
{
    public string? Name { get; init; }
    public bool Bold { get; init; }
    public bool Italic { get; init; }
    public bool Underline { get; init; }
    public bool Strikeout { get; init; }
    public string? Color { get; init; }
    public string? FontFamily { get; init; }
    public double? FontSizePt { get; init; }

    public static readonly HtmlFontStyle Default = new();
    public static readonly HtmlFontStyle BoldStyle = new() { Name = "Bold", Bold = true };
    public static readonly HtmlFontStyle ItalicStyle = new() { Name = "Italic", Italic = true };
    public static readonly HtmlFontStyle UnderlineStyle = new() { Name = "Underline", Underline = true };
    public static readonly HtmlFontStyle StrikeoutStyle = new() { Name = "Strikeout", Strikeout= true };
}
