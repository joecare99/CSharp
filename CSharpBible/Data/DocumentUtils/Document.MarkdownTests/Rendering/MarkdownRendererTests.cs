using Document.Base.Models.Interfaces;
using Document.Markdown.Model;
using Document.Markdown.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

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
        IDocSpan link = paragraph.AddLink("https://example.com", MarkdownFontStyle.Default);
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
    public void Render_RendersTOCChildElementsThatAreNotParagraphs()
    {
        MarkdownSection root = new();

        MarkdownTOC toc = new("Contents", 3);
        toc.AddChild(new MarkdownHeadline(2, "heading-1")
        {
            TextContent = "Section"
        });
        root.AddChild(toc);

        string markdown = MarkdownRenderer.Render(root);

        Assert.IsTrue(markdown.Contains("## Contents"));
        Assert.IsTrue(markdown.Contains("## Section"));
    }

    [TestMethod]
    public void Render_RendersNestedSectionAndUnknownContentBranch()
    {
        MarkdownSection root = new();

        MarkdownSection nestedSection = new();
        nestedSection.AddChild(new MarkdownParagraph { TextContent = "Nested" });
        root.AddChild(nestedSection);

        root.AddChild(new CustomContent { TextContent = "Plain*Text" });

        string markdown = MarkdownRenderer.Render(root);

        Assert.IsTrue(markdown.Contains("Nested"));
        Assert.IsTrue(markdown.Contains("Plain\\*Text"));
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

    [TestMethod]
    public void Render_EscapesEmptyStringReturnsEmpty()
    {
        Assert.AreEqual(string.Empty, InvokeEscapeMarkdown(string.Empty));
    }

    [TestMethod]
    public void Render_EscapesEmptyTextAndIgnoresUnknownNonContentElements()
    {
        MarkdownSection root = new();
        MarkdownParagraph paragraph = new();
        paragraph.TextContent = string.Empty;
        root.AddChild(paragraph);
        root.AddChild(new UnknownElement());

        string markdown = MarkdownRenderer.Render(root);

        Assert.AreEqual(Environment.NewLine, markdown);
    }

    [TestMethod]
    public void Render_RendersLineBreakNbSpaceAndTabAndFallbackContent()
    {
        MarkdownSection root = new();
        MarkdownParagraph paragraph = new();
        paragraph.TextContent = "A";
        paragraph.AddLineBreak();
        paragraph.AddNBSpace(MarkdownFontStyle.Default);
        paragraph.AddTab(MarkdownFontStyle.Default);
        paragraph.AddChild(new FallbackContent { TextContent = "B*" });
        root.AddChild(paragraph);

        string markdown = MarkdownRenderer.Render(root);

        Assert.IsTrue(markdown.Contains("A  "));
        Assert.IsTrue(markdown.Contains("&nbsp;"));
        Assert.IsTrue(markdown.Contains("    "));
        Assert.IsTrue(markdown.Contains("B\\*"));
    }

    [TestMethod]
    public void Render_RendersUnderlineAndStrikeoutSpans()
    {
        MarkdownSection root = new();
        MarkdownParagraph paragraph = new();
        paragraph.AddSpan("Under", new CustomFontStyle { Underline = true });
        paragraph.AddSpan("Strike", new CustomFontStyle { Strikeout = true });
        root.AddChild(paragraph);

        string markdown = MarkdownRenderer.Render(root);

        Assert.IsTrue(markdown.Contains("<u>Under</u>"));
        Assert.IsTrue(markdown.Contains("~~Strike~~"));
    }

    private sealed class FallbackContent : MarkdownContentBase
    {
        public override IDocStyleStyle GetStyle() => new MarkdownStyle("Fallback");
    }

    private sealed class CustomFontStyle : IDocFontStyle
    {
        public string? Name => "Custom";
        public bool Bold { get; init; }
        public bool Italic { get; init; }
        public bool Underline { get; init; }
        public bool Strikeout { get; init; }
        public string? Color => null;
        public string? FontFamily => null;
        public double? FontSizePt => null;
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

    private sealed class UnknownElement : IDOMElement
    {
        public IDictionary<string, string> Attributes { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public IList<IDOMElement> Nodes { get; } = new List<IDOMElement>();

        public IDOMElement AddChild(IDOMElement element)
        {
            Nodes.Add(element);
            return element;
        }

        public string? GetAttribute(string name) => Attributes.TryGetValue(name, out string? value) ? value : null;
    }

    private sealed class CustomContent : MarkdownContentBase
    {
        public override IDocStyleStyle GetStyle() => new MarkdownStyle("CustomContent");
    }

    private static string InvokeEscapeMarkdown(string value)
    {
        var method = typeof(MarkdownRenderer).GetMethod("EscapeMarkdown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        return (string)method!.Invoke(null, [value])!;
    }
}
