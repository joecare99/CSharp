using Document.Base.Models.Interfaces;
using Document.Pdf.Model;

namespace Document.Pdf.Render;

public static class PdfRenderer
{
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
                break;

            case PdfParagraph p:
                var text = p.GetTextContent(true);
                if (!string.IsNullOrWhiteSpace(text))
                    engine.WriteText(text);
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
}