using Document.Base.Models.Interfaces;

namespace Document.Xaml.Model;

public abstract class XamlNodeBase : IDocElement
{
    public IList<IDOMElement> Nodes { get; } = new List<IDOMElement>();
    public IDictionary<string, string> Attributes { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public XamlNodeBase? Parent { get; private set; }

    public IDOMElement AddChild(IDOMElement element)
    {
        if (element is XamlNodeBase x) x.Parent = this;
        Nodes.Add(element);
        return element;
    }

    public string? GetAttribute(string name)
      => Attributes.TryGetValue(name, out var v) ? v : null;

    public virtual IDocElement AppendDocElement(Enum aType)
        => AppendDocElement(aType, aClass: null);

    public virtual IDocElement AppendDocElement(Enum aType, Type? aClass)
        => AppendDocElement(aType, aAttribute: default!, value: string.Empty, aClass, null);

    public virtual IDocElement AppendDocElement(Enum aType, Enum aAttribute, string value, Type? aClass, string? Id)
    {
        if (aType is not XamlElementType type)
            throw new NotSupportedException($"Element type '{aType}' wird nicht unterstützt.");

        return type switch
        {
            XamlElementType.Section   => (IDocElement)AddChild(new XamlSection()),
            XamlElementType.Paragraph => (IDocElement)AddChild(new XamlParagraph(styleName: value)),
            XamlElementType.Headline  => (IDocElement)AddChild(new XamlHeadline(level: TryParseInt(value, 1),id: Id)),
            XamlElementType.TOC       => (IDocElement)AddChild(new XamlTOC(name: value, level: TryParseInt(value, 2))),
            XamlElementType.Span      => (IDocElement)AddChild(new XamlSpan(XamlFontStyle.Default)),
            XamlElementType.Link      => (IDocElement)AddChild(new XamlSpan(XamlFontStyle.Default) { Href = value }),
            XamlElementType.LineBreak => (IDocElement)AddChild(new XamlLineBreak()),
            XamlElementType.NbSpace   => (IDocElement)AddChild(new XamlNbSpace()),
            XamlElementType.Tab       => (IDocElement)AddChild(new XamlTab()),
            XamlElementType.Bookmark  => (IDocElement)AddChild(new XamlSpan(XamlFontStyle.Default) { Id = Id }),
            _ => throw new NotSupportedException($"Element type '{aType}' wird nicht unterstützt.")
        };
    }

    protected static int TryParseInt(string? s, int fallback)
        => int.TryParse(s, out var v) ? v : fallback;

    public IEnumerable<IDocElement> Enumerate()
    {
        var stack = new Stack<IDocElement>();
        stack.Push(this);
        while (stack.Count > 0)
        {
            var cur = stack.Pop();
            yield return cur;
            if (cur is XamlNodeBase b)
            {
                for (int i = b.Nodes.Count - 1; i >= 0; i--)
                {
                    stack.Push((IDocElement)b.Nodes[i]);
                }
            }
        }
    }
}

public enum XamlElementType
{
    Section,
    Paragraph,
    Headline,
    TOC,
    Span,
    Link,
    LineBreak,
    NbSpace,
    Tab,
    Bookmark
}