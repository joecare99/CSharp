using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Xml.Linq;
using Document.Base.Models.Interfaces;
using Document.Base.Factories;
using Document.Docx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OFBCreator.Abstractions.Interfaces;
using OFBCreator.Core.Models;
using OFBCreator.Core.Services;

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
    public async Task ExportAsync_WithDefaultEntryFormat_WritesGcMarriageAndPersonReferences()
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
    public async Task ExportAsync_WithAkEntryFormat_WritesCompactFamilyEntryWithResolvedNavigation()
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
                EntryFormat = OFBEntryFormat.AK,
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
