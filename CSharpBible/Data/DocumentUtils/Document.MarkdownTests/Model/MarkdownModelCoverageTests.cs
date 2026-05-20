using Document.Markdown.Model;
using Document.Base.Models.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Document.MarkdownTests.Model;

[TestClass]
public sealed class MarkdownModelCoverageTests
{
    [TestMethod]
    public void MarkdownSpan_CoversAttributesAndStyleMethods()
    {
        MarkdownSpan span = new(MarkdownFontStyle.Default);

        span.Href = "https://example.com";
        span.Id = "anchor-1";
        span.SetStyle(MarkdownFontStyle.BoldStyle);

        Assert.AreEqual("https://example.com", span.Href);
        Assert.AreEqual("anchor-1", span.Id);
        Assert.IsTrue(span.IsLink);
        Assert.AreEqual("Bold", span.GetStyle().Name);

        span.Href = null;
        span.Id = null;

        Assert.IsNull(span.Href);
        Assert.IsNull(span.Id);
        Assert.IsFalse(span.IsLink);
    }

    [TestMethod]
    public void MarkdownFontStyle_ExposesStaticStyles()
    {
        Assert.AreEqual("Default", MarkdownFontStyle.Default.Name);
        Assert.AreEqual("Bold", MarkdownFontStyle.BoldStyle.Name);
        Assert.AreEqual("Italic", MarkdownFontStyle.ItalicStyle.Name);
        Assert.AreEqual("Underline", MarkdownFontStyle.UnderlineStyle.Name);
        Assert.AreEqual("Strikeout", MarkdownFontStyle.StrikeoutStyle.Name);
        Assert.IsFalse(MarkdownFontStyle.Default.Bold);
        Assert.IsFalse(MarkdownFontStyle.Default.Italic);
        Assert.IsNull(MarkdownFontStyle.Default.Color);
        Assert.IsNull(MarkdownFontStyle.Default.FontFamily);
        Assert.IsNull(MarkdownFontStyle.Default.FontSizePt);
    }

    [TestMethod]
    public void MarkdownSpan_CoversSetStyleAndNullAttributePaths()
    {
        MarkdownSpan span = new(MarkdownFontStyle.Default);

        span.SetStyle(MarkdownFontStyle.ItalicStyle);
        span.SetStyle((Document.Base.Models.Interfaces.IDocFontStyle)MarkdownFontStyle.BoldStyle);
        span.SetStyle((Document.Base.Models.Interfaces.IUserDocument?)null!, (Document.Base.Models.Interfaces.IDocFontStyle)MarkdownFontStyle.Default);
        span.Href = "https://example.com/path)";
        span.Id = "anchor-1";
        span.Href = null;
        span.Id = null;

        Assert.AreEqual("Default", span.GetStyle().Name);
        Assert.IsNull(span.Href);
        Assert.IsNull(span.Id);
        Assert.IsFalse(span.IsLink);
    }

    [TestMethod]
    public void MarkdownSpan_NotImplementedMembersThrow()
    {
        MarkdownSpan span = new(MarkdownFontStyle.Default);

        Assert.ThrowsExactly<NotImplementedException>(() => span.SetStyle(new object()));
        Assert.ThrowsExactly<NotImplementedException>(() => span.SetStyle((Document.Base.Models.Interfaces.IUserDocument)null!, new object()));
        Assert.ThrowsExactly<NotImplementedException>(() => span.SetStyle("plain-style"));
    }

    [TestMethod]
    public void MarkdownStyle_StoresNameAndProperties()
    {
        MarkdownStyle style = new("TestStyle");
        style.Properties["x"] = "y";

        Assert.AreEqual("TestStyle", style.Name);
        Assert.AreEqual("y", style.Properties["x"]);
    }

    [TestMethod]
    public void MarkdownTOC_RebuildFrom_RebuildsEntriesAndAddsAnchors()
    {
        MarkdownSection root = new();

        MarkdownHeadline headline = new(2, "heading-1")
        {
            TextContent = "Section 1"
        };
        headline.AddChild(new MarkdownSpan(MarkdownFontStyle.Default)
        {
            TextContent = "Section 1"
        });
        root.AddChild(headline);

        MarkdownTOC toc = new("Contents", 3);
        toc.RebuildFrom(root);

        Assert.AreEqual("Contents", toc.Name);
        Assert.AreEqual(1, toc.Nodes.Count);

        MarkdownParagraph entry = (MarkdownParagraph)toc.Nodes[0];
        MarkdownSpan link = (MarkdownSpan)entry.Nodes[0];
        Assert.IsTrue(link.IsLink);
        Assert.IsTrue(link.Href!.StartsWith("#", StringComparison.Ordinal));
        Assert.IsTrue(headline.Nodes.OfType<MarkdownSpan>().Any(span => !string.IsNullOrEmpty(span.Id)));
    }

    [TestMethod]
    public void MarkdownParagraph_CoversConstructorAndBookmarkAndStyle()
    {
        MarkdownParagraph paragraph = new("BodyStyle");
        IDocSpan bookmark = paragraph.AddBookmark("bookmark-1", MarkdownFontStyle.Default);

        Assert.AreEqual("BodyStyle", paragraph.StyleName);
        Assert.AreEqual("BodyStyle", paragraph.GetStyle().Name);
        Assert.IsFalse(string.IsNullOrEmpty(bookmark.Id));
    }

    [TestMethod]
    public void MarkdownContentBase_CoversAppendTextAndAddSpanOverloadsAndGetTextContent()
    {
        MarkdownParagraph paragraph = new();

        paragraph.AppendText("A");
        paragraph.AppendText(string.Empty);
        IDocSpan styledSpan = paragraph.AddSpan("B", new List<object>());
        IDocSpan fontStyleSpan = paragraph.AddSpan("C", MarkdownFontStyle.ItalicStyle);
        IDocSpan enumStyleSpan = paragraph.AddSpan("D", Document.Base.Models.EFontStyle.UnderlineBoldItalic);
        IDocSpan link = paragraph.AddLink("https://example.com", MarkdownFontStyle.Default);
        paragraph.AddNBSpace(MarkdownFontStyle.Default);
        paragraph.AddTab(MarkdownFontStyle.Default);

        styledSpan.TextContent = "B";
        fontStyleSpan.TextContent = "C";
        enumStyleSpan.TextContent = "D";
        link.TextContent = "E";

        Assert.AreEqual("A", paragraph.GetTextContent(false));
        Assert.IsTrue(paragraph.GetTextContent(true).Contains("ABCDE"));
        Assert.IsInstanceOfType(styledSpan, typeof(MarkdownSpan));
        Assert.IsInstanceOfType(fontStyleSpan, typeof(MarkdownSpan));
        Assert.IsInstanceOfType(enumStyleSpan, typeof(MarkdownSpan));
        Assert.IsTrue(link is MarkdownSpan markdownLink && markdownLink.IsLink);
    }

    [TestMethod]
    public void MarkdownContentBase_CoversAddSpanByEnumStyleBranches()
    {
        MarkdownParagraph paragraph = new();

        IDocSpan bold = paragraph.AddSpan("B", Document.Base.Models.EFontStyle.Bold);
        IDocSpan italic = paragraph.AddSpan("I", Document.Base.Models.EFontStyle.Italic);
        IDocSpan underline = paragraph.AddSpan("U", Document.Base.Models.EFontStyle.Underline);
        IDocSpan strikeout = paragraph.AddSpan("S", Document.Base.Models.EFontStyle.Strikeout);

        Assert.IsInstanceOfType(bold, typeof(MarkdownSpan));
        Assert.IsInstanceOfType(italic, typeof(MarkdownSpan));
        Assert.IsInstanceOfType(underline, typeof(MarkdownSpan));
        Assert.IsInstanceOfType(strikeout, typeof(MarkdownSpan));
    }

    [TestMethod]
    public void MarkdownContentBase_AddSpanWithFontStyle_ReturnsChildSpan()
    {
        MarkdownParagraph paragraph = new();

        IDocSpan span = paragraph.AddSpan(MarkdownFontStyle.BoldStyle);

        Assert.IsInstanceOfType(span, typeof(MarkdownSpan));
        Assert.AreSame(paragraph, ((MarkdownNodeBase)span).Parent);
    }

    [TestMethod]
    public void MarkdownHeadline_GetStyle_ReturnsHeadlineStyleName()
    {
        MarkdownHeadline headline = new(4, "heading-4");

        Assert.AreEqual("H4", headline.GetStyle().Name);
    }

    [TestMethod]
    public void MarkdownTOC_RebuildFrom_UsesGeneratedAnchorWhenNoneExists()
    {
        MarkdownSection root = new();

        MarkdownHeadline headline = new(2, "heading-1")
        {
            TextContent = "Section 2"
        };
        root.AddChild(headline);

        MarkdownTOC toc = new("Contents", 3);
        toc.RebuildFrom(root);

        MarkdownParagraph entry = (MarkdownParagraph)toc.Nodes[0];
        MarkdownSpan link = (MarkdownSpan)entry.Nodes[0];

        Assert.IsTrue(link.Href!.StartsWith("#", StringComparison.Ordinal));
        Assert.IsTrue(headline.Nodes.OfType<MarkdownSpan>().Any(span => !string.IsNullOrEmpty(span.Id)));
    }

    [TestMethod]
    public void MarkdownTOC_RebuildFromElement_Throws()
    {
        MarkdownTOC toc = new("Contents", 2);

        Assert.ThrowsExactly<NotImplementedException>(() => toc.RebuildFrom((Document.Base.Models.Interfaces.IDocElement)new MarkdownSection()));
    }
}
