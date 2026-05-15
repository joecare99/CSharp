using Document.Base.Models.Interfaces;

namespace Document.Markdown.Model;

public sealed class MarkdownTOC : MarkdownContentBase, IDocTOC
{
    public string Name { get; }

    public int Level { get; }

    public MarkdownTOC(string name, int level)
    {
        Name = name;
        Level = Math.Clamp(level, 1, 6);
    }

    public override IDocStyleStyle GetStyle() => new MarkdownStyle("TOC");

    public void RebuildFrom(IDocSection root)
    {
        TextContent = string.Empty;
        Nodes.Clear();

        foreach (IDocHeadline headline in root.Enumerate().OfType<IDocHeadline>().Where(h => h.Level <= Level))
        {
            MarkdownParagraph paragraph = new("TOCEntry");
            MarkdownSpan link = (MarkdownSpan)paragraph.AddLink(headline.Id, MarkdownFontStyle.Default);
            string anchorId = headline.Nodes.OfType<MarkdownSpan>().FirstOrDefault(span => !string.IsNullOrEmpty(span.Id))?.Id ?? Guid.NewGuid().ToString("N");
            if (!headline.Nodes.OfType<MarkdownSpan>().Any(span => span.Id == anchorId))
            {
                headline.AddChild(new MarkdownSpan(MarkdownFontStyle.Default) { Id = anchorId });
            }
            link.Href = "#" + anchorId;
            link.TextContent = headline.GetTextContent(true);
            AddChild(paragraph);
        }
    }

    public void RebuildFrom(IDocElement root)
    {
        throw new NotImplementedException();
    }
}
