using Document.Base.Models.Interfaces;

namespace Document.Odf.Models;

public class OdfTextDocument : IUserDocument
{
    public static IUserDocument CreateUserDocument()
    {
        throw new NotImplementedException();
    }

    public IDocParagraph AddParagraph(string cStylename)
    {
        throw new NotImplementedException();
    }

    public void SaveTo(string cOutputPath)
    {
        throw new NotImplementedException();
    }
}
