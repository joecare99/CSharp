using System.Collections.Generic;
using FBParser;
using FBParser.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FBParserTests;

[TestClass]
public sealed class GenealogicalEventEmitterTests
{
    [TestMethod]
    public void SetIndiDate_WithInvalidDate_ReportsErrorWithoutEmittingDate()
    {
        var collector = new EventEmitterCollector();
        var sut = new GenealogicalEventEmitter(CreateConfiguration(collector));

        sut.SetIndiDate("I1", ParserEventType.evt_Birth, "12:03");

        Assert.AreEqual(1, collector.Errors.Count);
        Assert.AreEqual(0, collector.IndiDates.Count);
    }

    [TestMethod]
    public void SetFamilyPlace_WithInvalidPlace_ReportsErrorAndStillEmitsPlace()
    {
        var collector = new EventEmitterCollector();
        var sut = new GenealogicalEventEmitter(CreateConfiguration(collector));

        sut.SetFamilyPlace("F1", ParserEventType.evt_Marriage, "berlin");

        Assert.AreEqual(1, collector.Errors.Count);
        Assert.AreEqual(1, collector.FamilyPlaces.Count);
        Assert.AreEqual(new ParseResult("ParserFamilyPlace", "berlin", "F1", (int)ParserEventType.evt_Marriage), collector.FamilyPlaces[0]);
    }

    [TestMethod]
    public void SetIndiPlace_WithInvalidPlace_ReportsErrorAndStillEmitsPlace()
    {
        var collector = new EventEmitterCollector();
        var sut = new GenealogicalEventEmitter(CreateConfiguration(collector));

        sut.SetIndiPlace("I1", ParserEventType.evt_Residence, "berlin");

        Assert.AreEqual(1, collector.Errors.Count);
        Assert.AreEqual(1, collector.IndiPlaces.Count);
        Assert.AreEqual(new ParseResult("ParserIndiPlace", "berlin", "I1", (int)ParserEventType.evt_Residence), collector.IndiPlaces[0]);
    }

    [TestMethod]
    public void SetIndiRelat_WithZeroReferenceOutsideMainRef_ReportsErrorAndStillEmitsRelation()
    {
        var collector = new EventEmitterCollector();
        var sut = new GenealogicalEventEmitter(CreateConfiguration(collector));

        sut.SetIndiRelat("I1", "0", 1, "123");

        Assert.AreEqual(1, collector.Errors.Count);
        Assert.AreEqual(1, collector.IndiRelations.Count);
        Assert.AreEqual(new ParseResult("ParserIndiRel", "0", "I1", 1), collector.IndiRelations[0]);
    }

    [TestMethod]
    public void StartFamily_And_EndOfEntry_EmitLifecycleEvents()
    {
        var collector = new EventEmitterCollector();
        var sut = new GenealogicalEventEmitter(CreateConfiguration(collector));

        sut.StartFamily("123");
        sut.EndOfEntry("123");

        Assert.AreEqual(new ParseResult("ParserStartFamily", "123", string.Empty, 0), collector.StartFamilies[0]);
        Assert.AreEqual(new ParseResult("ParserEntryEnd", string.Empty, "123", -1), collector.EntryEnds[0]);
    }

    private static GenealogicalEventEmitterConfiguration CreateConfiguration(EventEmitterCollector collector)
        => new()
        {
            IsValidDate = static date => date != "12:03",
            IsValidPlace = static place => place != "berlin",
            IsValidReference = static reference => reference != "0",
            Error = collector.Errors.Add,
            OnStartFamily = famRef => collector.StartFamilies.Add(new ParseResult("ParserStartFamily", famRef, string.Empty, 0)),
            OnEntryEnd = famRef => collector.EntryEnds.Add(new ParseResult("ParserEntryEnd", string.Empty, famRef, -1)),
            OnFamilyDate = (data, reference, subType) => collector.FamilyDates.Add(new ParseResult("ParserFamilyDate", data, reference, subType)),
            OnFamilyType = (data, reference, subType) => collector.FamilyTypes.Add(new ParseResult("ParserFamilyType", data, reference, subType)),
            OnFamilyData = (data, reference, subType) => collector.FamilyData.Add(new ParseResult("ParserFamilyData", data, reference, subType)),
            OnFamilyPlace = (data, reference, subType) => collector.FamilyPlaces.Add(new ParseResult("ParserFamilyPlace", data, reference, subType)),
            OnFamilyIndiv = (data, reference, subType) => collector.FamilyMembers.Add(new ParseResult("ParserFamilyIndiv", data, reference, subType)),
            OnIndiName = (data, reference, subType) => collector.IndiNames.Add(new ParseResult("ParserIndiName", data, reference, subType)),
            OnIndiDate = (data, reference, subType) => collector.IndiDates.Add(new ParseResult("ParserIndiDate", data, reference, subType)),
            OnIndiPlace = (data, reference, subType) => collector.IndiPlaces.Add(new ParseResult("ParserIndiPlace", data, reference, subType)),
            OnIndiOccu = (data, reference, subType) => collector.IndiOccupations.Add(new ParseResult("ParserIndiOccu", data, reference, subType)),
            OnIndiRel = (data, reference, subType) => collector.IndiRelations.Add(new ParseResult("ParserIndiRel", data, reference, subType)),
            OnIndiRef = (data, reference, subType) => collector.IndiReferences.Add(new ParseResult("ParserIndiRef", data, reference, subType)),
            OnIndiData = (data, reference, subType) => collector.IndiData.Add(new ParseResult("ParserIndiData", data, reference, subType)),
        };

    private sealed class EventEmitterCollector
    {
        public List<string> Errors { get; } = [];

        public List<ParseResult> StartFamilies { get; } = [];

        public List<ParseResult> EntryEnds { get; } = [];

        public List<ParseResult> FamilyDates { get; } = [];

        public List<ParseResult> FamilyTypes { get; } = [];

        public List<ParseResult> FamilyData { get; } = [];

        public List<ParseResult> FamilyPlaces { get; } = [];

        public List<ParseResult> FamilyMembers { get; } = [];

        public List<ParseResult> IndiNames { get; } = [];

        public List<ParseResult> IndiDates { get; } = [];

        public List<ParseResult> IndiPlaces { get; } = [];

        public List<ParseResult> IndiOccupations { get; } = [];

        public List<ParseResult> IndiRelations { get; } = [];

        public List<ParseResult> IndiReferences { get; } = [];

        public List<ParseResult> IndiData { get; } = [];
    }
}
