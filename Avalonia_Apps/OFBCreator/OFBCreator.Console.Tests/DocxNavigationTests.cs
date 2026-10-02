using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using Document.Base.Models.Interfaces;
using Document.Docx;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OFBCreator.Console.Tests;

[TestClass]
public sealed class DocxNavigationTests
{
    [TestMethod]
    public void SaveTo_WritesBookmarksAndInternalHyperlinkTargets()
    {
        var outputPath = Path.Combine(Path.GetTempPath(), $"docx-navigation-{Guid.NewGuid():N}.docx");
        var document = new DocxDocument();
        var familyHeadline = document.AddHeadline(1, "family-0001");
        familyHeadline.TextContent = "Familie 0001";
        var indexParagraph = document.AddParagraph("Normaler Absatz");
        indexParagraph.AddBookmark("index-person-1", DocxFontStyle.Default).TextContent = "Beispiel, Ada";
        indexParagraph.AddLink("#family-0001", DocxFontStyle.UnderlineStyle).TextContent = " — Fam.0001";

        try
        {
            Assert.IsTrue(document.SaveTo(outputPath));
            using var archive = ZipFile.OpenRead(outputPath);
            var documentXmlEntry = archive.GetEntry("word/document.xml");
            Assert.IsNotNull(documentXmlEntry);
            using var stream = documentXmlEntry!.Open();
            var xml = XDocument.Load(stream);
            XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var bookmarkNames = xml.Descendants(word + "bookmarkStart")
                .Select(element => (string?)element.Attribute(word + "name"))
                .ToArray();
            var internalTargets = xml.Descendants(word + "hyperlink")
                .Select(element => (string?)element.Attribute(word + "anchor"))
                .ToArray();

            CollectionAssert.Contains(bookmarkNames, "family-0001");
            CollectionAssert.Contains(bookmarkNames, "index-person-1");
            CollectionAssert.Contains(internalTargets, "family-0001");
        }
        finally
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }
}
