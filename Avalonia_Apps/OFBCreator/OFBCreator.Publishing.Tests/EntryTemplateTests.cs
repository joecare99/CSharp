using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Document.Base.Models;
using Document.Base.Models.Interfaces;
using Document.Docx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OFBCreator.Publishing.Models;
using OFBCreator.Publishing.Services.Templates;
using OFBCreator.Projects.Models;

namespace OFBCreator.Publishing.Tests;

[TestClass]
public sealed class EntryTemplateTests
{
    [TestMethod]
    [DataRow("gc")]
    [DataRow("ak")]
    public void EntryTemplateStore_LoadsAndValidatesBuiltInTemplates(string name)
    {
        var template = new EntryTemplateStore().Load(name);

        Assert.AreEqual(name, template.Id);
        Assert.AreEqual("Family", template.EntryRoot);
        Assert.IsTrue(template.Blocks.Count > 0);
        Assert.IsTrue(template.Legend.Count > 0);
        Assert.IsTrue(template.Legend.Where(entry => entry.Symbol != "Kdr.").All(entry => entry.Event.HasValue));
        Assert.IsNull(template.Legend.Single(entry => entry.Symbol == "Kdr.").Event);
    }

    [TestMethod]
    public void EntryTemplateValidator_RejectsEmptyLegendSymbol()
    {
        const string json = """
            {"schemaVersion":1,"id":"legend","entryRoot":"Family",
             "legend":[{"symbol":" ","meaning":"geboren"}],"blocks":[]}
            """;

        Assert.ThrowsExactly<InvalidDataException>(() => EntryTemplateValidator.ParseAndValidate(json));
    }

    [TestMethod]
    public void EntryTemplateValidator_RejectsUnknownFragmentAndIncludeCycle()
    {
        const string unknownFragment = """
            {"schemaVersion":1,"id":"bad","entryRoot":"Family",
             "blocks":[{"kind":"include","fragment":"missing"}]}
            """;
        const string includeCycle = """
            {"schemaVersion":1,"id":"cycle","entryRoot":"Family",
             "fragments":{
               "one":[{"kind":"include","fragment":"two"}],
               "two":[{"kind":"include","fragment":"one"}]
             },
             "blocks":[{"kind":"include","fragment":"one"}]}
            """;

        Assert.ThrowsExactly<InvalidDataException>(() => EntryTemplateValidator.ParseAndValidate(unknownFragment));
        Assert.ThrowsExactly<InvalidDataException>(() => EntryTemplateValidator.ParseAndValidate(includeCycle));
    }

    [TestMethod]
    public void EntryTemplateValidator_RejectsExpressionsUnknownPropertiesAndInvalidSections()
    {
        const string expression = """
            {"schemaVersion":1,"id":"bad","entryRoot":"Family",
             "blocks":[{"kind":"field","path":"family.number","expression":"System.IO.File.Delete"}]}
            """;
        const string invalidColumns = """
            {"schemaVersion":1,"id":"bad","entryRoot":"Family",
             "blocks":[{"kind":"section","columns":5,"blocks":[]}]}
            """;
        const string nestedSections = """
            {"schemaVersion":1,"id":"bad","entryRoot":"Family",
             "blocks":[{"kind":"section","columns":2,"blocks":[
               {"kind":"section","columns":3,"blocks":[]}
             ]}]}
            """;

        Assert.ThrowsExactly<InvalidDataException>(() => EntryTemplateValidator.ParseAndValidate(expression));
        Assert.ThrowsExactly<InvalidDataException>(() => EntryTemplateValidator.ParseAndValidate(invalidColumns));
        Assert.ThrowsExactly<InvalidDataException>(() => EntryTemplateValidator.ParseAndValidate(nestedSections));
    }

    [TestMethod]
    public void EntryTemplateValidator_AcceptsParagraphSpacingAndFontStylesAndRejectsOutOfRangeIndent()
    {
        const string valid = """
            {"schemaVersion":1,"id":"styled","entryRoot":"Family","blocks":[
              {"kind":"paragraph","hangingIndent":18,"spaceBefore":6,"content":[
                {"kind":"field","path":"family.number","bold":true,"italic":true,"underline":true}
              ]}
            ]}
            """;
        const string invalid = """
            {"schemaVersion":1,"id":"invalid-indent","entryRoot":"Family","blocks":[
              {"kind":"paragraph","hangingIndent":1441,"content":[]}
            ]}
            """;
        const string invalidSpacing = """
            {"schemaVersion":1,"id":"invalid-spacing","entryRoot":"Family","blocks":[
              {"kind":"paragraph","spaceBefore":1441,"content":[]}
            ]}
            """;

        var definition = EntryTemplateValidator.ParseAndValidate(valid);
        Assert.AreEqual("styled", definition.Id);
        Assert.AreEqual(6, definition.Blocks[0].SpaceBefore);
        Assert.ThrowsExactly<InvalidDataException>(() => EntryTemplateValidator.ParseAndValidate(invalid));
        Assert.ThrowsExactly<InvalidDataException>(() => EntryTemplateValidator.ParseAndValidate(invalidSpacing));
    }

    [TestMethod]
    public void EntryTemplateValidator_AcceptsExactlyOneAndNegatedCollectionConditions()
    {
        const string json = """
            {"schemaVersion":1,"id":"conditions","entryRoot":"Family","blocks":[
              {"kind":"if","condition":"family.children.one","then":[]},
              {"kind":"if","condition":"!family.children.one","then":[]},
              {"kind":"if","condition":"!family.children.any","then":[]}
            ]}
            """;
        const string doubleNegation = """
            {"schemaVersion":1,"id":"invalid-condition","entryRoot":"Family","blocks":[
              {"kind":"if","condition":"!!family.children.one","then":[]}
            ]}
            """;

        Assert.AreEqual("conditions", EntryTemplateValidator.ParseAndValidate(json).Id);
        Assert.ThrowsExactly<InvalidDataException>(() => EntryTemplateValidator.ParseAndValidate(doubleNegation));
    }

    [TestMethod]
    public void EntryTemplateValidator_AcceptsOptionalPersonEventPlaceAndAdditionalConditions()
    {
        const string json = """
            {"schemaVersion":1,"id":"event-optional-condition","entryRoot":"Individual","blocks":[
              {"kind":"forEach","items":"person.events","as":"personEvent","template":[
                {"kind":"if","condition":"personEvent.place","then":[]},
                {"kind":"if","condition":"!personEvent.place","then":[]},
                {"kind":"if","condition":"personEvent.additional","then":[]},
                {"kind":"if","condition":"!personEvent.additional","then":[]}
              ]}
            ]}
            """;

        Assert.AreEqual("event-optional-condition", EntryTemplateValidator.ParseAndValidate(json).Id);
    }

    [TestMethod]
    public void EntryTemplateRenderer_RendersSingularPluralChildLabelsAndIndentedOrdinals()
    {
        const string json = """
            {"schemaVersion":1,"id":"children","entryRoot":"Family","blocks":[
              {"kind":"if","condition":"family.children.one","then":[
                {"kind":"paragraph","content":[{"kind":"text","value":"Kind:"}]}
              ]},
              {"kind":"if","condition":"!family.children.one","then":[
                {"kind":"paragraph","content":[{"kind":"text","value":"Kinder:"}]}
              ]},
              {"kind":"forEach","items":"family.children","as":"person","template":[
                {"kind":"paragraph","indent":36,"content":[
                  {"kind":"field","path":"person.ordinal","formatter":"ordinal"},
                  {"kind":"text","value":" "},
                  {"kind":"field","path":"person.nameAk"}
                ]}
              ]}
            ]}
            """;
        var template = EntryTemplateValidator.ParseAndValidate(json);
        var singleChildDocument = new DocxDocument();
        var singleChild = CreateTemplatePerson("Ada", 1);

        new EntryTemplateRenderer().RenderFamily(singleChildDocument, template, CreateTemplateFamily(singleChild));

        var singleParagraphs = singleChildDocument.Enumerate().OfType<IDocParagraph>().ToArray();
        var singleText = singleParagraphs.Select(paragraph => paragraph.GetTextContent()).ToArray();
        CollectionAssert.Contains(singleText, "Kind:");
        CollectionAssert.Contains(singleText, "1. Ada");
        Assert.IsFalse(singleText.Contains("Kinder:"));
        Assert.IsTrue(singleParagraphs.Any(paragraph => paragraph.GetTextContent() == "1. Ada"
            && paragraph.DocAttributes.Any(attribute => attribute.Name == DocAttributeNames.IndentationBefore
                && Equals(attribute.Value, 36))));
        Assert.IsFalse(singleParagraphs.Any(paragraph => paragraph.GetTextContent() == "1. Ada"
            && paragraph.DocAttributes.Any(attribute => attribute.Name == DocAttributeNames.IndentationHanging)));
        Assert.IsFalse(singleParagraphs.Any(paragraph => paragraph.GetTextContent() == "1. Ada"
            && paragraph.DocAttributes.Any(attribute => attribute.Name == DocAttributeNames.IndentationHanging)));

        var multipleChildrenDocument = new DocxDocument();
        new EntryTemplateRenderer().RenderFamily(
            multipleChildrenDocument,
            template,
            CreateTemplateFamily(CreateTemplatePerson("Ada", 1), CreateTemplatePerson("Bernd", 2)));

        var multipleText = multipleChildrenDocument.Enumerate()
            .OfType<IDocParagraph>()
            .Select(paragraph => paragraph.GetTextContent())
            .ToArray();
        CollectionAssert.Contains(multipleText, "Kinder:");
        CollectionAssert.Contains(multipleText, "1. Ada");
        CollectionAssert.Contains(multipleText, "2. Bernd");
        Assert.IsFalse(multipleText.Contains("Kind:"));
    }

    [TestMethod]
    [DataRow("gc")]
    [DataRow("ak")]
    public void EntryTemplateRenderer_RendersVisibleNonVitalEventsAsLinkedParagraphs(string templateName)
    {
        var template = new EntryTemplateStore().Load(templateName);
        var document = new DocxDocument();
        var person = new PersonEntryTemplateModel
        {
            Surname = "Beispiel",
            GivenNames = "Ada",
            Title = string.Empty,
            NameGc = "Beispiel, Ada",
            NameAk = "Ada Beispiel",
            Anchor = "person-1",
            Reference = "1",
            VitalEvents = string.Empty,
            IndexAnchor = string.Empty,
            Ordinal = 1,
            Occupations = Array.Empty<OccupationEntryTemplateModel>(),
            ShowNonVitalEvents = true,
            Events =
            [
                new PersonEventEntryTemplateModel
                {
                    Symbol = string.Empty,
                    EventName = "Ausbildung",
                    Date = "22.06.1912",
                    PlacePreposition = "bei",
                    Place = "Berlin",
                    PlaceAnchor = "place-Berlin",
                    Additional = "Besuch",
                    RelatedPersonName = "Emil Beispiel",
                    RelatedPersonAnchor = "person-I2",
                    RelatedFamilyNumber = "00002",
                    RelatedFamilyAnchor = "family-00002",
                    IsVital = false,
                    IsListableNonVital = true
                },
                new PersonEventEntryTemplateModel
                {
                    Symbol = string.Empty,
                    EventName = "Titel",
                    Date = string.Empty,
                    PlacePreposition = "in",
                    Place = string.Empty,
                    IsVital = false,
                    IsListableNonVital = false
                }
            ]
        };

        new EntryTemplateRenderer().RenderFamily(document, template, CreateTemplateFamily(person));

        var paragraphs = document.Enumerate().OfType<IDocParagraph>().ToArray();
        var eventParagraph = paragraphs.Single(paragraph =>
            paragraph.GetTextContent().Contains("22.06.1912: Ausbildung bei Berlin, Besuch", StringComparison.Ordinal));
        Assert.IsTrue(eventParagraph.Nodes.OfType<IDocSpan>().Any(span =>
            span.IsLink && span.Href == "#place-Berlin"));
        Assert.IsTrue(eventParagraph.Nodes.OfType<IDocSpan>().Any(span =>
            span.IsLink && span.Href == "#person-I2"));
        Assert.IsTrue(eventParagraph.Nodes.OfType<IDocSpan>().Any(span =>
            span.IsLink && span.Href == "#family-00002"));
        Assert.IsFalse(paragraphs.Any(paragraph =>
            paragraph.GetTextContent().Contains("Titel", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void EntryTemplateRenderer_FormatsParentAndChildSurnamesWithinIndexLinks()
    {
        var template = new EntryTemplateStore().Load("gc");
        var parent = new PersonEntryTemplateModel
        {
            Surname = "Elternname",
            GivenNames = "Ada Eltern",
            Title = "Dr.",
            NameGc = "Elternname, Ada Eltern, Dr.",
            NameAk = "Ada Eltern Dr. Elternname",
            Anchor = "person-parent",
            Reference = "1",
            VitalEvents = string.Empty,
            IndexAnchor = "index-parent",
            Ordinal = null,
            Occupations = Array.Empty<OccupationEntryTemplateModel>()
        };
        var child = new PersonEntryTemplateModel
        {
            Surname = "Kindname",
            GivenNames = "Bernd Kind",
            Title = "Prof.",
            NameGc = "Kindname, Bernd Kind, Prof.",
            NameAk = "Bernd Kind Prof. Kindname",
            Anchor = "person-child",
            Reference = "2",
            VitalEvents = string.Empty,
            IndexAnchor = "index-child",
            Ordinal = 1,
            Occupations = Array.Empty<OccupationEntryTemplateModel>()
        };
        var family = new FamilyEntryTemplateModel
        {
            Number = "00001",
            Anchor = "family-00001",
            Union = string.Empty,
            Parents = [parent],
            Children = [child]
        };
        var document = new DocxDocument();

        new EntryTemplateRenderer().RenderFamily(document, template, family);

        using var output = new MemoryStream();
        Assert.IsTrue(document.SaveTo(output));
        output.Position = 0;
        using var archive = new ZipArchive(output, ZipArchiveMode.Read, leaveOpen: true);
        using var stream = archive.GetEntry("word/document.xml")!.Open();
        var xml = XDocument.Load(stream);
        XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        AssertNameLinkFormatting(xml, word, "index-parent", "Elternname", "Ada Eltern", "Dr.");
        AssertNameLinkFormatting(xml, word, "index-child", "Kindname", "Bernd Kind", "Prof.");
    }

    [TestMethod]
    public void EntryTemplateRenderer_InheritsFamilyParagraphFontForInlinePlaceLinks()
    {
        var template = new EntryTemplateStore().Load("gc");
        var layout = new OFBProjectLayout();
        layout.FamilyEntry.FontFamily = "Arial";
        layout.FamilyEntry.FontSizePt = 9;
        var family = new FamilyEntryTemplateModel
        {
            Number = "00001",
            Anchor = "family-00001",
            Union = string.Empty,
            MarriageMark = "⚭",
            MarriagePlaceShort = "Kleinstadt",
            MarriagePlaceAnchor = "place-test",
            Parents = Array.Empty<PersonEntryTemplateModel>(),
            Children = Array.Empty<PersonEntryTemplateModel>()
        };
        var document = new DocxDocument();

        new EntryTemplateRenderer(layout).RenderFamily(document, template, family);

        using var output = new MemoryStream();
        Assert.IsTrue(document.SaveTo(output));
        output.Position = 0;
        using var archive = new ZipArchive(output, ZipArchiveMode.Read, leaveOpen: true);
        using var stream = archive.GetEntry("word/document.xml")!.Open();
        var xml = XDocument.Load(stream);
        XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        var linkedPlaceRun = xml.Descendants(word + "hyperlink")
            .Where(hyperlink => (string?)hyperlink.Attribute(word + "anchor") == "place-test")
            .SelectMany(hyperlink => hyperlink.Elements(word + "r"))
            .Single(run => string.Concat(run.Descendants(word + "t").Select(text => text.Value)) == "Kleinstadt");
        var linkedPlaceProperties = linkedPlaceRun.Element(word + "rPr");
        Assert.AreEqual("18", (string?)linkedPlaceProperties?.Element(word + "sz")?.Attribute(word + "val"));
        Assert.AreEqual("Arial", (string?)linkedPlaceProperties?.Element(word + "rFonts")?.Attribute(word + "ascii"));

        var familyParagraph = linkedPlaceRun.Ancestors(word + "p").Single();
        var prepositionRun = familyParagraph.Elements(word + "r")
            .Single(run => string.Concat(run.Descendants(word + "t").Select(text => text.Value)) == " in ");
        Assert.AreEqual("18", (string?)prepositionRun.Element(word + "rPr")?.Element(word + "sz")?.Attribute(word + "val"));
    }

    [TestMethod]
    public void EntryTemplateRenderer_PreservesFontPerRepeatedLinkTargetAndText()
    {
        const string json = """
            {"schemaVersion":1,"id":"repeated-link-fonts","entryRoot":"Family","blocks":[
              {"kind":"paragraph","role":"FamilyEntry","content":[
                {"kind":"link","target":"family.marriagePlaceAnchor","content":[{"kind":"text","value":"Kleinstadt"}]}
              ]},
              {"kind":"paragraph","role":"ParentEntry","content":[
                {"kind":"link","target":"family.marriagePlaceAnchor","content":[{"kind":"text","value":"Kleinstadt"}]}
              ]}
            ]}
            """;
        var template = EntryTemplateValidator.ParseAndValidate(json);
        var layout = new OFBProjectLayout();
        layout.FamilyEntry.FontFamily = "Arial";
        layout.FamilyEntry.FontSizePt = 9;
        layout.ParentEntry.FontFamily = "Georgia";
        layout.ParentEntry.FontSizePt = 14;
        var family = new FamilyEntryTemplateModel
        {
            Number = "00001",
            Anchor = "family-00001",
            Union = string.Empty,
            MarriagePlaceAnchor = "place-test",
            Parents = Array.Empty<PersonEntryTemplateModel>(),
            Children = Array.Empty<PersonEntryTemplateModel>()
        };
        var document = new DocxDocument();

        new EntryTemplateRenderer(layout).RenderFamily(document, template, family);

        using var output = new MemoryStream();
        Assert.IsTrue(document.SaveTo(output));
        output.Position = 0;
        using var archive = new ZipArchive(output, ZipArchiveMode.Read, leaveOpen: true);
        using var stream = archive.GetEntry("word/document.xml")!.Open();
        var xml = XDocument.Load(stream);
        XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        var linkRuns = xml.Descendants(word + "hyperlink")
            .Where(hyperlink => (string?)hyperlink.Attribute(word + "anchor") == "place-test")
            .SelectMany(hyperlink => hyperlink.Elements(word + "r"))
            .ToArray();

        Assert.AreEqual(2, linkRuns.Length);
        AssertRunFont(linkRuns[0], word, "18", "Arial");
        AssertRunFont(linkRuns[1], word, "28", "Georgia");
    }

    [TestMethod]
    [DataRow("gc")]
    [DataRow("ak")]
    public void EntryTemplateRenderer_UsesSpacingAndIndentationForFamilyHierarchy(string templateName)
    {
        var template = new EntryTemplateStore().Load(templateName);
        var parent = CreateTemplatePerson(
            "Parent",
            null,
            showNonVitalEvents: true,
            events: [CreateNonVitalEvent()],
            occupations: [CreateOccupation()]);
        var child = CreateTemplatePerson(
            "Child",
            2,
            showNonVitalEvents: true,
            events: [CreateNonVitalEvent()],
            occupations: [CreateOccupation()]);
        var family = new FamilyEntryTemplateModel
        {
            Number = "00001",
            Anchor = "family-00001",
            Union = string.Empty,
            Parents = [parent],
            Children = [child]
        };
        var document = new DocxDocument();

        new EntryTemplateRenderer().RenderFamily(document, template, family);

        var paragraphs = document.Enumerate().OfType<IDocParagraph>().ToArray();
        var familyParagraph = paragraphs.Single(paragraph =>
            paragraph.Nodes.OfType<IDocSpan>().Any(span => span.Id == "family-00001"));
        var parentParagraph = paragraphs.Single(paragraph => paragraph.GetTextContent().Contains("Parent", StringComparison.Ordinal));
        var childParagraph = paragraphs.Single(paragraph => paragraph.GetTextContent().Contains("Child", StringComparison.Ordinal));
        var childrenHeader = paragraphs.Single(paragraph =>
            paragraph.GetTextContent().Contains("Kind:", StringComparison.Ordinal));
        var eventParagraphs = paragraphs.Where(paragraph => paragraph.GetTextContent().Contains("Ausb.", StringComparison.Ordinal)).ToArray();

        Assert.AreEqual(6, GetParagraphAttribute(familyParagraph, DocAttributeNames.SpacingBeforePt));
        Assert.AreEqual(18, GetParagraphAttribute(childrenHeader, DocAttributeNames.IndentationBefore));
        Assert.AreEqual(0, GetParagraphAttribute(childrenHeader, DocAttributeNames.IndentationHanging) ?? 0);
        Assert.AreEqual(54, GetParagraphAttribute(childParagraph, DocAttributeNames.IndentationBefore));
        Assert.AreEqual(18, GetParagraphAttribute(childParagraph, DocAttributeNames.IndentationHanging));
        Assert.AreEqual(2, eventParagraphs.Length);
        Assert.AreEqual(36, GetParagraphAttribute(eventParagraphs[0], DocAttributeNames.IndentationBefore));
        Assert.AreEqual(72, GetParagraphAttribute(eventParagraphs[1], DocAttributeNames.IndentationBefore));

        Assert.AreEqual(36, GetParagraphAttribute(parentParagraph, DocAttributeNames.IndentationBefore));
        Assert.AreEqual(18, GetParagraphAttribute(parentParagraph, DocAttributeNames.IndentationHanging));

        using var output = new MemoryStream();
        Assert.IsTrue(document.SaveTo(output));
        output.Position = 0;
        using var archive = new ZipArchive(output, ZipArchiveMode.Read, leaveOpen: true);
        using var stream = archive.GetEntry("word/document.xml")!.Open();
        var xml = XDocument.Load(stream);
        XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        var xmlFamilyParagraph = xml.Descendants(word + "p")
            .Single(paragraph => paragraph.Descendants(word + "bookmarkStart")
                .Any(bookmark => (string?)bookmark.Attribute(word + "name") == "family-00001"));
        Assert.AreEqual("120", (string?)xmlFamilyParagraph.Element(word + "pPr")?
            .Element(word + "spacing")?.Attribute(word + "before"));
    }

    private static void AssertNameLinkFormatting(
        XDocument xml,
        XNamespace word,
        string anchor,
        string surname,
        string givenNames,
        string title)
    {
        var runs = xml.Descendants(word + "hyperlink")
            .Where(hyperlink => (string?)hyperlink.Attribute(word + "anchor") == anchor)
            .SelectMany(hyperlink => hyperlink.Elements(word + "r"))
            .ToArray();

        Assert.IsTrue(runs.Length > 0, $"Expected a hyperlink to '{anchor}'.");
        AssertRunFormatting(runs, word, surname, bold: true);
        AssertRunFormatting(runs, word, givenNames, bold: false);
        AssertRunFormatting(runs, word, title, bold: false);
    }

    private static void AssertRunFormatting(
        IEnumerable<XElement> runs,
        XNamespace word,
        string text,
        bool bold)
    {
        var run = runs.Single(candidate => string.Concat(candidate.Descendants(word + "t").Select(node => node.Value)) == text);
        var properties = run.Element(word + "rPr");

        Assert.AreEqual(bold, properties?.Element(word + "b") is not null, $"Unexpected bold formatting for '{text}'.");
        Assert.AreEqual("single", (string?)properties?.Element(word + "u")?.Attribute(word + "val"));
    }

    private static void AssertRunFont(XElement run, XNamespace word, string expectedSize, string expectedFamily)
    {
        var properties = run.Element(word + "rPr");
        Assert.AreEqual(expectedSize, (string?)properties?.Elements(word + "sz").SingleOrDefault()?.Attribute(word + "val"));
        Assert.AreEqual(expectedFamily, (string?)properties?.Elements(word + "rFonts").SingleOrDefault()?.Attribute(word + "ascii"));
        Assert.AreEqual(expectedFamily, (string?)properties?.Elements(word + "rFonts").SingleOrDefault()?.Attribute(word + "hAnsi"));
        Assert.AreEqual(1, properties?.Elements(word + "sz").Count());
        Assert.AreEqual(1, properties?.Elements(word + "rFonts").Count());
    }

    private static FamilyEntryTemplateModel CreateTemplateFamily(params PersonEntryTemplateModel[] children) => new()
    {
        Number = "00001",
        Anchor = "family-00001",
        Union = string.Empty,
        Parents = Array.Empty<PersonEntryTemplateModel>(),
        Children = children
    };

    private static PersonEntryTemplateModel CreateTemplatePerson(
        string name,
        int? ordinal,
        bool showNonVitalEvents = false,
        IReadOnlyList<PersonEventEntryTemplateModel>? events = null,
        IReadOnlyList<OccupationEntryTemplateModel>? occupations = null) => new()
    {
        Surname = string.Empty,
        GivenNames = name,
        Title = string.Empty,
        NameGc = name,
        NameAk = name,
        Anchor = $"person-{ordinal}",
        Reference = ordinal?.ToString() ?? string.Empty,
        VitalEvents = string.Empty,
        IndexAnchor = string.Empty,
        Ordinal = ordinal,
        Occupations = occupations ?? Array.Empty<OccupationEntryTemplateModel>(),
        ShowNonVitalEvents = showNonVitalEvents,
        Events = events ?? Array.Empty<PersonEventEntryTemplateModel>()
    };

    private static PersonEventEntryTemplateModel CreateNonVitalEvent() => new()
    {
        Symbol = "Ausb.",
        EventName = "Ausbildung",
        Date = "22.06.1912",
        IsVital = false,
        IsListableNonVital = true
    };

    private static OccupationEntryTemplateModel CreateOccupation() => new()
    {
        Name = "Beruf",
        IndexAnchor = "occupation-1"
    };

    private static object? GetParagraphAttribute(IDocParagraph paragraph, string attributeName) =>
        paragraph.DocAttributes.LastOrDefault(candidate => candidate.Name == attributeName)?.Value;

    [TestMethod]
    public void EntryTemplateRenderer_AppliesInlineFontOptionsAndHangingIndent()
    {
        const string json = """
            {"schemaVersion":1,"id":"styled-render","entryRoot":"Individual","blocks":[
              {"kind":"paragraph","role":"adult","indent":18,"hangingIndent":18,"content":[
                {"kind":"field","path":"individual.nameGc","bold":true,"italic":true,"underline":true}
              ]}
            ]}
            """;
        var template = EntryTemplateValidator.ParseAndValidate(json);
        var document = new DocxDocument();
        var person = new PersonEntryTemplateModel
        {
            Surname = "Beispiel",
            GivenNames = "Ada",
            Title = string.Empty,
            NameGc = "Beispiel, Ada",
            NameAk = "Ada Beispiel",
            Anchor = "person-I1",
            Reference = "I1",
            VitalEvents = string.Empty,
            IndexAnchor = string.Empty,
            Occupations = Array.Empty<OccupationEntryTemplateModel>()
        };

        new EntryTemplateRenderer().RenderIndividual(document, template, person);

        var paragraph = document.Root.Enumerate().OfType<IDocParagraph>().First();
        var span = paragraph.Nodes.OfType<IDocSpan>().First();
        Assert.IsTrue(paragraph.DocAttributes.Any(attribute => attribute.Name == DocAttributeNames.IndentationBefore && Equals(attribute.Value, 54)));
        Assert.IsTrue(paragraph.DocAttributes.Any(attribute => attribute.Name == DocAttributeNames.IndentationHanging && Equals(attribute.Value, 36)));
        Assert.AreEqual(1, paragraph.DocAttributes.Count(attribute => attribute.Name == DocAttributeNames.IndentationBefore));
        Assert.AreEqual(1, paragraph.DocAttributes.Count(attribute => attribute.Name == DocAttributeNames.IndentationHanging));
        Assert.IsTrue(span.DocAttributes.Any(attribute => attribute.Name == DocAttributeNames.Bold && Equals(attribute.Value, true)));
        Assert.IsTrue(span.DocAttributes.Any(attribute => attribute.Name == DocAttributeNames.Italic && Equals(attribute.Value, true)));
        Assert.IsTrue(span.DocAttributes.Any(attribute => attribute.Name == DocAttributeNames.Underline && Equals(attribute.Value, true)));
    }

    [TestMethod]
    public void EntryTemplateRenderer_RendersIndividualRootFromTypedModel()
    {
        const string json = """
            {"schemaVersion":1,"id":"individual-test","entryRoot":"Individual",
             "blocks":[{"kind":"paragraph","role":"adult","anchor":"individual.anchor","content":[
               {"kind":"field","path":"individual.nameGc"}
             ]}]}
            """;
        var template = EntryTemplateValidator.ParseAndValidate(json);
        var document = new DocxDocument();
        var person = new PersonEntryTemplateModel
        {
            Surname = "Beispiel",
            GivenNames = "Ada",
            Title = string.Empty,
            NameGc = "Beispiel, Ada",
            NameAk = "Ada Beispiel",
            Anchor = "person-I1",
            Reference = "I1",
            VitalEvents = string.Empty,
            IndexAnchor = string.Empty,
            Occupations = Array.Empty<OccupationEntryTemplateModel>()
        };

        new EntryTemplateRenderer().RenderIndividual(document, template, person);

        Assert.AreEqual("Beispiel, Ada", document.Enumerate()
            .OfType<Document.Base.Models.Interfaces.IDocContent>()
            .Select(element => element.GetTextContent())
            .FirstOrDefault(text => text.Length > 0));
    }

    [TestMethod]
    public void DocxDocument_AddSectionWritesRealSectionBreaksAndInheritedColumns()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), $"docx-columns-{Guid.NewGuid():N}.docx");
        var document = new DocxDocument();
        document.AddParagraph("Normal").TextContent = "One column";
        document.AddSection(2).AddParagraph("family-data").TextContent = "Two columns";
        document.AddSection().AddParagraph("family-data").TextContent = "Inherit two columns";
        document.AddSection(3).AddParagraph("family-data").TextContent = "Three columns";

        try
        {
            Assert.IsTrue(document.SaveTo(outputPath));
            using var archive = ZipFile.OpenRead(outputPath);
            using var stream = archive.GetEntry("word/document.xml")!.Open();
            var xml = XDocument.Load(stream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var sections = xml.Descendants(word + "sectPr").ToArray();
            Assert.AreEqual(4, sections.Length);
            CollectionAssert.AreEqual(
                new[] { "1", "2", "2", "3" },
                sections.Select(section => (string?)section.Element(word + "cols")?.Attribute(word + "num")).ToArray());
            var pageSizes = sections
                .Select(section => section.Element(word + "pgSz")?.ToString(SaveOptions.DisableFormatting))
                .ToArray();
            var pageMargins = sections
                .Select(section => section.Element(word + "pgMar")?.ToString(SaveOptions.DisableFormatting))
                .ToArray();
            CollectionAssert.AreEqual(Enumerable.Repeat(pageSizes[0], pageSizes.Length).ToArray(), pageSizes);
            CollectionAssert.AreEqual(Enumerable.Repeat(pageMargins[0], pageMargins.Length).ToArray(), pageMargins);
            var sectionBreaks = xml.Descendants(word + "pPr").Count(properties => properties.Element(word + "sectPr") is not null);
            Assert.AreEqual(3, sectionBreaks);
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [TestMethod]
    public void DocxDocument_SaveToStreamPreservesSectionColumnSettings()
    {
        var document = new DocxDocument();
        document.AddParagraph("Normal").TextContent = "Initial";
        document.AddSection(4).AddParagraph("family-data").TextContent = "Four columns";
        using var output = new MemoryStream();

        Assert.IsTrue(document.SaveTo(output));
        output.Position = 0;
        using var archive = new ZipArchive(output, ZipArchiveMode.Read, leaveOpen: true);
        using var stream = archive.GetEntry("word/document.xml")!.Open();
        var xml = XDocument.Load(stream);
        XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        var columns = xml.Descendants(word + "sectPr")
            .Select(section => (string?)section.Element(word + "cols")?.Attribute(word + "num"))
            .ToArray();

        CollectionAssert.AreEqual(new[] { "1", "4" }, columns);
    }

    [TestMethod]
    public void DocxDocument_SaveToStreamSerializesGenericParagraphAndCharacterAttributes()
    {
        var document = new DocxDocument();
        var paragraph = document.AddParagraph("Normal");
        paragraph.DocAttributes.Add(new DocAttribute(DocAttributeNames.IndentationBefore, 18));
        paragraph.DocAttributes.Add(new DocAttribute(DocAttributeNames.IndentationHanging, 18));
        paragraph.DocAttributes.Add(new DocAttribute(DocAttributeNames.SpacingBeforePt, 6));
        var span = paragraph.AddLink("#person-1", DocFontStyle.Default);
        span.TextContent = "Ada Beispiel";
        span.DocAttributes.Add(new DocAttribute(DocAttributeNames.Bold, true));
        span.DocAttributes.Add(new DocAttribute(DocAttributeNames.Italic, true));
        span.DocAttributes.Add(new DocAttribute(DocAttributeNames.Underline, true));
        span.DocAttributes.Add(new DocAttribute(DocAttributeNames.FontSizePt, 12d));

        using var output = new MemoryStream();
        Assert.IsTrue(document.SaveTo(output));
        output.Position = 0;
        using var archive = new ZipArchive(output, ZipArchiveMode.Read, leaveOpen: true);
        using var stream = archive.GetEntry("word/document.xml")!.Open();
        var xml = XDocument.Load(stream);
        XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        var paragraphProperties = xml.Descendants(word + "pPr").First();
        Assert.AreEqual("360", (string?)paragraphProperties.Element(word + "ind")?.Attribute(word + "left"));
        Assert.AreEqual("360", (string?)paragraphProperties.Element(word + "ind")?.Attribute(word + "hanging"));
        Assert.AreEqual("120", (string?)paragraphProperties.Element(word + "spacing")?.Attribute(word + "before"));
        var hyperlinkRunProperties = xml.Descendants(word + "hyperlink").Descendants(word + "rPr").First();
        Assert.IsNotNull(hyperlinkRunProperties.Element(word + "b"));
        Assert.IsNotNull(hyperlinkRunProperties.Element(word + "i"));
        Assert.AreEqual("single", (string?)hyperlinkRunProperties.Element(word + "u")?.Attribute(word + "val"));
        Assert.AreEqual("24", (string?)hyperlinkRunProperties.Element(word + "sz")?.Attribute(word + "val"));
    }

    [TestMethod]
    public void EntryTemplateStore_LoadsExternalJsonTemplate()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"ofb-template-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "custom.json");
        File.WriteAllText(path,
            "{\"schemaVersion\":1,\"id\":\"custom\",\"entryRoot\":\"Family\",\"blocks\":[]}",
            Encoding.UTF8);

        try
        {
            var template = new EntryTemplateStore().Load(path);
            Assert.AreEqual("custom", template.Id);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
