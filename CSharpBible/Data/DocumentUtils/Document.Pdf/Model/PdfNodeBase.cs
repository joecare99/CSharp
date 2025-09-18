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

    public virtual IDocElement AppendDocElement(Enum aType) => throw new NotSupportedException();
    public virtual IDocElement AppendDocElement(Enum aType, Type aClass) => AppendDocElement(aType);
    public virtual IDocElement AppendDocElement(Enum aType, Enum aAttribute, string value, Type aClass) => AppendDocElement(aType);

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