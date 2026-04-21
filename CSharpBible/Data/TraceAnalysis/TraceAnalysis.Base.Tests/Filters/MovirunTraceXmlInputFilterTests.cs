using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Text;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Filter.MovirunTrace.Filters;

namespace TraceAnalysis.Base.Tests.Filters;

[TestClass]
public class MovirunTraceXmlInputFilterTests
{
    [TestMethod]
    public void CanHandle_WithKnownTraceStructure_ReturnsTrue()
    {
        var filter = new MovirunTraceXmlInputFilter();

        using var stream = CreateStream(CreateTraceXml(
            microSeconds: false,
            valuesSeriesA: "714.325,716.325",
            timestampsSeriesA: "18,48",
            valuesSeriesB: "200,200",
            timestampsSeriesB: "18,48"));

        var canHandle = filter.CanHandle(stream, "sample.trace");

        Assert.IsTrue(canHandle);
    }

    [TestMethod]
    [DataRow(false, 18L, 714.325d, 200L)]
    [DataRow(true, 28324L, 1098.325d, 200L)]
    public void Read_WithKnownSeries_ParsesCanonicalRecords(bool microSeconds, long expectedTimestamp, double expectedPosition, long expectedSpeed)
    {
        var filter = new MovirunTraceXmlInputFilter();
        var timestampSeries = microSeconds ? "28324,59695" : "18,48";
        var positionSeries = microSeconds ? "1098.325,1100.325" : "714.325,716.325";

        using var stream = CreateStream(CreateTraceXml(
            microSeconds,
            positionSeries,
            timestampSeries,
            "200,200",
            timestampSeries));

        var dataSet = filter.Read(stream, new FilterSourceDescriptor("sample.trace", ".trace"));

        Assert.IsEmpty(dataSet.ParseErrors);
        Assert.HasCount(2, dataSet.Records);
        CollectionAssert.AreEqual(
            new[] { "PLC_PRG.lrAGVPos", "PLC_PRG.lrAGVSpeed" },
            dataSet.Metadata.Fields.Select(f => f.sName).ToArray());

        Assert.AreEqual(expectedTimestamp, dataSet.Records[0].Timestamp);
        Assert.AreEqual(expectedPosition, dataSet.Records[0].Values["PLC_PRG.lrAGVPos"]);
        Assert.AreEqual(expectedSpeed, dataSet.Records[0].Values["PLC_PRG.lrAGVSpeed"]);
    }

    [TestMethod]
    public void Read_WithMismatchedSeries_ReturnsPartialResultsAndParseErrors()
    {
        var filter = new MovirunTraceXmlInputFilter();

        using var stream = CreateStream(CreateTraceXml(
            microSeconds: false,
            valuesSeriesA: "714.325,716.325,718.325",
            timestampsSeriesA: "18,48",
            valuesSeriesB: "200,200",
            timestampsSeriesB: "18,48"));

        var dataSet = filter.Read(stream, new FilterSourceDescriptor("sample.trace", ".trace"));

        Assert.HasCount(2, dataSet.Records);
        Assert.HasCount(1, dataSet.ParseErrors);
        StringAssert.Contains(dataSet.ParseErrors[0], "PLC_PRG.lrAGVPos");
    }

    [TestMethod]
    public void Read_WithDifferentTimestampSets_MergesSeriesByTimestamp()
    {
        var filter = new MovirunTraceXmlInputFilter();

        using var stream = CreateStream(CreateTraceXml(
            microSeconds: false,
            valuesSeriesA: "714.325,716.325",
            timestampsSeriesA: "18,79",
            valuesSeriesB: "200,220",
            timestampsSeriesB: "18,48"));

        var dataSet = filter.Read(stream, new FilterSourceDescriptor("sample.trace", ".trace"));

        Assert.HasCount(3, dataSet.Records);
        Assert.AreEqual(18L, dataSet.Records[0].Timestamp);
        Assert.AreEqual(48L, dataSet.Records[1].Timestamp);
        Assert.AreEqual(79L, dataSet.Records[2].Timestamp);
        Assert.AreEqual(200L, dataSet.Records[0].Values["PLC_PRG.lrAGVSpeed"]);
        Assert.AreEqual(220L, dataSet.Records[1].Values["PLC_PRG.lrAGVSpeed"]);
        Assert.AreEqual(716.325d, dataSet.Records[2].Values["PLC_PRG.lrAGVPos"]);
    }

    private static MemoryStream CreateStream(string text)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }

    private static string CreateTraceXml(
        bool microSeconds,
        string valuesSeriesA,
        string timestampsSeriesA,
        string valuesSeriesB,
        string timestampsSeriesB)
    {
        var microSecondsText = microSeconds ? "True" : "False";
        return $"""
<Trace>
  <TraceConfiguration>
    <Single>
      <Single Name="Record">
        <Single Name="MicroSeconds" Type="bool">{microSecondsText}</Single>
      </Single>
    </Single>
  </TraceConfiguration>
  <TraceData Version="1.0.0.0">
    <TraceRecord>
      <TraceVariable VarName="PLC_PRG.lrAGVPos" Type="System.Double">
        <Values>{valuesSeriesA}</Values>
        <Timestamps>{timestampsSeriesA}</Timestamps>
      </TraceVariable>
      <TraceVariable VarName="PLC_PRG.lrAGVSpeed" Type="System.Double">
        <Values>{valuesSeriesB}</Values>
        <Timestamps>{timestampsSeriesB}</Timestamps>
      </TraceVariable>
    </TraceRecord>
  </TraceData>
</Trace>
""";
    }
}
