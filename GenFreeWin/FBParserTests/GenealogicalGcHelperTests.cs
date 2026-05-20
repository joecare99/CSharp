using System.Collections.Generic;
using System.Linq;
using FBParser;
using FBParser.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FBParserTests;

[TestClass]
public sealed class GenealogicalGcHelperTests
{
    [TestMethod]
    public void HandleGcDateEntry_RecognizesDeathKeyword()
    {
        var collector = new GcHelperCollector();
        var sut = new GenealogicalGcHelper(CreateConfiguration(collector));
        var position = 1;
        var mode = 112;
        var retMode = 0;
        var eventType = ParserEventType.evt_Anull;

        var result = sut.HandleGcDateEntry("gefallen", ref position, "I1M", ref mode, ref retMode, ref eventType);

        Assert.IsTrue(result);
        Assert.AreEqual(101, mode);
        Assert.AreEqual(112, retMode);
        Assert.AreEqual(ParserEventType.evt_Death, eventType);
        Assert.AreEqual(1, collector.IndiData.Count);
        Assert.AreEqual(new ParseResult("ParserIndiData", "gefallen", "I1M", (int)ParserEventType.evt_Death), collector.IndiData[0]);
    }

    [TestMethod]
    public void HandleGcDateEntry_RecognizesMissingKeyword()
    {
        var collector = new GcHelperCollector();
        var sut = new GenealogicalGcHelper(CreateConfiguration(collector));
        var position = 1;
        var mode = 112;
        var retMode = 0;
        var eventType = ParserEventType.evt_Anull;

        var result = sut.HandleGcDateEntry("vermisst", ref position, "I1M", ref mode, ref retMode, ref eventType);

        Assert.IsTrue(result);
        Assert.AreEqual(101, mode);
        Assert.AreEqual(112, retMode);
        Assert.AreEqual(ParserEventType.evt_Death, eventType);
        Assert.AreEqual(1, collector.IndiData.Count);
        Assert.AreEqual(new ParseResult("ParserIndiData", "vermisst", "I1M", (int)ParserEventType.evt_Death), collector.IndiData[0]);
    }

    [TestMethod]
    public void HandleGcDateEntry_RecognizesBirthKeyword()
    {
        var collector = new GcHelperCollector();
        var sut = new GenealogicalGcHelper(CreateConfiguration(collector));
        var position = 1;
        var mode = 112;
        var retMode = 0;
        var eventType = ParserEventType.evt_Anull;

        var result = sut.HandleGcDateEntry("*", ref position, "I1M", ref mode, ref retMode, ref eventType);

        Assert.IsTrue(result);
        Assert.AreEqual(ParserEventType.evt_Birth, eventType);
        Assert.AreEqual(0, collector.IndiData.Count);
    }

    [TestMethod]
    public void HandleGcDateEntry_RecognizesBaptismKeyword()
    {
        var collector = new GcHelperCollector();
        var sut = new GenealogicalGcHelper(CreateConfiguration(collector));
        var position = 1;
        var mode = 112;
        var retMode = 0;
        var eventType = ParserEventType.evt_Anull;

        var result = sut.HandleGcDateEntry("~", ref position, "I1M", ref mode, ref retMode, ref eventType);

        Assert.IsTrue(result);
        Assert.AreEqual(ParserEventType.evt_Baptism, eventType);
        Assert.AreEqual(0, collector.IndiData.Count);
    }

    [TestMethod]
    public void HandleGcDateEntry_RecognizesBurialKeyword()
    {
        var collector = new GcHelperCollector();
        var sut = new GenealogicalGcHelper(CreateConfiguration(collector));
        var position = 1;
        var mode = 112;
        var retMode = 0;
        var eventType = ParserEventType.evt_Anull;

        var result = sut.HandleGcDateEntry("=", ref position, "I1M", ref mode, ref retMode, ref eventType);

        Assert.IsTrue(result);
        Assert.AreEqual(ParserEventType.evt_Burial, eventType);
        Assert.AreEqual(0, collector.IndiData.Count);
    }

    [TestMethod]
    public void HandleGcDateEntry_ReturnsFalseForUnknownToken()
    {
        var collector = new GcHelperCollector();
        var sut = new GenealogicalGcHelper(CreateConfiguration(collector));
        var position = 1;
        var mode = 112;
        var retMode = 0;
        var eventType = ParserEventType.evt_Anull;

        var result = sut.HandleGcDateEntry("?", ref position, "I1M", ref mode, ref retMode, ref eventType);

        Assert.IsFalse(result);
        Assert.AreEqual(112, mode);
        Assert.AreEqual(0, collector.IndiData.Count);
    }

    [TestMethod]
    public void HandleGcNonPersonEntry_MapsReligionEntry()
    {
        var collector = new GcHelperCollector();
        var sut = new GenealogicalGcHelper(CreateConfiguration(collector));

        var result = sut.HandleGcNonPersonEntry("rk.", '.', "I1M");

        Assert.IsTrue(result);
        Assert.AreEqual(1, collector.IndiData.Count);
        Assert.AreEqual(new ParseResult("ParserIndiData", "rk..", "I1M", (int)ParserEventType.evt_Religion), collector.IndiData[0]);
    }

    [TestMethod]
    public void HandleGcNonPersonEntry_MapsTitleWithPlace()
    {
        var collector = new GcHelperCollector();
        var sut = new GenealogicalGcHelper(CreateConfiguration(collector));

        var result = sut.HandleGcNonPersonEntry("Graf in Bern", ',', "I1M");

        Assert.IsTrue(result);
        CollectionAssert.AreEqual(
        new List<ParseResult>
        {
            new("ParserIndiName", "Graf", "I1M", 4),
            new("ParserIndiPlace", "Bern", "I1M", (int)ParserEventType.evt_Occupation),
        },
        new List<ParseResult>(collector.IndiNames.Concat(collector.IndiPlaces)));
    }

    [TestMethod]
    public void ScanForEventDate_ExtractsChildEventDate()
    {
        var collector = new GcHelperCollector();
        var sut = new GenealogicalGcHelper(CreateConfiguration(collector));

        var result = sut.ScanForEventDate("prefix Kd: * 12.03.1900 suffix", 1);

        Assert.AreEqual(" 12.03.1900 ", result);
    }

    [TestMethod]
    public void ScanForEventDate_UsesAlternateChildDateMarker()
    {
        var collector = new GcHelperCollector();
        var sut = new GenealogicalGcHelper(CreateConfiguration(collector));

        var result = sut.ScanForEventDate("prefix Kdr: * 12.03.1900 suffix", 1);

        Assert.AreEqual(" 12.03.1900 ", result);
    }

    [TestMethod]
    public void ScanForEventDate_HandlesDeathAndBirthMarkers()
    {
        var collector = new GcHelperCollector();
        var sut = new GenealogicalGcHelper(CreateConfiguration(collector));

        var result = sut.ScanForEventDate("prefix Kd: †* 12.03.1900 suffix", 1);

        Assert.AreEqual(" 12.03.1900 ", result);
    }

    [TestMethod]
    public void ScanForEventDate_SkipsNameTextWithUmlautBeforeDate()
    {
        var collector = new GcHelperCollector();
        var sut = new GenealogicalGcHelper(CreateConfiguration(collector));

        var result = sut.ScanForEventDate("prefix Kd: Mädchen * 12.03.1900 suffix", 1);

        Assert.AreEqual(" 12.03.1900 ", result);
    }

    [TestMethod]
    public void ScanForEventDate_SkipsCommaBeforeDate()
    {
        var collector = new GcHelperCollector();
        var sut = new GenealogicalGcHelper(CreateConfiguration(collector));

        var result = sut.ScanForEventDate("prefix Kd:,* 12.03.1900 suffix", 1);

        Assert.AreEqual(" 12.03.1900 ", result);
    }

    [TestMethod]
    public void ScanForEventDate_ReturnsEmptyWhenNoChildMarkerIsPresent()
    {
        var collector = new GcHelperCollector();
        var sut = new GenealogicalGcHelper(CreateConfiguration(collector));

        var result = sut.ScanForEventDate("prefix without child marker", 1);

        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void ScanForEventDate_StopsAtFiveDigitRun()
    {
        var collector = new GcHelperCollector();
        var sut = new GenealogicalGcHelper(CreateConfiguration(collector));

        var result = sut.ScanForEventDate("prefix Kd: 12345. suffix", 1);

        Assert.AreEqual("12345", result);
    }

    private static GenealogicalGcHelperConfiguration CreateConfiguration(GcHelperCollector collector)
        => new()
        {
            BirthMarker = "*",
            BaptismMarker = "~",
            DeathMarker = "†",
            BurialMarker = "=",
            FallenMarker = "gefallen",
            MissingMarker = "vermisst",
            ChildDateMarker = "Kd:",
            ChildDateMarkerAlternate = "Kdr:",
            GetDefaultPlace = static () => "Bern",
            UmlautMarkers = ["ä", "ö", "ü", "Ä", "Ö", "Ü", "ß", "é"],
            TitleMarkers = ["Graf", "Erbgraf", "Gräfin", "Baron", "Baronin", "Prinz", "Prinzessin", "Junker", "Freiherr", "Freiin"],
            TestFor = static (string text, int position, string[] tests, out int found) => FBEntryParser.TestFor(text, position, tests, out found),
            TestForCaseInsensitive = static (string text, int position, string test) => string.Equals(PascalCompat.Copy(text, position, test.Length), test, System.StringComparison.OrdinalIgnoreCase),
            SetIndiData = (individualId, eventType, data) => collector.IndiData.Add(new ParseResult("ParserIndiData", data, individualId, (int)eventType)),
            SetIndiPlace = (individualId, eventType, place) => collector.IndiPlaces.Add(new ParseResult("ParserIndiPlace", place, individualId, (int)eventType)),
            SetIndiOccu = (individualId, eventType, occu) => collector.IndiOccupations.Add(new ParseResult("ParserIndiOccu", occu, individualId, (int)eventType)),
            SetIndiName = (individualId, nameType, name) => collector.IndiNames.Add(new ParseResult("ParserIndiName", name, individualId, nameType)),
        };

    private sealed class GcHelperCollector
    {
        public List<ParseResult> IndiData { get; } = [];

        public List<ParseResult> IndiPlaces { get; } = [];

        public List<ParseResult> IndiOccupations { get; } = [];

        public List<ParseResult> IndiNames { get; } = [];
    }
}
