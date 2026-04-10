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
    public void PdfText_Returns_Empty_String_For_Invalid_Pdf()
    {
        var sResult = PortedHelpers.PdfText([1, 2, 3, 4]);

        Assert.AreEqual(string.Empty, sResult);
    }
}
