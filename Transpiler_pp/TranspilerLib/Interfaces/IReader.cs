namespace TranspilerLib.Interfaces
{
    public interface IReader
    {
        bool IsEmptyElement { get; }
        bool HasValue { get; }

        bool EOF();
        int GetAttributeCount();
        string GetAttributeName(int i);
        object GetAttributeValue(int i);
        string GetLocalName();
        object getValue();
        bool IsEndElement();
        bool IsStartElement();
        bool Read();
    }
}