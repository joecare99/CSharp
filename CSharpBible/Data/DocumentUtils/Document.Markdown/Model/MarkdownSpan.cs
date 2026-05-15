using Document.Base.Models.Interfaces;

namespace Document.Markdown.Model;

public sealed class MarkdownSpan : MarkdownContentBase, IDocSpan
{
    public new IDictionary<string, string> Attributes { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public IDocFontStyle FontStyle { get; private set; }

    public bool IsLink { get; set; }

    public string? Href
    {
        get => Attributes.TryGetValue(MarkdownAttributeKeys.Href, out string? value) ? value : null;
        set
        {
            if (value is null)
            {
                Attributes.Remove(MarkdownAttributeKeys.Href);
            }
            else
            {
                Attributes[MarkdownAttributeKeys.Href] = value;
            }
            IsLink = value is not null;
        }
    }

    public string? Id
    {
        get => Attributes.TryGetValue(MarkdownAttributeKeys.Id, out string? value) ? value : null;
        set
        {
            if (value is null)
            {
                Attributes.Remove(MarkdownAttributeKeys.Id);
            }
            else
            {
                Attributes[MarkdownAttributeKeys.Id] = value;
            }
        }
    }

    public MarkdownSpan(IDocFontStyle style)
    {
        FontStyle = style;
    }

    public override IDocStyleStyle GetStyle() => new MarkdownStyle(FontStyle.Name);

    public void SetStyle(object fs) => throw new NotImplementedException();

    public void SetStyle(IDocFontStyle fs) => FontStyle = fs;

    public void SetStyle(IUserDocument doc, object aFont) => throw new NotImplementedException();

    public void SetStyle(IUserDocument doc, IDocFontStyle aFont) => FontStyle = aFont;

    public void SetStyle(string aStyleName) => throw new NotImplementedException();
}
