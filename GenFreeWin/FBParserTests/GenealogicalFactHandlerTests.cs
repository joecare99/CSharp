using System;
using System.Collections.Generic;
using System.Linq;
using FBParser;
using FBParser.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FBParserTests;

[TestClass]
public sealed class GenealogicalFactHandlerTests
{
    [TestMethod]
    public void HandleNonPersonEntry_WithLedigOccupation_EmitsSeparatedDescriptionAndOccupation()
    {
        var collector = new FactHandlerCollector();
        var sut = new GenealogicalFactHandler(CreateConfiguration(collector, CreateOccupationAnalyseEntry()));

        var result = sut.HandleNonPersonEntry("led.Rentnerin", "I61F", "Meißenheim");

        Assert.AreEqual(ParserEventType.evt_Occupation, result);
        CollectionAssert.AreEqual(
        new List<ParseResult>
        {
            new("ParserIndiData", "ledig", "I61F", (int)ParserEventType.evt_Description),
            new("ParserIndiOccu", "Rentnerin", "I61F", (int)ParserEventType.evt_Occupation),
            new("ParserIndiPlace", "Meißenheim", "I61F", (int)ParserEventType.evt_Occupation),
        },
        collector.Results.ToList());
    }

    [TestMethod]
    public void HandleNonPersonEntry_WithLeadingArticleOnly_StripsArticleBeforeOccupation()
    {
        var collector = new FactHandlerCollector();
        var sut = new GenealogicalFactHandler(CreateConfiguration(collector, CreateArticleOccupationAnalyseEntry()));

        var result = sut.HandleNonPersonEntry("ein Schneider", "I61M", "Bern");

        Assert.AreEqual(ParserEventType.evt_Occupation, result);
        CollectionAssert.AreEqual(
        new List<ParseResult>
        {
            new("ParserIndiOccu", "Schneider", "I61M", (int)ParserEventType.evt_Occupation),
            new("ParserIndiPlace", "Bern", "I61M", (int)ParserEventType.evt_Occupation),
        },
        collector.Results.ToList());
    }

    [TestMethod]
    public void HandleNonPersonEntry_WithDateOnlyOccupation_Warns()
    {
        var collector = new FactHandlerCollector();
        var sut = new GenealogicalFactHandler(CreateConfiguration(collector, CreateDateOnlyAnalyseEntry()));

        var result = sut.HandleNonPersonEntry("1900", "I61M", "Bern");

        Assert.AreEqual(ParserEventType.evt_Occupation, result);
        CollectionAssert.AreEqual(
        new List<ParseResult>
        {
            new("ParserIndiDate", "1900", "I61M", (int)ParserEventType.evt_Occupation),
            new("ParserIndiPlace", "Bern", "I61M", (int)ParserEventType.evt_Occupation),
        },
        collector.Results.ToList());
        CollectionAssert.AreEqual(new List<string> { "Entry contains no Marker, only a Date" }, collector.Warnings);
    }

    [TestMethod]
    public void HandleFamilyFact_IgnoresEmptyEntry()
    {
        var collector = new FactHandlerCollector();
        var sut = new GenealogicalFactHandler(CreateConfiguration(collector, CreateFamilyFallbackAnalyseEntry()));

        sut.HandleFamilyFact("123", ".");

        Assert.AreEqual(0, collector.Results.Count);
    }

    [TestMethod]
    public void HandleFamilyFact_WithDateDataAndPlace_EmitsStructuredFamilyFact()
    {
        var collector = new FactHandlerCollector();
        var sut = new GenealogicalFactHandler(CreateConfiguration(collector, CreateStructuredFamilyAnalyseEntry()));

        sut.HandleFamilyFact("123", "⚭ 12.03.1900 in Bern");

        CollectionAssert.AreEqual(
        new List<ParseResult>
        {
            new("ParserFamilyDate", "12.03.1900", "123", (int)ParserEventType.evt_Marriage),
            new("ParserFamilyPlace", "Bern", "123", (int)ParserEventType.evt_Marriage),
            new("ParserFamilyData", "Hinweis", "123", (int)ParserEventType.evt_Marriage),
        },
        collector.Results.ToList());
    }

    [TestMethod]
    public void HandleNonPersonEntry_WithLedigOnlyAfterOccupation_EmitsOccupationPlaceThenDescription()
    {
        var collector = new FactHandlerCollector();
        var sut = new GenealogicalFactHandler(CreateConfiguration(collector, CreateEmptyOccupationAnalyseEntry()));

        var result = sut.HandleNonPersonEntry("led.", "I100M", "Meißenheim", previousEntryType: ParserEventType.evt_Occupation);

        Assert.AreEqual(ParserEventType.evt_Occupation, result);
        CollectionAssert.AreEqual(
        new List<ParseResult>
        {
            new("ParserIndiPlace", "Meißenheim", "I100M", (int)ParserEventType.evt_Occupation),
            new("ParserIndiData", "ledig", "I100M", (int)ParserEventType.evt_Description),
        },
        collector.Results.ToList());
    }

    [TestMethod]
    public void HandleFamilyFact_EmitsResidenceFallbackData()
    {
        var collector = new FactHandlerCollector();
        var sut = new GenealogicalFactHandler(CreateConfiguration(collector, CreateFamilyFallbackAnalyseEntry()));

        sut.HandleFamilyFact("123", "Unbekannte Angabe");

        CollectionAssert.AreEqual(
        new List<ParseResult>
        {
            new("ParserFamilyPlace", "Bern", "123", (int)ParserEventType.evt_Residence),
            new("ParserFamilyData", "Unbekannte Angabe", "123", (int)ParserEventType.evt_Residence),
        },
        collector.Results.ToList());
    }

    private static GenealogicalFactHandlerConfiguration CreateConfiguration(FactHandlerCollector collector, AnalyseEntryDelegate analyseEntry)
        => new()
        {
            LedigMarkers = ["lediger", "ledige", "led.", "ledig."],
            IndefiniteArticles = ["eine", "ein"],
            TitleMarkers = ["Graf", "Erbgraf", "Gräfin", "Baron", "Baronin", "Prinz", "Prinzessin", "Junker", "Freiherr", "Freiin"],
            LedigText = "ledig",
            MarriagePartnerMarker = "mit",
            AnalyseEntry = analyseEntry,
            StartFamily = famRef => collector.Results.Add(new ParseResult("ParserStartFamily", famRef, string.Empty, 0)),
            SetFamilyMember = (famRef, individualId, famMember) => collector.Results.Add(new ParseResult("ParserFamilyIndiv", individualId, famRef, famMember)),
            SetFamilyDate = (famRef, eventType, date) => collector.Results.Add(new ParseResult("ParserFamilyDate", date, famRef, (int)eventType)),
            SetFamilyPlace = (famRef, eventType, place) => collector.Results.Add(new ParseResult("ParserFamilyPlace", place, famRef, (int)eventType)),
            SetFamilyData = (famRef, eventType, data) => collector.Results.Add(new ParseResult("ParserFamilyData", data, famRef, (int)eventType)),
            SetIndiDate = (individualId, eventType, date) => collector.Results.Add(new ParseResult("ParserIndiDate", date, individualId, (int)eventType)),
            SetIndiPlace = (individualId, eventType, place) => collector.Results.Add(new ParseResult("ParserIndiPlace", place, individualId, (int)eventType)),
            SetIndiData = (individualId, eventType, data) => collector.Results.Add(new ParseResult("ParserIndiData", data, individualId, (int)eventType)),
            SetIndiOccu = (individualId, eventType, occu) => collector.Results.Add(new ParseResult("ParserIndiOccu", occu, individualId, (int)eventType)),
            SetIndiName = (individualId, nameType, name) => collector.Results.Add(new ParseResult("ParserIndiName", name, individualId, nameType)),
            SetFamilyType = (famRef, famType, data) => collector.Results.Add(new ParseResult("ParserFamilyType", data, famRef, famType)),
            HandleAkPersonEntry = StubHandleAkPersonEntry,
            TryConsumeLeadingEntry = static (ref string subString, string[] testStrings, out string rest) =>
            {
                foreach (var testString in testStrings)
                {
                    if (subString.StartsWith(testString, StringComparison.Ordinal))
                    {
                        rest = subString[testString.Length..].TrimStart();
                        return true;
                    }
                }

                rest = string.Empty;
                return false;
            },
            TestFor = static (string text, int position, string[] tests, out int found) => FBEntryParser.TestFor(text, position, tests, out found),
            Warning = message => collector.Warnings.Add(message),
        };

    private static AnalyseEntryDelegate CreateOccupationAnalyseEntry()
        => static (ref string subString, out ParserEventType entryType, out string data, out string place, out string date) =>
        {
            entryType = ParserEventType.evt_Last;
            data = string.Empty;
            place = "Meißenheim";
            date = string.Empty;
            subString = "led.Rentnerin";
        };

    private static AnalyseEntryDelegate CreateEmptyOccupationAnalyseEntry()
        => static (ref string subString, out ParserEventType entryType, out string data, out string place, out string date) =>
        {
            entryType = ParserEventType.evt_Last;
            data = string.Empty;
            place = string.Empty;
            date = string.Empty;
            subString = "led.";
        };

    private static AnalyseEntryDelegate CreateFamilyFallbackAnalyseEntry()
        => static (ref string subString, out ParserEventType entryType, out string data, out string place, out string date) =>
        {
            entryType = ParserEventType.evt_Last;
            data = string.Empty;
            place = "Bern";
            date = string.Empty;
            subString = "Unbekannte Angabe";
        };

    private static AnalyseEntryDelegate CreateArticleOccupationAnalyseEntry()
        => static (ref string subString, out ParserEventType entryType, out string data, out string place, out string date) =>
        {
            entryType = ParserEventType.evt_Last;
            data = string.Empty;
            place = "Bern";
            date = string.Empty;
            subString = "ein Schneider";
        };

    private static AnalyseEntryDelegate CreateDateOnlyAnalyseEntry()
        => static (ref string subString, out ParserEventType entryType, out string data, out string place, out string date) =>
        {
            entryType = ParserEventType.evt_Last;
            data = string.Empty;
            place = "Bern";
            date = "1900";
            subString = string.Empty;
        };

    private static AnalyseEntryDelegate CreateStructuredFamilyAnalyseEntry()
        => static (ref string subString, out ParserEventType entryType, out string data, out string place, out string date) =>
        {
            entryType = ParserEventType.evt_Marriage;
            data = "Hinweis";
            place = "Bern";
            date = "12.03.1900";
            subString = "⚭ 12.03.1900 in Bern";
        };

    private static string StubHandleAkPersonEntry(string personEntry, string mainFamRef, char personType, int mode, out string lastName, out char personSex, string aka = "", string famName = "")
    {
        lastName = string.Empty;
        personSex = 'F';
        return "I" + mainFamRef + personSex;
    }

    private sealed class FactHandlerCollector
    {
        public List<ParseResult> Results { get; } = [];

        public List<string> Warnings { get; } = [];
    }
}
