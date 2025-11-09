using Document.Base.Models.Interfaces;

namespace Document.Xaml.Model;

public sealed class XamlSpan : XamlContentBase, IDocSpan
{
    public IDictionary<string, string> Attributes { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public IDocFontStyle FontStyle { get; private set; }
    public bool IsLink { get; set; }

    public string? Href
    {
        get => Attributes.TryGetValue("NavigateUri", out var v) ? v : null;
        set
        {
            if (value is null) Attributes.Remove("NavigateUri");
            else Attributes["NavigateUri"] = value;
            IsLink = value != null;
        }
    }

    public string? Id
    {
        get => Attributes.TryGetValue("Id", out var v) ? v : null;
        set
        {
            if (value is null) Attributes.Remove("Id");
            else Attributes["Id"] = value;
        }
    }

    public XamlSpan(IDocFontStyle style)
    {
        FontStyle = style;
    }

    public override IDocStyleStyle GetStyle() => new XamlStyle(FontStyle?.Name);

    public void SetStyle(object fs) => throw new NotImplementedException();
    public void SetStyle(IDocFontStyle fs) => FontStyle = fs;
    public void SetStyle(IUserDocument doc, object aFont) => throw new NotImplementedException();
    public void SetStyle(IUserDocument doc, IDocFontStyle aFont) => FontStyle = aFont;
    public void SetStyle(string aStyleName) => throw new NotImplementedException();
}