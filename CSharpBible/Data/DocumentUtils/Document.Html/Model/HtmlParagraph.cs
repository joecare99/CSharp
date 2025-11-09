using Document.Base.Models.Interfaces;

namespace Document.Html.Model;

public sealed class HtmlParagraph : HtmlContentBase, IDocParagraph
{
    public string? StyleName { get; }

    public HtmlParagraph(string? styleName = null)
    {
        StyleName = styleName;
    }

    public IDocSpan AddBookmark(string Id, IDocFontStyle docFontStyle)
    {
        var span = new HtmlSpan(docFontStyle);
        span.Id = Guid.NewGuid().ToString("N");
        return (IDocSpan)AddChild(span);
    }

    public override IDocStyleStyle GetStyle() => new HtmlStyle(StyleName);
}
