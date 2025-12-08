using System.Text;
using Document.Base.Models;
using Document.Base.Models.Interfaces;

namespace Document.Xaml.Model;

public abstract class XamlContentBase : XamlNodeBase, IDocContent
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
        AddChild(new XamlLineBreak());
        return this;
    }

    public virtual IDocContent AddNBSpace(IDocFontStyle docFontStyle)
    {
        AddChild(new XamlNbSpace());
        return this;
    }

    public virtual IDocContent AddTab(IDocFontStyle docFontStyle)
    {
        AddChild(new XamlTab());
        return this;
    }

    public virtual IDocSpan AddSpan(IDocFontStyle docFontStyle)
        => (IDocSpan)AddChild(new XamlSpan(docFontStyle));

    public virtual IDocSpan AddSpan(string text, IList<object> docFontStyle)
    {
        var span = new XamlSpan(XamlFontStyle.Default) { TextContent = text };
        return (IDocSpan)AddChild(span);
    }

    public virtual IDocSpan AddSpan(string text, IDocFontStyle docFontStyle)
    {
        var span = new XamlSpan(docFontStyle) { TextContent = text };
        return (IDocSpan)AddChild(span);
    }

    public virtual IDocSpan AddLink(string Href,IDocFontStyle docFontStyle)
    {
        var link = new XamlSpan(docFontStyle) { IsLink = true, Href = Href };
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

    public IDocSpan AddSpan(string text, EFontStyle eFontStyle)
    {
        return eFontStyle switch
        {
            EFontStyle.Bold => AddSpan(text, XamlFontStyle.BoldStyle),
            EFontStyle.Italic => AddSpan(text, XamlFontStyle.ItalicStyle),
            EFontStyle.Underline => AddSpan(text, XamlFontStyle.UnderlineStyle),
            EFontStyle.Strikeout => AddSpan(text, XamlFontStyle.StrikeoutStyle),
            _ => AddSpan(text, XamlFontStyle.Default)
        };
    }
}