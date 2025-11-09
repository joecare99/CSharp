using System;
using System.Collections.Generic;
using Document.Base.Models.Interfaces;

namespace Document.Docx.Model;

public abstract class DocxNodeBase : IDOMElement
{
    private readonly List<IDOMElement> _nodes = new();
    private readonly Dictionary<string,string> _attributes = new();

    public IDictionary<string, string> Attributes => _attributes;
    public IList<IDOMElement> Nodes => _nodes;

    public IDOMElement AddChild(IDOMElement element)
    {
        _nodes.Add(element);
        return element;
    }

    public string? GetAttribute(string name) => _attributes.TryGetValue(name, out var v) ? v : null;

    protected T AddChild<T>(T el) where T: IDOMElement
    {
        _nodes.Add(el);
        return el;
    }
}
