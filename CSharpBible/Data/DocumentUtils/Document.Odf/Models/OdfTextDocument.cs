using Document.Base.Models.Interfaces;

namespace Document.Odf.Models;

public class OdfTextDocument : IUserDocument
{
    public IDocElement Root => throw new NotImplementedException();

    public bool IsModified => throw new NotImplementedException();

    public static IUserDocument CreateUserDocument()
    {
        throw new NotImplementedException();
    }

    public IDocHeadline AddHeadline(int nLevel, string? Id = null)
    {
        throw new NotImplementedException();
    }

    public IDocParagraph AddParagraph(string cStylename)
    {
        throw new NotImplementedException();
    }

    public IDocTOC AddTOC(string cName, int nLevel)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IDocElement> Enumerate()
    {
        throw new NotImplementedException();
    }

    public bool LoadFrom(string cInputPath)
    {
        throw new NotImplementedException();
    }

    public bool LoadFrom(Stream sInputStream, object? options = null)
    {
        throw new NotImplementedException();
    }

    public void SaveTo(string cOutputPath)
    {
        throw new NotImplementedException();
    }

    public bool SaveTo(Stream sOutputStream, object? options = null)
    {
        throw new NotImplementedException();
    }

    bool IUserDocument.SaveTo(string cOutputPath)
    {
        throw new NotImplementedException();
    }
}
