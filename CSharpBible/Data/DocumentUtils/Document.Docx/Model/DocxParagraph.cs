using System.Collections.Generic;
using Document.Base.Models;
using Document.Base.Models.Interfaces;

namespace Document.Docx.Model;

public sealed class DocxParagraph : DocxContentBase, IDocParagraph
{
    public DocxParagraph(string styleName)
    {
        StyleName = styleName;
    }

    public string StyleName { get; }

    public new IList<IDocAttributes> DocAttributes { get; } = new List<IDocAttributes>();

    public int? HangingIndentPoints
    {
        get
        {
            for (var index = DocAttributes.Count - 1; index >= 0; index--)
            {
                if (DocAttributes[index].Name == DocAttributeNames.IndentationHanging && DocAttributes[index].Value is int indent)
                    return indent;
            }
            return null;
        }
        set
        {
            for (var index = DocAttributes.Count - 1; index >= 0; index--)
            {
                if (DocAttributes[index].Name == DocAttributeNames.IndentationHanging)
                    DocAttributes.RemoveAt(index);
            }
            if (value is int indent)
                DocAttributes.Add(new DocAttribute(DocAttributeNames.IndentationHanging, indent));
        }
    }

    public override IDocStyleStyle GetStyle() => new DocxStyle(StyleName);

    public IDocSpan AddBookmark(string Id, IDocFontStyle docFontStyle)
    {
        var span = new DocxSpan(docFontStyle) { Id = Id };
        AddChild(span);
        return span;
    }
}
