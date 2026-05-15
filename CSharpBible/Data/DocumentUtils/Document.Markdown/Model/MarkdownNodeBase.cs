using Document.Base.Models.Interfaces;

namespace Document.Markdown.Model;

public abstract class MarkdownNodeBase : IDocElement
{
    public IList<IDOMElement> Nodes { get; } = new List<IDOMElement>();

    public MarkdownNodeBase? Parent { get; private set; }

    public IDictionary<string, string> Attributes { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public IDOMElement AddChild(IDOMElement element)
    {
     if (element is MarkdownNodeBase node)
        {
            node.Parent = this;
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
        if (aType is not MarkdownElementType type)
        {
            throw new NotSupportedException($"Element type '{aType}' is not supported.");
        }

        return type switch
        {
            MarkdownElementType.Section => (IDocElement)AddChild(new MarkdownSection()),
            MarkdownElementType.Paragraph => (IDocElement)AddChild(new MarkdownParagraph(value)),
            MarkdownElementType.Headline => (IDocElement)AddChild(new MarkdownHeadline(TryParseInt(value, 1), Id ?? string.Empty)),
            MarkdownElementType.TOC => (IDocElement)AddChild(new MarkdownTOC(value, TryParseInt(value, 2))),
            MarkdownElementType.Span => (IDocElement)AddChild(new MarkdownSpan(MarkdownFontStyle.Default)),
            MarkdownElementType.Link => (IDocElement)AddChild(new MarkdownSpan(MarkdownFontStyle.Default) { Href = value, IsLink = true }),
            MarkdownElementType.LineBreak => (IDocElement)AddChild(new MarkdownLineBreak()),
            MarkdownElementType.NbSpace => (IDocElement)AddChild(new MarkdownNbSpace()),
            MarkdownElementType.Tab => (IDocElement)AddChild(new MarkdownTab()),
            MarkdownElementType.Bookmark => (IDocElement)AddChild(new MarkdownSpan(MarkdownFontStyle.Default) { Id = value }),
            _ => throw new NotSupportedException($"Element type '{aType}' is not supported.")
        };
    }

    protected static int TryParseInt(string? s, int fallback)
        => int.TryParse(s, out int value) ? value : fallback;

    public string? GetAttribute(string name)
    {
        Attributes.TryGetValue(name, out string? value);
        return value;
    }

    public IEnumerable<IDocElement> Enumerate()
    {
        Stack<IDocElement> stack = new();
        stack.Push(this);
        while (stack.Count > 0)
        {
            IDocElement current = stack.Pop();
            yield return current;
            if (current is MarkdownNodeBase node)
            {
                for (int index = node.Nodes.Count - 1; index >= 0; index--)
                {
                    stack.Push((IDocElement)node.Nodes[index]);
                }
            }
        }
    }
}
