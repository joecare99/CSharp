using Document.Base.Models.Interfaces;
using Document.Base.Registration;
using Document.Pdf.Model;
using Document.Pdf.Render;

namespace Document.Pdf;

[UserDocumentProvider("pdf", Extensions = new[] { ".pdf" }, ContentType = "application/pdf", DisplayName = "PDF Document")]
public sealed class PdfDocument : IUserDocument
{
    private bool _isModified;
    public IDocElement Root { get; private set; }

    public PdfDocument()
    {
        Root = new PdfSection();
    }

    public PdfDocument(IDocElement root)
    {
        Root = root is PdfSection
            ? root
            : throw new ArgumentException("Root muss vom Typ PdfSection sein.", nameof(root));
    }

    public bool IsModified => _isModified;

    public IDocParagraph AddParagraph(string cStylename)
    {
        _isModified = true;
        return EnsureRoot().AddParagraph(cStylename);
    }

    public IDocHeadline  AddHeadline(int nLevel)
    {
        _isModified = true;
        return EnsureRoot().AddHeadline(nLevel);
    }

    public IDocTOC AddTOC(string cName, int nLevel)
    {
        _isModified = true;
        return EnsureRoot().AddTOC(cName, nLevel);
    }

    public IEnumerable<IDocElement> Enumerate()
        => EnsureRoot().Enumerate();

    public bool SaveTo(string cOutputPath)
    {
        try
        {
            using var engine = new PdfSharpEngine();
            PdfRenderer.Render(EnsureRoot(), engine);
            engine.SaveToFile(cOutputPath);
            _isModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool SaveTo(Stream sOutputStream, object? options = null)
    {
        try
        {
            using var engine = new PdfSharpEngine();
            PdfRenderer.Render(EnsureRoot(), engine);
            engine.SaveToStream(sOutputStream);
            _isModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool LoadFrom(string cInputPath) => false; // PDF Laden nicht unterstützt (Minimal)
    public bool LoadFrom(Stream sInputStream, object? options = null) => false;

    private PdfSection EnsureRoot()
    {
        if (Root is PdfSection sec) return sec;
        sec = new PdfSection();
        Root = sec;
        _isModified = true;
        return sec;
    }
}