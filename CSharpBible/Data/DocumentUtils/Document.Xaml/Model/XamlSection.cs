using Document.Base.Models.Interfaces;

namespace Document.Xaml.Model;

public sealed class XamlSection : XamlNodeBase, IDocSection
{
    public IDocParagraph AddParagraph(string ATextStyleName)
    {
        var p = new XamlParagraph(ATextStyleName);
        return (IDocParagraph)AddChild(p);
    }

    public IDocHeadline AddHeadline(int aLevel, string Id)
    {
        var h = new XamlHeadline(aLevel,Id);
        return (IDocHeadline)AddChild(h);
    }

    public IDocTOC AddTOC(string aName, int aLevel)
    {
        var toc = new XamlTOC(aName, aLevel);
        AddChild(toc);
        return toc;
    }
}