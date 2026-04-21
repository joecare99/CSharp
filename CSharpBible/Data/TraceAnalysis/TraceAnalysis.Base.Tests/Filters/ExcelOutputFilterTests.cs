using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using TraceAnalysis.Base.Models;
using TraceAnalysis.Base.Models.Interfaces;
using TraceAnalysis.Filter.Excel.Filters;

namespace TraceAnalysis.Base.Tests.Filters;

[TestClass]
public class ExcelOutputFilterTests
{
    [TestMethod]
    public void Write_ProducesDataWorksheet_WithTimestampAndStableLexicalColumns()
    {
        var dataSet = CreateDataSet();
        var filter = new ExcelOutputFilter(_includeMetadataWorksheetWhenAvailable: false);

        using var stream = new MemoryStream();
        filter.Write(dataSet, stream);

        using var archive = OpenArchive(stream);
        Assert.IsNotNull(archive.GetEntry("xl/worksheets/sheet1.xml"));
        Assert.IsNull(archive.GetEntry("xl/worksheets/sheet2.xml"));

        var dataSheet = ReadXml(archive, "xl/worksheets/sheet1.xml");
        var rows = ReadRows(dataSheet);

        CollectionAssert.AreEqual(new[] { "timestamp", "a", "b", "z" }, rows[0]);
        CollectionAssert.AreEqual(new[] { "2024-02-01T12:00:00.0000000Z", "1", "", "9" }, rows[1]);
        CollectionAssert.AreEqual(new[] { "2024-02-01T12:01:00.0000000Z", "", "7", "" }, rows[2]);
    }

    [TestMethod]
    public void Write_WithMetadataEnabled_WritesMetadataWorksheet()
    {
        var dataSet = CreateDataSet();
        var filter = new ExcelOutputFilter(_includeMetadataWorksheetWhenAvailable: true);

        using var stream = new MemoryStream();
        filter.Write(dataSet, stream);

        using var archive = OpenArchive(stream);
        Assert.IsNotNull(archive.GetEntry("xl/worksheets/sheet1.xml"));
        Assert.IsNotNull(archive.GetEntry("xl/worksheets/sheet2.xml"));

        var metadataSheet = ReadXml(archive, "xl/worksheets/sheet2.xml");
        var rows = ReadRows(metadataSheet);

        CollectionAssert.AreEqual(new[] { "timestamp", "a", "b", "z" }, rows[0]);
        CollectionAssert.AreEqual(new[] { "", "System.Int32", "f2", "" }, rows[1]);
        CollectionAssert.AreEqual(new[] { "", "grpA", "grpB", "" }, rows[2]);
    }

    [TestMethod]
    public void Write_WithMandatoryMetadata_WritesMetadataWorksheetEvenWhenOptionalFlagDisabled()
    {
        var dataSet = CreateDataSet();
        var filter = new ExcelOutputFilter(
            _includeMetadataWorksheetWhenAvailable: false,
            _metadataWorksheetRequired: true,
            _metadataWorksheetName: "Header");

        using var stream = new MemoryStream();
        filter.Write(dataSet, stream);

        using var archive = OpenArchive(stream);
        Assert.IsNotNull(archive.GetEntry("xl/worksheets/sheet1.xml"));
        Assert.IsNotNull(archive.GetEntry("xl/worksheets/sheet2.xml"));
    }

    private static ZipArchive OpenArchive(MemoryStream stream)
    {
        stream.Position = 0;
        return new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false);
    }

    private static TraceDataSet CreateDataSet()
    {
        IReadOnlyList<ITraceFieldMetadata> fields =
        [
            new TraceFieldMetadata("z"),
            new TraceFieldMetadata("a", typeof(int), "grpA"),
            new TraceFieldMetadata("b", typeof(double), "grpB", "f2")
        ];

        IReadOnlyList<ITraceRecord> records =
        [
            new TraceRecord(
                new DateTime(2024, 2, 1, 12, 0, 0, DateTimeKind.Utc),
                new Dictionary<string, object?>
                {
                    ["z"] = 9,
                    ["a"] = 1
                }),
            new TraceRecord(
                new DateTime(2024, 2, 1, 12, 1, 0, DateTimeKind.Utc),
                new Dictionary<string, object?>
                {
                    ["b"] = 7
                })
        ];

        return new TraceDataSet(new TraceMetadata("source.csv", fields), records);
    }

    private static XDocument ReadXml(ZipArchive archive, string entryPath)
    {
        var entry = archive.GetEntry(entryPath);
        Assert.IsNotNull(entry, $"Entry '{entryPath}' was not found.");

        using var entryStream = entry.Open();
        return XDocument.Load(entryStream);
    }

    private static List<string[]> ReadRows(XDocument worksheetXml)
    {
        XNamespace namespaceMain = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        return worksheetXml
            .Descendants(namespaceMain + "row")
            .Select(row => row
                .Elements(namespaceMain + "c")
                .Select(cell => cell
                    .Element(namespaceMain + "is")?
                    .Element(namespaceMain + "t")?
                    .Value ?? string.Empty)
                .ToArray())
            .ToList();
    }
}
