using Document.Base.Models.Interfaces;
using Document.Odf.Parsing;

namespace Document.Odf.Adapters;

public sealed class OdfDocumentReader
{
    public bool CanRead(string path) => path != null && path.EndsWith(".odt", StringComparison.OrdinalIgnoreCase);

    public IUserDocument ReadIntoNew(string path, CancellationToken ct = default)
    {
        using var odf = OdfDocument.Open(path);
        var content = odf.ReadContentXml();
        var doc = new OdfTextDocument();
        OdfBlockParser.Populate(doc, content);
        return doc;
    }
}