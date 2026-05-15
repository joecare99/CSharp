using System.Text;
using Document.Base.Models.Interfaces;
using Document.Base.Registration;
using Document.Markdown.IO;
using Document.Markdown.Model;
using Document.Markdown.Serialization;

namespace Document.Markdown;

[UserDocumentProvider("markdown", "md", Extensions = new[] { ".md", ".markdown" }, ContentType = "text/markdown", DisplayName = "Markdown Document")]
public sealed class MarkdownDocument : IUserDocument
{
    private bool _isModified;

    public MarkdownDocument()
    {
        Root = new MarkdownSection();
    }

    public MarkdownDocument(IDocElement root)
    {
        Root = root is MarkdownSection
            ? root
            : throw new ArgumentException("Root must be a MarkdownSection.", nameof(root));
    }

    public IDocElement Root { get; private set; }

    public bool IsModified => _isModified;

    public IDocParagraph AddParagraph(string cStylename)
    {
        MarkdownSection section = EnsureRoot();
        _isModified = true;
        return section.AddParagraph(cStylename);
    }

    public IDocHeadline AddHeadline(int nLevel, string? Id = null)
    {
        MarkdownSection section = EnsureRoot();
        _isModified = true;
        return section.AddHeadline(nLevel, Id ?? string.Empty);
    }

    public IDocTOC AddTOC(string cName, int nLevel)
    {
        MarkdownSection section = EnsureRoot();
        _isModified = true;
        return section.AddTOC(cName, nLevel);
    }

    public IEnumerable<IDocElement> Enumerate() => Root.Enumerate();

    public bool SaveTo(string cOutputPath)
    {
        try
        {
            MarkdownSection section = EnsureRoot();
            MarkdownDocumentIO.SaveAsync(section, cOutputPath).GetAwaiter().GetResult();
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
            MarkdownSection section = EnsureRoot();
            string markdown = MarkdownDocumentSerializer.ToMarkdownString(section);

            using StreamWriter writer = new(sOutputStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), leaveOpen: true);
            writer.Write(markdown);
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
            MarkdownSection section = MarkdownDocumentIO.LoadAsync(cInputPath).GetAwaiter().GetResult();
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
            using StreamReader reader = new(sInputStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
            string markdown = reader.ReadToEnd();

            MarkdownSection section = MarkdownDocumentSerializer.FromMarkdownStringAsync(markdown).GetAwaiter().GetResult();
            Root = section;
            _isModified = false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private MarkdownSection EnsureRoot()
    {
        if (Root is MarkdownSection section)
        {
            return section;
        }

        section = new MarkdownSection();
        Root = section;
        _isModified = true;
        return section;
    }
}
