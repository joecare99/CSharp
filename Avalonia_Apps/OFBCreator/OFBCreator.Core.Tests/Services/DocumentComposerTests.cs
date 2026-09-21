using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Document.Base.Models;
using Document.Base.Models.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OFBCreator.Abstractions.Models;
using OFBCreator.Core.Services;

namespace OFBCreator.Core.Tests.Services;

/// <summary>
/// Comprehensive tests for <see cref="DocumentComposer"/>.
/// Verifies document structure, anchor integrity, and proper composition order.
/// </summary>
[TestClass]
public class DocumentComposerTests
{
    /// <summary>
    /// Tracks captured calls for verification.
    /// </summary>
    private sealed class DocumentRecorder : IUserDocument
    {
        public List<(int Level, string? Anchor)> Headlines { get; } = new();
        public List<string> Paragraphs { get; } = new();
        public List<(string Name, int Level)> Tocs { get; } = new();
        public bool IsModified { get; set; }
        public string? PrefaceText { get; set; }
        public string? LegendText { get; set; }

        public IDocParagraph AddParagraph(string cStylename)
        {
            Paragraphs.Add(cStylename);
            return ParagraphStub.Instance;
        }

        public IDocHeadline AddHeadline(int nLevel, string? Id = null)
        {
            Headlines.Add((nLevel, Id));
            return HeadlineStub.Instance;
        }

        public IDocTOC AddTOC(string cName, int nLevel)
        {
            Tocs.Add((cName, nLevel));
            return TocStub.Instance;
        }

        public IDocElement Root => ParagraphStub.Instance;
        public IEnumerable<IDocElement> Enumerate() => Array.Empty<IDocElement>();
        public bool SaveTo(string cOutputPath) => true;
        public bool SaveTo(Stream sOutputStream, object? options = null) => true;
        public bool LoadFrom(string cInputPath) => false;
        public bool LoadFrom(Stream sInputStream, object? options = null) => false;
    }

    /// <summary>
    /// Minimal IDOMElement implementation for stub base.
    /// </summary>
    private class DomElementStub : IDOMElement
    {
        protected static readonly Dictionary<string,string> _attrs = new();
        protected static readonly List<IDOMElement> _nodes = new();

        public static readonly DomElementStub Instance = new();
        protected DomElementStub() { }

        IDictionary<string, string> IDOMElement.Attributes => _attrs;
        string? IDOMElement.GetAttribute(string name) => null;
        IList<IDOMElement> IDOMElement.Nodes => _nodes;
        public IDOMElement AddChild(IDOMElement element) { ((IDOMElement)this).Nodes.Add(element); return this; }
    }

    /// <summary>
    /// Stub for IDocParagraph — captures no data, returns itself for chaining.
    /// </summary>
    private sealed class ParagraphStub : DomElementStub, IDocContent, IDocSpan, IDocFontStyle, IDocStyleStyle, IDocParagraph
    {
        public static readonly ParagraphStub Instance = new();
        private ParagraphStub() { }

        // IDocElement
        public IDocElement AppendDocElement(Enum aType) => (IDocElement)this;
        public IDocElement AppendDocElement(Enum aType, Type aClass) => (IDocElement)this;
        public IDocElement AppendDocElement(Enum aType, Enum aAttribute, string value, Type aClass, string? Id) => (IDocElement)this;
        public IEnumerable<IDocElement> Enumerate() => Array.Empty<IDocElement>();

        // IDocContent
        public string TextContent { get; set; } = string.Empty;
        public void AppendText(string text) { /* no-op */ }
        public IDocContent AddLineBreak() => (IDocContent)this;
        public IDocContent AddNBSpace(IDocFontStyle docFontStyle) => (IDocContent)this;
        public IDocContent AddTab(IDocFontStyle docFontStyle) => (IDocContent)this;
        public IDocSpan AddSpan(IDocFontStyle docFontStyle) => this;
        public IDocSpan AddSpan(string text, IList<object> docFontStyle) => this;
        public IDocSpan AddSpan(string text, IDocFontStyle docFontStyle) => this;
        public IDocSpan AddSpan(string text, EFontStyle eFontStyle) => this;
        public IDocSpan AddLink(string href, IDocFontStyle docFontStyle) => this;
        public IDocStyleStyle GetStyle() => (IDocStyleStyle)this;
        public string GetTextContent(bool xRecursive = true) => TextContent;

        // IDocParagraph
        public IDocSpan AddBookmark(string id, IDocFontStyle docFontStyle) => this;

        // IDocSpan (explicit)
        string? IDocSpan.Id { get; set; }
        void IDocSpan.SetStyle(object fs) { /* no-op */ }
        void IDocSpan.SetStyle(IDocFontStyle fs) { /* no-op */ }
        void IDocSpan.SetStyle(IUserDocument doc, object aFont) { /* no-op */ }
        void IDocSpan.SetStyle(IUserDocument doc, IDocFontStyle aFont) { /* no-op */ }
        void IDocSpan.SetStyle(string aStyleName) { /* no-op */ }

        // IDocFontStyle
        public string? Name => null;
        public bool Bold => false;
        public bool Italic => false;
        public bool Underline => false;
        public bool Strikeout => false;
        public string? Color => null;
        public string? FontFamily => null;
        public double? FontSizePt => null;

        // IDocStyleStyle
        IDictionary<string, string> IDocStyleStyle.Properties => new Dictionary<string, string>();
    }

    /// <summary>
    /// Stub for IDocHeadline — stores level and anchor id.
    /// </summary>
    private sealed class HeadlineStub : DomElementStub, IDocContent, IDocSpan, IDocFontStyle, IDocStyleStyle, IDocHeadline
    {
        public static readonly HeadlineStub Instance = new();
        private HeadlineStub() { }

        // IDocElement
        public IDocElement AppendDocElement(Enum aType) => (IDocElement)this;
        public IDocElement AppendDocElement(Enum aType, Type aClass) => (IDocElement)this;
        public IDocElement AppendDocElement(Enum aType, Enum aAttribute, string value, Type aClass, string? Id) => (IDocElement)this;
        public IEnumerable<IDocElement> Enumerate() => Array.Empty<IDocElement>();

        // IDocContent
        public string TextContent { get; set; } = string.Empty;
        public void AppendText(string text) { /* no-op */ }
        public IDocContent AddLineBreak() => (IDocContent)this;
        public IDocContent AddNBSpace(IDocFontStyle docFontStyle) => (IDocContent)this;
        public IDocContent AddTab(IDocFontStyle docFontStyle) => (IDocContent)this;
        public IDocSpan AddSpan(IDocFontStyle docFontStyle) => SpanStub.Instance;
        public IDocSpan AddSpan(string text, IList<object> docFontStyle) => SpanStub.Instance;
        public IDocSpan AddSpan(string text, IDocFontStyle docFontStyle) => SpanStub.Instance;
        public IDocSpan AddSpan(string text, EFontStyle eFontStyle) => SpanStub.Instance;
        public IDocSpan AddLink(string href, IDocFontStyle docFontStyle) => SpanStub.Instance;
        public IDocStyleStyle GetStyle() => FontStyleStub.Instance;
        public string GetTextContent(bool xRecursive = true) => TextContent;

        // IDocHeadline
        public int Level { get; set; }
        string IDocHeadline.Id => string.Empty;

        // IDocSpan (explicit to avoid conflict with IDocHeadline.Id)
        string? IDocSpan.Id { get; set; }
        void IDocSpan.SetStyle(object fs) { /* no-op */ }
        void IDocSpan.SetStyle(IDocFontStyle fs) { /* no-op */ }
        void IDocSpan.SetStyle(IUserDocument doc, object aFont) { /* no-op */ }
        void IDocSpan.SetStyle(IUserDocument doc, IDocFontStyle aFont) { /* no-op */ }
        void IDocSpan.SetStyle(string aStyleName) { /* no-op */ }

        // IDocFontStyle
        public string? Name => null;
        public bool Bold => false;
        public bool Italic => false;
        public bool Underline => false;
        public bool Strikeout => false;
        public string? Color => null;
        public string? FontFamily => null;
        public double? FontSizePt => null;

        // IDocStyleStyle
        IDictionary<string, string> IDocStyleStyle.Properties => new Dictionary<string, string>();
    }

    /// <summary>
    /// Stub for IDocTOC — rebuilds from a root section (no-op).
    /// </summary>
    private sealed class TocStub : DomElementStub, IDocContent, IDocSpan, IDocFontStyle, IDocStyleStyle, IDocTOC
    {
        public static readonly TocStub Instance = new();
        private TocStub() { }

        // IDocElement
        public IDocElement AppendDocElement(Enum aType) => (IDocElement)this;
        public IDocElement AppendDocElement(Enum aType, Type aClass) => (IDocElement)this;
        public IDocElement AppendDocElement(Enum aType, Enum aAttribute, string value, Type aClass, string? Id) => (IDocElement)this;
        public IEnumerable<IDocElement> Enumerate() => Array.Empty<IDocElement>();

        // IDocContent
        public string TextContent { get; set; } = string.Empty;
        public void AppendText(string text) { /* no-op */ }
        public IDocContent AddLineBreak() => (IDocContent)this;
        public IDocContent AddNBSpace(IDocFontStyle docFontStyle) => (IDocContent)this;
        public IDocContent AddTab(IDocFontStyle docFontStyle) => (IDocContent)this;
        public IDocSpan AddSpan(IDocFontStyle docFontStyle) => SpanStub.Instance;
        public IDocSpan AddSpan(string text, IList<object> docFontStyle) => SpanStub.Instance;
        public IDocSpan AddSpan(string text, IDocFontStyle docFontStyle) => SpanStub.Instance;
        public IDocSpan AddSpan(string text, EFontStyle eFontStyle) => SpanStub.Instance;
        public IDocSpan AddLink(string href, IDocFontStyle docFontStyle) => SpanStub.Instance;
        public IDocStyleStyle GetStyle() => FontStyleStub.Instance;
        public string GetTextContent(bool xRecursive = true) => TextContent;

        // IDocTOC
        public void RebuildFrom(IDocSection root) { /* no-op */ }

        // IDocSpan (explicit)
        string? IDocSpan.Id { get; set; }
        void IDocSpan.SetStyle(object fs) { /* no-op */ }
        void IDocSpan.SetStyle(IDocFontStyle fs) { /* no-op */ }
        void IDocSpan.SetStyle(IUserDocument doc, object aFont) { /* no-op */ }
        void IDocSpan.SetStyle(IUserDocument doc, IDocFontStyle aFont) { /* no-op */ }
        void IDocSpan.SetStyle(string aStyleName) { /* no-op */ }

        // IDocFontStyle
        public string? Name => null;
        public bool Bold => false;
        public bool Italic => false;
        public bool Underline => false;
        public bool Strikeout => false;
        public string? Color => null;
        public string? FontFamily => null;
        public double? FontSizePt => null;

        // IDocStyleStyle
        IDictionary<string, string> IDocStyleStyle.Properties => new Dictionary<string, string>();
    }

    /// <summary>
    /// Stub for IDocSpan — manages styles and a unique id.
    /// </summary>
    private sealed class SpanStub : DomElementStub, IDocContent, IDocSpan, IDocFontStyle, IDocStyleStyle
    {
        public static readonly SpanStub Instance = new();
        private SpanStub() { }

        // IDocElement
        public IDocElement AppendDocElement(Enum aType) => (IDocElement)this;
        public IDocElement AppendDocElement(Enum aType, Type aClass) => (IDocElement)this;
        public IDocElement AppendDocElement(Enum aType, Enum aAttribute, string value, Type aClass, string? Id) => (IDocElement)this;
        public IEnumerable<IDocElement> Enumerate() => Array.Empty<IDocElement>();

        // IDocContent
        public string TextContent { get; set; } = string.Empty;
        public void AppendText(string text) { /* no-op */ }
        public IDocContent AddLineBreak() => (IDocContent)this;
        public IDocContent AddNBSpace(IDocFontStyle docFontStyle) => (IDocContent)this;
        public IDocContent AddTab(IDocFontStyle docFontStyle) => (IDocContent)this;
        public IDocSpan AddSpan(IDocFontStyle docFontStyle) => this;
        public IDocSpan AddSpan(string text, IList<object> docFontStyle) => this;
        public IDocSpan AddSpan(string text, IDocFontStyle docFontStyle) => this;
        public IDocSpan AddSpan(string text, EFontStyle eFontStyle) => this;
        public IDocSpan AddLink(string href, IDocFontStyle docFontStyle) => this;
        public IDocStyleStyle GetStyle() => FontStyleStub.Instance;
        public string GetTextContent(bool xRecursive = true) => TextContent;

        // IDocSpan (explicit)
        string? IDocSpan.Id { get; set; }
        void IDocSpan.SetStyle(object fs) { /* no-op */ }
        void IDocSpan.SetStyle(IDocFontStyle fs) { /* no-op */ }
        void IDocSpan.SetStyle(IUserDocument doc, object aFont) { /* no-op */ }
        void IDocSpan.SetStyle(IUserDocument doc, IDocFontStyle aFont) { /* no-op */ }
        void IDocSpan.SetStyle(string aStyleName) { /* no-op */ }

        // IDocFontStyle
        public string? Name => null;
        public bool Bold => false;
        public bool Italic => false;
        public bool Underline => false;
        public bool Strikeout => false;
        public string? Color => null;
        public string? FontFamily => null;
        public double? FontSizePt => null;

        // IDocStyleStyle
        IDictionary<string, string> IDocStyleStyle.Properties => new Dictionary<string, string>();
    }

    /// <summary>
    /// Minimal IDocFontStyle for interface compliance.
    /// </summary>
    private sealed class FontStyleStub : IDocFontStyle, IDocStyleStyle
    {
        public static readonly FontStyleStub Instance = new();
        private FontStyleStub() { }
        public string? Name => null;
        public bool Bold => false;
        public bool Italic => false;
        public bool Underline => false;
        public bool Strikeout => false;
        public string? Color => null;
        public string? FontFamily => null;
        public double? FontSizePt => null;
        IDictionary<string, string> IDocStyleStyle.Properties => new Dictionary<string, string>();
    }

    #region Null input validation

    [TestMethod]
    public void ComposeAsync_NullDocument_ThrowsArgumentNullException()
    {
        var composer = new DocumentComposer();
        var ex = Assert.Throws<System.ArgumentNullException>(
            () => { composer.ComposeAsync(null!, CreateEmptySelection(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>()).Wait(); });
    }

    [TestMethod]
    public void ComposeAsync_NullSelection_ThrowsArgumentNullException()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder();
        var ex = Assert.Throws<System.ArgumentNullException>(
            () => { composer.ComposeAsync(recorder, null!, Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>()).Wait(); });
    }

    [TestMethod]
    public void ComposeAsync_UnsupportedDocument_ThrowsInvalidOperationException()
    {
        var composer = new DocumentComposer();
        var fakeDoc = new object(); // Does not implement IUserDocument contract
        var ex = Assert.Throws<System.InvalidOperationException>(
            () => { composer.ComposeAsync(fakeDoc, CreateEmptySelection(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>()).Wait(); });
    }

    #endregion

    #region Composition order validation

    [TestMethod]
    public async Task ComposeAsync_Order_TitleThenFamiliesBeforeIndices()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder();
        var families = CreateTestFamilies(count: 3);
        var selection = CreateSelection(families);

        await composer.ComposeAsync(recorder, selection, Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>());

        // Title page headline is first
        Assert.AreEqual("title-page", recorder.Headlines[0].Anchor);

        // Families section before person index
        var familiesIdx = recorder.Headlines.FindIndex(h => h.Anchor == "families-section");
        var personIdx = recorder.Headlines.FindIndex(h => h.Anchor == "person-index");
        Assert.IsTrue(familiesIdx >= 0 && familiesIdx < personIdx, "Families section should come before person index");

        // All five indices present
        Assert.IsTrue(recorder.Headlines.Any(h => h.Anchor == "person-index"));
        Assert.IsTrue(recorder.Headlines.Any(h => h.Anchor == "occupation-index"));
        Assert.IsTrue(recorder.Headlines.Any(h => h.Anchor == "property-index"));
        Assert.IsTrue(recorder.Headlines.Any(h => h.Anchor == "place-alpha-index"));
        Assert.IsTrue(recorder.Headlines.Any(h => h.Anchor == "place-hierarchy-index"));

        // Document marked as modified
        Assert.IsTrue(recorder.IsModified);
    }

    [TestMethod]
    public async Task ComposeAsync_Sequence_CorrectCalls()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder();

        await composer.ComposeAsync(recorder, CreateEmptySelection(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>());

        // Verify AddHeadline(title-page) was called
        Assert.IsTrue(recorder.Headlines.Any(h => h.Anchor == "title-page"));

        // TOC added once
        Assert.AreEqual(1, recorder.Tocs.Count);
        Assert.AreEqual("Inhalt", recorder.Tocs[0].Name);
    }

    #endregion

    #region Preamble composition

    [TestMethod]
    public async Task ComposeAsync_WithoutPreface_NoPreambleSection()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder(); // PrefaceText/LegendText are null by default

        await composer.ComposeAsync(recorder, CreateEmptySelection(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>());

        Assert.IsFalse(recorder.Headlines.Any(h => h.Anchor == "preamble"), "Preamble headline should NOT be present without preface text");
        Assert.IsFalse(recorder.Headlines.Any(h => h.Anchor == "legend"), "Legend headline should NOT be present without legend text");
    }

    [TestMethod]
    public async Task ComposeAsync_WithPreface_PrefaceSectionRendered()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder { PrefaceText = "Vorwortzeile 1\nVorwortzeile 2" };

        await composer.ComposeAsync(recorder, CreateEmptySelection(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>());

        Assert.IsTrue(recorder.Headlines.Any(h => h.Anchor == "preamble"), "Preamble headline should be present with preface text");
        Assert.IsTrue(recorder.Paragraphs.Count >= 2, "Should render at least 2 preface paragraphs");
    }

    [TestMethod]
    public async Task ComposeAsync_WithLegend_LegendSectionRendered()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder { LegendText = "Zeichenerklärung Linie 1" };

        await composer.ComposeAsync(recorder, CreateEmptySelection(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>());

        Assert.IsTrue(recorder.Headlines.Any(h => h.Anchor == "legend"), "Legend headline should be present with legend text");
    }

    #endregion

    #region Family composition

    [TestMethod]
    public async Task ComposeAsync_WithFamilies_FamilyEntriesRendered()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder();
        var families = CreateTestFamilies(count: 2);
        var selection = CreateSelection(families);

        await composer.ComposeAsync(recorder, selection, Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>());

        Assert.IsTrue(recorder.Headlines.Any(h => h.Anchor == "families-section"));

        var familyAnchors = recorder.Headlines.Where(h => h.Anchor != null && h.Anchor.StartsWith("family-")).ToList();
        Assert.AreEqual(2, familyAnchors.Count);
    }

    [TestMethod]
    public async Task ComposeAsync_EmptyFamilies_NoFamilyEntries()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder();

        await composer.ComposeAsync(recorder, CreateEmptySelection(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>());

        var familyAnchors = recorder.Headlines.Where(h => h.Anchor != null && h.Anchor.StartsWith("family-")).ToList();
        Assert.AreEqual(0, familyAnchors.Count);
    }

    [TestMethod]
    public async Task ComposeAsync_FamilyWithSpouses_SpouseLineRendered()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder();
        var families = CreateTestFamilies(count: 1);
        var selection = CreateSelection(families);

        await composer.ComposeAsync(recorder, selection, Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>());

        var hasSpouseLine = recorder.Paragraphs.Any(p => p.Contains("H ") && p.Contains("\\") && p.Contains("F "));
        Assert.IsTrue(hasSpouseLine, "Should render spouse line with both H and F");
    }

    [TestMethod]
    public async Task ComposeAsync_FamilyWithChildren_ChildLinesRendered()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder();
        var families = CreateTestFamilies(count: 2); // Even-indexed have children
        var selection = CreateSelection(families);

        await composer.ComposeAsync(recorder, selection, Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>());

        var childLines = recorder.Paragraphs.Where(p => p.StartsWith("K ")).ToList();
        Assert.IsTrue(childLines.Count > 0, "Should render at least one child line");
    }

    #endregion

    #region Index composition

    [TestMethod]
    public async Task ComposeAsync_PersonIndex_EntriesRendered()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder();
        var personEntries = new[]
        {
            OFBIndexEntry.FromCombined("Müller", "Hans", "00001"),
            OFBIndexEntry.FromCombined("Schmidt", "Anna", "00002")
        };

        await composer.ComposeAsync(recorder, CreateEmptySelection(), personEntries, Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>());

        var indexLines = recorder.Paragraphs.Where(p => p.StartsWith("P ")).ToList();
        Assert.AreEqual(2, indexLines.Count);
    }

    [TestMethod]
    public async Task ComposeAsync_OccupationIndex_EntriesRendered()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder();
        var occEntries = new[] { OFBIndexEntry.FromSingle("Schneider", "00001") };

        await composer.ComposeAsync(recorder, CreateEmptySelection(), Array.Empty<OFBIndexEntry>(), occEntries, Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>());

        var indexLines = recorder.Paragraphs.Where(p => p.StartsWith("B ")).ToList();
        Assert.AreEqual(1, indexLines.Count);
    }

    [TestMethod]
    public async Task ComposeAsync_PropertyIndex_EntriesRendered()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder();
        var propEntries = new[] { OFBIndexEntry.FromSingle("Gutshaus X", "00001") };

        await composer.ComposeAsync(recorder, CreateEmptySelection(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), propEntries, Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>());

        var indexLines = recorder.Paragraphs.Where(p => p.StartsWith("E ")).ToList();
        Assert.AreEqual(1, indexLines.Count);
    }

    [TestMethod]
    public async Task ComposeAsync_AlphabeticalPlaceIndex_EntriesRendered()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder();
        var placeEntries = new[] { OFBIndexEntry.FromSingle("München", "00001") };

        await composer.ComposeAsync(recorder, CreateEmptySelection(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), placeEntries);

        var indexLines = recorder.Paragraphs.Where(p => p.StartsWith("O ")).ToList();
        Assert.AreEqual(1, indexLines.Count);
    }

    [TestMethod]
    public async Task ComposeAsync_HierarchicalPlaceIndex_NodesRendered()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder();
        var hierarchyNodes = new[]
        {
            new OFBPlaceHierarchyNode("Deutschland", "DE")
            {
                Children = new[]
                {
                    new OFBPlaceHierarchyNode("Bayern", "DE-BY")
                    {
                        Children = new[]
                        {
                            new OFBPlaceHierarchyNode("München", "DE-BY-MUC")
                        }
                    }
                }
            }
        };

        await composer.ComposeAsync(recorder, CreateEmptySelection(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), hierarchyNodes, Array.Empty<OFBIndexEntry>());

        var placeLines = recorder.Paragraphs.Where(p => p.Contains("Deutschland") || p.Contains("Bayern") || p.Contains("München")).ToList();
        Assert.AreEqual(3, placeLines.Count); // Three levels of hierarchy
    }

    #endregion

    #region Anchor integrity validation

    [TestMethod]
    public async Task ComposeAsync_Anchors_AreUnique()
    {
        var composer = new DocumentComposer();
        var recorder = new DocumentRecorder();
        var families = new List<OFBFamilyModel>();
        for (int i = 0; i < 5; i++)
        {
            var husband = CreateMockPerson("Nachname" + (i % 3), "Vorname_" + i, "I" + (100 + i * 2));
            families.Add(new OFBFamilyModel
            {
                GlobalNumber = i.ToString("D5"),
                FamilyName = "Nachname",
                Husband = husband,
                Wife = null!,
                Children = Array.Empty<IGenPerson>(),
                SourceRefId = "Fam" + i // Unique ID for anchor generation
            });
        }
        var selection = CreateSelection(families);

        await composer.ComposeAsync(recorder, selection, Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>());

        var anchors = recorder.Headlines.Select(h => h.Anchor).Where(a => a != null).ToList();
        var duplicates = anchors.GroupBy(a => a!).FirstOrDefault(g => g.Count() > 1);
        Assert.IsNull(duplicates, "Duplicate anchor found: " + (duplicates?.Key ?? "null"));
    }

    #endregion

    #region Cancellation support

    [TestMethod]
    public void ComposeAsync_CancellationToken_ThrowsOnCancellationDuringFamilies()
    {
        var composer = new DocumentComposer();

        // Create many families using OFBFamilyModel (implements IGenFamily) with unique SourceRefId
        var families = new List<OFBFamilyModel>();
        for (int i = 0; i < 50; i++)
        {
            var husband = CreateMockPerson("Name" + i, "Vorname" + i, "I" + i);
            families.Add(new OFBFamilyModel
            {
                GlobalNumber = "0000" + i.ToString("D4"),
                FamilyName = "Name",
                Husband = husband,
                Wife = null!,
                Children = Array.Empty<IGenPerson>(),
                SourceRefId = "Fam" + i // Unique ID for anchor generation
            });
        }

        var selection = CreateSelection(families);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var recorder = new DocumentRecorder();
        var ex = Assert.Throws<System.OperationCanceledException>(
            () => { composer.ComposeAsync(recorder, selection, Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBIndexEntry>(), Array.Empty<OFBPlaceHierarchyNode>(), Array.Empty<OFBIndexEntry>(), cts.Token).Wait(); });
        Assert.IsInstanceOfType(ex, typeof(OperationCanceledException));
    }

    #endregion

    #region Helper methods

    private static IGenPerson CreateMockPerson(string surname, string givenName, string xid)
    {
        var person = Substitute.For<IGenPerson>();
        person.Surname.Returns(surname);
        person.GivenName.Returns(givenName);
        person.Name.Returns($"{surname} {givenName}");
        person.IndRefID.Returns(xid);
        person.BirthDate.Returns((IGenDate?)null!);
        person.DeathDate.Returns((IGenDate?)null!);
        person.BirthPlace.Returns((IGenPlace?)null!);
        person.DeathPlace.Returns((IGenPlace?)null!);
        return person;
    }

    private static OFBFamilyModel CreateMockFamily(IGenPerson? husband, IGenPerson? wife, IReadOnlyList<IGenPerson>? children = null, int index = 0)
    {
        var surname = !string.IsNullOrEmpty(husband?.Surname) ? husband!.Surname : (wife?.Surname ?? "Unknown");
        return new OFBFamilyModel
        {
            GlobalNumber = index.ToString("D5"),
            FamilyName = surname,
            Husband = husband,
            Wife = wife,
            Children = children ?? Array.Empty<IGenPerson>().AsReadOnly(),
            SourceRefId = "F" + index
        };
    }

    private static List<OFBFamilyModel> CreateTestFamilies(int count)
    {
        var families = new List<OFBFamilyModel>();
        for (int i = 0; i < count; i++)
        {
            var surname = "Nachname" + (i % 3);
            var husband = CreateMockPerson(surname, "Vorname_" + i, "I" + (100 + i * 2));
            var wifeSurname = (i % 3 == 0) ? surname : "Ehefrau" + surname;
            var wife = CreateMockPerson(wifeSurname, "Ehefrau_" + i, "I" + (100 + i * 2 + 1));

            // Even-indexed families get children for testing child line rendering
            IReadOnlyList<IGenPerson> children = Array.Empty<IGenPerson>();
            if (i % 2 == 0)
            {
                children = new List<IGenPerson>
                {
                    CreateMockPerson(surname, "Kind_" + i, "C" + i)
                }.AsReadOnly();
            }

            families.Add(new OFBFamilyModel
            {
                GlobalNumber = i.ToString("D5"),
                FamilyName = surname,
                Husband = husband,
                Wife = wife,
                Children = children,
                SourceRefId = "Fam" + i  // Unique ID for anchor generation
            });
        }
        return families;
    }

    private static OFBSourceSelection CreateSelection(IReadOnlyList<OFBFamilyModel> families)
    {
        var selection = new OFBSourceSelection { LastGlobalNumber = families.Count };
        // Set test data directly for duck-typed access to SourceRefId, Husband, Wife etc.
        selection.TestFamilyData = families;
        return selection;
    }

    private static OFBSourceSelection CreateEmptySelection() => new() { LastGlobalNumber = 0 };

    #endregion
}
