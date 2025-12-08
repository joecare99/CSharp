using Document.Base.Models.Interfaces;

namespace Document.Odf.Models;

public sealed class OdfSection : OdfNodeBase, IDocSection
{
    public IDocParagraph AddParagraph(string ATextStyleName)
    {
        var p = new OdfParagraph(ATextStyleName);
        return (IDocParagraph)AddChild(p);
    }

    public IDocHeadline AddHeadline(int aLevel, string Id)
    {
        var h = new OdfHeadline(aLevel, Id);
        return (IDocHeadline)AddChild(h);
    }

    public IDocTOC AddTOC(string aName, int aLevel)
    {
        var toc = new OdfTOC(aName, aLevel);
        AddChild(toc);
        return toc;
    }
}
