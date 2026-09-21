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
/// Comprehensive tests for HierarchicalPlaceIndexGenerator.
/// </summary>
[TestClass]
public class HierarchicalPlaceIndexGeneratorTests
{
    private ILogger<HierarchicalPlaceIndexGenerator>? _logger;

    [TestInitialize]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<HierarchicalPlaceIndexGenerator>>();
    }

    #region Null and empty inputs

    [TestMethod]
    public async Task GenerateAsync_NullFamilies_ThrowsArgumentNullException()
    {
        var generator = new HierarchicalPlaceIndexGenerator(_logger);
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => generator.GenerateAsync(null!));
    }

    [TestMethod]
    public async Task GenerateAsync_EmptyFamilies_ReturnsEmptyList()
    {
        var generator = new HierarchicalPlaceIndexGenerator(_logger);
        var result = await generator.GenerateAsync(Array.Empty<OFBFamilyModel>());
        Assert.AreEqual(0, result.Count);
    }

    #endregion

    #region Single place extraction

    [TestMethod]
    public async Task GenerateAsync_SimplePlace_ReturnsSingleNode()
    {
        var generator = new HierarchicalPlaceIndexGenerator(_logger);

        var place = CreatePlace("München", "PLACE_MUC");
        var husband = CreateMockPerson("Müller", "Hans", "I1");
        husband.BirthPlace.Returns(place);

        var family = new OFBFamilyModel
        {
            GlobalNumber = "00001", FamilyName = "Müller",
            Husband = husband, Wife = null!,
            Children = Array.Empty<IGenPerson>().AsReadOnly(),
            SourceRefId = "F1"
        };

        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("München", result[0].Name);
        Assert.AreEqual("PLACE_MUC", result[0].PlaceId);
    }

    [TestMethod]
    public async Task GenerateAsync_PlaceWithParent_CreatesHierarchy()
    {
        var generator = new HierarchicalPlaceIndexGenerator(_logger);

        var city = CreatePlace("München", "PLACE_MUC");
        var state = CreatePlace("Bayern", "PLACE_BY");
        state.Parent.Returns((IGenPlace?)null!);
        city.Parent.Returns(state);

        var husband = CreateMockPerson("Müller", "Hans", "I1");
        husband.BirthPlace.Returns(city);

        var family = new OFBFamilyModel
        {
            GlobalNumber = "00001", FamilyName = "Müller",
            Husband = husband, Wife = null!,
            Children = Array.Empty<IGenPerson>().AsReadOnly(),
            SourceRefId = "F1"
        };

        var result = await generator.GenerateAsync(new[] { family });

        // Root node should be the state (Bayern)
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Bayern", result[0].Name);
        Assert.AreEqual(1, result[0].Children.Length);
        Assert.AreEqual("München", result[0].Children[0].Name);
        Assert.AreEqual("PLACE_MUC", result[0].Children[0].PlaceId);
    }

    [TestMethod]
    public async Task GenerateAsync_FullPlaceHierarchy_CreatesThreeLevelTree()
    {
        var generator = new HierarchicalPlaceIndexGenerator(_logger);

        var country = CreatePlace("Deutschland", "DE");
        var state = CreatePlace("Bayern", "BY");
        var county = CreatePlace("Oberbayern", "OB");
        var city = CreatePlace("München", "MUC");

        country.Parent.Returns((IGenPlace?)null!);  // Root
        state.Parent.Returns(country);               // Bayern → Deutschland
        county.Parent.Returns(state);                // Oberbayern → Bayern
        city.Parent.Returns(county);                 // München → Oberbayern

        var husband = CreateMockPerson("Müller", "Hans", "I1");
        husband.BirthPlace.Returns(city);

        var family = new OFBFamilyModel
        {
            GlobalNumber = "00001", FamilyName = "Müller",
            Husband = husband, Wife = null!,
            Children = Array.Empty<IGenPerson>().AsReadOnly(),
            SourceRefId = "F1"
        };

        var result = await generator.GenerateAsync(new[] { family });

        // Root: Deutschland → Bayern → Oberbayern → München (4-stufig)
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Deutschland", result[0].Name);
        Assert.AreEqual(1, result[0].Children.Length);
        Assert.AreEqual("Bayern", result[0].Children[0].Name);
        Assert.AreEqual(1, result[0].Children[0].Children.Length);
        Assert.AreEqual("Oberbayern", result[0].Children[0].Children[0].Name);
        Assert.AreEqual(1, result[0].Children[0].Children[0].Children.Length);
        Assert.AreEqual("München", result[0].Children[0].Children[0].Children[0].Name);
    }

    #endregion

    #region Multiple places merging

    [TestMethod]
    public async Task GenerateAsync_SamePlaceMultipleFamilies_MergesNodes()
    {
        var generator = new HierarchicalPlaceIndexGenerator(_logger);

        var place1 = CreatePlace("Berlin", "BER1");
        var place2 = CreatePlace("Berlin", "BER2"); // Same name, different ID (shouldn't happen in practice)

        var husband1 = CreateMockPerson("Müller", "Hans", "I1");
        husband1.BirthPlace.Returns(place1);

        var husband2 = CreateMockPerson("Schmidt", "Peter", "I2");
        husband2.BirthPlace.Returns(place2);

        var family1 = new OFBFamilyModel
        { GlobalNumber = "00001", FamilyName = "Müller", Husband = husband1, Wife = null!, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F1" };
        var family2 = new OFBFamilyModel
        { GlobalNumber = "00002", FamilyName = "Schmidt", Husband = husband2, Wife = null!, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F2" };

        var result = await generator.GenerateAsync(new[] { family1, family2 });

        // Should have two separate entries since they have different IDs
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public async Task GenerateAsync_MultipleUniquePlaces_AllIncluded()
    {
        var generator = new HierarchicalPlaceIndexGenerator(_logger);

        var place1 = CreatePlace("München", "MUC");
        var place2 = CreatePlace("Berlin", "BER");

        var husband1 = CreateMockPerson("Müller", "Hans", "I1");
        husband1.BirthPlace.Returns(place1);

        var husband2 = CreateMockPerson("Schmidt", "Peter", "I2");
        husband2.BirthPlace.Returns(place2);

        var family1 = new OFBFamilyModel
        { GlobalNumber = "00001", FamilyName = "Müller", Husband = husband1, Wife = null!, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F1" };
        var family2 = new OFBFamilyModel
        { GlobalNumber = "00002", FamilyName = "Schmidt", Husband = husband2, Wife = null!, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F2" };

        var result = await generator.GenerateAsync(new[] { family1, family2 });

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(n => n.Name == "Berlin"));
        Assert.IsTrue(result.Any(n => n.Name == "München"));
    }

    #endregion

    #region Place ID fallback

    [TestMethod]
    public async Task GenerateAsync_PlaceWithoutGOV_ID_UsesName()
    {
        var generator = new HierarchicalPlaceIndexGenerator(_logger);

        var place = CreatePlace("Erfurt", null!);
        var husband = CreateMockPerson("Müller", "Hans", "I1");
        husband.BirthPlace.Returns(place);

        var family = new OFBFamilyModel
        {
            GlobalNumber = "00001", FamilyName = "Müller",
            Husband = husband, Wife = null!,
            Children = Array.Empty<IGenPerson>().AsReadOnly(),
            SourceRefId = "F1"
        };

        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Erfurt", result[0].PlaceId); // Should use name as ID
    }

    #endregion

    #region Cancellation support

    [TestMethod]
    public async Task GenerateAsync_CancellationToken_ThrowsOnCancellation()
    {
        var generator = new HierarchicalPlaceIndexGenerator(_logger);

        var families = new List<OFBFamilyModel>();
        for (int i = 0; i < 10; i++)
        {
            var person = CreateMockPerson("Name", "Person" + i, "I" + i);
            var place = CreatePlace("Ort" + i, "LOC" + i);
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

    private static IGenPlace CreatePlace(string name, string? govId)
    {
        var place = Substitute.For<IGenPlace>();
        place.Name.Returns(name);
        place.GOV_ID.Returns(govId);
        place.Parent.Returns((IGenPlace?)null!);
        return place;
    }

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
