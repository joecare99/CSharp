using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Xml.Linq;
using Document.Base.Models.Interfaces;
using Document.Base.Factories;
using Document.Docx;
using Genealogy.Gedcom;
using Genealogy.Drivers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Core.Models;
using OFBCreator.Core.Services;
using OFBCreator.Publishing.Models;
using OFBCreator.Publishing.Services;
using OFBCreator.Projects.Models;

namespace OFBCreator.Publishing.Tests;

[TestClass]
public sealed class OFBExportServiceTests
{
    [TestMethod]
    public async Task ExportAsync_WithSyntheticGedcom_WritesOrderedDocxWithResolvedNavigation()
    {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "TestData", "gedcom-5.5.1-synthetic.ged");
        Assert.IsTrue(File.Exists(fixturePath), $"Synthetic GEDCOM fixture was not found: {fixturePath}");

        UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
        var documentFactory = Substitute.For<IUserDocumentFactory>();
        documentFactory.CreateDocument(OFBOutputFormat.Docx).Returns(_ => UserDocumentFactory.Create(".docx"));
        var service = new OFBExportService(new GedComDataSource(), documentFactory);
        var outputPath = Path.Combine(Path.GetTempPath(), $"ofb-test-{Guid.NewGuid():N}", "sample.docx");

        await service.ExportAsync(new OFBGenerateOptions
        {
            InputPath = fixturePath,
            OutputPath = outputPath,
            Title = "Synthetisches Ortsfamilienbuch",
            Preface = "Synthetisches Vorwort",
            Legend = "Synthetische Beispieldaten",
            UseDocxFormat = true
        });

        documentFactory.Received(1).CreateDocument(OFBOutputFormat.Docx);
        Assert.IsTrue(File.Exists(outputPath));
        using var archive = ZipFile.OpenRead(outputPath);
        var documentXmlEntry = archive.GetEntry("word/document.xml");
        Assert.IsNotNull(documentXmlEntry);
        using var documentStream = documentXmlEntry!.Open();
        var documentXml = XDocument.Load(documentStream);
        XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        var paragraphTexts = documentXml.Descendants(word + "p")
            .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(text => text.Value)))
            .ToArray();
        var prefaceIndex = Array.FindIndex(paragraphTexts, text => text == "Vorwort");
        var groupIndex = Array.FindIndex(paragraphTexts, text => text == "Beispiel");
        var familyIndex = Array.FindIndex(paragraphTexts, text => text.StartsWith("00001 ", StringComparison.Ordinal));
        var personIndex = Array.FindIndex(paragraphTexts, text => text == "Personenindex");
        Assert.IsTrue(prefaceIndex >= 0 && groupIndex > prefaceIndex && familyIndex >= groupIndex && personIndex > familyIndex,
            $"preface={prefaceIndex}, group={groupIndex}, family={familyIndex}, personIndex={personIndex}; "
            + string.Join(" | ", paragraphTexts.Where(text => text is "Vorwort" or "Beispiel" or "00001" or "Personenindex")));
        Assert.IsFalse(paragraphTexts.Skip(groupIndex + 1).Take(personIndex - groupIndex - 1)
            .Any(text => text == "Beispiel"));

        var bookmarkNames = documentXml.Descendants(word + "bookmarkStart")
            .Select(element => (string?)element.Attribute(word + "name"))
            .Where(name => name is not null)
            .ToHashSet(StringComparer.Ordinal);
        var linkTargets = documentXml.Descendants(word + "hyperlink")
            .Select(element => (string?)element.Attribute(word + "anchor"))
            .Where(target => target is not null)
            .ToArray();
        Assert.IsTrue(bookmarkNames.Contains("family-00001"));
        Assert.IsTrue(bookmarkNames.Contains("person-I1"));
        Assert.IsTrue(bookmarkNames.Contains("index-person-1"));
        Assert.IsTrue(linkTargets.Length > 0);
        Assert.IsTrue(linkTargets.Contains("index-person-1"));
        Assert.IsFalse(linkTargets.Any(target => !bookmarkNames.Contains(target!)));
        Assert.IsTrue(string.Concat(paragraphTexts).Contains("Synthetische Beispieldaten", StringComparison.Ordinal));
        Assert.IsTrue(paragraphTexts.Contains("* = geboren", StringComparer.Ordinal));
        Assert.IsTrue(paragraphTexts.Contains("† = gestorben", StringComparer.Ordinal));
        Assert.IsTrue(paragraphTexts.Contains("## Zeichen und Abkürzungen") == false);
        Assert.IsTrue(paragraphTexts.Contains("Aufbau und Gliederung", StringComparer.Ordinal));
        Assert.IsTrue(paragraphTexts.Contains("Synthetisches Vorwort", StringComparer.Ordinal));
        documentStream.Dispose();
        archive.Dispose();
        File.Delete(outputPath);
    }

    [TestMethod]
    public async Task ExportAsync_PublishesFamiliesWithoutSurnameAfterNamedGroupsWithContinuousNumbers()
    {
        const string gedcom =
            "0 HEAD\r\n1 GEDC\r\n2 VERS 5.5.1\r\n2 FORM LINEAGE-LINKED\r\n1 CHAR UTF-8\r\n"
            + "0 @I1@ INDI\r\n1 NAME Anna /NN/\r\n1 FAMS @F1@\r\n"
            + "0 @I2@ INDI\r\n1 NAME Bernd /Miller/\r\n1 FAMS @F2@\r\n"
            + "0 @F1@ FAM\r\n1 HUSB @I1@\r\n1 MARR\r\n"
            + "0 @F2@ FAM\r\n1 HUSB @I2@\r\n1 MARR\r\n"
            + "0 TRLR\r\n";
        var testDirectory = Path.Combine(Path.GetTempPath(), $"ofb-no-name-order-{Guid.NewGuid():N}");
        Directory.CreateDirectory(testDirectory);
        var inputPath = Path.Combine(testDirectory, "families.ged");
        var outputPath = Path.Combine(testDirectory, "families.docx");
        await File.WriteAllTextAsync(inputPath, gedcom, Encoding.UTF8);

        try
        {
            UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
            var documentFactory = Substitute.For<IUserDocumentFactory>();
            documentFactory.CreateDocument(OFBOutputFormat.Docx).Returns(_ => UserDocumentFactory.Create(".docx"));
            await new OFBExportService(new GedComDataSource(), documentFactory).ExportAsync(new OFBGenerateOptions
            {
                InputPath = inputPath,
                OutputPath = outputPath,
                Title = "Namenlose Familien",
                UseDocxFormat = true
            });

            using var archive = ZipFile.OpenRead(outputPath);
            using var documentStream = archive.GetEntry("word/document.xml")!.Open();
            var documentXml = XDocument.Load(documentStream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var paragraphTexts = documentXml.Descendants(word + "p")
                .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(text => text.Value)))
                .ToArray();
            var namedGroupIndex = Array.IndexOf(paragraphTexts, "Miller");
            var noNameGroupIndex = Array.IndexOf(paragraphTexts, "Familien ohne Namen");
            var namedFamilyIndex = Array.FindIndex(paragraphTexts, text => text.StartsWith("00001 ", StringComparison.Ordinal));
            var noNameFamilyIndex = Array.FindIndex(paragraphTexts, text => text.StartsWith("00002 ", StringComparison.Ordinal));

            Assert.IsTrue(namedGroupIndex >= 0 && namedFamilyIndex > namedGroupIndex, string.Join(" | ", paragraphTexts));
            Assert.IsTrue(noNameGroupIndex > namedFamilyIndex && noNameFamilyIndex > noNameGroupIndex,
                string.Join(" | ", paragraphTexts));
            Assert.IsTrue(paragraphTexts[namedFamilyIndex].StartsWith("00001 ", StringComparison.Ordinal));
            Assert.IsTrue(paragraphTexts[noNameFamilyIndex].StartsWith("00002 ", StringComparison.Ordinal));
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }

    [TestMethod]
    public async Task ExportAsync_RendersParentAliasesAndSurnameVariantsInPersonIndex()
    {
        const string gedcom =
            "0 HEAD\r\n1 GEDC\r\n2 VERS 5.5.1\r\n2 FORM LINEAGE-LINKED\r\n1 CHAR UTF-8\r\n" +
            "0 @I1@ INDI\r\n1 NAME Anna /Muster/\r\n2 GIVN Anna\r\n2 SURN Muster\r\n2 TYPE BIRTH\r\n1 NAME Anna /Beispiel/\r\n2 GIVN Anna\r\n2 SURN Beispiel\r\n2 TYPE AKA\r\n1 ALIA Anna /Alias/\r\n1 SEX F\r\n1 BIRT\r\n2 DATE 1901\r\n1 DEAT\r\n2 DATE 1980\r\n1 FAMS @F1@\r\n" +
            "0 @I2@ INDI\r\n1 NAME Emil /Partner/\r\n1 SEX M\r\n" +
            "0 @I3@ INDI\r\n1 NAME Kind /Muster/\r\n1 FAMC @F1@\r\n" +
            "0 @F1@ FAM\r\n1 HUSB @I2@\r\n1 WIFE @I1@\r\n1 CHIL @I3@\r\n0 TRLR\r\n";
        var testDirectory = Path.Combine(Path.GetTempPath(), $"ofb-index-aka-{Guid.NewGuid():N}");
        Directory.CreateDirectory(testDirectory);
        var inputPath = Path.Combine(testDirectory, "names.ged");
        var outputPath = Path.Combine(testDirectory, "names.docx");
        await File.WriteAllTextAsync(inputPath, gedcom, Encoding.UTF8);

        try
        {
            UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
            var documentFactory = Substitute.For<IUserDocumentFactory>();
            documentFactory.CreateDocument(OFBOutputFormat.Docx).Returns(_ => UserDocumentFactory.Create(".docx"));
            await new OFBExportService(
                new CanonicalGedcomFamilyDataSource(new GedcomInputDriver(), new CanonicalGenealogyAdapter()),
                documentFactory).ExportAsync(new OFBGenerateOptions
            {
                InputPath = inputPath,
                OutputPath = outputPath,
                Title = "Namenregister",
                UseDocxFormat = true
            });

            using var archive = ZipFile.OpenRead(outputPath);
            using var stream = archive.GetEntry("word/document.xml")!.Open();
            var xml = XDocument.Load(stream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var texts = xml.Descendants(word + "p")
                .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(text => text.Value)))
                .ToArray();
            Assert.IsTrue(texts.Any(text => text.Contains("Beispiel, Anna (Anna Alias)", StringComparison.Ordinal)),
                string.Join(" | ", texts.Where(text => text.Contains("Anna", StringComparison.OrdinalIgnoreCase))));
            Assert.IsTrue(texts.Any(text => text.Contains("Anna [00001]", StringComparison.Ordinal)),
                string.Join(" | ", texts.Where(text => text.Contains("Anna", StringComparison.Ordinal))));
            Assert.IsTrue(texts.Count(text => text.Contains("Anna [00001]", StringComparison.Ordinal)) >= 2);
            var personIndexPosition = Array.IndexOf(texts, "Personenindex");
            Assert.IsTrue(personIndexPosition >= 0);
            var personIndexTexts = texts.Skip(personIndexPosition + 1).ToArray();
            Assert.IsTrue(personIndexTexts.Any(text => text.Contains("00001", StringComparison.Ordinal)), string.Join(" | ", personIndexTexts));
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }

    [TestMethod]
    public async Task ExportAsync_WithGcTemplateRendersFamilyNumberMarriagePlaceAndPropertyFacts()
    {
        const string gedcom =
            "0 HEAD\r\n1 CHAR UTF-8\r\n" +
            "0 @I1@ INDI\r\n1 NAME Emil /Muster/\r\n1 SEX M\r\n1 FAMS @F1@\r\n1 PROP Hofgut\r\n2 DATE 1 JAN 1890\r\n1 BIRT\r\n2 PLAC München, Oberbayern, Bayern\r\n1 DEAT\r\n2 PLAC Tübingen, Baden-Würtemberg\r\n" +
            "0 @I2@ INDI\r\n1 NAME Anna /Muster/\r\n1 SEX F\r\n1 FAMS @F1@\r\n1 RESI\r\n2 PLAC München, Oberbayern, Bayern\r\n" +
            "0 @I3@ INDI\r\n1 NAME Kind /Muster/\r\n1 FAMC @F1@\r\n" +
            "0 @F1@ FAM\r\n1 HUSB @I1@\r\n1 WIFE @I2@\r\n1 CHIL @I3@\r\n1 MARR\r\n2 DATE 1 JAN 1888\r\n2 PLAC München, Oberbayern, Bayern\r\n1 PROP Mühle\r\n2 DATE 1 JAN 1880\r\n0 TRLR\r\n";
        var testDirectory = Path.Combine(Path.GetTempPath(), $"ofb-gc-properties-{Guid.NewGuid():N}");
        Directory.CreateDirectory(testDirectory);
        var inputPath = Path.Combine(testDirectory, "properties.ged");
        var outputPath = Path.Combine(testDirectory, "properties.docx");
        await File.WriteAllTextAsync(inputPath, gedcom, Encoding.UTF8);

        try
        {
            UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
            var documentFactory = Substitute.For<IUserDocumentFactory>();
            documentFactory.CreateDocument(OFBOutputFormat.Docx).Returns(_ => UserDocumentFactory.Create(".docx"));
            await new OFBExportService(
                new CanonicalGedcomFamilyDataSource(new GedcomInputDriver(), new CanonicalGenealogyAdapter()),
                documentFactory).ExportAsync(new OFBGenerateOptions
            {
                InputPath = inputPath,
                OutputPath = outputPath,
                Title = "GC Besitzdaten",
                UseDocxFormat = true
            });

            using var archive = ZipFile.OpenRead(outputPath);
            using var stream = archive.GetEntry("word/document.xml")!.Open();
            var xml = XDocument.Load(stream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var paragraphs = xml.Descendants(word + "p").ToArray();
            var texts = paragraphs.Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(text => text.Value))).ToArray();
            Assert.IsTrue(texts.Any(text => text.Contains("00001 ⚭", StringComparison.Ordinal)), string.Join(Environment.NewLine, texts));
            Assert.IsTrue(texts.Any(text => text.Contains("Mühle", StringComparison.Ordinal)));
            Assert.IsTrue(texts.Any(text => text.Contains("Hofgut", StringComparison.Ordinal)));
            Assert.IsTrue(texts.Any(text => text.Contains("Wohnort: München, Oberbayern, Bayern", StringComparison.Ordinal)));
            var placeIndex = Array.FindIndex(texts, text => text == "Ortsindex");
            Assert.IsTrue(placeIndex >= 0);
            var alphaPlaceIndex = Array.FindIndex(texts, placeIndex + 1, text => text == "Ortsindex Alphabetisch");
            Assert.IsTrue(alphaPlaceIndex > placeIndex);
            var hierarchyParagraphs = paragraphs.Skip(placeIndex + 1).Take(alphaPlaceIndex - placeIndex - 1).ToArray();
            var hierarchyTexts = hierarchyParagraphs
                .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(text => text.Value)))
                .ToArray();
            Assert.IsTrue(hierarchyTexts.Contains("Deutschland"));
            Assert.IsTrue(hierarchyTexts.Contains("Baden-Württemberg"), string.Join(" | ", hierarchyTexts));
            Assert.IsTrue(hierarchyTexts.Contains("Tübingen"));
            Assert.IsFalse(hierarchyTexts.Contains("Baden-Würtemberg"));
            Assert.IsTrue(hierarchyTexts.Contains("Bayern"));
            Assert.IsTrue(hierarchyTexts.Contains("Oberbayern"));
            Assert.IsTrue(hierarchyTexts.Contains("München"));
            var headingIndexes = new[] { "Deutschland", "Baden-Württemberg", "Bayern", "Oberbayern", "Tübingen", "München" }
                .Select(name => Array.IndexOf(hierarchyTexts, name))
                .ToArray();
            Assert.IsTrue(headingIndexes.All(index => index >= 0));
            Assert.IsTrue(headingIndexes.All(index => !hierarchyParagraphs[index].Descendants(word + "hyperlink").Any()),
                "Hierarchy headings must not carry inherited family links.");
            var tuebingenIndex = Array.IndexOf(hierarchyTexts, "Tübingen");
            var muenchenIndex = Array.IndexOf(hierarchyTexts, "München");
            Assert.AreEqual("      [00001]", hierarchyTexts[tuebingenIndex + 1]);
            Assert.AreEqual("        [00001]", hierarchyTexts[muenchenIndex + 1]);
            var hierarchyHeadings = headingIndexes
                .Select(index => (string?)hierarchyParagraphs[index].Descendants(word + "pStyle").FirstOrDefault()?.Attribute(word + "val"))
                .ToArray();
            Assert.IsTrue(hierarchyHeadings.All(style => !string.IsNullOrWhiteSpace(style) && style.StartsWith("Heading", StringComparison.OrdinalIgnoreCase)));
            var headingLevels = hierarchyHeadings
                .Select(style => int.Parse(new string(style!.Where(char.IsDigit).ToArray()), System.Globalization.CultureInfo.InvariantCulture))
                .ToArray();
            Assert.AreEqual(2, headingLevels[0]);
            Assert.AreEqual(3, headingLevels[1]);
            Assert.AreEqual(4, headingLevels[4]);
            var bookmarks = xml.Descendants(word + "bookmarkStart").Select(element => (string?)element.Attribute(word + "name")).ToHashSet(StringComparer.Ordinal);
            var links = xml.Descendants(word + "hyperlink").Select(element => (string?)element.Attribute(word + "anchor")).Where(target => target is not null).ToArray();
            Assert.IsTrue(bookmarks.Contains("index-property-1"));
            Assert.IsTrue(links.Contains("index-property-1"));
            Assert.IsTrue(paragraphs.Any(paragraph => paragraph.Descendants(word + "t").Any(text => text.Value.Contains("Muster, Emil", StringComparison.Ordinal))));
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }

    [TestMethod]
    public async Task ExportAsync_WithGcTemplate_WritesLinkedParentAndSubsequentFamilyReferences()
    {
        const string gedcom =
            "0 HEAD\r\n1 CHAR UTF-8\r\n" +
            "0 @I1@ INDI\r\n1 NAME Emil /Muster/\r\n1 SEX M\r\n1 FAMS @F1@\r\n" +
            "0 @I2@ INDI\r\n1 NAME Anna /Muster/\r\n1 SEX F\r\n1 FAMS @F1@\r\n" +
            "0 @I3@ INDI\r\n1 NAME Ältestes /Muster/\r\n1 REFN PN-003\r\n1 FAMC @F1@\r\n1 FAMS @F2@\r\n1 BIRT\r\n2 DATE 1 JAN 1920\r\n" +
            "0 @I4@ INDI\r\n1 NAME Jüngstes /Muster/\r\n1 FAMC @F1@\r\n1 BIRT\r\n2 DATE 1 JAN 1930\r\n" +
            "0 @I5@ INDI\r\n1 NAME Partner /Beispiel/\r\n1 SEX F\r\n1 FAMS @F2@\r\n" +
            "0 @I11@ INDI\r\n1 NAME Fremdname /Beispiel/\r\n1 FAMC @F1@\r\n" +
            "0 @I6@ INDI\r\n1 NAME Partner Zwei /Muster/\r\n1 SEX F\r\n" +
            "0 @I7@ INDI\r\n1 NAME Partner Drei /Muster/\r\n1 SEX F\r\n" +
            "0 @I8@ INDI\r\n1 NAME Elternteil Eins /Muster/\r\n1 SEX M\r\n" +
            "0 @I9@ INDI\r\n1 NAME Elternteil Zwei /Muster/\r\n1 SEX F\r\n" +
            "0 @I10@ INDI\r\n1 NAME Kind Lücke /Muster/\r\n" +
            "0 @F1@ FAM\r\n1 HUSB @I1@\r\n1 WIFE @I2@\r\n1 CHIL @I3@\r\n1 CHIL @I4@\r\n1 CHIL @I11@\r\n1 MARR\r\n2 DATE 1 JAN 1900\r\n" +
            "0 @F2@ FAM\r\n1 HUSB @I3@\r\n1 WIFE @I5@\r\n1 CHIL @I4@\r\n1 MARR\r\n2 DATE 1 JAN 1940\r\n" +
            "0 @F3@ FAM\r\n1 HUSB @I3@\r\n1 WIFE @I6@\r\n1 MARR\r\n2 DATE 1 JAN 1950\r\n" +
            "0 @F4@ FAM\r\n1 HUSB @I3@\r\n1 WIFE @I7@\r\n1 MARR\r\n2 DATE 1 JAN 1960\r\n" +
            "0 @F5@ FAM\r\n1 HUSB @I8@\r\n1 WIFE @I9@\r\n1 CHIL @I10@\r\n1 MARR\r\n2 DATE 1 JAN 1970\r\n" +
            "0 @F6@ FAM\r\n1 HUSB @I3@\r\n1 WIFE @I8@\r\n1 MARR\r\n2 DATE 1 JAN 1980\r\n0 TRLR\r\n";
        var testDirectory = Path.Combine(Path.GetTempPath(), $"ofb-gc-references-{Guid.NewGuid():N}");
        Directory.CreateDirectory(testDirectory);
        var inputPath = Path.Combine(testDirectory, "references.ged");
        var outputPath = Path.Combine(testDirectory, "references.docx");
        await File.WriteAllTextAsync(inputPath, gedcom, Encoding.UTF8);

        try
        {
            UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
            var documentFactory = Substitute.For<IUserDocumentFactory>();
            documentFactory.CreateDocument(OFBOutputFormat.Docx).Returns(_ => UserDocumentFactory.Create(".docx"));

            await new OFBExportService(new GedComDataSource(), documentFactory).ExportAsync(new OFBGenerateOptions
            {
                InputPath = inputPath,
                OutputPath = outputPath,
                Title = "GC-Verweise",
                UseDocxFormat = true
            });

            using var archive = ZipFile.OpenRead(outputPath);
            using var documentStream = archive.GetEntry("word/document.xml")!.Open();
            var documentXml = XDocument.Load(documentStream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var paragraphs = documentXml.Descendants(word + "p").ToArray();
            var paragraphTexts = paragraphs
                .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(text => text.Value)))
                .ToArray();
            var childrenHeaderIndex = Array.FindIndex(paragraphTexts, text => text.EndsWith("Kinder:", StringComparison.Ordinal));
            Assert.IsTrue(childrenHeaderIndex >= 0, string.Join(" | ", paragraphTexts));
            Assert.IsTrue(paragraphTexts.Skip(childrenHeaderIndex + 1).Take(3)
                .Any(text => text.StartsWith("1. Ältestes", StringComparison.Ordinal)),
                string.Join(" | ", paragraphTexts.Skip(childrenHeaderIndex).Take(5)));
            var childParagraph = paragraphTexts.Single(text => text.StartsWith("1. Ältestes", StringComparison.Ordinal));
            Assert.IsTrue(childParagraph.Contains("[00002 - 00004, 00006]", StringComparison.Ordinal), childParagraph);
            Assert.IsTrue(paragraphTexts.Any(text => text.StartsWith("3. Beispiel, Fremdname", StringComparison.Ordinal)));
            Assert.IsTrue(paragraphTexts.Any(text => text.Contains("Ältestes (1920–?)", StringComparison.Ordinal)
                && text.Contains("<00001>", StringComparison.Ordinal)),
                string.Join(" | ", paragraphTexts.Where(text => text.Contains("Ältestes", StringComparison.Ordinal))));
            var personIndexParagraphIndex = Array.FindIndex(paragraphTexts, text =>
                text.Contains("Ältestes (1920–?)", StringComparison.Ordinal));
            Assert.IsTrue(personIndexParagraphIndex >= 0, string.Join(" | ", paragraphTexts));
            var personIndexParagraph = paragraphTexts[personIndexParagraphIndex];
            Assert.IsTrue(personIndexParagraph.Contains("Ältestes (1920–?)", StringComparison.Ordinal)
                && personIndexParagraph.Contains("<00001>", StringComparison.Ordinal)
                && personIndexParagraph.Contains("[00002 - 00004, 00006]", StringComparison.Ordinal),
                personIndexParagraph);
            var siblingIndexParagraph = paragraphTexts.Single(text =>
                text.Contains("Jüngstes (1930–?)", StringComparison.Ordinal));
            Assert.IsTrue(siblingIndexParagraph.Contains("<00001, 00002>", StringComparison.Ordinal), siblingIndexParagraph);
            Assert.IsTrue(paragraphTexts.Any(text => text.Contains("<PN=PN-003>", StringComparison.Ordinal)),
                string.Join(" | ", paragraphTexts));
            Assert.IsFalse(paragraphTexts.Any(text => text.Contains("<PN=I3>", StringComparison.Ordinal)));
            Assert.IsTrue(Array.FindIndex(paragraphTexts, text => text.StartsWith("1. Ältestes", StringComparison.Ordinal))
                < Array.FindIndex(paragraphTexts, text => text.StartsWith("2. Jüngstes", StringComparison.Ordinal)));

            var bookmarkNames = documentXml.Descendants(word + "bookmarkStart")
                .Select(element => (string?)element.Attribute(word + "name"))
                .Where(name => name is not null)
                .ToHashSet(StringComparer.Ordinal);
            var familyLinks = documentXml.Descendants(word + "hyperlink")
                .Select(element => (string?)element.Attribute(word + "anchor"))
                .Where(anchor => anchor is not null && anchor.StartsWith("family-", StringComparison.Ordinal))
                .ToArray();
            Assert.IsTrue(familyLinks.All(anchor => bookmarkNames.Contains(anchor!)));
            var personIndexFamilyLinks = paragraphs[personIndexParagraphIndex]
                .Descendants(word + "hyperlink")
                .Select(element => (string?)element.Attribute(word + "anchor"))
                .Where(anchor => anchor is not null && anchor.StartsWith("family-", StringComparison.Ordinal))
                .ToArray();
            CollectionAssert.AreEqual(new[] { "family-00001", "family-00002", "family-00004", "family-00006" },
                personIndexFamilyLinks!);
            var siblingIndexParagraphIndex = Array.FindIndex(paragraphTexts, text =>
                text.Contains("Jüngstes (1930–?)", StringComparison.Ordinal));
            var siblingFamilyLinks = paragraphs[siblingIndexParagraphIndex]
                .Descendants(word + "hyperlink")
                .Select(element => (string?)element.Attribute(word + "anchor"))
                .Where(anchor => anchor is not null && anchor.StartsWith("family-", StringComparison.Ordinal))
                .ToArray();
            CollectionAssert.AreEqual(new[] { "family-00001", "family-00002" }, siblingFamilyLinks!);
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }

    [TestMethod]
    public async Task ExportAsync_WithGcTemplate_RendersOnlyPreferredVitalEventsForParentsAndChildren()
    {
        const string gedcom =
            "0 HEAD\r\n1 GEDC\r\n2 VERS 5.5.1\r\n2 FORM LINEAGE-LINKED\r\n1 CHAR UTF-8\r\n" +
            "0 @I1@ INDI\r\n1 NAME Eltern /Muster/\r\n1 SEX M\r\n1 FAMS @F1@\r\n" +
            "1 DEAT\r\n2 DATE 1980\r\n1 CENS\r\n2 DATE 1940\r\n" +
            "1 BIRT\r\n2 DATE 1900\r\n2 PLAC Beispielort\r\n1 BIRT\r\n2 DATE 1901\r\n2 PLAC Alternativort\r\n1 DEAT\r\n2 DATE 1981\r\n" +
            "0 @I2@ INDI\r\n1 NAME Elternteil /Beispiel/\r\n1 SEX F\r\n1 FAMS @F1@\r\n" +
            "0 @I3@ INDI\r\n1 NAME Kind /Muster/\r\n1 FAMC @F1@\r\n1 FAMS @F2@\r\n" +
            "1 CENS\r\n2 DATE 1930\r\n1 BAPM\r\n2 DATE 1923\r\n1 BAPM\r\n2 DATE 1924\r\n" +
            "1 BIRT\r\n2 DATE 1920\r\n2 PLAC Kinderort\r\n1 BIRT\r\n2 DATE 1921\r\n" +
            "0 @I4@ INDI\r\n1 NAME Partner /Beispiel/\r\n1 FAMS @F2@\r\n" +
            "0 @I5@ INDI\r\n1 NAME Unverlinktes Kind /Muster/\r\n1 FAMC @F1@\r\n" +
            "1 CENS\r\n2 DATE 1955\r\n1 BIRT\r\n2 DATE 1950\r\n" +
            "0 @F1@ FAM\r\n1 HUSB @I1@\r\n1 WIFE @I2@\r\n1 CHIL @I3@\r\n1 CHIL @I5@\r\n" +
            "0 @F2@ FAM\r\n1 HUSB @I3@\r\n1 WIFE @I4@\r\n1 MARR\r\n0 TRLR\r\n";
        var testDirectory = Path.Combine(Path.GetTempPath(), $"ofb-gc-preferred-events-{Guid.NewGuid():N}");
        Directory.CreateDirectory(testDirectory);
        var inputPath = Path.Combine(testDirectory, "preferred-events.ged");
        var outputPath = Path.Combine(testDirectory, "preferred-events.docx");
        await File.WriteAllTextAsync(inputPath, gedcom, Encoding.UTF8);

        try
        {
            UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
            var documentFactory = Substitute.For<IUserDocumentFactory>();
            documentFactory.CreateDocument(OFBOutputFormat.Docx).Returns(_ => UserDocumentFactory.Create(".docx"));

            var dataSource = new CanonicalGedcomFamilyDataSource(new GedcomInputDriver(), new CanonicalGenealogyAdapter());
            await new OFBExportService(dataSource, documentFactory).ExportAsync(new OFBGenerateOptions
            {
                InputPath = inputPath,
                OutputPath = outputPath,
                Title = "Bevorzugte Ereignisse",
                UseDocxFormat = true
            });

            using var archive = ZipFile.OpenRead(outputPath);
            using var documentStream = archive.GetEntry("word/document.xml")!.Open();
            var documentXml = XDocument.Load(documentStream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var paragraphTexts = documentXml.Descendants(word + "p")
                .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(text => text.Value)))
                .ToArray();

            var parentEvents = paragraphTexts.Single(text => text.Contains("* 1900", StringComparison.Ordinal));
            var linkedChildEvents = paragraphTexts.Single(text => text.StartsWith("1. Kind", StringComparison.Ordinal));
            var unlinkedChildEvents = paragraphTexts.Single(text => text.StartsWith("2. Unverlinktes Kind", StringComparison.Ordinal));
            var linkedChildAsParentEvents = paragraphTexts.Single(text => text.Contains("1930", StringComparison.Ordinal));
            var birthIndex = parentEvents.IndexOf("* 1900", StringComparison.Ordinal);
            var deathIndex = parentEvents.IndexOf("† 1980", StringComparison.Ordinal);
            Assert.IsTrue(birthIndex >= 0 && birthIndex < deathIndex, parentEvents);
            Assert.IsTrue(paragraphTexts.Any(text => text.Contains("1940: Zähl.", StringComparison.Ordinal)));
            Assert.IsFalse(parentEvents.Contains("1901", StringComparison.Ordinal), parentEvents);
            Assert.IsFalse(parentEvents.Contains("1981", StringComparison.Ordinal), parentEvents);
            Assert.IsTrue(linkedChildEvents.Contains(", * 1920 in Kinderort, ≈ 1923", StringComparison.Ordinal), linkedChildEvents);
            Assert.IsFalse(linkedChildEvents.Contains("1930", StringComparison.Ordinal), linkedChildEvents);
            Assert.IsFalse(linkedChildEvents.Contains("1921", StringComparison.Ordinal), linkedChildEvents);
            Assert.IsFalse(linkedChildEvents.Contains("1924", StringComparison.Ordinal), linkedChildEvents);
            Assert.IsTrue(linkedChildAsParentEvents.Contains("1930", StringComparison.Ordinal), linkedChildAsParentEvents);
            Assert.IsTrue(unlinkedChildEvents.Contains("* 1950", StringComparison.Ordinal), unlinkedChildEvents);
            var unlinkedCensusIndex = Array.FindIndex(paragraphTexts, text =>
                text.Contains("1955", StringComparison.Ordinal)
                && text.Contains("Zähl.", StringComparison.Ordinal));
            Assert.IsTrue(unlinkedCensusIndex > Array.IndexOf(paragraphTexts, unlinkedChildEvents),
                string.Join(" | ", paragraphTexts));

            var placeLinks = documentXml.Descendants(word + "hyperlink")
                .Select(link => new
                {
                    Text = string.Concat(link.Descendants(word + "t").Select(text => text.Value)),
                    Target = (string?)link.Attribute(word + "anchor")
                })
                .Where(link => link.Text is "Beispielort" or "Kinderort")
                .ToArray();
            Assert.AreEqual(3, placeLinks.Length);
            CollectionAssert.Contains(placeLinks.Select(link => link.Text).ToArray(), "Beispielort");
            CollectionAssert.Contains(placeLinks.Select(link => link.Text).ToArray(), "Kinderort");
            var placeBookmarks = documentXml.Descendants(word + "bookmarkStart")
                .Select(bookmark => (string?)bookmark.Attribute(word + "name"))
                .Where(name => name is not null)
                .ToHashSet(StringComparer.Ordinal);
            foreach (var placeLink in placeLinks)
            {
                Assert.IsNotNull(placeLink.Target);
                Assert.IsTrue(placeBookmarks.Contains(placeLink.Target!), placeLink.Target);
            }
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }

    [TestMethod]
    [DataRow("gc")]
    [DataRow("ak")]
    public async Task ExportAsync_RendersPersonTitleAndReligionInNameOrderAndListsOnlyDatedEvents(string templateName)
    {
        const string gedcom =
            "0 HEAD\r\n1 GEDC\r\n2 VERS 5.5.1\r\n2 FORM LINEAGE-LINKED\r\n1 CHAR UTF-8\r\n"
            + "0 @I1@ INDI\r\n1 NAME Anna /Muster/\r\n1 SEX F\r\n2 DATE 1939\r\n1 TITL Gräfin\r\n1 RELI katholisch\r\n1 REFN PN-1\r\n1 FAMS @F1@\r\n"
            + "1 BIRT\r\n2 DATE 1900\r\n1 CENS\r\n2 DATE 1940\r\n1 CENS\r\n"
            + "1 INFO Private Information\r\n2 DATE 1942\r\n1 EVEN Geheime Beschreibung\r\n2 DATE 1943\r\n"
            + "0 @I2@ INDI\r\n1 NAME Kind /Muster/\r\n1 FAMC @F1@\r\n1 CENS\r\n2 DATE 1960\r\n"
            + "0 @F1@ FAM\r\n1 WIFE @I1@\r\n1 CHIL @I2@\r\n0 TRLR\r\n";
        var testDirectory = Path.Combine(Path.GetTempPath(), $"ofb-person-fact-layout-{Guid.NewGuid():N}");
        Directory.CreateDirectory(testDirectory);
        var inputPath = Path.Combine(testDirectory, "people.ged");
        var outputPath = Path.Combine(testDirectory, "people.docx");
        await File.WriteAllTextAsync(inputPath, gedcom, Encoding.UTF8);

        try
        {
            UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
            var documentFactory = Substitute.For<IUserDocumentFactory>();
            documentFactory.CreateDocument(OFBOutputFormat.Docx).Returns(_ => UserDocumentFactory.Create(".docx"));
            await new OFBExportService(
                new CanonicalGedcomFamilyDataSource(new GedcomInputDriver(), new CanonicalGenealogyAdapter()),
                documentFactory).ExportAsync(new OFBGenerateOptions
            {
                InputPath = inputPath,
                OutputPath = outputPath,
                Title = "Personenfakten",
                Template = templateName,
                UseDocxFormat = true
            });

            using var archive = ZipFile.OpenRead(outputPath);
            using var documentStream = archive.GetEntry("word/document.xml")!.Open();
            var documentXml = XDocument.Load(documentStream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var paragraphs = documentXml.Descendants(word + "p").ToArray();
            var paragraphTexts = paragraphs
                .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(text => text.Value)))
                .ToArray();
            var parentIndex = Array.FindIndex(paragraphTexts, text => text.Contains("Gräfin", StringComparison.Ordinal));
            Assert.IsTrue(parentIndex >= 0, string.Join(" | ", paragraphTexts));
            var parentText = paragraphTexts[parentIndex];
            var expectedNameAndTitle = templateName == "gc"
                ? "Muster, Anna, Gräfin"
                : "Anna Muster, Gräfin";
            Assert.IsTrue(parentText.Contains(expectedNameAndTitle, StringComparison.Ordinal), parentText);
            var titleIndex = parentText.IndexOf("Gräfin", StringComparison.Ordinal);
            var referenceIndex = parentText.IndexOf("PN-1", StringComparison.Ordinal);
            var religionIndex = parentText.IndexOf("katholisch", StringComparison.Ordinal);
            var birthIndex = parentText.IndexOf("* 1900", StringComparison.Ordinal);
            Assert.IsTrue(titleIndex >= 0 && referenceIndex > titleIndex, parentText);
            Assert.IsTrue(religionIndex > referenceIndex && birthIndex > religionIndex, parentText);

            Assert.IsTrue(paragraphTexts.Any(text => text.Contains("1940: Zähl.", StringComparison.Ordinal)));
            Assert.IsTrue(paragraphTexts.Any(text => text.Contains("1960: Zähl.", StringComparison.Ordinal)));
            Assert.IsFalse(paragraphTexts.Any(text => text.Contains("Private Information", StringComparison.Ordinal)));
            Assert.IsFalse(paragraphTexts.Any(text => text.Contains("Geheime Beschreibung", StringComparison.Ordinal)));
            Assert.IsFalse(paragraphTexts.Any(text => text.Contains("1942", StringComparison.Ordinal)));
            Assert.IsFalse(paragraphTexts.Any(text => text.Contains("1943", StringComparison.Ordinal)));
            Assert.IsFalse(paragraphTexts.Any(text => text.Contains("1939", StringComparison.Ordinal)));
            Assert.AreEqual(2, paragraphTexts.Count(text =>
                text.StartsWith("1940: Zähl.", StringComparison.Ordinal)
                || text.StartsWith("1960: Zähl.", StringComparison.Ordinal)));

            var childIndex = Array.FindIndex(paragraphTexts, text =>
                text.StartsWith("1. Kind", StringComparison.Ordinal)
                || text.StartsWith("- Kind", StringComparison.Ordinal));
            var childEventIndex = Array.FindIndex(paragraphTexts, text => text.Contains("1960: Zähl.", StringComparison.Ordinal));
            Assert.IsTrue(childIndex >= 0 && childEventIndex > childIndex,
                string.Join(" | ", paragraphTexts));
            AssertParagraphIndent(paragraphs[childIndex], left: "1080", hanging: "360");
            AssertParagraphIndent(paragraphs[childEventIndex], left: "720", hanging: null);
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }

    private static void AssertParagraphIndent(XElement paragraph, string left, string? hanging)
    {
        XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        var indentation = paragraph.Element(word + "pPr")?.Element(word + "ind");
        Assert.AreEqual(left, (string?)indentation?.Attribute(word + "left"), paragraph.ToString(SaveOptions.DisableFormatting));
        Assert.AreEqual(hanging, (string?)indentation?.Attribute(word + "hanging"), paragraph.ToString(SaveOptions.DisableFormatting));
    }

    [TestMethod]
    public async Task ExportAsync_GroupsFamiliesTogetherNumbersThemSequentiallyAndAddsSubtitleOnlyForMultipleSurnames()
    {
        const string gedcom = "0 HEAD\r\n1 CHAR UTF-8\r\n"
            + "0 @I1@ INDI\r\n1 NAME Anna /Müller/\r\n1 FAMS @F1@\r\n"
            + "0 @I2@ INDI\r\n1 NAME Bernd /Müller/\r\n1 FAMS @F2@\r\n"
            + "0 @I3@ INDI\r\n1 NAME Carla /Meier/\r\n1 FAMS @F3@\r\n"
            + "0 @I4@ INDI\r\n1 NAME Dora /Zeller/\r\n1 FAMS @F4@\r\n"
            + "0 @F1@ FAM\r\n1 HUSB @I1@\r\n1 MARR\r\n"
            + "0 @F2@ FAM\r\n1 HUSB @I2@\r\n1 MARR\r\n"
            + "0 @F3@ FAM\r\n1 HUSB @I3@\r\n1 MARR\r\n"
            + "0 @F4@ FAM\r\n1 HUSB @I4@\r\n1 MARR\r\n"
            + "0 TRLR\r\n";
        var testDirectory = Path.Combine(Path.GetTempPath(), $"ofb-group-export-{Guid.NewGuid():N}");
        Directory.CreateDirectory(testDirectory);
        var inputPath = Path.Combine(testDirectory, "groups.ged");
        var outputPath = Path.Combine(testDirectory, "groups.docx");
        await File.WriteAllTextAsync(inputPath, gedcom, Encoding.UTF8);

        try
        {
            UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
            var documentFactory = Substitute.For<IUserDocumentFactory>();
            documentFactory.CreateDocument(OFBOutputFormat.Docx).Returns(_ => UserDocumentFactory.Create(".docx"));
            var decision = new OFBGroupingDecision
            {
                Order = 1,
                LeftFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F1"),
                RightFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F3"),
                Action = "manualMerge",
                GroupName = "A-Gruppe"
            };

            await new OFBExportService(new GedComDataSource(), documentFactory).ExportAsync(new OFBGenerateOptions
            {
                InputPath = inputPath,
                OutputPath = outputPath,
                Title = "Gruppentest",
                GroupingDecisions = [decision],
                UseDocxFormat = true
            });

            using var archive = ZipFile.OpenRead(outputPath);
            using var documentStream = archive.GetEntry("word/document.xml")!.Open();
            var documentXml = XDocument.Load(documentStream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var paragraphs = documentXml.Descendants(word + "p")
                .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(text => text.Value)))
                .ToArray();
            var groupIndex = Array.IndexOf(paragraphs, "A-Gruppe");
            var subtitleIndex = Array.IndexOf(paragraphs, "Meier, Müller");
            var firstNumberIndex = Array.FindIndex(paragraphs, text => text.Contains("00001 ", StringComparison.Ordinal));
            var secondNumberIndex = Array.FindIndex(paragraphs, text => text.Contains("00002 ", StringComparison.Ordinal));
            var thirdNumberIndex = Array.FindIndex(paragraphs, text => text.Contains("00003 ", StringComparison.Ordinal));
            var standaloneGroupIndex = Array.IndexOf(paragraphs, "Zeller");
            var fourthNumberIndex = Array.FindIndex(paragraphs, text => text.Contains("00004 ", StringComparison.Ordinal));

            Assert.IsTrue(groupIndex >= 0 && subtitleIndex > groupIndex,
                $"group={groupIndex}, subtitle={subtitleIndex}; {string.Join(" | ", paragraphs)}");
            Assert.IsTrue(firstNumberIndex > subtitleIndex && secondNumberIndex > firstNumberIndex
                && thirdNumberIndex > secondNumberIndex && standaloneGroupIndex > thirdNumberIndex
                && fourthNumberIndex > standaloneGroupIndex,
                $"subtitle={subtitleIndex}, numbers={firstNumberIndex}/{secondNumberIndex}/{thirdNumberIndex}/{fourthNumberIndex}, "
                + $"standalone={standaloneGroupIndex}; {string.Join(" | ", paragraphs)}");
            foreach (var familyIndex in new[] { firstNumberIndex, secondNumberIndex, thirdNumberIndex, fourthNumberIndex })
                Assert.IsFalse(string.IsNullOrWhiteSpace(paragraphs[familyIndex - 1]),
                    $"A blank paragraph preceded family entry {paragraphs[familyIndex]}.");
            Assert.IsTrue(paragraphs[fourthNumberIndex].StartsWith("00004 ", StringComparison.Ordinal));
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }

    [TestMethod]
    public async Task ExportAsync_WithDefaultTemplate_WritesGcMarriageAndPersonReferences()
    {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "TestData", "gedcom-5.5.1-synthetic.ged");
        UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
        var documentFactory = Substitute.For<IUserDocumentFactory>();
        documentFactory.CreateDocument(OFBOutputFormat.Docx).Returns(_ => UserDocumentFactory.Create(".docx"));
        var service = new OFBExportService(new GedComDataSource(), documentFactory);
        var outputPath = Path.Combine(Path.GetTempPath(), $"ofb-test-{Guid.NewGuid():N}", "gc-sample.docx");

        try
        {
            await service.ExportAsync(new OFBGenerateOptions
            {
                InputPath = fixturePath,
                OutputPath = outputPath,
                Title = "GC Testbuch",
                UseDocxFormat = true
            });

            using var archive = ZipFile.OpenRead(outputPath);
            using var documentStream = archive.GetEntry("word/document.xml")!.Open();
            var documentXml = XDocument.Load(documentStream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var paragraphTexts = documentXml.Descendants(word + "p")
                .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(text => text.Value)))
                .ToArray();
            Assert.IsTrue(paragraphTexts.Any(text => text.StartsWith("00001 ⚭", StringComparison.Ordinal)));
            Assert.IsTrue(paragraphTexts.Any(text => text.Contains("Muster, Emil", StringComparison.Ordinal)));
            Assert.IsFalse(paragraphTexts.Any(text => text.Contains("<PN=I2>", StringComparison.Ordinal)));
            Assert.IsFalse(paragraphTexts.Any(text => text.Contains("Emil /Muster/", StringComparison.Ordinal)));
            var targets = documentXml.Descendants(word + "hyperlink")
                .Select(element => (string?)element.Attribute(word + "anchor"))
                .Where(target => target is not null)
                .ToArray();
            Assert.IsTrue(targets.Contains("index-person-3"));
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [TestMethod]
    public async Task ExportAsync_AppliesProjectPseudonymToEntriesAndIndexes()
    {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "TestData", "gedcom-5.5.1-synthetic.ged");
        UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
        var documentFactory = Substitute.For<IUserDocumentFactory>();
        documentFactory.CreateDocument(OFBOutputFormat.Docx).Returns(_ => UserDocumentFactory.Create(".docx"));
        var service = new OFBExportService(new GedComDataSource(), documentFactory);
        var outputDirectory = Path.Combine(Path.GetTempPath(), $"ofb-overlay-{Guid.NewGuid():N}");
        var outputPath = Path.Combine(outputDirectory, "pseudonym.docx");

        try
        {
            await service.ExportAsync(new OFBGenerateOptions
            {
                InputPath = fixturePath,
                OutputPath = outputPath,
                Title = "Pseudonym Testbuch",
                PlaceId = "München",
                ExportRules =
                [
                    new OFBExportRule
                    {
                        Order = 1,
                        TargetKind = "person",
                        TargetId = OFBExportRuleTarget.Person("gedcom", "I2"),
                        Action = "replace",
                        Field = "displayName",
                        Value = "Beispielperson"
                    },
                    new OFBExportRule
                    {
                        Order = 2,
                        TargetKind = "family",
                        TargetId = OFBExportRuleTarget.Family("gedcom", "F1"),
                        Action = "redact",
                        Field = "marriagePlace"
                    }
                ]
            });

            using var archive = ZipFile.OpenRead(outputPath);
            using var documentStream = archive.GetEntry("word/document.xml")!.Open();
            var documentXml = XDocument.Load(documentStream);
            var documentText = string.Concat(documentXml.Descendants()
                .Where(element => element.Name.LocalName == "t")
                .Select(element => element.Value));

            Assert.IsTrue(documentText.Contains("Beispielperson", StringComparison.Ordinal));
            Assert.IsFalse(documentText.Contains("Emil", StringComparison.Ordinal));
            Assert.IsFalse(documentText.Contains("Muster", StringComparison.Ordinal));
            Assert.IsTrue(documentText.Contains("00001", StringComparison.Ordinal));
            Assert.IsFalse(documentText.Contains("Ehe: München", StringComparison.Ordinal));
        }
        finally
        {
            if (Directory.Exists(outputDirectory))
                Directory.Delete(outputDirectory, recursive: true);
        }
    }

    [TestMethod]
    public async Task ExportAsync_WithAkTemplate_WritesCompactFamilyEntryWithResolvedNavigation()
    {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "TestData", "gedcom-5.5.1-synthetic.ged");
        UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
        var documentFactory = Substitute.For<IUserDocumentFactory>();
        documentFactory.CreateDocument(OFBOutputFormat.Docx).Returns(_ => UserDocumentFactory.Create(".docx"));
        var service = new OFBExportService(new GedComDataSource(), documentFactory);
        var outputPath = Path.Combine(Path.GetTempPath(), $"ofb-test-{Guid.NewGuid():N}", "ak-sample.docx");

        try
        {
            await service.ExportAsync(new OFBGenerateOptions
            {
                InputPath = fixturePath,
                OutputPath = outputPath,
                Title = "AK Testbuch",
                Template = "ak",
                UseDocxFormat = true
            });

            using var archive = ZipFile.OpenRead(outputPath);
            using var documentStream = archive.GetEntry("word/document.xml")!.Open();
            var documentXml = XDocument.Load(documentStream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var paragraphTexts = documentXml.Descendants(word + "p")
                .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(text => text.Value)))
                .ToArray();
            Assert.IsTrue(paragraphTexts.Any(text => text.StartsWith("⚭ 1 JAN 2000 in München", StringComparison.Ordinal)),
                string.Join(" | ", paragraphTexts.Where(text => text.Contains("⚭", StringComparison.Ordinal))));
            Assert.IsTrue(paragraphTexts.Any(text => text.Contains("Kinder", StringComparison.Ordinal)),
                string.Join(" | ", paragraphTexts.Where(text => text.Contains("Kdr", StringComparison.Ordinal)
                    || text.Contains("Kinder", StringComparison.Ordinal))));
            Assert.IsTrue(paragraphTexts.Any(text => text.Contains("Emil Muster", StringComparison.Ordinal)));

            var bookmarks = documentXml.Descendants(word + "bookmarkStart")
                .Select(element => (string?)element.Attribute(word + "name"))
                .Where(name => name is not null)
                .ToHashSet(StringComparer.Ordinal);
            var targets = documentXml.Descendants(word + "hyperlink")
                .Select(element => (string?)element.Attribute(word + "anchor"))
                .Where(target => target is not null)
                .ToArray();
            Assert.IsTrue(bookmarks.Contains("family-00001"));
            Assert.IsTrue(bookmarks.Contains("person-I2"));
            Assert.IsTrue(targets.Contains("index-person-3"));
            Assert.IsFalse(targets.Any(target => !bookmarks.Contains(target!)));
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [TestMethod]
    [DataRow("gc")]
    [DataRow("ak")]
    public async Task ExportAsync_RendersPreferredInlineAndDatedOccupationEventsWithIndexLinks(string templateName)
    {
        const string gedcom =
            "0 HEAD\r\n1 GEDC\r\n2 VERS 5.5.1\r\n2 FORM LINEAGE-LINKED\r\n1 CHAR UTF-8\r\n" +
            "0 @I1@ INDI\r\n1 NAME Ada /Beispiel/\r\n2 GIVN Ada\r\n2 SURN Beispiel\r\n1 SEX F\r\n" +
            "1 OCCU Ackersmann, Taglöhner\r\n2 PLAC München, Oberbayern, Bayern, Deutschland\r\n1 OCCU Schneiderin und Näherin\r\n2 DATE BET 1920 AND 1930\r\n1 OCCU Lehrerin\r\n2 DATE 1 JAN 1940\r\n" +
            "0 @I2@ INDI\r\n1 NAME Emil /Muster/\r\n1 SEX M\r\n1 OCCU Bauer bei Graf Müller\r\n" +
            "0 @I3@ INDI\r\n1 NAME Carla /Beispiel/\r\n1 FAMS @F2@\r\n1 OCCU Schreiner\r\n2 DATE 1 JAN 1900\r\n1 OCCU Lehrerin\r\n2 DATE 1 JAN 1910\r\n" +
            "0 @I4@ INDI\r\n1 NAME Emil /Beispiel/\r\n1 FAMC @F2@\r\n" +
            "0 @I5@ INDI\r\n1 NAME Entfernt /Leereintrag/\r\n1 FAMS @F3@\r\n" +
            "0 @I6@ INDI\r\n1 NAME Ehe /OhneDatum/\r\n1 FAMS @F4@\r\n" +
            "0 @F1@ FAM\r\n1 HUSB @I2@\r\n1 WIFE @I1@\r\n1 MARR\r\n2 DATE 1 JAN 1940\r\n" +
            "0 @F2@ FAM\r\n1 WIFE @I3@\r\n1 CHIL @I4@\r\n" +
            "0 @F3@ FAM\r\n1 HUSB @I5@\r\n" +
            "0 @F4@ FAM\r\n1 WIFE @I6@\r\n1 MARR\r\n0 TRLR\r\n";
        var testDirectory = Path.Combine(Path.GetTempPath(), $"ofb-occupations-{Guid.NewGuid():N}");
        Directory.CreateDirectory(testDirectory);
        var inputPath = Path.Combine(testDirectory, "occupations.ged");
        var outputPath = Path.Combine(testDirectory, "occupations.docx");
        await File.WriteAllTextAsync(inputPath, gedcom, Encoding.UTF8);

        try
        {
            UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
            var documentFactory = Substitute.For<IUserDocumentFactory>();
            documentFactory.CreateDocument(OFBOutputFormat.Docx).Returns(_ => UserDocumentFactory.Create(".docx"));
            var dataSource = new CanonicalGedcomFamilyDataSource(
                new GedcomInputDriver(),
                new CanonicalGenealogyAdapter());
            var service = new OFBExportService(dataSource, documentFactory);

            await service.ExportAsync(new OFBGenerateOptions
            {
                InputPath = inputPath,
                OutputPath = outputPath,
                Title = "Berufe Testbuch",
                Template = templateName,
                UseDocxFormat = true
            });

            using var archive = ZipFile.OpenRead(outputPath);
            using var documentStream = archive.GetEntry("word/document.xml")!.Open();
            var documentXml = XDocument.Load(documentStream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var paragraphTexts = documentXml.Descendants(word + "p")
                .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(text => text.Value)))
                .ToArray();
            Assert.IsTrue(paragraphTexts.Any(text => text.Contains("BET 1920 AND 1930: Ber., Schneiderin", StringComparison.Ordinal)),
                string.Join(" | ", paragraphTexts.Where(text => text.Contains("Schneiderin", StringComparison.Ordinal)
                    || text.Contains("1920", StringComparison.Ordinal))));
            Assert.IsTrue(paragraphTexts.Any(text => text.Contains("BET 1920 AND 1930: Ber., Näherin", StringComparison.Ordinal)));
            Assert.IsTrue(paragraphTexts.Any(text => text.Contains("1940: Ber., Lehrerin", StringComparison.Ordinal)),
                string.Join(" | ", paragraphTexts.Where(text => text.Contains("Lehrerin", StringComparison.Ordinal)
                    || text.Contains("1940", StringComparison.Ordinal))));
            Assert.IsTrue(paragraphTexts.Any(text => text.Contains("1910: Ber., Lehrerin", StringComparison.Ordinal)));
            Assert.AreEqual(2, paragraphTexts.Count(text => text.Contains("1910", StringComparison.Ordinal)
                && text.Contains("Lehrerin", StringComparison.Ordinal)),
                "When all occupations have dates, the last one must appear both inline and in the dated event list.");
            if (templateName == "gc")
            {
                Assert.IsTrue(paragraphTexts.Any(text => text.Contains("Ackersmann in München", StringComparison.Ordinal)
                    && text.Contains("Taglöhner in München", StringComparison.Ordinal)));
            }
            else
            {
                Assert.IsTrue(paragraphTexts.Any(text => text.Contains("Ackersmann in München", StringComparison.Ordinal)));
                Assert.IsTrue(paragraphTexts.Any(text => text.Contains("Taglöhner in München", StringComparison.Ordinal)));
            }
            Assert.IsTrue(paragraphTexts.Any(text => text.Contains("Bauer bei Graf Müller", StringComparison.Ordinal)),
                string.Join(" | ", paragraphTexts.Where(text => text.Contains("Bauer", StringComparison.OrdinalIgnoreCase))));
            Assert.IsTrue(paragraphTexts.Any(text => text.StartsWith("1. Emil", StringComparison.Ordinal)
                || text.StartsWith("- Emil", StringComparison.Ordinal)));
            Assert.IsTrue(paragraphTexts.Any(text => text.Contains("Ehe", StringComparison.Ordinal)));
            Assert.IsTrue(paragraphTexts.Contains("Berufsindex"));
            Assert.IsFalse(paragraphTexts.Any(text => text.Contains("Entfernt", StringComparison.Ordinal)));

            var bookmarks = documentXml.Descendants(word + "bookmarkStart")
                .Select(element => (string?)element.Attribute(word + "name"))
                .Where(name => name is not null)
                .ToHashSet(StringComparer.Ordinal);
            Assert.AreEqual(3, bookmarks.Count(name => name!.StartsWith("family-", StringComparison.Ordinal)));
            var occupationTargets = documentXml.Descendants(word + "hyperlink")
                .Select(element => (string?)element.Attribute(word + "anchor"))
                .Where(target => target is not null && target.StartsWith("index-occupation-", StringComparison.Ordinal))
                .ToArray();
            Assert.AreEqual(7, bookmarks.Count(name => name!.StartsWith("index-occupation-", StringComparison.Ordinal)));
            Assert.AreEqual(9, occupationTargets.Length);
            Assert.IsTrue(occupationTargets.All(target => bookmarks.Contains(target!)));
            var occupationIndexPosition = Array.IndexOf(paragraphTexts, "Berufsindex");
            var propertyIndexPosition = Array.IndexOf(paragraphTexts, "Eigentums-/Besitz-Index");
            var occupationIndexTexts = paragraphTexts.Skip(occupationIndexPosition + 1)
                .Take(propertyIndexPosition - occupationIndexPosition - 1)
                .ToArray();
            Assert.IsTrue(occupationIndexTexts.Any(text => text.StartsWith("Ackersmann", StringComparison.Ordinal)));
            Assert.IsTrue(occupationIndexTexts.Any(text => text.StartsWith("Taglöhner", StringComparison.Ordinal)));
            Assert.IsFalse(occupationIndexTexts.Any(text => text.Contains("Graf Müller", StringComparison.Ordinal)));
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }

    [TestMethod]
    public async Task ExportAsync_WithExternalTemplate_UsesTemplateSectionsAndPreservesNavigation()
    {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "TestData", "gedcom-5.5.1-synthetic.ged");
        var testDirectory = Path.Combine(Path.GetTempPath(), $"ofb-template-export-{Guid.NewGuid():N}");
        Directory.CreateDirectory(testDirectory);
        var templatePath = Path.Combine(testDirectory, "sections.json");
        var outputPath = Path.Combine(testDirectory, "sections.docx");
        await File.WriteAllTextAsync(templatePath,
            """
            {
              "schemaVersion": 1,
              "id": "sections",
              "entryRoot": "Family",
              "blocks": [
                {
                  "kind": "section",
                  "columns": 2,
                  "blocks": [
                    {
                      "kind": "paragraph",
                      "role": "family-data",
                      "content": [{ "kind": "field", "path": "family.union" }]
                    }
                  ]
                }
              ]
            }
            """);

        try
        {
            UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
            var documentFactory = Substitute.For<IUserDocumentFactory>();
            documentFactory.CreateDocument(OFBOutputFormat.Docx).Returns(_ => UserDocumentFactory.Create(".docx"));
            var service = new OFBExportService(new GedComDataSource(), documentFactory);

            await service.ExportAsync(new OFBGenerateOptions
            {
                InputPath = fixturePath,
                OutputPath = outputPath,
                Title = "Externes Template",
                Template = templatePath,
                UseDocxFormat = true
            });

            using var archive = ZipFile.OpenRead(outputPath);
            using var documentStream = archive.GetEntry("word/document.xml")!.Open();
            var documentXml = XDocument.Load(documentStream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            Assert.IsTrue(documentXml.Descendants(word + "sectPr").Any());
            Assert.IsTrue(documentXml.Descendants(word + "sectPr")
                .Any(section => (string?)section.Element(word + "cols")?.Attribute(word + "num") == "2"));
            var bookmarks = documentXml.Descendants(word + "bookmarkStart")
                .Select(element => (string?)element.Attribute(word + "name"))
                .Where(name => name is not null)
                .ToHashSet(StringComparer.Ordinal);
            Assert.IsTrue(bookmarks.Contains("family-00001"));
            var linkTargets = documentXml.Descendants(word + "hyperlink")
                .Select(element => (string?)element.Attribute(word + "anchor"))
                .Where(target => target is not null)
                .ToArray();
            Assert.IsTrue(linkTargets.Length > 0);
            Assert.IsFalse(linkTargets.Any(target => !bookmarks.Contains(target!)));
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }

    [TestMethod]
    public async Task ExportAsync_WhenInputFileDoesNotExist_ThrowsFileNotFoundException()
    {
        var documentFactory = Substitute.For<IUserDocumentFactory>();
        var service = new OFBExportService(new GedComDataSource(), documentFactory);

        await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => service.ExportAsync(new OFBGenerateOptions
        {
            InputPath = Path.Combine(Path.GetTempPath(), $"missing-{Guid.NewGuid():N}.ged"),
            OutputPath = Path.Combine(Path.GetTempPath(), "sample.docx"),
            Title = "Test"
        }));

        documentFactory.DidNotReceiveWithAnyArgs().CreateDocument(default);
    }
}
