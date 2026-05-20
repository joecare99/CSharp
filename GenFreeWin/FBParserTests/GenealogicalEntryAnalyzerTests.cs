using System;
using FBParser;
using FBParser.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FBParserTests;

[TestClass]
public sealed class GenealogicalEntryAnalyzerTests
{
    private static readonly GenealogicalEntryAnalyzerConfiguration TestConfiguration = new()
    {
        ProtectSpace = " ",
        BirthMarker = "*",
        BaptismMarkers = ["*)", "~"],
        BurialMarkers = ["†)", "="],
        MarriageMarkers = ["⚭", "oo", "∞"],
        DeathMarkers = ["†", "+"],
        StillbornMarkers = ["†*", "+*"],
        FallenMarker = "gefallen",
        DivorceMarker = "o/o",
        MissingMarkers = ["vermisst", "vermißt"],
        EmigrationMarkers = ["ausgewandert", "wanderte"],
        DateModifiers = ["ca", "um", "ab", "von", "vor", "nach", "err.", "seit", "zwischen", "frühestens", "spätestens"],
        SinceDateModifier = "seit",
        AgeMarker = "alt",
        IndefiniteArticles = ["eine", "ein"],
        DefiniteArticles = ["der", "die", "das"],
        AkaMarker = "genannt",
        BecameMarker = "wurde",
        DescriptionMarkers = ["ledig", "Witwer", "Witwe"],
        ResidenceMarkers = ["lebte", "leb", "wohnte", "wohnhaft", "wohnt", "Herkunft"],
        PlaceMarkers = ["in", "aus", "nach", "am", "bei", "im", "auf der"],
        UnknownMarkers = ["…", "...", ".."],
        PropertyMarkers = ["baute", "kaufte", "erwarb"],
        AddressMarkers = ["str.", "siedl", "straße", "gasse", "weg", "platz", "pfad"],
        ReligionMarkers = ["rk.", "kath.", "ev.", "evang.", "ref.", "reform.", "luth."],
        MonthNames = ["", "Januar", "Februar", "März", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "November", "Dezember"],
        UpperUmlautMarkers = ["Ä", "Ö", "Ü"],
        InPlaceMarker = "in",
        ToPlaceMarker = "nach",
        FromPlaceMarker = "aus",
        InMonthPlaceMarker = "im",
        OnDatePlaceMarker = "am",
        MarriagePartnerMarker = "mit",
        IsValidDate = static date =>
            !date.Contains('\n')
            && !date.Contains('\r')
            && !date.Contains('\t')
            && !date.Contains(',')
            && !date.Contains(':')
            && !date.Contains(';')
            && !date.Contains('<')
            && !date.Contains('>')
            && !date.Contains('+')
            && !date.Contains('*')
            && !date.Contains('|'),
        IsValidPlace = static place => place == string.Empty
            || place == "..."
            || PascalCompat.UpperCharset.Contains(place[0])
            || place[0] is '"' or '“'
            || FBEntryParser.TestFor(place, 1, ["Ä", "Ö", "Ü"]),
        Warning = static _ => { },
    };

    [TestMethod]
    [DataRow("* 12.03.1900", ParserEventType.evt_Birth)]
    [DataRow("† 12.03.1900", ParserEventType.evt_Death)]
    [DataRow("⚭ 12.03.1900", ParserEventType.evt_Marriage)]
    [DataRow("o/o 12.03.1900", ParserEventType.evt_Divorce)]
    [DataRow("rk.", ParserEventType.evt_Religion)]
    public void GetEntryType_DetectsKnownEntryMarkers(string text, ParserEventType expected)
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);

        var result = sut.GetEntryType(text, out _, out _);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void TrimPlaceByMonth_RemovesTrailingMonth()
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);
        var place = "Berlin März";

        sut.TrimPlaceByMonth(ref place);

        Assert.AreEqual("Berlin", place);
    }

    [TestMethod]
    public void TrimPlaceByModif_RemovesTrailingDateModifier()
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);
        var place = "Berlin vor";

        sut.TrimPlaceByModif(ref place);

        Assert.AreEqual("Berlin", place);
    }

    [TestMethod]
    public void AnalyseEntry_AssignsDefaultPlaceForOccupationEntry()
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);
        var entry = "Schneider 12.03.1900";

        sut.AnalyseEntry(ref entry, "Bern", 0, out var entryType, out var data, out var place, out var date);

        Assert.AreEqual(ParserEventType.evt_Last, entryType);
        Assert.AreEqual("Schneider", entry);
        Assert.AreEqual(string.Empty, data);
        Assert.AreEqual("Bern", place);
        Assert.AreEqual("12.03.1900", date);
    }

    [TestMethod]
    public void GetEntryType_ReturnsStillbornData()
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);

        var result = sut.GetEntryType("†* 12.03.1900", out var date, out var data);

        Assert.AreEqual(ParserEventType.evt_Stillborn, result);
        Assert.AreEqual("12.03.1900", date);
        Assert.AreEqual("totgeboren", data);
    }

    [TestMethod]
    public void GetEntryType_ExtractsEmigrationData()
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);

        var result = sut.GetEntryType("ist 1901 nach Bern ausgewandert", out var date, out var data);

        Assert.AreEqual(ParserEventType.evt_AddEmigration, result);
        Assert.AreEqual(string.Empty, date);
        Assert.AreEqual("1901 nach Bern", data);
    }

    [TestMethod]
    public void GetEntryType_DetectsAgeDescription()
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);

        var result = sut.GetEntryType("23 alt", out var date, out var data);

        Assert.AreEqual(ParserEventType.evt_Age, result);
        Assert.AreEqual(string.Empty, date);
        Assert.AreEqual("23 alt", data);
    }

    [TestMethod]
    public void GetEntryType_ExtractsAkaAfterBecameMarker()
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);

        var result = sut.GetEntryType("wurde Hans genannt Hannes", out var date, out var data);

        Assert.AreEqual(ParserEventType.evt_AKA, result);
        Assert.AreEqual(string.Empty, date);
        Assert.AreEqual("Hans Hannes", data);
    }

    [TestMethod]
    public void GetEntryType_DetectsDefiniteArticleAsAka()
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);

        var result = sut.GetEntryType("der Lange", out _, out var data);

        Assert.AreEqual(ParserEventType.evt_AKA, result);
        Assert.AreEqual("der Lange", data);
    }

    [TestMethod]
    public void GetEntryType_DetectsStandaloneDescriptionMarker()
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);

        var result = sut.GetEntryType("ledig", out _, out var data);

        Assert.AreEqual(ParserEventType.evt_Description, result);
        Assert.AreEqual("ledig", data);
    }

    [TestMethod]
    public void AnalyseEntry_UsesInPlaceAsResidenceWithoutDate()
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);
        var entry = "in Berlin";

        sut.AnalyseEntry(ref entry, "Bern", 0, out var entryType, out var data, out var place, out var date);

        Assert.AreEqual(ParserEventType.evt_Residence, entryType);
        Assert.AreEqual(string.Empty, entry);
        Assert.AreEqual(string.Empty, data);
        Assert.AreEqual("Berlin", place);
        Assert.AreEqual(string.Empty, date);
    }

    [TestMethod]
    public void AnalyseEntry_ExtractsPlaceFromProtectedSpaceMarker()
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);
        var entry = "lebte in Berlin";

        sut.AnalyseEntry(ref entry, "Bern", 0, out var entryType, out var data, out var place, out var date);

        Assert.AreEqual(ParserEventType.evt_Residence, entryType);
        Assert.AreEqual("lebte", data);
        Assert.AreEqual("Berlin", place);
        Assert.AreEqual(string.Empty, date);
    }

    [TestMethod]
    public void AnalyseEntry_ConvertsIndefiniteDescriptionWithPlaceToOccupation()
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);
        var entry = "ein Schneider in Berlin";

        sut.AnalyseEntry(ref entry, string.Empty, 0, out var entryType, out var data, out var place, out var date);

        Assert.AreEqual(ParserEventType.evt_Occupation, entryType);
        Assert.AreEqual("ein Schneider", data);
        Assert.AreEqual("Berlin", place);
        Assert.AreEqual(string.Empty, date);
        Assert.AreEqual("Schneider", entry);
    }

    [TestMethod]
    public void AnalyseEntry_ParsesMarriagePartnerFromDateFragment()
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);
        var entry = "⚭ 12.03.1900 mit Anna";

        sut.AnalyseEntry(ref entry, "Bern", 0, out var entryType, out var data, out var place, out var date);

        Assert.AreEqual(ParserEventType.evt_Marriage, entryType);
        Assert.AreEqual("mit Anna", entry);
        Assert.AreEqual("Bern", place);
        Assert.AreEqual("12.03.1900", date);
        Assert.AreEqual(string.Empty, data);
    }

    [TestMethod]
    public void AnalyseEntry_MapsMissingMarkerToDeath()
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);
        var entry = "vermisst 1944";

        sut.AnalyseEntry(ref entry, "Bern", 0, out var entryType, out var data, out var place, out var date);

        Assert.AreEqual(ParserEventType.evt_Death, entryType);
        Assert.AreEqual("vermisst", data);
        Assert.AreEqual("Bern", place);
        Assert.AreEqual("1944", date);
    }

    [TestMethod]
    public void TrimPlaceByMonth_RemovesShortMonthAbbreviation()
    {
        var sut = new GenealogicalEntryAnalyzer(TestConfiguration);
        var place = "Berlin Mär.";

        sut.TrimPlaceByMonth(ref place);

        Assert.AreEqual("Berlin", place);
    }
}
