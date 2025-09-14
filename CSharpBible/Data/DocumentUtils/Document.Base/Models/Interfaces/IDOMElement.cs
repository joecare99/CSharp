namespace Document.Base.Models.Interfaces;

public interface IDOMElement
{
    IDictionary<string, string> Attributes { get; }
    string? GetAttribute(string name);
    IList<IDOMElement> Nodes { get; }
    IDOMElement AddChild(IDOMElement element);

}