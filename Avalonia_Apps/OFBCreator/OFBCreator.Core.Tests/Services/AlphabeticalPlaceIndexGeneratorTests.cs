using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Microsoft.Extensions.Logging;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Models;
using OFBCreator.Core.Services;

namespace OFBCreator.Core.Tests.Services;

/// <summary>
/// Comprehensive tests for AlphabeticalPlaceIndexGenerator.
/// </summary>
[TestClass]
public class AlphabeticalPlaceIndexGeneratorTests
{
    private ILogger<AlphabeticalPlaceIndexGenerator>? _logger;

    [TestInitialize]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<AlphabeticalPlaceIndexGenerator>>();
    }

    #region Null and empty inputs

    [TestMethod]
    public async Task GenerateAsync_NullFamilies_ThrowsArgumentNullException()
    {
        var generator = new AlphabeticalPlaceIndexGenerator(_logger);
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => generator.GenerateAsync(null!));
    }

    [TestMethod]
    public async Task GenerateAsync_EmptyFamilies_ReturnsEmptyList()
    {
        var generator = new AlphabeticalPlaceIndexGenerator(_logger);
        var result = await generator.GenerateAsync(Array.Empty<OFBFamilyModel>());
        CollectionAssert.AreEqual(Array.Empty<OFBIndexEntry>(), result.ToArray());
    }

    #endregion

    #region Single place extraction

    [TestMethod]
    public async Task GenerateAsync_MarriagePlace_ExtractedCorrectly()
    {
        var generator = new AlphabeticalPlaceIndexGenerator(_logger);

        var marriagePlace = Substitute.For<IGenPlace>();
        marriagePlace.Name.Returns("München");

        var husband = CreateMockPerson("Müller", "Hans", "I1");
        husband.BirthPlace.Returns((IGenPlace?)null!);
        husband.DeathPlace.Returns((IGenPlace?)null!);

        var wife = CreateMockPerson("Müller", "Anna", "I2");
        wife.BirthPlace.Returns((IGenPlace?)null!);
        wife.DeathPlace.Returns((IGenPlace?)null!);

        var family = new OFBFamilyModel
        {
            GlobalNumber = "00001", FamilyName = "Müller",
            Husband = husband, Wife = wife,
            Children = Array.Empty<IGenPerson>().AsReadOnly(),
            SourceRefId = "F1", MarriagePlace = marriagePlace
        };

        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("münchen", result[0].SortKey);
        Assert.AreEqual("München", result[0].Name);
        Assert.AreEqual("00001", result[0].Ref);
    }

    [TestMethod]
    public async Task GenerateAsync_BirthPlace_Extracted()
    {
        var generator = new AlphabeticalPlaceIndexGenerator(_logger);

        var birthPlace = Substitute.For<IGenPlace>();
        birthPlace.Name.Returns("Berlin");

        var husband = CreateMockPerson("Schmidt", "Peter", "I1");
        husband.BirthPlace.Returns(birthPlace);
        husband.DeathPlace.Returns((IGenPlace?)null!);

        var family = new OFBFamilyModel
        {
            GlobalNumber = "00002", FamilyName = "Schmidt",
            Husband = husband, Wife = null!,
            Children = Array.Empty<IGenPerson>().AsReadOnly(),
            SourceRefId = "F2"
        };

        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("berlin", result[0].SortKey);
    }

    [TestMethod]
    public async Task GenerateAsync_ChildBirthPlace_Extracted()
    {
        var generator = new AlphabeticalPlaceIndexGenerator(_logger);

        var childBirthPlace = Substitute.For<IGenPlace>();
        childBirthPlace.Name.Returns("Hamburg");

        var child = CreateMockPerson("Müller", "Karl", "I3");
        child.BirthPlace.Returns(childBirthPlace);

        var family = new OFBFamilyModel
        {
            GlobalNumber = "00003", FamilyName = "Müller",
            Husband = null!, Wife = null!,
            Children = new[] { child }.AsReadOnly(),
            SourceRefId = "F3"
        };

        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("hamburg", result[0].SortKey);
    }

    #endregion

    #region Sorting and normalization

    [TestMethod]
    public async Task GenerateAsync_MultiplePlaces_SortedAlphabetically()
    {
        var generator = new AlphabeticalPlaceIndexGenerator(_logger);

        var husband = CreateMockPerson("Ziegler", "Anton", "I1");
        var husbandDeathPlace = Substitute.For<IGenPlace>();
        husbandDeathPlace.Name.Returns("Zürich");
        husband.DeathPlace.Returns(husbandDeathPlace);

        var wife = CreateMockPerson("Adler", "Bernd", "I2");
        var wifeBirthPlace = Substitute.For<IGenPlace>();
        wifeBirthPlace.Name.Returns("Aachen");
        wife.BirthPlace.Returns(wifeBirthPlace);

        var family = new OFBFamilyModel
        {
            GlobalNumber = "00001", FamilyName = "Ziegler",
            Husband = husband, Wife = wife,
            Children = Array.Empty<IGenPerson>().AsReadOnly(),
            SourceRefId = "F1"
        };

        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("aachen", result[0].SortKey);
        Assert.AreEqual("Aachen", result[0].Name);
        Assert.AreEqual("zürich", result[1].SortKey);
        Assert.AreEqual("Zürich", result[1].Name);
    }

    [TestMethod]
    public async Task GenerateAsync_CaseInsensitive_MergesSamePlace()
    {
        var generator = new AlphabeticalPlaceIndexGenerator(_logger);

        var person1 = CreateMockPerson("Müller", "Hans", "I1");
        var place1 = Substitute.For<IGenPlace>();
        place1.Name.Returns("GUTSHAUS");
        person1.BirthPlace.Returns(place1);

        var person2 = CreateMockPerson("Schmidt", "Anna", "I2");
        var place2 = Substitute.For<IGenPlace>();
        place2.Name.Returns("gutshaus");
        person2.BirthPlace.Returns(place2);

        var family1 = new OFBFamilyModel
        { GlobalNumber = "00001", FamilyName = "Müller", Husband = person1, Wife = null!, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F1" };
        var family2 = new OFBFamilyModel
        { GlobalNumber = "00002", FamilyName = "Schmidt", Husband = null!, Wife = person2, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F2" };

        var result = await generator.GenerateAsync(new[] { family1, family2 });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("gutshaus", result[0].SortKey);
    }

    [TestMethod]
    public async Task GenerateAsync_TrimsWhitespace_NormalizesCorrectly()
    {
        var generator = new AlphabeticalPlaceIndexGenerator(_logger);

        var person = CreateMockPerson("Müller", "Hans", "I1");
        var place = Substitute.For<IGenPlace>();
        place.Name.Returns("  Frankfurt  ");
        person.BirthPlace.Returns(place);

        var family = new OFBFamilyModel
        { GlobalNumber = "00001", FamilyName = "Müller", Husband = person, Wife = null!, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F1" };

        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("frankfurt", result[0].SortKey);
        Assert.AreEqual("Frankfurt", result[0].Name);
    }

    #endregion

    #region Edge cases

    [TestMethod]
    public async Task GenerateAsync_EmptyPlaceName_Excluded()
    {
        var generator = new AlphabeticalPlaceIndexGenerator(_logger);

        var person = CreateMockPerson("Müller", "Hans", "I1");
        var place = Substitute.For<IGenPlace>();
        place.Name.Returns(string.Empty);
        person.BirthPlace.Returns(place);

        var family = new OFBFamilyModel
        { GlobalNumber = "00001", FamilyName = "Müller", Husband = person, Wife = null!, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F1" };

        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task GenerateAsync_NullPlaceName_Excluded()
    {
        var generator = new AlphabeticalPlaceIndexGenerator(_logger);

        var person = CreateMockPerson("Müller", "Hans", "I1");
        person.BirthPlace.Returns((IGenPlace?)null!);

        var family = new OFBFamilyModel
        { GlobalNumber = "00001", FamilyName = "Müller", Husband = person, Wife = null!, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F1" };

        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(0, result.Count);
    }

    #endregion

    #region Cancellation support

    [TestMethod]
    public async Task GenerateAsync_CancellationToken_ThrowsOnCancellation()
    {
        var generator = new AlphabeticalPlaceIndexGenerator(_logger);

        var families = new List<OFBFamilyModel>();
        for (int i = 0; i < 10; i++)
        {
            var person = CreateMockPerson("Name", "Person" + i, "I" + i);
            var place = Substitute.For<IGenPlace>();
            place.Name.Returns("Ort");
            person.BirthPlace.Returns(place);
            families.Add(new OFBFamilyModel
            { GlobalNumber = "0000" + i.ToString("D5"), FamilyName = "Name", Husband = person, Wife = null!, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F" + i });
        }

        var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => generator.GenerateAsync(families.ToArray(), cts.Token));
    }

    #endregion

    #region Helper methods

    private static IGenPerson CreateMockPerson(string surname, string givenName, string xid)
    {
        var person = Substitute.For<IGenPerson>();
        person.Surname.Returns(surname);
        person.GivenName.Returns(givenName);
        person.Name.Returns(!string.IsNullOrEmpty(surname) ? surname + " " + givenName : givenName);
        person.IndRefID.Returns(xid);
        person.BirthDate.Returns((IGenDate?)null!);
        person.DeathDate.Returns((IGenDate?)null!);
        person.BirthPlace.Returns((IGenPlace?)null!);
        person.DeathPlace.Returns((IGenPlace?)null!);
        return person;
    }

    #endregion
}
