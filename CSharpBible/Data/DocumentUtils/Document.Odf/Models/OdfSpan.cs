using Document.Base.Models.Interfaces;

namespace Document.Odf.Models;

public sealed class OdfSpan : OdfContentBase, IDocSpan
{
    public IDictionary<string, string> Attributes { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public IDocFontStyle FontStyle { get; private set; }
    public bool IsLink { get; set; }
    public string? Href
    {
        get => Attributes.TryGetValue("href", out var v) ? v : null;
        set { if (value is null) Attributes.Remove("href"); else Attributes["href"] = value; IsLink = value != null; }
    }
    public string? Id
    {
        get => Attributes.TryGetValue("id", out var v) ? v : null;
        set { if (value is null) Attributes.Remove("id"); else Attributes["id"] = value; }
    }

    public OdfSpan(IDocFontStyle style)
    {
        FontStyle = style;
    }

    public override IDocStyleStyle GetStyle()
        => new OdfStyle(FontStyle.Name);

    public void SetStyle(object fs)
    {
        // not used in this minimal implementation
    }

    public void SetStyle(IDocFontStyle fs)
    {
        FontStyle = fs;
    }

    public void SetStyle(IUserDocument doc, object aFont)
    {
        // not used in this minimal implementation
    }

    public void SetStyle(IUserDocument doc, IDocFontStyle aFont)
    {
        FontStyle = aFont;
    }

    public void SetStyle(string aStyleName)
    {
        // not used in this minimal implementation
    }
}
