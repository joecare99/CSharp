using Document.Base.Models.Interfaces;

namespace Document.Docx.Model;

public sealed class DocxParagraph : DocxContentBase, IDocParagraph
{
    public DocxParagraph(string styleName)
    {
        StyleName = styleName;
    }

    public string StyleName { get; }

    public override IDocStyleStyle GetStyle() => new DocxStyle(StyleName);

    public IDocSpan AddBookmark(string Id, IDocFontStyle docFontStyle)
    {
        var span = new DocxSpan(docFontStyle) { Id = Id };
        AddChild(span);
        return span;
    }
}
