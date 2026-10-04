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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Core.Models;
using OFBCreator.Core.Services;
using OFBCreator.Console.Services;
using OFBCreator.Projects.Models;

namespace OFBCreator.Console.Tests;

[TestClass]
public sealed class ConsoleExportServiceTests
{
    [TestMethod]
    public async Task ExportAsync_WithSyntheticGedcom_WritesOrderedDocxWithResolvedNavigation()
    {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "TestData", "gedcom-5.5.1-synthetic.ged");
        Assert.IsTrue(File.Exists(fixturePath), $"Synthetic GEDCOM fixture was not found: {fixturePath}");

        UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
        var documentFactory = Substitute.For<IUserDocumentFactory>();
        documentFactory.CreateDocument(OFBOutputFormat.Docx).Returns(_ => UserDocumentFactory.Create(".docx"));
        var service = new ConsoleExportService(new GedComDataSource(), documentFactory);
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
        var familyNameIndex = Array.FindIndex(paragraphTexts, groupIndex + 1, text => text == "Beispiel");
        var familyIndex = Array.FindIndex(paragraphTexts, text => text == "00001");
        var personIndex = Array.FindIndex(paragraphTexts, text => text == "Personenindex");
        Assert.IsTrue(prefaceIndex >= 0 && groupIndex > prefaceIndex && familyNameIndex > groupIndex && familyIndex > familyNameIndex && personIndex > familyIndex);

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
        documentStream.Dispose();
        archive.Dispose();
        File.Delete(outputPath);
    }

    [TestMethod]
    public async Task ExportAsync_WithDefaultTemplate_WritesGcMarriageAndPersonReferences()
    {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "TestData", "gedcom-5.5.1-synthetic.ged");
        UserDocumentFactory.ScanAssemblies(new[] { typeof(DocxDocument).Assembly });
        var documentFactory = Substitute.For<IUserDocumentFactory>();
        documentFactory.CreateDocument(OFBOutputFormat.Docx).Returns(_ => UserDocumentFactory.Create(".docx"));
        var service = new ConsoleExportService(new GedComDataSource(), documentFactory);
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
            Assert.IsTrue(paragraphTexts.Any(text => text == "Ehe: München"));
            Assert.IsTrue(paragraphTexts.Any(text => text.Contains("Muster, Emil", StringComparison.Ordinal)));
            Assert.IsTrue(paragraphTexts.Any(text => text.StartsWith("PN = I2", StringComparison.Ordinal)));
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
        var service = new ConsoleExportService(new GedComDataSource(), documentFactory);
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
        var service = new ConsoleExportService(new GedComDataSource(), documentFactory);
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
            Assert.IsTrue(paragraphTexts.Any(text => text.StartsWith("⚭ München", StringComparison.Ordinal)));
            Assert.IsTrue(paragraphTexts.Contains("1 Kdr:"));
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
    public async Task ExportAsync_RendersEveryDatedAndUndatedOccupationWithIndexLinks()
    {
        const string gedcom =
            "0 HEAD\r\n1 GEDC\r\n2 VERS 5.5.1\r\n2 FORM LINEAGE-LINKED\r\n1 CHAR UTF-8\r\n" +
            "0 @I1@ INDI\r\n1 NAME Ada /Beispiel/\r\n2 GIVN Ada\r\n2 SURN Beispiel\r\n1 SEX F\r\n" +
            "1 OCCU Schneiderin\r\n2 DATE BET 1920 AND 1930\r\n1 OCCU Hebamme\r\n" +
            "0 @I2@ INDI\r\n1 NAME Emil /Muster/\r\n1 SEX M\r\n1 OCCU Bauer\r\n" +
            "0 @F1@ FAM\r\n1 HUSB @I2@\r\n1 WIFE @I1@\r\n1 MARR\r\n2 DATE 1 JAN 1940\r\n0 TRLR\r\n";
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
            var service = new ConsoleExportService(dataSource, documentFactory);

            await service.ExportAsync(new OFBGenerateOptions
            {
                InputPath = inputPath,
                OutputPath = outputPath,
                Title = "Berufe Testbuch",
                UseDocxFormat = true
            });

            using var archive = ZipFile.OpenRead(outputPath);
            using var documentStream = archive.GetEntry("word/document.xml")!.Open();
            var documentXml = XDocument.Load(documentStream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var paragraphTexts = documentXml.Descendants(word + "p")
                .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(text => text.Value)))
                .ToArray();
            Assert.IsTrue(paragraphTexts.Contains("BET 1920 AND 1930: Schneiderin"));
            Assert.IsTrue(paragraphTexts.Contains("Hebamme"));
            Assert.IsTrue(paragraphTexts.Contains("Bauer"));
            Assert.IsTrue(paragraphTexts.Contains("Berufsindex"));

            var bookmarks = documentXml.Descendants(word + "bookmarkStart")
                .Select(element => (string?)element.Attribute(word + "name"))
                .Where(name => name is not null)
                .ToHashSet(StringComparer.Ordinal);
            var occupationTargets = documentXml.Descendants(word + "hyperlink")
                .Select(element => (string?)element.Attribute(word + "anchor"))
                .Where(target => target is not null && target.StartsWith("index-occupation-", StringComparison.Ordinal))
                .ToArray();
            Assert.IsTrue(bookmarks.Contains("index-occupation-1"));
            Assert.IsTrue(bookmarks.Contains("index-occupation-2"));
            Assert.AreEqual(3, occupationTargets.Length);
            Assert.IsTrue(occupationTargets.All(target => bookmarks.Contains(target!)));
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
            var service = new ConsoleExportService(new GedComDataSource(), documentFactory);

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
        var service = new ConsoleExportService(new GedComDataSource(), documentFactory);

        await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => service.ExportAsync(new OFBGenerateOptions
        {
            InputPath = Path.Combine(Path.GetTempPath(), $"missing-{Guid.NewGuid():N}.ged"),
            OutputPath = Path.Combine(Path.GetTempPath(), "sample.docx"),
            Title = "Test"
        }));

        documentFactory.DidNotReceiveWithAnyArgs().CreateDocument(default);
    }
}
