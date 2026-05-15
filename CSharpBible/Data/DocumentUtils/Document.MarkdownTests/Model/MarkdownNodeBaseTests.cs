using Document.Markdown.Model;
using Document.Base.Models.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Document.MarkdownTests.Model;

[TestClass]
public sealed class MarkdownNodeBaseTests
{
    [TestMethod]
    public void AddChild_SetsParentForNestedMarkdownNodes()
    {
        MarkdownSection root = new();
        MarkdownParagraph paragraph = new();
        root.AddChild(paragraph);

        MarkdownSpan span = new(MarkdownFontStyle.Default)
        {
            TextContent = "Child"
        };
        paragraph.AddChild(span);

        Assert.AreSame(root, paragraph.Parent);
        Assert.AreSame(paragraph, span.Parent);
    }

    [TestMethod]
    public void AddChild_DoesNotSetParentForNonMarkdownNodes()
    {
        MarkdownSection root = new();
        PlainElement plain = new();

        root.AddChild(plain);

        Assert.AreEqual(1, root.Nodes.Count);
        Assert.IsNull(plain.Parent);
    }

    [TestMethod]
    public void Enumerate_ReturnsDepthFirstTraversal()
    {
        MarkdownSection root = new();
        MarkdownParagraph paragraph = new();
        MarkdownSpan span = new(MarkdownFontStyle.Default)
        {
            TextContent = "Child"
        };

        paragraph.AddChild(span);
        root.AddChild(paragraph);

        List<Type> types = root.Enumerate().Select(item => item.GetType()).ToList();

        CollectionAssert.AreEqual(new[] { typeof(MarkdownSection), typeof(MarkdownParagraph), typeof(MarkdownSpan) }, types);
    }

    [TestMethod]
    public void AppendDocElement_CoversSupportedElementTypes()
    {
        MarkdownSection root = new();

        IDocElement section = root.AppendDocElement(MarkdownElementType.Section);
        IDocElement paragraph = root.AppendDocElement(MarkdownElementType.Paragraph, null!, "Body", typeof(MarkdownParagraph));
        IDocElement headline = root.AppendDocElement(MarkdownElementType.Headline, null!, "2", typeof(MarkdownHeadline), "heading-1");
        IDocElement toc = root.AppendDocElement(MarkdownElementType.TOC, null!, "Contents", typeof(MarkdownTOC));
        IDocElement span = root.AppendDocElement(MarkdownElementType.Span, null!, string.Empty, typeof(MarkdownSpan));
        IDocElement link = root.AppendDocElement(MarkdownElementType.Link, null!, "https://example.com", typeof(MarkdownSpan));
        IDocElement lineBreak = root.AppendDocElement(MarkdownElementType.LineBreak, null!, string.Empty, typeof(MarkdownLineBreak));
        IDocElement nbSpace = root.AppendDocElement(MarkdownElementType.NbSpace, null!, string.Empty, typeof(MarkdownNbSpace));
        IDocElement tab = root.AppendDocElement(MarkdownElementType.Tab, null!, string.Empty, typeof(MarkdownTab));
        IDocElement bookmark = root.AppendDocElement(MarkdownElementType.Bookmark, null!, "bookmark-1", typeof(MarkdownSpan));

        Assert.AreSame(root, ((MarkdownNodeBase)section).Parent);
        Assert.AreSame(root, ((MarkdownNodeBase)paragraph).Parent);
        Assert.AreSame(root, ((MarkdownNodeBase)headline).Parent);
        Assert.AreSame(root, ((MarkdownNodeBase)toc).Parent);
        Assert.AreSame(root, ((MarkdownNodeBase)span).Parent);
        Assert.AreSame(root, ((MarkdownNodeBase)link).Parent);
        Assert.AreSame(root, ((MarkdownNodeBase)lineBreak).Parent);
        Assert.AreSame(root, ((MarkdownNodeBase)nbSpace).Parent);
        Assert.AreSame(root, ((MarkdownNodeBase)tab).Parent);
        Assert.AreSame(root, ((MarkdownNodeBase)bookmark).Parent);
    }

    [TestMethod]
    public void AppendDocElement_CoversTypeBasedOverload()
    {
        MarkdownSection root = new();

        IDocElement element = root.AppendDocElement(MarkdownElementType.Section, typeof(MarkdownSection));

        Assert.IsInstanceOfType(element, typeof(MarkdownSection));
        Assert.AreSame(root, ((MarkdownNodeBase)element).Parent);
    }

    [TestMethod]
    public void AppendDocElement_CoversUnknownMarkdownElementFallback()
    {
        MarkdownSection root = new();

        Assert.ThrowsExactly<NotSupportedException>(() => root.AppendDocElement((MarkdownElementType)999, null!, string.Empty, typeof(MarkdownSection)));
    }

    [TestMethod]
    public void AppendDocElement_CoversNullValueCases()
    {
        MarkdownSection root = new();

        IDocElement paragraph = root.AppendDocElement(MarkdownElementType.Paragraph, null!, string.Empty, typeof(MarkdownParagraph));
        IDocElement headline = root.AppendDocElement(MarkdownElementType.Headline, null!, null!, typeof(MarkdownHeadline), "heading-1");
        IDocElement toc = root.AppendDocElement(MarkdownElementType.TOC, null!, null!, typeof(MarkdownTOC));
        IDocElement bookmark = root.AppendDocElement(MarkdownElementType.Bookmark, null!, null!, typeof(MarkdownSpan));

        Assert.IsNotNull(paragraph);
        Assert.IsNotNull(headline);
        Assert.IsNotNull(toc);
        Assert.IsNotNull(bookmark);
    }

    [TestMethod]
    public void AppendDocElement_ThrowsForUnsupportedType()
    {
        MarkdownSection root = new();

        Assert.ThrowsExactly<NotSupportedException>(() => root.AppendDocElement(DayOfWeek.Friday, null!, string.Empty, typeof(MarkdownSection)));
    }

    [TestMethod]
    public void GetAttribute_ReturnsStoredValue()
    {
        MarkdownSection root = new();
        root.Attributes["key"] = "value";

        Assert.AreEqual("value", root.GetAttribute("key"));
    }

    private sealed class PlainElement : IDOMElement
    {
        public MarkdownNodeBase? Parent { get; private set; }
        public IDictionary<string, string> Attributes { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public IList<IDOMElement> Nodes { get; } = new List<IDOMElement>();

        public IDOMElement AddChild(IDOMElement element)
        {
            Nodes.Add(element);
            return element;
        }

        public string? GetAttribute(string name) => Attributes.TryGetValue(name, out string? value) ? value : null;
    }
}
