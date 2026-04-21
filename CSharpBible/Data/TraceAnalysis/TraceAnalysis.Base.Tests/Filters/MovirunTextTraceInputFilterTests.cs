using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Text;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Filter.MovirunText.Filters;

namespace TraceAnalysis.Base.Tests.Filters;

[TestClass]
public class MovirunTextTraceInputFilterTests
{
    [TestMethod]
    [DataRow("Zeitstempel(ms)")]
    [DataRow("Zeitstempel(µs)")]
    public void CanHandle_WithKnownHeaderVariants_ReturnsTrue(string timestampHeader)
    {
        var filter = new MovirunTextTraceInputFilter();

        using var stream = CreateStream($"""
MOVIRUN open Editor V3.5 SP20 Patch 4 Trace: Trace
C:\Trace\Sample.project
{timestampHeader} PLC_PRG.lrAGVPos PLC_PRG.lrAGVSpeed
18 714.325 200
""");

        var canHandle = filter.CanHandle(stream, "sample.txt");

        Assert.IsTrue(canHandle);
    }

    [TestMethod]
    [DataRow("Zeitstempel(ms)", 18L, 714.325d, 200L)]
    [DataRow("Zeitstempel(µs)", 28324L, 1098.325d, 200L)]
    public void Read_WithKnownHeaderVariants_ParsesCanonicalRecords(string timestampHeader, long expectedTimestamp, double expectedPosition, long expectedSpeed)
    {
        var filter = new MovirunTextTraceInputFilter();

        using var stream = CreateStream($"""
MOVIRUN open Editor V3.5 SP20 Patch 4 Trace: Trace
C:\Trace\Sample.project
{timestampHeader} PLC_PRG.lrAGVPos PLC_PRG.lrAGVSpeed PLC_PRG.uiTranspID
{expectedTimestamp} {expectedPosition.ToString(System.Globalization.CultureInfo.InvariantCulture)} {expectedSpeed} 1000
{expectedTimestamp + 30} {(expectedPosition + 2).ToString(System.Globalization.CultureInfo.InvariantCulture)} {expectedSpeed} 1001
""");

        var dataSet = filter.Read(stream, new FilterSourceDescriptor("sample.txt", ".txt"));

        Assert.IsEmpty(dataSet.ParseErrors);
        Assert.HasCount(2, dataSet.Records);
        Assert.HasCount(3, dataSet.Metadata.Fields);
        CollectionAssert.AreEqual(
            new[] { "PLC_PRG.lrAGVPos", "PLC_PRG.lrAGVSpeed", "PLC_PRG.uiTranspID" },
            dataSet.Metadata.Fields.Select(f => f.sName).ToArray());

        Assert.AreEqual(expectedTimestamp, dataSet.Records[0].Timestamp);
        Assert.AreEqual(expectedPosition, dataSet.Records[0].Values["PLC_PRG.lrAGVPos"]);
        Assert.AreEqual(expectedSpeed, dataSet.Records[0].Values["PLC_PRG.lrAGVSpeed"]);
        Assert.AreEqual(1000L, dataSet.Records[0].Values["PLC_PRG.uiTranspID"]);
    }

    [TestMethod]
    public void Read_WithMalformedRow_ReturnsPartialResultsAndParseErrors()
    {
        var filter = new MovirunTextTraceInputFilter();

        using var stream = CreateStream("""
MOVIRUN open Editor V3.5 SP20 Patch 4 Trace: Trace
C:\Trace\Sample.project
Zeitstempel(ms) PLC_PRG.lrAGVPos PLC_PRG.lrAGVSpeed
18 714.325 200
48 716.325
79 718.325 200
""");

        var dataSet = filter.Read(stream, new FilterSourceDescriptor("sample.txt", ".txt"));

        Assert.HasCount(2, dataSet.Records);
        Assert.HasCount(1, dataSet.ParseErrors);
        StringAssert.Contains(dataSet.ParseErrors[0], "Line 5");
    }

    private static MemoryStream CreateStream(string text)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }
}
