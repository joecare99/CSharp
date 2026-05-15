using System.Text;
using System.Reflection;
using System.Linq;
using Document.Base.Models.Interfaces;
using Document.Markdown;
using Document.Markdown.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Document.MarkdownTests;

[TestClass]
public sealed class MarkdownDocumentTests
{
    [TestMethod]
    public void Constructor_CreatesMarkdownSectionRoot_AndIsNotModified()
    {
        MarkdownDocument document = new();

        Assert.IsInstanceOfType(document.Root, typeof(MarkdownSection));
        Assert.IsFalse(document.IsModified);
        Assert.IsNull(((MarkdownSection)document.Root).Parent);
    }

    [TestMethod]
    public void Constructor_WithMarkdownSectionRoot_UsesProvidedRoot()
    {
        MarkdownSection root = new();

        MarkdownDocument document = new(root);

        Assert.AreSame(root, document.Root);
        Assert.IsFalse(document.IsModified);
    }

    [TestMethod]
    public void Constructor_WithNonSectionRoot_Throws()
    {
        Assert.Throws<ArgumentException>(() => new MarkdownDocument(new MarkdownParagraph()));
    }

    [TestMethod]
    public void AddParagraph_AddHeadline_AndAddTOC_SetModifiedAndParentRelationships()
    {
        MarkdownDocument document = new();

        IDocParagraph paragraph = document.AddParagraph("Body");
        IDocHeadline headline = document.AddHeadline(2, "heading-1");
        IDocTOC toc = document.AddTOC("Contents", 2);

        MarkdownSection root = document.Root as MarkdownSection ?? throw new InvalidOperationException();

        Assert.IsTrue(document.IsModified);
        Assert.AreSame(root, ((MarkdownParagraph)paragraph).Parent);
        Assert.AreSame(root, ((MarkdownHeadline)headline).Parent);
        Assert.AreSame(root, ((MarkdownTOC)toc).Parent);
    }

    [TestMethod]
    public void AddParagraph_RepairsNonSectionRoot()
    {
        MarkdownDocument document = new();
        typeof(MarkdownDocument).GetProperty(nameof(MarkdownDocument.Root), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(document, new MarkdownParagraph());

        document.AddParagraph("Body");

        Assert.IsInstanceOfType(document.Root, typeof(MarkdownSection));
        Assert.AreEqual(1, ((MarkdownSection)document.Root).Nodes.Count);
    }

    [TestMethod]
    public void Enumerate_ReturnsDocumentElementsDepthFirst()
    {
        MarkdownDocument document = new();
        document.AddHeadline(1, "intro");
        document.AddParagraph("Body");

        List<Type> types = document.Enumerate().Select(item => item.GetType()).ToList();

        CollectionAssert.AreEqual(new[] { typeof(MarkdownSection), typeof(MarkdownHeadline), typeof(MarkdownParagraph) }, types);
    }

    [TestMethod]
    public void AddTOC_RebuildsFromSectionContent()
    {
        MarkdownDocument document = new();
        MarkdownHeadline headline = (MarkdownHeadline)document.AddHeadline(1, "intro");
        headline.TextContent = "intro";

        MarkdownTOC toc = (MarkdownTOC)document.AddTOC("Contents", 2);
        toc.RebuildFrom((MarkdownSection)document.Root);

        Assert.AreEqual("TOC", toc.GetStyle().Name);
        Assert.IsTrue(toc.Nodes.Count > 0);
    }

    [TestMethod]
    public void SaveTo_File_WritesMarkdownContent()
    {
        MarkdownDocument document = new();
        document.AddHeadline(1, "intro");
        document.AddParagraph(string.Empty).TextContent = "Hello world";

        string path = Path.Combine(Path.GetTempPath(), $"markdown-doc-{Guid.NewGuid():N}.md");
        try
        {
            bool saved = document.SaveTo(path);

            Assert.IsTrue(saved);
            string markdown = File.ReadAllText(path, Encoding.UTF8);
            Assert.IsTrue(markdown.Contains("#"));
            Assert.IsTrue(markdown.Contains("Hello world"));
            Assert.IsFalse(document.IsModified);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    [TestMethod]
    public void SaveTo_Stream_WritesMarkdownContent()
    {
        MarkdownDocument document = new();
        document.AddHeadline(1, "intro").TextContent = "intro";
        document.AddParagraph(string.Empty).TextContent = "Hello world";

        using MemoryStream stream = new();

        bool saved = document.SaveTo(stream);

        Assert.IsTrue(saved);
        Assert.IsFalse(document.IsModified);

        string markdown = Encoding.UTF8.GetString(stream.ToArray());
        Assert.IsTrue(markdown.Contains("# intro"));
        Assert.IsTrue(markdown.Contains("Hello world"));
    }

    [TestMethod]
    public void SaveTo_Stream_ReturnsFalseWhenWriterFails()
    {
        MarkdownDocument document = new();
        document.AddHeadline(1, "intro").TextContent = "intro";

        bool saved = document.SaveTo(new ThrowingWriteStream());

        Assert.IsFalse(saved);
    }

    [TestMethod]
    public void SaveTo_InvalidPath_ReturnsFalse()
    {
        MarkdownDocument document = new();

        bool saved = document.SaveTo(string.Empty);

        Assert.IsFalse(saved);
    }

    [TestMethod]
    public void LoadFrom_Stream_CreatesParagraphFromMarkdownText()
    {
        MarkdownDocument document = new();
        using MemoryStream stream = new(Encoding.UTF8.GetBytes("# Title\n\nBody text"));

        bool loaded = document.LoadFrom(stream);

        Assert.IsTrue(loaded);
        Assert.IsFalse(document.IsModified);

        MarkdownSection root = document.Root as MarkdownSection ?? throw new InvalidOperationException();
        Assert.AreEqual(1, root.Nodes.Count);
        MarkdownParagraph paragraph = root.Nodes[0] as MarkdownParagraph ?? throw new AssertFailedException();
        Assert.AreEqual("# Title\n\nBody text", paragraph.TextContent);
    }

    [TestMethod]
    public void LoadFrom_Stream_WithWhitespace_CreatesEmptyRoot()
    {
        MarkdownDocument document = new();
        using MemoryStream stream = new(Encoding.UTF8.GetBytes(string.Empty));

        bool loaded = document.LoadFrom(stream);

        Assert.IsTrue(loaded);
        Assert.IsFalse(document.IsModified);

        MarkdownSection root = document.Root as MarkdownSection ?? throw new InvalidOperationException();
        Assert.AreEqual(0, root.Nodes.Count);
    }

    [TestMethod]
    public void LoadFrom_Stream_ReturnsFalseWhenReaderFails()
    {
        MarkdownDocument document = new();

        bool loaded = document.LoadFrom(new ThrowingReadStream());

        Assert.IsFalse(loaded);
    }

    [TestMethod]
    public void LoadFrom_File_CreatesParagraphFromMarkdownText()
    {
        string path = Path.Combine(Path.GetTempPath(), $"markdown-load-{Guid.NewGuid():N}.md");
        File.WriteAllText(path, "# Title\n\nBody text", Encoding.UTF8);

        try
        {
            MarkdownDocument document = new();

            bool loaded = document.LoadFrom(path);

            Assert.IsTrue(loaded);
            Assert.IsFalse(document.IsModified);

        MarkdownSection root = document.Root as MarkdownSection ?? throw new InvalidOperationException();
            Assert.AreEqual(1, root.Nodes.Count);
            MarkdownParagraph paragraph = root.Nodes[0] as MarkdownParagraph ?? throw new AssertFailedException();
            Assert.AreEqual("# Title\n\nBody text", paragraph.TextContent);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    [TestMethod]
    public void LoadFrom_InvalidPath_ReturnsFalse()
    {
        MarkdownDocument document = new();

        bool loaded = document.LoadFrom(string.Empty);

        Assert.IsFalse(loaded);
    }

    private sealed class ThrowingWriteStream : MemoryStream
    {
        public override void Write(byte[] buffer, int offset, int count) => throw new InvalidOperationException();
        public override void Write(ReadOnlySpan<byte> buffer) => throw new InvalidOperationException();
    }

    private sealed class ThrowingReadStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => 0;
        public override long Position { get => 0; set => throw new NotSupportedException(); }

        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => throw new InvalidOperationException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
