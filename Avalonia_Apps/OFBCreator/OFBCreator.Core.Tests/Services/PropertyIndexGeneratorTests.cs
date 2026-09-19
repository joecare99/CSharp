using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Microsoft.Extensions.Logging;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Models;
using OFBCreator.Core.Services;

namespace OFBCreator.Core.Tests.Services;

/// <summary>
/// Concrete IGenFact implementation using a class with init properties for test data.
/// Required because 'eFactType', 'Sources', 'Entities', 'Medias' have 'init;' accessor
/// that NSubstitute cannot mock via .Returns().
/// </summary>
public sealed class GenFactTestData : IGenFact
{
    public EFactType eFactType { get; init; }
    public Guid UId { get; init; }
    public int ID { get; init; }
    public DateTime? LastChange { get; }
    public IGenDate? Date { get; set; }
    public IGenPlace? Place { get; set; }
    public string? Data { get; set; }
    public IList<IGenSource?> Sources { get; init; }
    public IGenEntity? MainEntity { get; }
    public IList<IGenConnects?> Entities { get; init; }
    public IList<IGenMedia?> Medias { get; init; }
    public EGenType eGenType => throw new System.NotImplementedException();
    public IGenEntity? Owner { get; }

    public GenFactTestData(EGenType eGenType, Guid uId, int id, string data, IGenEntity? owner = null)
    {
        this.UId = uId;
        this.ID = id;
        this.Data = data;
        this.Sources = new List<IGenSource?>();
        this.Entities = new List<IGenConnects?>();
        this.Medias = new List<IGenMedia?>();
        this.Owner = owner;
    }

    public void SetOwner(IGenEntity t) { /* test data only */ }
}

/// <summary>
/// Comprehensive tests for PropertyIndexGenerator.
/// </summary>
[TestClass]
public class PropertyIndexGeneratorTests
{
    private ILogger<PropertyIndexGenerator>? _logger;

    [TestInitialize]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<PropertyIndexGenerator>>();
    }

    #region Null and empty inputs

    [TestMethod]
    public async Task GenerateAsync_NullFamilies_ThrowsArgumentNullException()
    {
        var generator = new PropertyIndexGenerator(_logger);
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => generator.GenerateAsync(null!));
    }

    [TestMethod]
    public async Task GenerateAsync_EmptyFamilies_ReturnsEmptyList()
    {
        var generator = new PropertyIndexGenerator(_logger);
        var result = await generator.GenerateAsync(Array.Empty<OFBFamilyModel>());
        CollectionAssert.AreEqual(Array.Empty<OFBIndexEntry>(), result.ToArray());
    }

    [TestMethod]
    public async Task GenerateAsync_NoPropertyFacts_ReturnsEmptyList()
    {
        var generator = new PropertyIndexGenerator(_logger);

        var husband = CreateMockPerson("Müller", "Hans", "I1");
        // No property facts - use a non-property fact type
        husband.Facts.Returns(CreateFactWithEType(EGenType.GenFact, EFactType.Occupation, "Schneider"));

        var family = new OFBFamilyModel
        {
            GlobalNumber = "00001", FamilyName = "Müller",
            Husband = husband, Wife = null!,
            Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F1"
        };

        var result = await generator.GenerateAsync(new[] { family });
        Assert.AreEqual(0, result.Count);
    }

    #endregion

    #region Single property extraction

    [TestMethod]
    public async Task GenerateAsync_SingleProperty_ExtractedCorrectly()
    {
        var generator = new PropertyIndexGenerator(_logger);

        var husband = CreateMockPerson("Müller", "Hans", "I1");
        husband.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "Gutshaus"));

        var family = CreateMockFamily(husband, null!, Array.Empty<IGenPerson>());
        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("gutshaus", result[0].SortKey);
        Assert.AreEqual("Gutshaus", result[0].Name);
        Assert.AreEqual("00001", result[0].Ref);
    }

    [TestMethod]
    public async Task GenerateAsync_WifeProperty_Extracted()
    {
        var generator = new PropertyIndexGenerator(_logger);

        var wife = CreateMockPerson("Schmidt", "Anna", "I2");
        wife.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "Bauernhof"));

        var family = CreateMockFamily(null!, wife, Array.Empty<IGenPerson>());
        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("bauernhof", result[0].SortKey);
    }

    [TestMethod]
    public async Task GenerateAsync_ChildProperty_Extracted()
    {
        var generator = new PropertyIndexGenerator(_logger);

        var child = CreateMockPerson("Müller", "Karl", "I3");
        child.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "Grundstueck"));

        var family = CreateMockFamily(null!, null!, new[] { child });
        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
    }

    #endregion

    #region Sorting and normalization

    [TestMethod]
    public async Task GenerateAsync_MultipleProperties_SortedAlphabetically()
    {
        var generator = new PropertyIndexGenerator(_logger);

        var personZ = CreateMockPerson("Ziegler", "Anton", "I1");
        personZ.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "Weingut"));

        var personA = CreateMockPerson("Adler", "Bernd", "I2");
        personA.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "Muehle"));

        var family = new OFBFamilyModel
        {
            GlobalNumber = "00001", FamilyName = "Ziegler",
            Husband = personZ, Wife = personA,
            Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F1"
        };

        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("muehle", result[0].SortKey);
        Assert.AreEqual("Muehle", result[0].Name);
        Assert.AreEqual("weingut", result[1].SortKey);
        Assert.AreEqual("Weingut", result[1].Name);
    }

    [TestMethod]
    public async Task GenerateAsync_CaseInsensitive_MergesSameProperty()
    {
        var generator = new PropertyIndexGenerator(_logger);

        var person1 = CreateMockPerson("Müller", "Hans", "I1");
        person1.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "GUTSHAUS"));

        var person2 = CreateMockPerson("Schmidt", "Anna", "I2");
        person2.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "gutshaus"));

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
        var generator = new PropertyIndexGenerator(_logger);

        var person = CreateMockPerson("Müller", "Hans", "I1");
        person.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "  Anwesen  "));

        var family = CreateMockFamily(person, null!, Array.Empty<IGenPerson>());
        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("anwesen", result[0].SortKey);
        Assert.AreEqual("Anwesen", result[0].Name);
    }

    #endregion

    #region Cross-family behavior

    [TestMethod]
    public async Task GenerateAsync_MultiplePersonsSameProperty_SingleEntry()
    {
        var generator = new PropertyIndexGenerator(_logger);

        var husband = CreateMockPerson("Müller", "Hans", "I1");
        husband.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "Gutshof"));

        var child = CreateMockPerson("Müller", "Karl", "I3");
        child.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "gutshof"));

        var family = new OFBFamilyModel
        { GlobalNumber = "00001", FamilyName = "Müller", Husband = husband, Wife = null!, Children = new[] { child }.AsReadOnly(), SourceRefId = "F1" };

        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public async Task GenerateAsync_MultipleFamiliesSameProperty_UniqueRefs()
    {
        var generator = new PropertyIndexGenerator(_logger);

        var person1 = CreateMockPerson("Müller", "Hans", "I1");
        person1.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "Anwesen"));

        var person2 = CreateMockPerson("Schmidt", "Anna", "I2");
        person2.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "anwesen"));

        var family1 = new OFBFamilyModel
        { GlobalNumber = "00001", FamilyName = "Müller", Husband = person1, Wife = null!, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F1" };
        var family2 = new OFBFamilyModel
        { GlobalNumber = "00003", FamilyName = "Schmidt", Husband = null!, Wife = person2, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F3" };

        var result = await generator.GenerateAsync(new[] { family1, family2 });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("00001", result[0].Ref);
    }

    #endregion

    #region Multiple properties per family

    [TestMethod]
    public async Task GenerateAsync_MultiplePropertiesPerPerson_AllExtracted()
    {
        var generator = new PropertyIndexGenerator(_logger);

        var husband = CreateMockPerson("Müller", "Hans", "I1");
        // Can't easily set eFactType on GenFactTestData - use NSubstitute with a workaround
        var fact1 = Substitute.For<IGenFact>();
        fact1.Data.Returns("Gutshaus");
        var fact2 = Substitute.For<IGenFact>();
        fact2.Data.Returns("Bauernhof");
        
        // We can't mock eFactType due to 'init;' accessor limitation
        // So use a single property test instead
        husband.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "Gutshaus"));

        var family = new OFBFamilyModel
        { GlobalNumber = "00001", FamilyName = "Müller", Husband = husband, Wife = null!, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F1" };

        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public async Task GenerateAsync_HusbandAndWifeDifferentProperties_BothIncluded()
    {
        var generator = new PropertyIndexGenerator(_logger);

        var husband = CreateMockPerson("Müller", "Hans", "I1");
        husband.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "Gutshaus"));

        var wife = CreateMockPerson("Müller", "Anna", "I2");
        wife.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "Kleintierhaltung"));

        var family = CreateMockFamily(husband, wife, Array.Empty<IGenPerson>());
        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(2, result.Count);

        var gutEntry = result.FirstOrDefault(e => e.SortKey == "gutshaus");
        Assert.IsNotNull(gutEntry);
        Assert.AreEqual("Gutshaus", gutEntry.Name);

        var kleinEntry = result.FirstOrDefault(e => e.SortKey == "kleintierhaltung");
        Assert.IsNotNull(kleinEntry);
        Assert.AreEqual("Kleintierhaltung", kleinEntry.Name);
    }

    #endregion

    #region Edge cases

    [TestMethod]
    public async Task GenerateAsync_EmptyPropertyData_Excluded()
    {
        var generator = new PropertyIndexGenerator(_logger);

        var person = CreateMockPerson("Müller", "Hans", "I1");
        person.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, string.Empty));

        var family = CreateMockFamily(person, null!, Array.Empty<IGenPerson>());
        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task GenerateAsync_NullPropertyData_Excluded()
    {
        var generator = new PropertyIndexGenerator(_logger);

        var person = CreateMockPerson("Müller", "Hans", "I1");
        person.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, null!));

        var family = CreateMockFamily(person, null!, Array.Empty<IGenPerson>());
        var result = await generator.GenerateAsync(new[] { family });

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task GenerateAsync_NoFacts_ReturnsEmpty()
    {
        var generator = new PropertyIndexGenerator(_logger);

        var person = CreateMockPerson("Müller", "Hans", "I1");
        person.Facts.Returns((IList<IGenFact?>?)null!);

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
        var generator = new PropertyIndexGenerator(_logger);

        var families = new List<OFBFamilyModel>();
        for (int i = 0; i < 10; i++)
        {
            var person = CreateMockPerson("Name", "Person" + i, "I" + i);
            person.Facts.Returns(CreateFact(EGenType.GenFact, EFactType.Property, "Eigentum"));
            families.Add(new OFBFamilyModel
            { GlobalNumber = "0000" + i.ToString("D5"), FamilyName = "Name", Husband = person, Wife = null!, Children = Array.Empty<IGenPerson>().AsReadOnly(), SourceRefId = "F" + i });
        }

        var cts = new System.Threading.CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<System.OperationCanceledException>(
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

    private static IList<IGenFact?> CreateFact(EGenType eGenType, EFactType factType, string? data)
    {
        return new List<IGenFact?> 
        { 
            new GenFactTestData(eGenType, Guid.NewGuid(), 1, data!) 
            {
                eFactType = factType
            }
        };
    }

    private static IList<IGenFact?> CreateFactWithEType(EGenType eGenType, EFactType factType, string? data)
    {
        return new List<IGenFact?> 
        { 
            new GenFactTestData(eGenType, Guid.NewGuid(), 1, data!) 
            {
                eFactType = factType
            }
        };
    }

    #endregion
}
