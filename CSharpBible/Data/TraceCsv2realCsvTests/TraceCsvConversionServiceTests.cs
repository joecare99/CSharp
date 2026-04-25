using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Filter.CSV.Filters;
using TraceAnalysis.Filter.CSV.Model;
using TraceAnalysis.Filter.JSON.Filters;
using TraceAnalysis.Filter.JSON.Model;
using TraceAnalysis.Filter.MovirunText.Filters;
using TraceAnalysis.Filter.MovirunTrace.Filters;

namespace TraceCsv2realCsv.Tests;

[TestClass]
public class TraceCsvConversionServiceTests
{
    [TestMethod]
    public void Convert_WithTraceCsvInput_WritesFlatCsvOutput()
    {
        var service = new TraceCsvConversionService(
            new InputFilterSelector(),
            CreateInputFilters(),
            CreateOutputFilters());
        var outputModel = ConvertToCsvModel(service, CreateTraceCsvInput(), "sample.trace.csv");
        var timestampColumn = GetColumnName(outputModel, "TimeBase");
        var positionColumn = GetColumnName(outputModel, "PLC_PRG.lrAGVPos");
        var speedColumn = GetColumnName(outputModel, "PLC_PRG.lrAGVSpeed");

        Assert.AreEqual(2, outputModel.Rows.Count);
        Assert.AreEqual(18d, ToDouble(outputModel.Rows[0][timestampColumn]));
        Assert.AreEqual(714.325d, ToDouble(outputModel.Rows[0][positionColumn]));
        Assert.AreEqual(200d, ToDouble(outputModel.Rows[0][speedColumn]));
    }

    [TestMethod]
    public void Convert_WithMovirunTextInput_WritesFlatCsvOutput()
    {
        var service = new TraceCsvConversionService(
            new InputFilterSelector(),
            CreateInputFilters(),
            CreateOutputFilters());
        var outputModel = ConvertToCsvModel(service, CreateMovirunTextInput(), "sample.txt");
        var timestampColumn = GetColumnName(outputModel, "TimeBase");
        var positionColumn = GetColumnName(outputModel, "PLC_PRG.lrAGVPos");
        var speedColumn = GetColumnName(outputModel, "PLC_PRG.lrAGVSpeed");

        Assert.AreEqual(2, outputModel.Rows.Count);
        Assert.AreEqual(18d, ToDouble(outputModel.Rows[0][timestampColumn]));
        Assert.AreEqual(714.325d, ToDouble(outputModel.Rows[0][positionColumn]));
        Assert.AreEqual(200d, ToDouble(outputModel.Rows[0][speedColumn]));
    }

    [TestMethod]
    public void Convert_WithMovirunTraceXmlInput_WritesFlatCsvOutput()
    {
        var service = new TraceCsvConversionService(
            new InputFilterSelector(),
            CreateInputFilters(),
            CreateOutputFilters());
        var outputModel = ConvertToCsvModel(service, CreateMovirunTraceXmlInput(), "sample.trace");
        var timestampColumn = GetColumnName(outputModel, "TimeBase");
        var positionColumn = GetColumnName(outputModel, "PLC_PRG.lrAGVPos");
        var speedColumn = GetColumnName(outputModel, "PLC_PRG.lrAGVSpeed");

        Assert.AreEqual(2, outputModel.Rows.Count);
        Assert.AreEqual(18d, ToDouble(outputModel.Rows[0][timestampColumn]));
        Assert.AreEqual(714.325d, ToDouble(outputModel.Rows[0][positionColumn]));
        Assert.AreEqual(200d, ToDouble(outputModel.Rows[0][speedColumn]));
    }

    [TestMethod]
    public void Convert_WithTraceCsvInputAndJsonTarget_WritesJsonOutput()
    {
        var service = new TraceCsvConversionService(
            new InputFilterSelector(),
            CreateInputFilters(),
            CreateOutputFilters());

        using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(CreateTraceCsvInput()));
        using var outputStream = new MemoryStream();
        service.Convert(inputStream, "sample.trace.csv", outputStream, "sample.json");

        outputStream.Position = 0;
        var jsonModel = JsonTraceModel.Read(outputStream);

        Assert.AreEqual(JsonTraceModel.PayloadFormat, jsonModel.Format);
        Assert.AreEqual("sample.trace.csv", jsonModel.SourceId);
        Assert.AreEqual(2, jsonModel.Records.Count);
        Assert.AreEqual("PLC_PRG.lrAGVPos", jsonModel.Fields[0].Name);
        Assert.AreEqual("double", jsonModel.Records[0].Values[0].Value?.Kind);
    }

    [TestMethod]
    public void Convert_WithJsonInput_WritesFlatCsvOutput()
    {
        var service = new TraceCsvConversionService(
            new InputFilterSelector(),
            CreateInputFilters(),
            CreateOutputFilters());

        var outputModel = ConvertToCsvModel(service, CreateJsonTraceInput(), "sample.json", "sample.csv");
        var timestampColumn = GetColumnName(outputModel, "TimeBase");
        var positionColumn = GetColumnName(outputModel, "PLC_PRG.lrAGVPos");
        var speedColumn = GetColumnName(outputModel, "PLC_PRG.lrAGVSpeed");

        Assert.AreEqual(2, outputModel.Rows.Count);
        Assert.AreEqual(18d, ToDouble(outputModel.Rows[0][timestampColumn]));
        Assert.AreEqual(714.325d, ToDouble(outputModel.Rows[0][positionColumn]));
        Assert.AreEqual(200d, ToDouble(outputModel.Rows[0][speedColumn]));
    }

    private static CsvModel ConvertToCsvModel(TraceCsvConversionService service, string inputContent, string sourceId)
    {
        return ConvertToCsvModel(service, inputContent, sourceId, "output.csv");
    }

    private static CsvModel ConvertToCsvModel(TraceCsvConversionService service, string inputContent, string sourceId, string outputId)
    {
        using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(inputContent));
        using var outputStream = new MemoryStream();
        service.Convert(inputStream, sourceId, outputStream, outputId);

        var outputBytes = outputStream.ToArray();
        using var resultStream = new MemoryStream(outputBytes);
        var outputModel = new CsvModel();
        outputModel.ReadCsv(resultStream);
        return outputModel;
    }

    private static string GetColumnName(CsvModel csvModel, string expectedLogicalName)
    {
        foreach (var header in csvModel.Header)
        {
            var normalizedHeader = header.name.Trim('"');
            if (string.Equals(normalizedHeader, expectedLogicalName, StringComparison.Ordinal))
                return header.name;
        }

        Assert.Fail($"Column '{expectedLogicalName}' was not found in the CSV output header.");
        return string.Empty;
    }

    private static double ToDouble(object value)
    {
        return System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
    }

    private static IReadOnlyList<IAnalyzableInputFilter> CreateInputFilters()
    {
        return
        [
            new TraceCsvInputFilter(),
            new FlatCsvInputFilter(),
            new JsonInputFilter(),
            new MovirunTextTraceInputFilter(),
            new MovirunTraceXmlInputFilter()
        ];
    }

    private static IReadOnlyList<IOutputFilter> CreateOutputFilters()
    {
        return
        [
            new CsvOutputFilter(),
            new JsonOutputFilter()
        ];
    }

    private static string CreateTraceCsvInput()
    {
        return string.Join(
            Environment.NewLine,
            new[]
            {
                "[key]; [value]",
                "Version; 0x03050000",
                "0.Variable; PLC_PRG.lrAGVPos",
                "0.Data; ",
                "; 18; 714.325",
                "; 48; 716.325",
                "1.Variable; PLC_PRG.lrAGVSpeed",
                "1.Data; ",
                "; 18; 200",
                "; 48; 200",
                string.Empty
            });
    }

    private static string CreateMovirunTextInput()
    {
        return string.Join(
            Environment.NewLine,
            new[]
            {
                "MOVIRUN open Editor V3.5 SP20 Patch 4 Trace: Trace",
                "C:\\Trace\\Sample.project",
                "Zeitstempel(ms) PLC_PRG.lrAGVPos PLC_PRG.lrAGVSpeed",
                $"18 {714.325d.ToString(CultureInfo.InvariantCulture)} 200",
                $"48 {716.325d.ToString(CultureInfo.InvariantCulture)} 200",
                string.Empty
            });
    }

    private static string CreateMovirunTraceXmlInput()
    {
        return """
<Trace>
  <TraceData Version="1.0.0.0">
    <TraceRecord>
      <TraceVariable VarName="PLC_PRG.lrAGVPos" Type="System.Double">
        <Values>714.325,716.325</Values>
        <Timestamps>18,48</Timestamps>
      </TraceVariable>
      <TraceVariable VarName="PLC_PRG.lrAGVSpeed" Type="System.Double">
        <Values>200,200</Values>
        <Timestamps>18,48</Timestamps>
      </TraceVariable>
    </TraceRecord>
  </TraceData>
</Trace>
""";
    }

    private static string CreateJsonTraceInput()
    {
        return """
{"format":"TraceAnalysis.JsonTrace/1.0","sourceId":"sample.trace.csv","fields":[{"name":"PLC_PRG.lrAGVPos","type":"System.Double, System.Private.CoreLib"},{"name":"PLC_PRG.lrAGVSpeed","type":"System.Double, System.Private.CoreLib"}],"records":[{"timestamp":{"kind":"double","value":"18"},"values":[{"name":"PLC_PRG.lrAGVPos","value":{"kind":"double","value":"714.325"}},{"name":"PLC_PRG.lrAGVSpeed","value":{"kind":"double","value":"200"}}]},{"timestamp":{"kind":"double","value":"48"},"values":[{"name":"PLC_PRG.lrAGVPos","value":{"kind":"double","value":"716.325"}},{"name":"PLC_PRG.lrAGVSpeed","value":{"kind":"double","value":"200"}}]}],"parseErrors":[]}
""";
    }
}
