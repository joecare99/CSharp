using Document.Base.Models.Interfaces;

namespace Document.Odf.Models;

// Simple implementations of interfaces just to make parsing possible
public abstract class OdfNodeBase : IDocElement
{
    public IDictionary<string, string> Attributes { get; } = new Dictionary<string, string>();
    public IList<IDOMElement> Nodes { get; } = new List<IDOMElement>();
    public IDOMElement AddChild(IDOMElement element) { Nodes.Add(element); return element; }
    public string? GetAttribute(string name) { Attributes.TryGetValue(name, out var v); return v; }

    public IDocElement AppendDocElement(Enum aType) => throw new NotSupportedException();
    public IDocElement AppendDocElement(Enum aType, Type aClass) => throw new NotSupportedException();
    public IDocElement AppendDocElement(Enum aType, Enum aAttribute, string value, Type aClass, string? Id) => throw new NotSupportedException();
    public IEnumerable<IDocElement> Enumerate() => Nodes.OfType<IDocElement>();
}
