using Document.Base.Models.Interfaces;

namespace Document.Html.Model;

public abstract class HtmlNodeBase : IDocElement
{
    public IList<IDOMElement> Nodes { get; } = new List<IDOMElement>();

    public HtmlNodeBase? Parent { get; private set; }

    public IDictionary<string, string> Attributes { get; } = new Dictionary<string, string>();

    public IDOMElement AddChild(IDOMElement element)
    {
        if (element is HtmlNodeBase h)
        {
            h.Parent = this;
        }
        Nodes.Add(element);
        return element;
    }

    public virtual IDocElement AppendDocElement(Enum aType)
        => AppendDocElement(aType, aClass: null);

    public virtual IDocElement AppendDocElement(Enum aType, Type? aClass)
        => AppendDocElement(aType, aAttribute: default!, value: string.Empty, aClass: aClass);

    public virtual IDocElement AppendDocElement(Enum aType, Enum aAttribute, string value, Type? aClass, string? Id = null)
    {
        if (aType is not HtmlElementType type)
            throw new NotSupportedException($"Element type '{aType}' wird nicht unterstützt.");

        return type switch
        {
            HtmlElementType.Section   => (IDocSpan)AddChild(new HtmlSection()),
            HtmlElementType.Paragraph => (IDocSpan)AddChild(new HtmlParagraph(styleName: value)),
            HtmlElementType.Headline  => (IDocSpan)AddChild(new HtmlHeadline(level: TryParseInt(value, 1), id: Id)),
            HtmlElementType.TOC       => (IDocSpan)AddChild(new HtmlTOC(name: value, level: TryParseInt(value, 2))),
            HtmlElementType.Span      => (IDocSpan)AddChild(new HtmlSpan(style: HtmlFontStyle.Default)),
            HtmlElementType.Link      => (IDocSpan)AddChild(new HtmlSpan(style: HtmlFontStyle.Default) { Href = value }),
            HtmlElementType.LineBreak => (IDocSpan)AddChild(new HtmlLineBreak()),
            HtmlElementType.NbSpace   => (IDocSpan)AddChild(new HtmlNbSpace()),
            HtmlElementType.Tab       => (IDocSpan)AddChild(new HtmlTab()),
            HtmlElementType.Bookmark  => (IDocSpan)AddChild(new HtmlSpan(style: HtmlFontStyle.Default) { Id = value }),
            _ => throw new NotSupportedException($"Element type '{aType}' wird nicht unterstützt.")
        };
    }

    protected static int TryParseInt(string? s, int fallback)
        => int.TryParse(s, out var v) ? v : fallback;

    public string? GetAttribute(string name)
    {
        Attributes.TryGetValue(name, out var value);
        return value;
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
