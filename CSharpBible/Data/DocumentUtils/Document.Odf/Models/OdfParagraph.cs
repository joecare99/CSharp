using Document.Base.Models.Interfaces;

namespace Document.Odf.Models;

public sealed class OdfParagraph : OdfContentBase, IDocParagraph
{
    public string? StyleName { get; }

    public OdfParagraph(string? styleName = null)
    {
        StyleName = styleName;
    }

    public IDocSpan AddBookmark(string Id, IDocFontStyle docFontStyle)
    {
        var span = new OdfSpan(docFontStyle) { Id = Guid.NewGuid().ToString("N") };
        return (IDocSpan)AddChild(span);
    }

    public override IDocStyleStyle GetStyle() => new OdfStyle(StyleName);
}
