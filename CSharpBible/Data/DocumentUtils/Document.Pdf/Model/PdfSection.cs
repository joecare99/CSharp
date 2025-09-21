using Document.Base.Models.Interfaces;

namespace Document.Pdf.Model;

public sealed class PdfSection : PdfNodeBase, IDocSection
{
    public IDocParagraph AddParagraph(string ATextStyleName)
    {
        var p = new PdfParagraph(ATextStyleName);
        return AddChild(p);
    }

    public IDocHeadline AddHeadline(int aLevel, string Id)
    {
        var h = new PdfHeadline(aLevel, Id);
        return AddChild(h);
    }

    public IDocTOC AddTOC(string aName, int aLevel)
    {
        var p = new PdfTOC(aName,aLevel);
        p.AppendText($"{aName} (bis H{aLevel})");
        return AddChild(p);
    }

    public IEnumerable<IDocElement> Enumerate()
    {
        var stack = new Stack<IDocElement>();
        stack.Push(this);
        while (stack.Count > 0)
        {
            var cur = stack.Pop();
            yield return cur;
            if (cur is PdfNodeBase b)
            {
                for (int i = b.Children.Count - 1; i >= 0; i--)
                    stack.Push(b.Children[i]);
            }
        }
    }
}