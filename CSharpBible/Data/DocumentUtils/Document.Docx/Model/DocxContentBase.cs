using System;
using System.Collections.Generic;
using System.Text;
using Document.Base.Models.Interfaces;
using Document.Base.Models; // for EFontStyle

namespace Document.Docx.Model;

public abstract class DocxContentBase : DocxNodeBase, IDocContent, IDocElement
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
        AddChild(new DocxLineBreak());
        return this;
    }

    public virtual IDocContent AddNBSpace(IDocFontStyle docFontStyle)
    {
        AddChild(new DocxNbSpace());
        return this;
    }

    public virtual IDocContent AddTab(IDocFontStyle docFontStyle)
    {
        AddChild(new DocxTab());
        return this;
    }

    public virtual IDocSpan AddSpan(IDocFontStyle docFontStyle)
        => (IDocSpan)AddChild(new DocxSpan(docFontStyle));

    public virtual IDocSpan AddSpan(string text, IList<object> docFontStyle)
    {
        var span = new DocxSpan(DocxFontStyle.Default) { TextContent = text };
        return (IDocSpan)AddChild(span);
    }

    public virtual IDocSpan AddSpan(string text, IDocFontStyle docFontStyle)
    {
        var span = new DocxSpan(docFontStyle) { TextContent = text };
        return (IDocSpan)AddChild(span);
    }

    public IDocSpan AddSpan(string text, EFontStyle eFontStyle)
    {
        return eFontStyle switch
        {
            EFontStyle.Bold => AddSpan(text, DocxFontStyle.BoldStyle),
            EFontStyle.Italic => AddSpan(text, DocxFontStyle.ItalicStyle),
            EFontStyle.Underline => AddSpan(text, DocxFontStyle.UnderlineStyle),
            EFontStyle.Strikeout => AddSpan(text, DocxFontStyle.StrikeoutStyle),
            _ => AddSpan(text, DocxFontStyle.Default)
        };
    }

    public virtual IDocSpan AddLink(string Href, IDocFontStyle docFontStyle)
    {
        var span = new DocxSpan(docFontStyle) { IsLink = true, Href = Href };
        return (IDocSpan)AddChild(span);
    }

    public abstract IDocStyleStyle GetStyle();

    public virtual string GetTextContent(bool xRecursive = true)
    {
        if (!xRecursive) return TextContent;
        var sb = new StringBuilder();
        sb.Append(TextContent);
        foreach (var c in Nodes)
            if (c is IDocContent dc) sb.Append(dc.GetTextContent(true));
        return sb.ToString();
    }

    // IDocElement implementation (basic no-op element factory)
    public IDocElement AppendDocElement(Enum aType) => throw new NotSupportedException();
    public IDocElement AppendDocElement(Enum aType, Type aClass) => throw new NotSupportedException();
    public IDocElement AppendDocElement(Enum aType, Enum aAttribute, string value, Type aClass, string? Id) => throw new NotSupportedException();
    public IEnumerable<IDocElement> Enumerate()
    {
        foreach (var n in Nodes)
            if (n is IDocElement de) yield return de;
    }
}
