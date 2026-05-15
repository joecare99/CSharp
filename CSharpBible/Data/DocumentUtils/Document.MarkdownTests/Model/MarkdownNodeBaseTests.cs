using Document.Markdown.Model;

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
}
