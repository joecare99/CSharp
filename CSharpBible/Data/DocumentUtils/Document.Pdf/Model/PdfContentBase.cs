using System.Text;
using Document.Base.Models.Interfaces;

namespace Document.Pdf.Model;

public abstract class PdfContentBase : PdfNodeBase, IDocContent
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
        _text.AppendLine();
        return this;
    }

    public virtual IDocContent AddNBSpace(IDocFontStyle docFontStyle)
    {
        _text.Append('\u00A0');
        return this;
    }

    public virtual IDocContent AddTab(IDocFontStyle docFontStyle)
    {
        _text.Append('\t');
        return this;
    }

    public virtual IDocSpan AddSpan(IDocFontStyle docFontStyle)
        => AddChild(new PdfSpan(docFontStyle));

    public virtual IDocSpan AddSpan(string text, IList<object> docFontStyle)
    {
        var span = new PdfSpan(PdfFontStyle.Default) { TextContent = text };
        return AddChild(span);
    }

    public virtual IDocSpan AddSpan(string text, IDocFontStyle docFontStyle)
    {
        var span = new PdfSpan(docFontStyle) { TextContent = text };
        return AddChild(span);
    }

    public virtual IDocSpan AddLink(IDocFontStyle docFontStyle)
    {
        var span = new PdfSpan(docFontStyle) { IsLink = true };
        return AddChild(span);
    }

    public abstract IDocStyleStyle GetStyle();

    public virtual string GetTextContent(bool xRecursive = true)
    {
        if (!xRecursive) return TextContent;
        var sb = new StringBuilder();
        sb.Append(TextContent);
        foreach (var c in _children)
            if (c is IDocContent ic) sb.Append(ic.GetTextContent(true));
        return sb.ToString();
    }
}