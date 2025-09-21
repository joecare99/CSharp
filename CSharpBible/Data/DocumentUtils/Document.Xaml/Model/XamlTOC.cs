using Document.Base.Models.Interfaces;

namespace Document.Xaml.Model;

public sealed class XamlTOC : XamlContentBase, IDocTOC
{
    public string Name { get; }
    public int Level { get; }

    public XamlTOC(string name, int level)
    {
        Name = name;
        Level = Math.Clamp(level, 1, 6);
    }

    public override IDocStyleStyle GetStyle() => new XamlStyle("TOC");

    public void RebuildFrom(IDocSection root)
    {
        TextContent = string.Empty;
        Nodes.Clear();

        // Headline-IDs sicherstellen und Links anlegen
        foreach (var h in root.Enumerate().OfType<IDocHeadline>().Where(h => h.Level <= Level))
        {
            var p = new XamlParagraph("TOCEntry");
            var anchorText = h.GetTextContent(true);

            // Anker-ID an Headline vorhanden?
            var id = h.Id;

            var link = (XamlSpan)p.AddLink(id,XamlFontStyle.Default);
            link.TextContent = anchorText;

            AddChild(p);
        }
    }
}