using System.Text;
using Document.Base.Models.Interfaces;
using Document.Markdown;
using Document.Markdown.Model;

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
        Assert.ThrowsException<ArgumentException>(() => new MarkdownDocument(new MarkdownParagraph()));
    }

    [TestMethod]
    public void AddParagraph_AddHeadline_AndAddTOC_SetModifiedAndParentRelationships()
    {
        MarkdownDocument document = new();

        IDocParagraph paragraph = document.AddParagraph("Body");
        IDocHeadline headline = document.AddHeadline(2, "heading-1");
        IDocTOC toc = document.AddTOC("Contents", 2);

        MarkdownSection root = document.Root as MarkdownSection ?? throw new AssertFailedException();

        Assert.IsTrue(document.IsModified);
        Assert.AreSame(root, ((MarkdownParagraph)paragraph).Parent);
        Assert.AreSame(root, ((MarkdownHeadline)headline).Parent);
        Assert.AreSame(root, ((MarkdownTOC)toc).Parent);
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
    public void SaveTo_File_WritesMarkdownContent()
    {
        MarkdownDocument document = new();
        document.AddHeadline(1, "intro");
        document.AddParagraph(string.Empty);
        ((MarkdownParagraph)((MarkdownSection)document.Root).Nodes[1]).TextContent = "Hello world";

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
        document.AddHeadline(1, "intro");
        document.AddParagraph("Hello world");

        using MemoryStream stream = new();

        bool saved = document.SaveTo(stream);

        Assert.IsTrue(saved);
        Assert.IsFalse(document.IsModified);

        string markdown = Encoding.UTF8.GetString(stream.ToArray());
        Assert.IsTrue(markdown.Contains("# intro"));
        Assert.IsTrue(markdown.Contains("Hello world"));
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

        MarkdownSection root = document.Root as MarkdownSection ?? throw new AssertFailedException();
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

        MarkdownSection root = document.Root as MarkdownSection ?? throw new AssertFailedException();
        Assert.AreEqual(0, root.Nodes.Count);
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

            MarkdownSection root = document.Root as MarkdownSection ?? throw new AssertFailedException();
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
}
