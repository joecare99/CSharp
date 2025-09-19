using Document.Base.Models.Interfaces;

namespace Document.Html.Model;

public sealed class HtmlSection : HtmlNodeBase, IDocSection
{
    public IDocParagraph AddParagraph(string ATextStyleName)
    {
        var p = new HtmlParagraph(ATextStyleName);
        return (IDocParagraph)AddChild(p);
    }

    public IDocHeadline AddHeadline(int aLevel)
    {
        var h = new HtmlHeadline(aLevel);
        return (IDocHeadline)AddChild(h);
    }

    public IDocTOC AddTOC(string aName, int aLevel)
    {
        var toc = new HtmlTOC(aName, aLevel);
        AddChild(toc);
        return toc;
    }

}
