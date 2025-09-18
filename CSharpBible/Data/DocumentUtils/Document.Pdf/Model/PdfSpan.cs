using Document.Base.Models.Interfaces;

namespace Document.Pdf.Model;

public sealed class PdfSpan : PdfContentBase, IDocSpan
{
    public IDictionary<string, string> Attributes { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public IDocFontStyle FontStyle { get; private set; }
    public bool IsLink { get; set; }
    public string? Href
    {
        get => Attributes.TryGetValue("href", out var v) ? v : null;
        set { if (value is null) Attributes.Remove("href"); else Attributes["href"] = value; IsLink = value != null; }
    }

    public PdfSpan(IDocFontStyle style) => FontStyle = style;

    public override IDocStyleStyle GetStyle() => new PdfStyle(FontStyle.Name);

    public void SetStyle(object fs)
    {
        throw new NotImplementedException();
    }

    public void SetStyle(IDocFontStyle fs)
    {
        FontStyle = fs;
    }

    public void SetStyle(IUserDocument doc, object aFont)
    {
        throw new NotImplementedException();
    }

    public void SetStyle(IUserDocument doc, IDocFontStyle aFont)
    {
        FontStyle = aFont;
    }

    public void SetStyle(string aStyleName)
    {
        throw new NotImplementedException();
    }
}