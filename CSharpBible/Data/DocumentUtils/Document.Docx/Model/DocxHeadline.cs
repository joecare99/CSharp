using Document.Base.Models.Interfaces;

namespace Document.Docx.Model;

public sealed class DocxHeadline : DocxContentBase, IDocHeadline
{
    public DocxHeadline(int level, string id)
    {
        Level = level;
        Id = id;
    }
    public int Level { get; }
    public string Id { get; }

    public override IDocStyleStyle GetStyle() => new DocxStyle($"Heading{Level}");
}
