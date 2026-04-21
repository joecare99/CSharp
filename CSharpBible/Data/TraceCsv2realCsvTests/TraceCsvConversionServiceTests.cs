using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using TraceAnalysis.Filter.CSV.Model;

namespace TraceCsv2realCsv.Tests
{
    [TestClass]
    public class TraceCsvConversionServiceTests
    {
        [TestMethod]
        public void Convert_WithTraceCsvInput_WritesFlatCsvOutput()
        {
            var service = new TraceCsvConversionService();
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
            var service = new TraceCsvConversionService();
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
            var service = new TraceCsvConversionService();
            var outputModel = ConvertToCsvModel(service, CreateMovirunTraceXmlInput(), "sample.trace");
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
            using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(inputContent));
            using var outputStream = new MemoryStream();
            service.Convert(inputStream, sourceId, outputStream);

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
    }
}
