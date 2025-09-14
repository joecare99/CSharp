using System.Text;
using Document.Base.Models.Interfaces;

namespace Document.Html.Model;

public abstract class HtmlContentBase : HtmlNodeBase, IDocContent
{
    private readonly StringBuilder _text = new();

    public virtual string TextContent
    {
        get => _text.ToString();
        set
        {
            _text.Clear();
            if (!string.IsNullOrEmpty(value)) _text.Append(value);
        }
    }

    public void AppendText(string text)
    {
        if (!string.IsNullOrEmpty(text)) _text.Append(text);
    }

    public virtual IDocContent AddLineBreak()
    {
        AddChild(new HtmlLineBreak());
        return this;
    }

    public virtual IDocContent AddNBSpace(IDocFontStyle docFontStyle)
    {
        AddChild(new HtmlNbSpace());
        return this;
    }

    public virtual IDocContent AddTab(IDocFontStyle docFontStyle)
    {
        AddChild(new HtmlTab());
        return this;
    }

    public virtual IDocSpan AddSpan(IDocFontStyle docFontStyle)
        => (IDocSpan)AddChild(new HtmlSpan(docFontStyle));

    public virtual IDocSpan AddSpan(string text, IList<object> docFontStyle)
    {
        var span = new HtmlSpan(HtmlFontStyle.Default);
        span.TextContent = text;
        return (IDocSpan)AddChild(span);
    }

    public virtual IDocSpan AddSpan(string text, IDocFontStyle docFontStyle)
    {
        var span = new HtmlSpan(docFontStyle) { TextContent = text };
        return (IDocSpan)AddChild(span);
    }

    public virtual IDocSpan AddLink(IDocFontStyle docFontStyle)
    {
        var link = new HtmlSpan(docFontStyle) { IsLink = true };
        return (IDocSpan)AddChild(link);
    }

    public abstract IDocStyleStyle GetStyle();

    public virtual string GetTextContent(bool xRecursive = true)
    {
        if (!xRecursive) return TextContent;

        var sb = new StringBuilder();
        sb.Append(TextContent);
        foreach (var c in Nodes)
        {
            if (c is IDocContent dc)
            {
                sb.Append(dc.GetTextContent(true));
            }
        }
        return sb.ToString();
    }
}
