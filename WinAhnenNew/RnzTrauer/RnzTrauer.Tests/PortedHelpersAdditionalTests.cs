using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core;

namespace RnzTrauer.Tests;

[TestClass]
public sealed class PortedHelpersAdditionalTests
{
    [TestMethod]
    public void Str2Date_Returns_Date_For_Valid_Input()
    {
        var dtResult = PortedHelpers.Str2Date("24.12.2024");

        Assert.AreEqual(new DateOnly(2024, 12, 24), dtResult);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("24-12-2024")]
    [DataRow("24.12")]
    [DataRow("31.02.2024")]
    [DataRow("aa.bb.cccc")]
    public void Str2Date_Returns_Null_For_Invalid_Input(string? sValue)
    {
        Assert.IsNull(PortedHelpers.Str2Date(sValue));
    }

    [TestMethod]
    public void Cond_Returns_Trimmed_String_For_Existing_Key()
    {
        IReadOnlyDictionary<string, object?> dValues = new Dictionary<string, object?>
        {
            ["name"] = " RNZ "
        };

        Assert.AreEqual("RNZ", dValues.Cond("name"));
    }

    [TestMethod]
    public void Cond_Returns_Empty_String_For_Missing_Or_Null_Value()
    {
        IReadOnlyDictionary<string, object?> dValues = new Dictionary<string, object?>
        {
            ["name"] = null
        };

        Assert.AreEqual(string.Empty, dValues.Cond("missing"));
        Assert.AreEqual(string.Empty, dValues.Cond("name"));
    }

    [DataTestMethod]
    [DataRow("", "")]
    [DataRow("a", "A")]
    [DataRow("äPFEL", "Äpfel")]
    [DataRow("rnZ", "Rnz")]
    public void Capitalize_Normalizes_Text(string sValue, string sExpected)
    {
        Assert.AreEqual(sExpected, sValue.Capitalize());
    }

    [TestMethod]
    public void ToJsonNode_Creates_Deep_Clone_For_JsonNode()
    {
        var xOriginal = JsonNode.Parse("""{"name":"RNZ"}""")!;

        var xClone = xOriginal.ToJsonNode();
        xClone!["name"] = "Changed";

        Assert.AreEqual("RNZ", xOriginal["name"]!.ToString());
        Assert.AreEqual("Changed", xClone["name"]!.ToString());
    }

    [TestMethod]
    public void ToJsonNode_Converts_DateOnly_And_ByteArray()
    {
        var xDateNode = new DateOnly(2024, 12, 24).ToJsonNode();
        var xBytesNode = new byte[] { 1, 2, 3 }.ToJsonNode();

        Assert.AreEqual("2024-12-24", xDateNode!.ToString());
        Assert.AreEqual("AQID", xBytesNode!.ToString());
    }

    [TestMethod]
    public void ToJsonNode_Converts_Dictionary_And_Array()
    {
        var xObjectNode = new Dictionary<string, object?>
        {
            ["name"] = "RNZ",
            ["count"] = 2
        }.ToJsonNode();
        var xArrayNode = new object?[] { "RNZ", 2, true }.ToJsonNode();

        Assert.IsInstanceOfType<JsonObject>(xObjectNode);
        Assert.IsInstanceOfType<JsonArray>(xArrayNode);
        Assert.AreEqual("RNZ", xObjectNode!["name"]!.ToString());
        Assert.AreEqual("2", xObjectNode["count"]!.ToString());
        Assert.AreEqual("RNZ", xArrayNode![0]!.ToString());
        Assert.AreEqual("2", xArrayNode[1]!.ToString());
        Assert.AreEqual("true", xArrayNode[2]!.ToString());
    }

    [TestMethod]
    public void ToJsonObject_Converts_Dictionary_Entries()
    {
        IReadOnlyDictionary<string, object?> dValues = new Dictionary<string, object?>
        {
            ["name"] = "RNZ",
            ["items"] = new object?[] { 1, "two" }
        };

        var xObject = dValues.ToJsonObject();

        Assert.AreEqual("RNZ", xObject["name"]!.ToString());
        Assert.IsInstanceOfType<JsonArray>(xObject["items"]);
        Assert.AreEqual("1", xObject["items"]![0]!.ToString());
        Assert.AreEqual("two", xObject["items"]![1]!.ToString());
    }

    [TestMethod]
    public void ToJsonNode_Converts_Primitive_Collections_And_Fallback_Object()
    {
        Assert.IsNull(((object?)null).ToJsonNode());
        Assert.AreEqual("true", ((object)true).ToJsonNode()!.ToString());
        Assert.AreEqual("17", ((object)17).ToJsonNode()!.ToString());
        Assert.AreEqual("18", ((object)18L).ToJsonNode()!.ToString());
        Assert.AreEqual("1.5", ((object)1.5d).ToJsonNode()!.ToString());
        Assert.AreEqual("2.5", ((object)2.5m).ToJsonNode()!.ToString());
        Assert.AreEqual("2024-12-24 08:09:10", new DateTime(2024, 12, 24, 8, 9, 10).ToJsonNode()!.ToString());

        var xStringDictNode = new Dictionary<string, string>
        {
            ["name"] = "RNZ"
        }.ToJsonNode();
        Assert.IsInstanceOfType<JsonObject>(xStringDictNode);
        Assert.AreEqual("RNZ", xStringDictNode!["name"]!.ToString());

        var xDictionaryArrayNode = new List<Dictionary<string, object?>>
        {
            new(StringComparer.Ordinal) { ["count"] = 1 }
        }.ToJsonNode();
        Assert.IsInstanceOfType<JsonArray>(xDictionaryArrayNode);
        Assert.AreEqual("1", xDictionaryArrayNode![0]!["count"]!.ToString());

        var xObjectArrayNode = new List<object?> { 1, "two", false }.ToJsonNode();
        Assert.IsInstanceOfType<JsonArray>(xObjectArrayNode);
        Assert.AreEqual("1", xObjectArrayNode![0]!.ToString());
        Assert.AreEqual("two", xObjectArrayNode[1]!.ToString());
        Assert.AreEqual("false", xObjectArrayNode[2]!.ToString());

        var xFallbackNode = new FallbackSample { Name = "RNZ" }.ToJsonNode();
        Assert.IsInstanceOfType<JsonObject>(xFallbackNode);
        Assert.AreEqual("RNZ", xFallbackNode!["Name"]!.ToString());
    }

    [TestMethod]
    public void PdfText_Returns_Empty_String_For_Invalid_Pdf()
    {
        var sResult = PortedHelpers.PdfText([1, 2, 3, 4]);

        Assert.AreEqual(string.Empty, sResult);
    }

    [TestMethod]
    public void PdfText_Returns_Text_For_Valid_Pdf()
    {
        var arrBytes = CreateMinimalPdf("RNZ");

        var sResult = PortedHelpers.PdfText(arrBytes);

        StringAssert.Contains(sResult, "RNZ");
    }

    [TestMethod]
    public void PdfText_Inserts_LineBreak_When_Font_Changes_Without_CrLf()
    {
        var arrBytes = CreateMinimalPdfWithFontChanges("AB", "Helvetica", "Times-Roman");

        var sResult = PortedHelpers.PdfText(arrBytes);

        StringAssert.Contains(sResult, "A");
        StringAssert.Contains(sResult, "B");
        StringAssert.Contains(sResult, Environment.NewLine);
    }

    private static byte[] CreateMinimalPdf(string sText)
    {
        var arrOffsets = new List<int>();
        var xBuilder = new System.Text.StringBuilder();

        void AppendObject(string sObject)
        {
            arrOffsets.Add(xBuilder.Length);
            xBuilder.Append(sObject);
        }

        var sContent = $"BT /F1 24 Tf 72 100 Td ({sText.Replace("(", "\\(", StringComparison.Ordinal).Replace(")", "\\)", StringComparison.Ordinal)}) Tj ET";

        xBuilder.Append("%PDF-1.4\n");
        AppendObject("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");
        AppendObject("2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n");
        AppendObject("3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 300 144] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>\nendobj\n");
        AppendObject("4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\nendobj\n");
        AppendObject($"5 0 obj\n<< /Length {System.Text.Encoding.ASCII.GetByteCount(sContent)} >>\nstream\n{sContent}\nendstream\nendobj\n");

        var iStartXref = xBuilder.Length;
        xBuilder.Append("xref\n");
        xBuilder.Append("0 6\n");
        xBuilder.Append("0000000000 65535 f \n");
        foreach (var iOffset in arrOffsets)
        {
            xBuilder.AppendLine($"{iOffset:0000000000} 00000 n ");
        }

        xBuilder.Append("trailer\n<< /Size 6 /Root 1 0 R >>\nstartxref\n");
        xBuilder.Append(iStartXref);
        xBuilder.Append("\n%%EOF\n");

        return System.Text.Encoding.ASCII.GetBytes(xBuilder.ToString());
    }

    private static byte[] CreateMinimalPdfWithFontChanges(string sTextA, string sFontA, string sFontB)
    {
        var arrOffsets = new List<int>();
        var xBuilder = new System.Text.StringBuilder();

        void AppendObject(string sObject)
        {
            arrOffsets.Add(xBuilder.Length);
            xBuilder.Append(sObject);
        }

        var sContent = $"BT /F1 24 Tf 72 100 Td ({sTextA}) Tj /F2 24 Tf 100 100 Td (B) Tj ET";

        xBuilder.Append("%PDF-1.4\n");
        AppendObject("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");
        AppendObject("2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n");
        AppendObject("3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 300 144] /Resources << /Font << /F1 4 0 R /F2 6 0 R >> >> /Contents 5 0 R >>\nendobj\n");
        AppendObject($"4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /{sFontA} >>\nendobj\n");
        AppendObject($"5 0 obj\n<< /Length {System.Text.Encoding.ASCII.GetByteCount(sContent)} >>\nstream\n{sContent}\nendstream\nendobj\n");
        AppendObject($"6 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /{sFontB} >>\nendobj\n");

        var iStartXref = xBuilder.Length;
        xBuilder.Append("xref\n");
        xBuilder.Append("0 7\n");
        xBuilder.Append("0000000000 65535 f \n");
        foreach (var iOffset in arrOffsets)
        {
            xBuilder.AppendLine($"{iOffset:0000000000} 00000 n ");
        }

        xBuilder.Append("trailer\n<< /Size 7 /Root 1 0 R >>\nstartxref\n");
        xBuilder.Append(iStartXref);
        xBuilder.Append("\n%%EOF\n");

        return System.Text.Encoding.ASCII.GetBytes(xBuilder.ToString());
    }

    private sealed class FallbackSample
    {
        public string Name { get; set; } = string.Empty;
    }
}
