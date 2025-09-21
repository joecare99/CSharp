using Document.Base.Models.Interfaces;

namespace Document.Pdf.Model;

public abstract class PdfNodeBase : IDocElement
{
    protected readonly List<IDocElement> _children = new();
    public IReadOnlyList<IDocElement> Children => _children;
    public PdfNodeBase? Parent { get; private set; }

    public IDictionary<string, string> Attributes => throw new NotImplementedException();

    public IList<IDOMElement> Nodes => throw new NotImplementedException();

    protected T AddChild<T>(T element) where T : IDocElement
    {
        if (element is PdfNodeBase p) p.Parent = this;
        _children.Add(element);
        return element;
    }

    public virtual IDocElement AppendDocElement(Enum aType) => AppendDocElement(aType, null);
    public virtual IDocElement AppendDocElement(Enum aType, Type? aClass) => AppendDocElement(aType,default!, string.Empty,aClass);
    public virtual IDocElement AppendDocElement(Enum aType, Enum aAttribute, string value, Type aClass, string? Id = null)
    {
        if (aType is not PdfElementType type)
            throw new NotSupportedException($"Element type '{aType}' wird nicht unterstützt.");

        switch (type)
        {
            case PdfElementType.Section: return (IDocElement)AddChild(new PdfSection());
            case PdfElementType.Paragraph: return (IDocElement)AddChild(new PdfParagraph(value));
            case PdfElementType.Headline: return (IDocElement)AddChild(new PdfHeadline(TryParseInt(value, 1), Id));
            case PdfElementType.TOC: return (IDocElement)AddChild(new PdfTOC(value, TryParseInt(value, 2)));
            case PdfElementType.Span: return (IDocElement)AddChild(new PdfSpan(PdfFontStyle.Default));
            case PdfElementType.Link: return (IDocElement)AddChild(new PdfSpan(PdfFontStyle.Default) { Href = value });
            case PdfElementType.Bookmark: return (IDocElement)AddChild(new PdfSpan(PdfFontStyle.Default) { Id = Id });
            default: throw new NotSupportedException($"Element type '{aType}' wird nicht unterstützt.");
        }
    }
    protected static int TryParseInt(string? s, int fallback)
    => int.TryParse(s, out var v) ? v : fallback;

    public IEnumerable<IDocElement> Enumerate()
    {
        yield return this;
        foreach (var child in _children)
        {
            foreach (var desc in child.Enumerate())
            {
                yield return desc;
            }
        }
    }

    public string? GetAttribute(string name) 
        => Attributes.TryGetValue(name, out string? value) ? value : null;

    public IDOMElement AddChild(IDOMElement element)
    {
        Nodes.Add(element);
        return element;
    }
}

public enum PdfElementType
{
    Section,
    Paragraph,
    Headline,
    TOC,
    Span,
    Link,
    Bookmark
}