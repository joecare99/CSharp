using System.Text;
using Document.Base.Models.Interfaces;

namespace Document.Html.Model;

public sealed class HtmlTOC : HtmlContentBase
{
    public string Name { get; }
    public int Level { get; }

    public HtmlTOC(string name, int level)
    {
        Name = name;
        Level = Math.Clamp(level, 1, 6);
    }

    public override IDocStyleStyle GetStyle() => new HtmlStyle("TOC");

    public void RebuildFrom(IDocContent root)
    {
        // Einfacher TOC: Alle Headlines bis Level einsammeln
        TextContent = string.Empty;
        Nodes.Clear();

        foreach (var h in root.Enumerate().OfType<HtmlHeadline>().Where(h => h.Level <= Level))
        {
            var p = new HtmlParagraph("TOCEntry");
            var anchorText = h.GetTextContent(true);
            var span = (HtmlSpan)p.AddLink(HtmlFontStyle.Default);
            // Generiere (oder finde) eine ID am Headline-Knoten
            var id = h.Nodes.OfType<HtmlSpan>().FirstOrDefault(s => !string.IsNullOrEmpty(s.Id))?.Id
                     ?? Guid.NewGuid().ToString("N");
            // Stelle sicher, dass Headline einen Anker trägt
            if (!h.Nodes.OfType<HtmlSpan>().Any(s => s.Id == id))
            {
                var bm = new HtmlSpan(HtmlFontStyle.Default) { Id = id };
                h.AddChild(bm);
            }
            span.Href = "#" + id;
            span.TextContent = anchorText;
            AddChild(p);
        }
    }
}
