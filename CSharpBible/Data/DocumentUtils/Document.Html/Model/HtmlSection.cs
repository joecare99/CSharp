using Document.Base.Models.Interfaces;

namespace Document.Html.Model;

public sealed class HtmlSection : HtmlNodeBase, IDocSection
{
    public IDocParagraph AddParagraph(string ATextStyleName)
    {
        var p = new HtmlParagraph(ATextStyleName);
        return (IDocParagraph)AddChild(p);
    }

    public IDocContent AddHeadline(int aLevel)
    {
        var h = new HtmlHeadline(aLevel);
        return (IDocContent)AddChild(h);
    }

    public IDocContent AddTOC(string aName, int aLevel)
    {
        var toc = new HtmlTOC(aName, aLevel);
        AddChild(toc);
        return toc;
    }

    public IEnumerable<IDocElement> Enumerate()
    {
        var stack = new Stack<IDocElement>();
        stack.Push(this);
        while (stack.Count > 0)
        {
            var cur = stack.Pop();
            yield return cur;
            if (cur is HtmlNodeBase b)
            {
                for (int i = b.Nodes.Count - 1; i >= 0; i--)
                {
                    stack.Push((IDocElement)b.Nodes[i]);
                }
            }
        }
    }
}
