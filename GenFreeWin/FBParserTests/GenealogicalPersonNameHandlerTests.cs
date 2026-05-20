using System.Collections.Generic;
using System.Linq;
using FBParser;
using FBParser.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FBParserTests;

[TestClass]
public sealed class GenealogicalPersonNameHandlerTests
{
    [TestMethod]
    public void HandleAkPersonEntry_EmitsNameSexAndFamilyMembership()
    {
        var collector = new PersonNameHandlerCollector();
        var sut = new GenealogicalPersonNameHandler(CreateConfiguration(collector));

        var id = sut.HandleAkPersonEntry("Anna Müller", "123", 'U', 5, out var lastName, out var personSex);

        Assert.AreEqual("I123U", id);
        Assert.AreEqual("Müller", lastName);
        Assert.AreEqual('U', personSex);
        Assert.IsTrue(collector.Names.Any(n => n.Data == "Anna Müller" && n.Reference == id && n.SubType == 0));
        Assert.IsTrue(collector.Data.Any(d => d.Reference == id && d.SubType == (int)ParserEventType.evt_Sex));
        Assert.IsTrue(collector.FamilyMembers.Any(m => m.Reference == "123"));
    }

    [TestMethod]
    public void HandleAkPersonEntry_WithAkaStartingQuestion_AssignsLastNameFromAka()
    {
        var collector = new PersonNameHandlerCollector();
        var sut = new GenealogicalPersonNameHandler(CreateConfiguration(collector));

        var id = sut.HandleAkPersonEntry("Anna", "123", 'U', 5, out var lastName, out var personSex, "? Müller", "");

        Assert.AreEqual("Müller", lastName);
        Assert.IsTrue(collector.Names.Any(n => n.Reference == id && n.Data.Contains("Müller")));
        Assert.IsTrue(collector.FamilyMembers.Any(m => m.Reference == "123"));
    }

    [TestMethod]
    public void HandleAkPersonEntry_WithMaidenName_SetsFamilyTypeAndUsesMaidenSurname()
    {
        var collector = new PersonNameHandlerCollector();
        var sut = new GenealogicalPersonNameHandler(CreateConfiguration(collector));

        var id = sut.HandleAkPersonEntry("Anna Müller geb. Schmidt", "123", 'U', 5, out var lastName, out var personSex);

        Assert.AreEqual("Schmidt", lastName);
        Assert.IsTrue(collector.FamilyTypes.Any(f => f.Reference == "123" && f.SubType == 1));
    }

    private static GenealogicalPersonNameHandlerConfiguration CreateConfiguration(PersonNameHandlerCollector collector)
        => new()
        {
            UnknownShortMarker = "...",
            MaidenNameMarker = "geb.",
            AcademicTitleMarkers = ["Dr. med.", "Dr. rer. nat.", "Dr. theol.", "Pfarrer", "Geheimrat", "Prof.", "Prof. Dr.", "Dr."],
            TestFor = static (string text, int position, string[] tests, out int found) => FBEntryParser.TestFor(text, position, tests, out found),
            GuessSexOfGivenName = static (string name, bool learn) => 'U',
            LearnSexOfGivenName = static (string name, char sex) => { },
            SetIndiName = (individualId, nameType, name) => collector.Names.Add(new ParseResult("ParserIndiName", name, individualId, nameType)),
            SetIndiData = (individualId, eventType, data) => collector.Data.Add(new ParseResult("ParserIndiData", data, individualId, (int)eventType)),
            SetFamilyMember = (famRef, individualId, famMember) => collector.FamilyMembers.Add(new ParseResult("ParserFamilyIndiv", individualId, famRef, famMember)),
            SetFamilyType = (famRef, famType, data) => collector.FamilyTypes.Add(new ParseResult("ParserFamilyType", data, famRef, famType)),
        };

    private sealed class PersonNameHandlerCollector
    {
        public List<ParseResult> Names { get; } = [];

        public List<ParseResult> Data { get; } = [];

        public List<ParseResult> FamilyMembers { get; } = [];

        public List<ParseResult> FamilyTypes { get; } = [];
    }
}
