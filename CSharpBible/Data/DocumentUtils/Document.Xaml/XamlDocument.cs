using System.Text;
using Document.Base.Models.Interfaces;
using Document.Base.Registration;
using Document.Xaml.IO;
using Document.Xaml.Model;
using Document.Xaml.Serialization;

namespace Document.Xaml;

[UserDocumentProvider("xaml", "flowdoc", Extensions = new[] { ".xaml" }, ContentType = "application/xaml+xml", DisplayName = "FlowDocument XAML")]
public sealed class XamlDocument : IUserDocument
{
    private bool _isModified;
    public IDocElement Root { get; private set; }

    public XamlDocument()
    {
        Root = new XamlSection();
    }

    public XamlDocument(IDocElement root)
    {
        Root = root is XamlSection
            ? root
            : throw new ArgumentException("Root muss vom Typ XamlSection sein.", nameof(root));
    }

    public bool IsModified => _isModified;

    public IDocParagraph AddParagraph(string cStylename)
    {
        _isModified = true;
        return EnsureRoot().AddParagraph(cStylename);
    }

    public IDocHeadline AddHeadline(int nLevel, string? Id= null)
    {
        _isModified = true;
        return EnsureRoot().AddHeadline(nLevel, Id);
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
            var section = EnsureRoot();
            XamlDocumentIO.SaveAsync(section, cOutputPath).GetAwaiter().GetResult();
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
            var section = EnsureRoot();
            var xaml = XamlDocumentSerializer.ToXamlString(section);
            using var writer = new StreamWriter(sOutputStream, new UTF8Encoding(false), leaveOpen: true);
            writer.Write(xaml);
            writer.Flush();
            _isModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Optional: Einfache Loader könnten später ergänzt werden
    public bool LoadFrom(string cInputPath) => false;
    public bool LoadFrom(Stream sInputStream, object? options = null) => false;

    private XamlSection EnsureRoot()
    {
        if (Root is XamlSection sec) return sec;
        sec = new XamlSection();
        Root = sec;
        _isModified = true;
        return sec;
    }

}