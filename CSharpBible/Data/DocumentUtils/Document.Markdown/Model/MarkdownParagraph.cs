using Document.Base.Models.Interfaces;

namespace Document.Markdown.Model;

public sealed class MarkdownParagraph : MarkdownContentBase, IDocParagraph
{
    public string? StyleName { get; }

    public MarkdownParagraph(string? styleName = null)
    {
        StyleName = styleName;
    }

    public IDocSpan AddBookmark(string Id, IDocFontStyle docFontStyle)
    {
        MarkdownSpan span = new(docFontStyle)
        {
            Id = Guid.NewGuid().ToString("N")
        };
        return (IDocSpan)AddChild(span);
    }

    public override IDocStyleStyle GetStyle() => new MarkdownStyle(StyleName);
}
