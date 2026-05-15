using Document.Base.Models.Interfaces;
using Document.Markdown.Model;
using Document.Markdown.Rendering;

namespace Document.MarkdownTests.Rendering;

[TestClass]
public sealed class MarkdownRendererTests
{
    [DataTestMethod]
    [DataRow(true, false, "**Text**")]
    [DataRow(false, true, "*Text*")]
    [DataRow(false, false, "Text")]
    public void Render_RendersSpanStyles(bool bold, bool italic, string expectedInline)
    {
        MarkdownSection root = new();
        MarkdownParagraph paragraph = new();
        MarkdownSpan span = new(bold ? MarkdownFontStyle.BoldStyle : italic ? MarkdownFontStyle.ItalicStyle : MarkdownFontStyle.Default)
        {
            TextContent = "Text"
        };
        paragraph.AddChild(span);
        root.AddChild(paragraph);

        string markdown = MarkdownRenderer.Render(root);

        Assert.AreEqual(expectedInline + Environment.NewLine, markdown);
    }

    [TestMethod]
    public void Render_RendersCombinedBoldItalicSpan()
    {
        MarkdownSection root = new();
        MarkdownParagraph paragraph = new();
        MarkdownSpan span = new(new CombinedFontStyle())
        {
            TextContent = "Text"
        };
        paragraph.AddChild(span);
        root.AddChild(paragraph);

        string markdown = MarkdownRenderer.Render(root);

        Assert.AreEqual("***Text***" + Environment.NewLine, markdown);
    }

    [TestMethod]
    public void Render_RendersHeadlineLinkAndTOCStructure()
    {
        MarkdownSection root = new();

        MarkdownHeadline headline = new(2, "heading-1")
        {
            TextContent = "Section"
        };
        root.AddChild(headline);

        MarkdownParagraph paragraph = new();
        MarkdownSpan link = (MarkdownSpan)paragraph.AddLink("https://example.com", MarkdownFontStyle.Default);
        link.TextContent = "Example";
        root.AddChild(paragraph);

        MarkdownTOC toc = new("Contents", 3);
        MarkdownParagraph tocEntry = new("TOCEntry");
        MarkdownSpan tocLink = (MarkdownSpan)tocEntry.AddLink("#heading-1", MarkdownFontStyle.Default);
        tocLink.TextContent = "Section";
        toc.AddChild(tocEntry);
        root.AddChild(toc);

        string markdown = MarkdownRenderer.Render(root);

        Assert.IsTrue(markdown.Contains("## Contents"));
        Assert.IsTrue(markdown.Contains("## Section"));
        Assert.IsTrue(markdown.Contains("[Example](https://example.com)"));
        Assert.IsTrue(markdown.Contains("- [Section](#heading-1)"));
    }

    [TestMethod]
    public void Render_EscapesMarkdownCharactersInPlainText()
    {
        MarkdownSection root = new();
        MarkdownParagraph paragraph = new()
        {
            TextContent = "a*b_[c]#d`e\\f"
        };
        root.AddChild(paragraph);

        string markdown = MarkdownRenderer.Render(root);

        Assert.AreEqual("a\\*b\\_\\[c\\]\\#d\\`e\\\\f" + Environment.NewLine, markdown);
    }

    private sealed class CombinedFontStyle : IDocFontStyle
    {
        public string? Name => "Combined";
        public bool Bold => true;
        public bool Italic => true;
        public bool Underline => false;
        public bool Strikeout => false;
        public string? Color => null;
        public string? FontFamily => null;
        public double? FontSizePt => null;
    }
}
