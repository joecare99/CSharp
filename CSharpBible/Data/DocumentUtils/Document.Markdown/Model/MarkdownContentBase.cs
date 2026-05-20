using System.Text;
using Document.Base.Models;
using Document.Base.Models.Interfaces;

namespace Document.Markdown.Model;

public abstract class MarkdownContentBase : MarkdownNodeBase, IDocContent
{
    private readonly StringBuilder _text = new();

    public virtual string TextContent
    {
        get => _text.ToString();
        set
        {
            _text.Clear();
            if (!string.IsNullOrEmpty(value))
            {
                _text.Append(value);
            }
        }
    }

    public void AppendText(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            _text.Append(text);
        }
    }

    public virtual IDocContent AddLineBreak()
    {
        AddChild(new MarkdownLineBreak());
        return this;
    }

    public virtual IDocContent AddNBSpace(IDocFontStyle docFontStyle)
    {
        AddChild(new MarkdownNbSpace());
        return this;
    }

    public virtual IDocContent AddTab(IDocFontStyle docFontStyle)
    {
        AddChild(new MarkdownTab());
        return this;
    }

    public virtual IDocSpan AddSpan(IDocFontStyle docFontStyle)
        => (IDocSpan)AddChild(new MarkdownSpan(docFontStyle));

    public virtual IDocSpan AddSpan(string text, IList<object> docFontStyle)
    {
        MarkdownSpan span = new(MarkdownFontStyle.Default)
        {
            TextContent = text
        };
        return (IDocSpan)AddChild(span);
    }

    public virtual IDocSpan AddSpan(string text, IDocFontStyle docFontStyle)
    {
        MarkdownSpan span = new(docFontStyle)
        {
            TextContent = text
        };
        return (IDocSpan)AddChild(span);
    }

    public virtual IDocSpan AddLink(string Href, IDocFontStyle docFontStyle)
    {
        MarkdownSpan link = new(docFontStyle)
        {
            IsLink = true,
            Href = Href
        };
        return (IDocSpan)AddChild(link);
    }

    public abstract IDocStyleStyle GetStyle();

    public virtual string GetTextContent(bool xRecursive = true)
    {
        if (!xRecursive)
        {
            return TextContent;
        }

        StringBuilder sb = new();
        sb.Append(TextContent);
        foreach (IDOMElement child in Nodes)
        {
            if (child is IDocContent content)
            {
                sb.Append(content.GetTextContent(true));
            }
        }

        return sb.ToString();
    }

    public IDocSpan AddSpan(string text, EFontStyle eFontStyle)
    {
        return eFontStyle switch
        {
            EFontStyle.Bold => AddSpan(text, MarkdownFontStyle.BoldStyle),
            EFontStyle.Italic => AddSpan(text, MarkdownFontStyle.ItalicStyle),
            EFontStyle.Underline => AddSpan(text, MarkdownFontStyle.UnderlineStyle),
            EFontStyle.Strikeout => AddSpan(text, MarkdownFontStyle.StrikeoutStyle),
            _ => AddSpan(text, MarkdownFontStyle.Default)
        };
    }
}
