using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Microsoft.Extensions.Logging;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Models;
using OFBCreator.Core.Services;

namespace OFBCreator.Core.Tests.Services;

/// <summary>
/// Comprehensive tests for OccupationIndexGenerator.
/// </summary>
[TestClass]
public class OccupationIndexGeneratorTests
{
    private ILogger<OccupationIndexGenerator>? _logger;

    [TestInitialize]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<OccupationIndexGenerator>>();
    }

    #region Null and empty inputs

    [TestMethod]
    public async Task GenerateAsync_NullFamilies_ThrowsArgumentNullException()
    {
        var generator = new OccupationIndexGenerator(_logger);
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => generator.GenerateAsync(null!));
    }

    [TestMethod]
    public async Task GenerateAsync_EmptyFamilies_ReturnsEmptyList()
    {
        var generator = new OccupationIndexGenerator(_logger);
        var result = await generator.GenerateAsync(Array.Empty<OFBFamilyModel>());
        CollectionAssert.AreEqual(Array.Empty<OFBIndexEntry>(), result.ToArray());
    }

    [TestMethod]
    public async Task GenerateAsync_NoOccupations_ReturnsEmptyList()
    {
        var generator = new OccupationIndexGenerator(_logger);
        
        var husband = CreateMockPerson("Müller", "Hans", "I1");
        husband.Occupation.Returns((string?)null!);
        
        var wife = CreateMockPerson("Müller", "Anna", "I2");
        wife.Occupation.Returns(string.Empty);
        
        var family = new OFBFamilyModel
        {
            GlobalNumber = "00001", FamilyName = "Müller",
            Husband = husband, Wife = wife,
            Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F1"
        };

        var result = await generator.GenerateAsync(new[] { family });
        Assert.AreEqual(0, result.Count);
    }

    #endregion

    #region Single occupation extraction

    [TestMethod]
    public async Task GenerateAsync_SingleOccupation_ExtractedCorrectly()
    {
        var generator = new OccupationIndexGenerator(_logger);
        
        var husband = CreateMockPerson("Müller", "Hans", "I1");
        husband.Occupation.Returns("Schneider");
        
        var family = CreateMockFamily(husband, null!, Array.Empty<IGenPerson>());
        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("schneider", result[0].SortKey);
        Assert.AreEqual("Schneider", result[0].Name);
        Assert.AreEqual("00001", result[0].Ref);
    }

    [TestMethod]
    public async Task GenerateAsync_WifeOccupation_Extracted()
    {
        var generator = new OccupationIndexGenerator(_logger);
        
        var wife = CreateMockPerson("Schmidt", "Anna", "I2");
        wife.Occupation.Returns("Köchin");
        
        var family = CreateMockFamily(null!, wife, Array.Empty<IGenPerson>());
        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("köchin", result[0].SortKey);
    }

    #endregion

    #region Sorting and normalization

    [TestMethod]
    public async Task GenerateAsync_MultipleOccupations_SortedAlphabetically()
    {
        var generator = new OccupationIndexGenerator(_logger);
        
        var zPerson = CreateMockPerson("Ziegler", "Anton", "I1");
        zPerson.Occupation.Returns("Zimmerer");
        
        var aPerson = CreateMockPerson("Adler", "Bernd", "I2");
        aPerson.Occupation.Returns("Bäcker");
        
        var mPerson = CreateMockPerson("Müller", "Carl", "I3");
        mPerson.Occupation.Returns("Schneider");

        var family = new OFBFamilyModel
        {
            GlobalNumber = "00001", FamilyName = "Ziegler",
            Husband = zPerson, Wife = aPerson,
            Children = new[] { mPerson }.AsReadOnly(), SourceRefId = "F1"
        };

        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("bäcker", result[0].SortKey);
        Assert.AreEqual("Bäcker", result[0].Name);
        Assert.AreEqual("schneider", result[1].SortKey);
        Assert.AreEqual("Schneider", result[1].Name);
        Assert.AreEqual("zimmerer", result[2].SortKey);
        Assert.AreEqual("Zimmerer", result[2].Name);
    }

    [TestMethod]
    public async Task GenerateAsync_CaseInsensitive_MergesSameOccupation()
    {
        var generator = new OccupationIndexGenerator(_logger);
        
        var person1 = CreateMockPerson("Müller", "Hans", "I1");
        person1.Occupation.Returns("SCHNEIDER");
        
        var person2 = CreateMockPerson("Schmidt", "Anna", "I2");
        person2.Occupation.Returns("schneider");

        var family1 = new OFBFamilyModel
        { GlobalNumber = "00001", FamilyName = "Müller", Husband = person1, Wife = null!, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F1" };
        var family2 = new OFBFamilyModel
        { GlobalNumber = "00002", FamilyName = "Schmidt", Husband = null!, Wife = person2, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F2" };

        var result = await generator.GenerateAsync(new[] { family1, family2 });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("schneider", result[0].SortKey);
    }

    [TestMethod]
    public async Task GenerateAsync_TrimsWhitespace_NormalizesCorrectly()
    {
        var generator = new OccupationIndexGenerator(_logger);
        
        var person = CreateMockPerson("Müller", "Hans", "I1");
        person.Occupation.Returns("  Schneider  ");

        var family = CreateMockFamily(person, null!, Array.Empty<IGenPerson>());
        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("schneider", result[0].SortKey);
        Assert.AreEqual("Schneider", result[0].Name);
    }

    #endregion

    #region Cross-family behavior

    [TestMethod]
    public async Task GenerateAsync_MultiplePersonsSameOccupation_SingleEntry()
    {
        var generator = new OccupationIndexGenerator(_logger);
        
        var husband = CreateMockPerson("Müller", "Hans", "I1");
        husband.Occupation.Returns("Schneider");
        
        var child = CreateMockPerson("Müller", "Karl", "I3");
        child.Occupation.Returns("Schneider");

        var family = new OFBFamilyModel
        { GlobalNumber = "00001", FamilyName = "Müller", Husband = husband, Wife = null!, Children = new[] { child }.AsReadOnly(), SourceRefId = "F1" };

        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public async Task GenerateAsync_MultipleFamiliesSameOccupation_UniqueRefs()
    {
        var generator = new OccupationIndexGenerator(_logger);
        
        var person1 = CreateMockPerson("Müller", "Hans", "I1");
        person1.Occupation.Returns("Schneider");
        
        var person2 = CreateMockPerson("Schmidt", "Anna", "I2");
        person2.Occupation.Returns("Schneider");

        var family1 = new OFBFamilyModel
        { GlobalNumber = "00001", FamilyName = "Müller", Husband = person1, Wife = null!, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F1" };
        var family2 = new OFBFamilyModel
        { GlobalNumber = "00003", FamilyName = "Schmidt", Husband = null!, Wife = person2, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F3" };

        var result = await generator.GenerateAsync(new[] { family1, family2 });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("00001", result[0].Ref);
    }

    #endregion

    #region Multiple occupations per family

    [TestMethod]
    public async Task GenerateAsync_HusbandAndWifeDifferentOccupations_BothIncluded()
    {
        var generator = new OccupationIndexGenerator(_logger);
        
        var husband = CreateMockPerson("Müller", "Hans", "I1");
        husband.Occupation.Returns("Schneider");
        
        var wife = CreateMockPerson("Müller", "Anna", "I2");
        wife.Occupation.Returns("Köchin");

        var family = CreateMockFamily(husband, wife, Array.Empty<IGenPerson>());
        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(2, result.Count);
        
        var schneiderEntry = result.FirstOrDefault(e => e.SortKey == "schneider");
        Assert.IsNotNull(schneiderEntry);
        Assert.AreEqual("Schneider", schneiderEntry.Name);
        
        var kochinEntry = result.FirstOrDefault(e => e.SortKey == "köchin");
        Assert.IsNotNull(kochinEntry);
        Assert.AreEqual("Köchin", kochinEntry.Name);
    }

    [TestMethod]
    public async Task GenerateAsync_ChildHasOccupation_AllExtracted()
    {
        var generator = new OccupationIndexGenerator(_logger);
        
        var husband = CreateMockPerson("Müller", "Hans", "I1");
        husband.Occupation.Returns("Schneider");
        
        var child = CreateMockPerson("Müller", "Karl", "I3");
        child.Occupation.Returns("Lehrer");

        var family = CreateMockFamily(husband, null!, new[] { child });
        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(2, result.Count);
    }

    #endregion

    #region Cancellation support

    [TestMethod]
    public async Task GenerateAsync_CancellationToken_ThrowsOnCancellation()
    {
        var generator = new OccupationIndexGenerator(_logger);
        
        var families = Enumerable.Range(0, 10)
            .Select(i =>
            {
                var person = CreateMockPerson("Name", "Person" + i, "I" + i);
                person.Occupation.Returns("Beruf");
                return new OFBFamilyModel
                { GlobalNumber = "0000" + i.ToString("D5"), FamilyName = "Name", Husband = person, Wife = null!, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F" + i };
            })
            .ToArray();

        var cts = new System.Threading.CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<System.OperationCanceledException>(
            () => generator.GenerateAsync(families, cts.Token));
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
        return person;
    }

    private static OFBFamilyModel CreateMockFamily(IGenPerson? husband, IGenPerson? wife, IReadOnlyList<IGenPerson>? children = null)
    {
        string familyName = !string.IsNullOrEmpty(husband?.Surname) ? husband!.Surname : (wife?.Surname ?? "Unknown");
        return new OFBFamilyModel
        {
            GlobalNumber = "00001", FamilyName = familyName,
            Husband = husband, Wife = wife,
            Children = children != null ? (IReadOnlyList<IGenPerson>)children : Array.Empty<IGenPerson>().AsReadOnly(),
            SourceRefId = "F1"
        };
    }

    #endregion
}
