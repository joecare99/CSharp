namespace Document.Base.Models.Interfaces;

public interface IUserDocument
{
    IDocParagraph AddParagraph(string cStylename);
    IDocHeadline AddHeadline(int nLevel,string? Id = null);
    IDocTOC AddTOC(string cName, int nLevel);
    IDocElement Root { get; }
    IEnumerable<IDocElement> Enumerate();
    bool IsModified { get; }

    bool SaveTo(string cOutputPath);
    bool SaveTo(Stream sOutputStream,object? options=null);
    bool LoadFrom(string cInputPath);
    bool LoadFrom(Stream sInputStream,object? options=null);

    }
