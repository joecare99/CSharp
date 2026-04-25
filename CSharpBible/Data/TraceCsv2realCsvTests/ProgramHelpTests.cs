using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.IO;

namespace TraceCsv2realCsv.Tests;

[TestClass]
public class ProgramHelpTests
{
    [TestMethod]
    [DataRow("de-DE", "Aufruf: TraceCsv2realCsv <Eingabedatei> [Ausgabedatei]")]
    [DataRow("en-US", "Usage: TraceCsv2realCsv <input-file> [output-file]")]
    public void Run_WithoutArguments_WritesLocalizedHelp(string cultureName, string expectedUsageLine)
    {
        using var writer = new StringWriter();

        var result = Program.Run(System.Array.Empty<string>(), writer, new CultureInfo(cultureName));
        var output = writer.ToString();

        Assert.AreEqual(1, result);
        StringAssert.Contains(output, expectedUsageLine);
        StringAssert.Contains(output, "TraceCsv");
        StringAssert.Contains(output, "FlatCsv");
        StringAssert.Contains(output, "JsonTrace");
        StringAssert.Contains(output, "MovirunTextTrace");
        StringAssert.Contains(output, "MovirunTraceXml");
        StringAssert.Contains(output, "CsvOutputFilter");
        StringAssert.Contains(output, "JsonOutputFilter");
    }
}
