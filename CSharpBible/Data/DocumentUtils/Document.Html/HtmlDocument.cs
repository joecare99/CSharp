using System.Text;
using Document.Base.Models.Interfaces;
using Document.Html.IO;
using Document.Html.Model;
using Document.Html.Serialization;

namespace Document.Html;

public sealed class HtmlDocument : IUserDocument
{
    private bool _isModified;

    public HtmlDocument()
    {
        Root = new HtmlSection();
    }

    public HtmlDocument(IDocElement root)
    {
        Root = root is HtmlSection
            ? root
            : throw new ArgumentException("Root muss vom Typ HtmlSection sein.", nameof(root));
    }

    public IDocElement Root { get; private set; }

    public bool IsModified => _isModified;

    public IDocParagraph AddParagraph(string cStylename)
    {
        var section = EnsureRoot();
        _isModified = true;
        return section.AddParagraph(cStylename);
    }

    public IDocContent AddHeadline(int nLevel)
    {
        var section = EnsureRoot();
        _isModified = true;
        return section.AddHeadline(nLevel);
    }

    public IDocContent AddTOC(string cName, int nLevel)
    {
        var section = EnsureRoot();
        _isModified = true;
        return section.AddTOC(cName, nLevel);
    }

    public IEnumerable<IDocElement> Enumerate()
        => Root.Enumerate();

    public bool SaveTo(string cOutputPath)
    {
        try
        {
            var section = EnsureRoot();
            HtmlDocumentIO.SaveAsync(section, cOutputPath).GetAwaiter().GetResult();
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
            var html = HtmlDocumentSerializer.ToHtmlString(section);

            using var writer = new StreamWriter(sOutputStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), leaveOpen: true);
            writer.Write(html);
            writer.Flush();

            _isModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool LoadFrom(string cInputPath)
    {
        try
        {
            var section = HtmlDocumentIO.LoadAsync(cInputPath).GetAwaiter().GetResult();
            Root = section;
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
            using var reader = new StreamReader(sInputStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
            var html = reader.ReadToEnd();

            var section = HtmlDocumentSerializer.FromHtmlStringAsync(html).GetAwaiter().GetResult();
            Root = section;
            _isModified = false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    private HtmlSection EnsureRoot()
    {
        if (Root is HtmlSection sec)
            return sec;

        sec = new HtmlSection();
        Root = sec;
        _isModified = true;
        return sec;
    }
}