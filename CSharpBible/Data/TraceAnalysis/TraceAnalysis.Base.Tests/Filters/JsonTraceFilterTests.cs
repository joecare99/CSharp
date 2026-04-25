using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Base.Models;
using TraceAnalysis.Base.Models.Interfaces;
using TraceAnalysis.Filter.JSON.Filters;

namespace TraceAnalysis.Base.Tests.Filters;

[TestClass]
public class JsonTraceFilterTests
{
    [TestMethod]
    public void CanHandle_WithKnownJsonPayload_ReturnsTrue()
    {
        var filter = new JsonInputFilter();

        using var stream = CreateStream("""
{"format":"TraceAnalysis.JsonTrace/1.0","sourceId":"cache.csv","fields":[],"records":[],"parseErrors":[]}
""");

        var canHandle = filter.CanHandle(stream, "cache.json");

        Assert.IsTrue(canHandle);
    }

    [TestMethod]
    public void Read_WithKnownJsonPayload_ParsesCanonicalRecords()
    {
        var filter = new JsonInputFilter();

        using var stream = CreateStream("""
{"format":"TraceAnalysis.JsonTrace/1.0","sourceId":"cache.csv","fields":[{"name":"Speed","type":"System.Double, System.Private.CoreLib","group":"Drive","format":"f2"}],"records":[{"timestamp":{"kind":"datetime","value":"2024-02-01T12:00:00.0000000Z"},"values":[{"name":"Speed","value":{"kind":"double","value":"7.5"}}]}],"parseErrors":["warning"]}
""");

        var dataSet = filter.Read(stream, new FilterSourceDescriptor("cache.json", ".json"));

        Assert.AreEqual("cache.csv", dataSet.Metadata.sSourceId);
        Assert.HasCount(1, dataSet.Metadata.Fields);
        Assert.AreEqual("Speed", dataSet.Metadata.Fields[0].sName);
        Assert.AreEqual("Drive", dataSet.Metadata.Fields[0].sGroup);
        Assert.AreEqual("f2", dataSet.Metadata.Fields[0].sFormat);
        Assert.HasCount(1, dataSet.Records);
        Assert.AreEqual(new DateTime(2024, 2, 1, 12, 0, 0, DateTimeKind.Utc), dataSet.Records[0].Timestamp);
        Assert.AreEqual(7.5d, dataSet.Records[0].Values["Speed"]);
        CollectionAssert.AreEqual(new[] { "warning" }, dataSet.ParseErrors.ToArray());
    }

    [TestMethod]
    public void Write_RoundTripsMetadataRecordsAndErrors()
    {
        var dataSet = CreateDataSet();
        var outputFilter = new JsonOutputFilter();
        var inputFilter = new JsonInputFilter();

        using var stream = new MemoryStream();
        outputFilter.Write(dataSet, stream);
        stream.Position = 0;
        var roundtrip = inputFilter.Read(stream, new FilterSourceDescriptor("cache.json", ".json"));

        Assert.AreEqual(dataSet.Metadata.sSourceId, roundtrip.Metadata.sSourceId);
        Assert.AreEqual(dataSet.Metadata.Fields.Count, roundtrip.Metadata.Fields.Count);
        Assert.AreEqual(dataSet.Records.Count, roundtrip.Records.Count);
        Assert.AreEqual(dataSet.ParseErrors.Count, roundtrip.ParseErrors.Count);
        Assert.AreEqual(dataSet.Metadata.Fields[0].sName, roundtrip.Metadata.Fields[0].sName);
        Assert.AreEqual(dataSet.Metadata.Fields[0].sGroup, roundtrip.Metadata.Fields[0].sGroup);
        Assert.AreEqual(dataSet.Metadata.Fields[0].sFormat, roundtrip.Metadata.Fields[0].sFormat);
        Assert.AreEqual(dataSet.Records[0].Timestamp, roundtrip.Records[0].Timestamp);
        Assert.AreEqual(dataSet.Records[0].Values["Speed"], roundtrip.Records[0].Values["Speed"]);
        Assert.IsTrue(roundtrip.Records[1].Values.ContainsKey("State"));
        Assert.IsNull(roundtrip.Records[1].Values["State"]);
    }

    [TestMethod]
    public void Read_WithInvalidPayload_ReturnsParseError()
    {
        var filter = new JsonInputFilter();

        using var stream = CreateStream("{" + "\"format\":\"Other\"}");
        var dataSet = filter.Read(stream, new FilterSourceDescriptor("cache.json", ".json"));

        Assert.HasCount(1, dataSet.ParseErrors);
        StringAssert.Contains(dataSet.ParseErrors[0], "Failed to read trace JSON");
        Assert.HasCount(0, dataSet.Records);
    }

    private static MemoryStream CreateStream(string text)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }

    private static TraceDataSet CreateDataSet()
    {
        IReadOnlyList<ITraceFieldMetadata> fields =
        [
            new TraceFieldMetadata("Speed", typeof(double), "Drive", "f2"),
            new TraceFieldMetadata("State", typeof(string))
        ];

        IReadOnlyList<ITraceRecord> records =
        [
            new TraceRecord(
                new DateTime(2024, 2, 1, 12, 0, 0, DateTimeKind.Utc),
                new Dictionary<string, object?>
                {
                    ["Speed"] = 7.5d,
                    ["State"] = "Run"
                }),
            new TraceRecord(
                2L,
                new Dictionary<string, object?>
                {
                    ["Speed"] = 8.25d,
                    ["State"] = null
                })
        ];

        return new TraceDataSet(new TraceMetadata("source.csv", fields), records, new[] { "warning" });
    }
}
