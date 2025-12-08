using Document.Base.Models.Interfaces;

namespace Document.Odf;

public sealed class OdfTextDocument : IUserDocument
{
    private bool _isModified;

    public OdfTextDocument()
    {
        Root = new Document.Odf.Models.OdfSection();
    }

    public OdfTextDocument(IDocElement root)
    {
        Root = root is Document.Odf.Models.OdfSection
            ? root
            : throw new ArgumentException("Root muss vom Typ OdfSection sein.", nameof(root));
    }

    public IDocElement Root { get; private set; }

    public bool IsModified => _isModified;

    public IDocParagraph AddParagraph(string cStylename)
    {
        var section = EnsureRoot();
        _isModified = true;
        return section.AddParagraph(cStylename);
    }

    public IDocHeadline AddHeadline(int nLevel, string? Id = null)
    {
        var section = EnsureRoot();
        _isModified = true;
        return section.AddHeadline(nLevel, Id ?? Guid.NewGuid().ToString("N"));
    }

    public IDocTOC AddTOC(string cName, int nLevel)
    {
        var section = EnsureRoot();
        _isModified = true;
        return section.AddTOC(cName, nLevel);
    }

    public IEnumerable<IDocElement> Enumerate()
        => Root.Enumerate();

    public bool SaveTo(string cOutputPath)
        => throw new NotSupportedException();

    public bool SaveTo(Stream sOutputStream, object? options = null)
        => throw new NotSupportedException();

    public bool LoadFrom(string cInputPath)
    {
        try
        {
            using var odf = OdfDocument.Open(cInputPath);
            var xdoc = odf.ReadContentXml();
            Document.Odf.Parsing.OdfBlockParser.Populate(this, xdoc);
            _isModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool LoadFrom(Stream sInputStream, object? options = null)
    {
        try
        {
            using var zip = new System.IO.Compression.ZipArchive(sInputStream, System.IO.Compression.ZipArchiveMode.Read, leaveOpen: true);
            var entry = zip.GetEntry("content.xml") ?? throw new FileNotFoundException("content.xml not found");
            using var stream = entry.Open();
            var xdoc = System.Xml.Linq.XDocument.Load(stream);
            Document.Odf.Parsing.OdfBlockParser.Populate(this, xdoc);
            _isModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private Models.OdfSection EnsureRoot()
        => Root as Models.OdfSection ?? throw new InvalidOperationException("Root ist nicht OdfSection");
}
