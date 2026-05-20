using System.Collections.Generic;
using FBParser;
using FBParser.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FBParserTests;

[TestClass]
public sealed class GenealogicalNameTokenBuilderTests
{
    [TestMethod]
    public void BuildNameToken_AppendsRegularLetters()
    {
        var warnings = new List<string>();
        var sut = new GenealogicalNameTokenBuilder(CreateConfiguration(warnings));
        var offset = 1;
        var charCount = 0;
        var subString = string.Empty;

        var result = sut.BuildNameToken("Anna", ref offset, ref charCount, ref subString, out var additional);

        Assert.IsTrue(result);
        Assert.AreEqual("A", subString);
        Assert.AreEqual(1, charCount);
        Assert.AreEqual(string.Empty, additional);
        Assert.AreEqual(0, warnings.Count);
    }

    [TestMethod]
    public void BuildNameToken_ParsesAdditionalFragment()
    {
        var warnings = new List<string>();
        var sut = new GenealogicalNameTokenBuilder(CreateConfiguration(warnings));
        var offset = 1;
        var charCount = 0;
        var subString = string.Empty;

        var result = sut.BuildNameToken("(Zw)", ref offset, ref charCount, ref subString, out var additional);

        Assert.IsTrue(result);
        Assert.AreEqual("Zw", additional);
        Assert.AreEqual(string.Empty, subString);
        Assert.AreEqual(4, offset);
        Assert.AreEqual(0, warnings.Count);
    }

    [TestMethod]
    public void BuildNameToken_AppendsProtectSpaceMarkerAndAdvancesOffset()
    {
        var warnings = new List<string>();
        var sut = new GenealogicalNameTokenBuilder(CreateConfiguration(warnings));
        var offset = 1;
        var charCount = 2;
        var subString = string.Empty;

        var result = sut.BuildNameToken(" Anna", ref offset, ref charCount, ref subString, out var additional);

        Assert.IsTrue(result);
        Assert.AreEqual(" ", subString);
        Assert.AreEqual(0, charCount);
        Assert.AreEqual(1, offset);
        Assert.AreEqual(string.Empty, additional);
        Assert.AreEqual(0, warnings.Count);
    }

    [TestMethod]
    public void BuildNameToken_PreservesUppercaseHyphenatedName()
    {
        var warnings = new List<string>();
        var sut = new GenealogicalNameTokenBuilder(CreateConfiguration(warnings));
        var offset = 4;
        var charCount = 3;
        var subString = "Eva";

        var result = sut.BuildNameToken("Eva-Maria", ref offset, ref charCount, ref subString, out var additional);

        Assert.IsTrue(result);
        Assert.AreEqual("Eva-", subString);
        Assert.AreEqual(0, charCount);
        Assert.AreEqual(string.Empty, additional);
        Assert.AreEqual(0, warnings.Count);
    }

    [TestMethod]
    public void BuildNameToken_IgnoresSoftHyphenBetweenLowerCaseLetters()
    {
        var warnings = new List<string>();
        var sut = new GenealogicalNameTokenBuilder(CreateConfiguration(warnings));
        var offset = 3;
        var charCount = 2;
        var subString = "an";

        var result = sut.BuildNameToken("an­na", ref offset, ref charCount, ref subString, out var additional);

        Assert.IsTrue(result);
        Assert.AreEqual("an", subString);
        Assert.AreEqual(0, charCount);
        Assert.AreEqual(4, offset);
        Assert.AreEqual(string.Empty, additional);
        Assert.AreEqual(0, warnings.Count);
    }

    [TestMethod]
    public void BuildNameToken_TransformsUmlautMarker()
    {
        var warnings = new List<string>();
        var sut = new GenealogicalNameTokenBuilder(CreateConfiguration(warnings));
        var offset = 1;
        var charCount = 0;
        var subString = string.Empty;

        var result = sut.BuildNameToken("ä", ref offset, ref charCount, ref subString, out var additional);

        Assert.IsTrue(result);
        Assert.AreEqual("ä", subString);
        Assert.AreEqual(1, charCount);
        Assert.AreEqual(string.Empty, additional);
        Assert.AreEqual(0, warnings.Count);
    }

    [TestMethod]
    public void BuildName_WithTwinAdditional_AppendsTwinData()
    {
        var warnings = new List<string>();
        var sut = new GenealogicalNameTokenBuilder(CreateConfiguration(warnings));
        var offset = 1;
        var subString = string.Empty;
        var data = string.Empty;
        var charCount = 0;
        string? aka = null;
        var addEvent = ParserEventType.evt_Anull;

        var result = sut.BuildName("(Zw)", ref offset, ref subString, ref data, ref charCount, ref aka, ref addEvent);

        Assert.IsFalse(result);
        Assert.AreEqual("Zwilling", data);
        Assert.IsNull(aka);
        Assert.AreEqual(ParserEventType.evt_Anull, addEvent);
    }

    [TestMethod]
    public void BuildName_WithAkaAdditional_SetsAkaAndEvent()
    {
        var warnings = new List<string>();
        var sut = new GenealogicalNameTokenBuilder(CreateConfiguration(warnings));
        var offset = 1;
        var subString = string.Empty;
        var data = string.Empty;
        var charCount = 0;
        string? aka = null;
        var addEvent = ParserEventType.evt_Anull;

        var result = sut.BuildName("(genannt Bubi)", ref offset, ref subString, ref data, ref charCount, ref aka, ref addEvent);

        Assert.IsFalse(result);
        Assert.AreEqual("genannt Bubi", aka);
        Assert.AreEqual(ParserEventType.evt_AKA, addEvent);
        Assert.AreEqual(0, charCount);
    }

    [TestMethod]
    public void BuildName_WithExistingTwinData_AppendsTwinMarkerText()
    {
        var warnings = new List<string>();
        var sut = new GenealogicalNameTokenBuilder(CreateConfiguration(warnings));
        var offset = 1;
        var subString = string.Empty;
        var data = "Hinweis";
        var charCount = 0;
        string? aka = null;
        var addEvent = ParserEventType.evt_Anull;

        var result = sut.BuildName("(Zw)", ref offset, ref subString, ref data, ref charCount, ref aka, ref addEvent);

        Assert.IsFalse(result);
        Assert.AreEqual("Hinweis; Zwilling", data);
        Assert.IsNull(aka);
        Assert.AreEqual(ParserEventType.evt_Anull, addEvent);
    }

    private static GenealogicalNameTokenBuilderConfiguration CreateConfiguration(List<string> warnings)
        => new()
        {
            ProtectSpaceMarker = " ",
            UnknownMarker = "…",
            TwinMarker = "Zw",
            Separator2Marker = "‚",
            UmlautMarkers = ["ä", "ö", "ü", "Ä", "Ö", "Ü", "ß", "é"],
            TestFor = static (string text, int position, string[] tests, out int found) => FBEntryParser.TestFor(text, position, tests, out found),
            ParseAdditional = static (string text, ref int position, out string output) =>
            {
                output = string.Empty;
                if (text.Length <= position || text[position - 1] != '(')
                {
                    return false;
                }

                position++;
                while (position <= text.Length && text[position - 1] != ')')
                {
                    output += text[position - 1];
                    position++;
                }

                return true;
            },
            Warning = warnings.Add,
        };
}
