using Document.Base.Models.Interfaces;

namespace Document.Docx.Model;

public sealed class DocxTOC : DocxContentBase, IDocTOC
{
    public string Name { get; }
    public int Level { get; }

    public DocxTOC(string name, int level)
    {
        Name = name;
        Level = System.Math.Clamp(level, 1, 9);
    }

    public override IDocStyleStyle GetStyle() => new DocxStyle("TOC");

    public void RebuildFrom(IDocSection root)
    {
        // Placeholder: actual TOC will be generated on save using DocX fields
        TextContent = string.Empty;
        Nodes.Clear();
    }
}
