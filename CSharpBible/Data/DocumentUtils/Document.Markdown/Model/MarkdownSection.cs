using Document.Base.Models.Interfaces;

namespace Document.Markdown.Model;

public sealed class MarkdownSection : MarkdownNodeBase, IDocSection
{
    public IDocParagraph AddParagraph(string ATextStyleName)
    {
        MarkdownParagraph paragraph = new(ATextStyleName);
        return (IDocParagraph)AddChild(paragraph);
    }

    public IDocHeadline AddHeadline(int aLevel, string Id)
    {
        MarkdownHeadline headline = new(aLevel, Id);
        return (IDocHeadline)AddChild(headline);
    }

    public IDocTOC AddTOC(string aName, int aLevel)
    {
        MarkdownTOC toc = new(aName, aLevel);
        AddChild(toc);
        return toc;
    }
}
