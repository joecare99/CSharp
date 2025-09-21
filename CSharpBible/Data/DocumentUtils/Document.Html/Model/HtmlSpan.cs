using Document.Base.Models.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace Document.Html.Model;

public sealed class HtmlSpan : HtmlContentBase, IDocSpan
{
    public IDictionary<string, string> Attributes { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public IDocFontStyle FontStyle { get; private set; }
    public bool IsLink { get; set; }
    public string? Href
    {
        get => Attributes.TryGetValue(HtmlAttributeKeys.Href, out var v) ? v : null;
        set { if (value is null) Attributes.Remove(HtmlAttributeKeys.Href); else Attributes[HtmlAttributeKeys.Href] = value; IsLink = value != null; }
    }
    public string? Id
    {
        get => Attributes.TryGetValue(HtmlAttributeKeys.Id, out var v) ? v : null;
        set { if (value is null) Attributes.Remove(HtmlAttributeKeys.Id); else Attributes[HtmlAttributeKeys.Id] = value; }
    }

    public HtmlSpan(IDocFontStyle style)
    {
        FontStyle = style;
    }

    public override IDocStyleStyle GetStyle()
        => new HtmlStyle(FontStyle.Name);

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
