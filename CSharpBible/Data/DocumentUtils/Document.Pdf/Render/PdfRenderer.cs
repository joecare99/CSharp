using Document.Base.Models.Interfaces;
using Document.Pdf.Model;

namespace Document.Pdf.Render;

public static class PdfRenderer
{
    /*
     Pseudocode (DE):
     - Ziel: Rendering eines Inhaltsverzeichnisses (PdfTOC) implementieren.
     - Vorgehen:
       1) Beim Treffen auf einen PdfTOC:
          - Ermittele die Wurzel-Sektion (oberste IDocSection) durch Hochlaufen über Parent.
          - Falls Wurzel vorhanden: t.RebuildFrom(rootSection) aufrufen, um das TOC zu generieren.
       2) Überschrift für das TOC ausgeben:
          - Wenn t.Name nicht leer/whitespace ist: engine.AddHeadline(t.Level, t.Name).
       3) Inhalt des TOC ausgeben:
          - text = t.GetTextContent(true)
          - Wenn text nicht leer/whitespace: engine.WriteText(text).
       4) break.
     - Hilfsfunktion:
       - FindRootSection(IDocElement node): Läuft über PdfNodeBase.Parent bis zum obersten Knoten
         und gibt diesen als IDocSection zurück, wenn möglich.
    */

    public static void Render(PdfSection root, IPdfEngine engine)
    {
        engine.BeginDocument();
        foreach (var node in root.Children)
            WriteNode(node, engine);
    }

    private static void WriteNode(IDocElement node, IPdfEngine engine)
    {
        switch (node)
        {
            case PdfHeadline h:
                engine.AddHeadline(h.Level, h.GetTextContent(true));
                h.Page = engine.CurrentPageNumber;
                break;

            case PdfParagraph p:
                var text = p.GetTextContent(true);
                if (!string.IsNullOrWhiteSpace(text))
                    engine.WriteText(text);
                break;

            case PdfTOC t:
                var rootSection = FindRootSection(t);
                if (rootSection is not null)
                    t.RebuildFrom(rootSection);

                if (!string.IsNullOrWhiteSpace(t.Name))
                    engine.AddHeadline(t.Level, t.Name);

                var tocText = t.GetTextContent(true);
                if (!string.IsNullOrWhiteSpace(tocText))
                    engine.WriteText(tocText);
                break;

            case PdfSection s:
                foreach (var c in s.Children)
                    WriteNode(c, engine);
                break;

            case PdfContentBase c:
                var inline = c.GetTextContent(true);
                if (!string.IsNullOrWhiteSpace(inline))
                    engine.WriteLine(inline);
                break;
        }
    }

    private static IDocSection? FindRootSection(IDocElement node)
    {
        var current = node as PdfNodeBase;
        while (current?.Parent is PdfNodeBase parent)
            current = parent;

        return current as IDocSection;
    }
}