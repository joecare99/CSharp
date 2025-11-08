using System;
using System.Collections.Generic;
using Document.Base.Models.Interfaces;

namespace Document.Docx.Model;

public sealed class DocxSection : DocxNodeBase, IDocSection
{
    public IDocParagraph AddParagraph(string ATextStyleName)
        => (IDocParagraph)AddChild(new DocxParagraph(ATextStyleName));

    public IDocHeadline AddHeadline(int aLevel, string Id)
        => (IDocHeadline)AddChild(new DocxHeadline(aLevel, Id));

    public IDocTOC AddTOC(string aName, int aLevel)
        => (IDocTOC)AddChild(new DocxTOC(aName, aLevel));

    public IDocElement AppendDocElement(System.Enum aType)
        => throw new System.NotSupportedException();

    public IDocElement AppendDocElement(System.Enum aType, System.Type aClass)
        => throw new System.NotSupportedException();

    public IDocElement AppendDocElement(System.Enum aType, System.Enum aAttribute, string value, System.Type aClass, string? Id)
        => throw new System.NotSupportedException();

    public System.Collections.Generic.IEnumerable<IDocElement> Enumerate()
    {
        foreach (var n in Nodes)
            if (n is IDocElement de)
            {
                yield return de;
                foreach (var c in de.Enumerate())
                    yield return c;
            }
    }
}
