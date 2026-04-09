namespace Document.Base.Models.Interfaces;

public interface IDocElement : IDOMElement
{
    public IDocElement AppendDocElement(Enum aType);
    public IDocElement AppendDocElement(Enum aType,Type aClass);
    public IDocElement AppendDocElement(Enum aType,Enum aAttribute,string value,Type aClass,string? Id);
    IEnumerable<IDocElement> Enumerate();
}
