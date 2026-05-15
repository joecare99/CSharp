using Document.Base.Models.Interfaces;

namespace Document.Markdown.Model;

public sealed class MarkdownHeadline : MarkdownContentBase, IDocHeadline
{
    public int Level { get; }

    public string Id { get; }

    public MarkdownHeadline(int level, string id)
    {
        Level = Math.Clamp(level, 1, 6);
        Id = id;
    }

    public override IDocStyleStyle GetStyle() => new MarkdownStyle($"H{Level}");
}
