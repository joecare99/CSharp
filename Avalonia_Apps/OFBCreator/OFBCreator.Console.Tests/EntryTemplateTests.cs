using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Document.Docx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OFBCreator.Console.Models;
using OFBCreator.Console.Services.Templates;

namespace OFBCreator.Console.Tests;

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
            NameGc = "Beispiel, Ada",
            NameAk = "Ada Beispiel",
            Anchor = "person-I1",
            Reference = "I1",
            VitalEventsGc = string.Empty,
            VitalEventsAk = string.Empty,
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
