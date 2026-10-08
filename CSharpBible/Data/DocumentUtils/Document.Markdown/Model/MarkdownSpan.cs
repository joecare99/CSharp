using Document.Base.Models.Interfaces;
using Document.Base.Models;

namespace Document.Markdown.Model;

public sealed class MarkdownSpan : MarkdownContentBase, IDocSpan
{
    public new IList<IDocAttributes> DocAttributes { get; } = new List<IDocAttributes>();

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
        SetStyle(style);
    }

    public override IDocStyleStyle GetStyle() => new MarkdownStyle(FontStyle.Name);

    public void SetStyle(object fs)
    {
        if (fs is IDocFontStyle fontStyle)
            SetStyle(fontStyle);
    }

    public void SetStyle(IDocFontStyle fs)
    {
        FontStyle = fs;
        SetAttribute(DocAttributeNames.Bold, fs.Bold);
        SetAttribute(DocAttributeNames.Italic, fs.Italic);
        SetAttribute(DocAttributeNames.Underline, fs.Underline);
        SetAttribute(DocAttributeNames.Strikeout, fs.Strikeout);
        SetAttribute(DocAttributeNames.Color, fs.Color);
        SetAttribute(DocAttributeNames.FontFamily, fs.FontFamily);
        SetAttribute(DocAttributeNames.FontSizePt, fs.FontSizePt);
    }

    public void SetStyle(IUserDocument doc, object aFont) => SetStyle(aFont);

    public void SetStyle(IUserDocument doc, IDocFontStyle aFont) => SetStyle(aFont);

    public void SetStyle(string aStyleName) => throw new NotSupportedException("Named styles must be resolved by the owning document provider.");

    private void SetAttribute(string name, object? value)
    {
        var attributes = DocAttributes;
        for (var index = attributes.Count - 1; index >= 0; index--)
        {
            if (attributes[index].Name == name)
                attributes.RemoveAt(index);
        }

        if (value is not null)
            attributes.Add(new DocAttribute(name, value));
    }
}
