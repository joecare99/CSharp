using Document.Base.Models.Interfaces;

namespace Document.Odf.Models;

public sealed class OdfTOC : OdfContentBase, IDocTOC
{
    public string Name { get; }
    public int Level { get; }

    public OdfTOC(string name, int level)
    {
        Name = name;
        Level = Math.Clamp(level, 1, 6);
    }

    public override IDocStyleStyle GetStyle() => new OdfStyle("TOC");

    public void RebuildFrom(IDocSection root)
    {
        TextContent = string.Empty;
        Nodes.Clear();

        foreach (var h in root.Enumerate().OfType<IDocHeadline>().Where(h => h.Level <= Level))
        {
            var p = new OdfParagraph("TOCEntry");
            var anchorText = h.GetTextContent(true);
            var span = (OdfSpan)p.AddLink(h.Id, OdfFontStyle.Default);
            var id = h.Nodes.OfType<OdfSpan>().FirstOrDefault(s => !string.IsNullOrEmpty(s.Id))?.Id
                     ?? Guid.NewGuid().ToString("N");
            if (!h.Nodes.OfType<OdfSpan>().Any(s => s.Id == id))
            {
                var bm = new OdfSpan(OdfFontStyle.Default) { Id = id };
                h.AddChild(bm);
            }
            span.Href = "#" + id;
            span.TextContent = anchorText;
            AddChild(p);
        }
    }
}
