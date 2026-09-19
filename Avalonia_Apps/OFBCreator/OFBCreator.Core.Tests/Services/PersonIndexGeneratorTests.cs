using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Microsoft.Extensions.Logging;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Abstractions.Models;
using OFBCreator.Core.Services;

namespace OFBCreator.Core.Tests.Services;

/// <summary>
/// Comprehensive tests for <see cref="PersonIndexGenerator"/>.
/// </summary>
[TestClass]
public class PersonIndexGeneratorTests
{
    private ILogger<PersonIndexGenerator>? _logger;

    [TestInitialize]
    public void Initialize()
    {
        _logger = Substitute.For<ILogger<PersonIndexGenerator>>();
    }

    #region Null and empty inputs

    [TestMethod]
    public void GenerateAsync_NullFamilies_ThrowsArgumentNullException()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);

        // Act & Assert
        var ex = Assert.Throws<System.AggregateException>(
            () => { _ = generator.GenerateAsync(null!).Result; });
        Assert.IsInstanceOfType(ex.InnerException ?? ex, typeof(System.ArgumentNullException));
    }

    [TestMethod]
    public async Task GenerateAsync_EmptyFamilies_ReturnsEmptyList()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        var emptyFamilies = Array.Empty<OFBFamilyModel>();

        // Act
        var result = await generator.GenerateAsync(emptyFamilies);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    #endregion

    #region Single family tests

    [TestMethod]
    public async Task GenerateAsync_SingleFamily_ExtractsHusbandAndWife()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        var husband = CreateMockPerson("Müller", "Hans", "I1");
        var wife = CreateMockPerson("Müller", "Anna", "I2");
        var family = CreateMockFamily(husband, wife, Array.Empty<IGenPerson>());

        // Act
        var result = await generator.GenerateAsync(new[] { family });

        // Assert - sorted alphabetically by full name within same surname
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Müller, Anna", result[0].Name);
        Assert.AreEqual("Müller, Hans", result[1].Name);

        // Verify life dates are null (mocks have no explicit dates set)
        var hansEntry = result.First(e => e.Name == "Müller, Hans");
        var annaEntry = result.First(e => e.Name == "Müller, Anna");
        Assert.IsNull(hansEntry.BirthDate);
        Assert.IsNull(hansEntry.DeathDate);
        Assert.IsNull(annaEntry.BirthDate);
        Assert.IsNull(annaEntry.DeathDate);
        Assert.IsNull(result[0].BirthDate);
        Assert.IsNull(result[0].DeathDate);
    }

    [TestMethod]
    public async Task GenerateAsync_SingleFamily_HusbandSpouseFamilyCorrect()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        var husband = CreateMockPerson("Müller", "Hans", "I1");
        var wife = CreateMockPerson("Müller", "Anna", "I2");
        var family = CreateMockFamily(husband, wife, Array.Empty<IGenPerson>());

        // Act
        var result = await generator.GenerateAsync(new[] { family });

        // Assert - husband has this family as spouse (formation family)
        var husbandEntry = result.First(e => e.Name == "Müller, Hans");
        CollectionAssert.AreEqual(new[] { "00001" }, husbandEntry.SpouseFamilyNumbers.ToArray());
    }

    [TestMethod]
    public async Task GenerateAsync_SingleFamily_WifeSpouseFamilyCorrect()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        var husband = CreateMockPerson("Müller", "Hans", "I1");
        var wife = CreateMockPerson("Müller", "Anna", "I2");
        var family = CreateMockFamily(husband, wife, Array.Empty<IGenPerson>());

        // Act
        var result = await generator.GenerateAsync(new[] { family });

        // Assert - wife has this family as spouse (formation family)
        var wifeEntry = result.First(e => e.Name == "Müller, Anna");
        CollectionAssert.AreEqual(new[] { "00001" }, wifeEntry.SpouseFamilyNumbers.ToArray());
    }

    [TestMethod]
    public async Task GenerateAsync_SingleFamilyWithChild_ChildParentFamilyCorrect()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        var husband = CreateMockPerson("Müller", "Hans", "I1");
        var wife = CreateMockPerson("Müller", "Anna", "I2");
        var child = CreateMockPerson("Müller", "Karl", "I3");
        var family = CreateMockFamily(husband, wife, new[] { child });

        // Act
        var result = await generator.GenerateAsync(new[] { family });

        // Assert - child has this family as parent
        var childEntry = result.First(e => e.Name == "Müller, Karl");
        CollectionAssert.AreEqual(new[] { "00001" }, childEntry.ParentFamilyNumbers.ToArray());
    }

    [TestMethod]
    public async Task GenerateAsync_SingleFamilyWithChild_ChildSpouseFamilyEmpty()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        var husband = CreateMockPerson("Müller", "Hans", "I1");
        var wife = CreateMockPerson("Müller", "Anna", "I2");
        var child = CreateMockPerson("Müller", "Karl", "I3");
        var family = CreateMockFamily(husband, wife, new[] { child });

        // Act
        var result = await generator.GenerateAsync(new[] { family });

        // Assert - child should NOT have this family as spouse
        var childEntry = result.First(e => e.Name == "Müller, Karl");
        Assert.AreEqual(0, childEntry.SpouseFamilyNumbers.Count);
    }

    [TestMethod]
    public async Task GenerateAsync_SingleFamily_AllPersonCountCorrect()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        var husband = CreateMockPerson("Müller", "Hans", "I1");
        var wife = CreateMockPerson("Müller", "Anna", "I2");
        var child1 = CreateMockPerson("Müller", "Karl", "I3");
        var child2 = CreateMockPerson("Müller", "Lina", "I4");
        var family = CreateMockFamily(husband, wife, new[] { child1, child2 });

        // Act
        var result = await generator.GenerateAsync(new[] { family });

        // Assert
        Assert.AreEqual(4, result.Count); // husband + wife + 2 children
    }

    #endregion

    #region Multiple families tests

    [TestMethod]
    public async Task GenerateAsync_MultipleFamilies_PersonAppearsOnce()
    {
        // Arrange: husband remarries - same person appears in two families as husband
        var generator = new PersonIndexGenerator(_logger);
        var husband = CreateMockPerson("Müller", "Hans", "I1");
        var wife1 = CreateMockPerson("Weber", "Greta", "I2");
        var family1 = new OFBFamilyModel
        {
            GlobalNumber = "00001",
            FamilyName = "Müller",
            Husband = husband,
            Wife = wife1,
            Children = Array.Empty<IGenPerson>().AsReadOnly(),
            SourceRefId = "F1"
        };

        // Second family with same husband (same reference ID)
        var wife2 = CreateMockPerson("Schmidt", "Maria", "I3");
        var family2 = new OFBFamilyModel
        {
            GlobalNumber = "00002",
            FamilyName = "Müller",
            Husband = husband, // Same husband reference!
            Wife = wife2,
            Children = new[] { CreateMockPerson("Müller", "Tom", "I4") }.AsReadOnly(),
            SourceRefId = "F2"
        };

        // Act
        var result = await generator.GenerateAsync(new[] { family1, family2 });

        // Assert - Hans Müller should appear only ONCE in the index
        var hansEntries = result.Where(e => e.Name == "Müller, Hans").ToList();
        Assert.AreEqual(1, hansEntries.Count);

        // His spouse families should include both
        var hansEntry = hansEntries[0];
        CollectionAssert.AreEqual(new[] { "00001", "00002" }, hansEntry.SpouseFamilyNumbers.ToArray());
    }

    [TestMethod]
    public async Task GenerateAsync_MultipleFamilies_AllUniquePersonsExtracted()
    {
        // Arrange: two unrelated families
        var generator = new PersonIndexGenerator(_logger);
        
        var family1 = CreateMockFamily(
            husband: CreateMockPerson("Müller", "Hans", "I1"),
            wife: CreateMockPerson("Müller", "Anna", "I2"),
            children: new[] { CreateMockPerson("Müller", "Karl", "I3") });

        var family2 = CreateMockFamily(
            husband: CreateMockPerson("Schmidt", "Fritz", "I4"),
            wife: CreateMockPerson("Schmidt", "Else", "I5"),
            children: new[] { CreateMockPerson("Schmidt", "Tom", "I6") });

        // Act
        var result = await generator.GenerateAsync(new[] { family1, family2 });

        // Assert - 6 unique persons total
        Assert.AreEqual(6, result.Count);
    }

    #endregion

    #region Sorting tests

    [TestMethod]
    public async Task GenerateAsync_Sorting_BySurnameThenGivenName()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        
        var zPerson = CreateMockPerson("Zebra", "Anton", "I1");
        var aPerson = CreateMockPerson("Adler", "Bernd", "I2");
        var mPerson = CreateMockPerson("Müller", "Carl", "I3");
        var sameSurnameDiffGiven = CreateMockPerson("Müller", "Anna", "I4");

        var family = CreateMockFamily(
            husband: zPerson,
            wife: aPerson,
            children: new[] { mPerson, sameSurnameDiffGiven });

        // Act
        var result = await generator.GenerateAsync(new[] { family });

        // Assert - sorted by surname then given name
        Assert.AreEqual("Adler, Bernd", result[0].Name);
        Assert.AreEqual("Müller, Anna", result[1].Name);
        Assert.AreEqual("Müller, Carl", result[2].Name);
        Assert.AreEqual("Zebra, Anton", result[3].Name);
    }

    [TestMethod]
    public async Task GenerateAsync_Sorting_SameSurname_DifferentGivenNames()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        
        // Same surname, different given names - test secondary sort order
        var aPerson = CreateMockPerson("Müller", "Anna", "I1");
        var bPerson = CreateMockPerson("Müller", "Bernd", "I2");
        var cPerson = CreateMockPerson("Müller", "Anton", "I3");

        var family = CreateMockFamily(
            husband: cPerson,
            wife: aPerson,
            children: new[] { bPerson });

        // Act
        var result = await generator.GenerateAsync(new[] { family });

        // Assert - Within same surname, sorted by Name (which includes given name)
        Assert.AreEqual("Müller, Anna", result[0].Name);
        Assert.AreEqual("Müller, Anton", result[1].Name);
        Assert.AreEqual("Müller, Bernd", result[2].Name);
    }

    #endregion

    #region Name formatting tests

    [TestMethod]
    public async Task GenerateAsync_NameFormat_SurnameAndGivenName_FormatsCorrectly()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        var person = CreateMockPerson("Müller", "Hans", "I1");
        var family = CreateMockFamily(person, null!, Array.Empty<IGenPerson>());

        // Act
        var result = await generator.GenerateAsync(new[] { family });

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Müller, Hans", result[0].Name);
    }

    [TestMethod]
    public async Task GenerateAsync_NameFormat_OnlyGivenName_UsesJustGivenName()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        var person = CreateMockPerson("", "Hans", "I1");
        var family = CreateMockFamily(person, null!, Array.Empty<IGenPerson>());

        // Act
        var result = await generator.GenerateAsync(new[] { family });

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Hans", result[0].Name);
    }

    [TestMethod]
    public async Task GenerateAsync_NameFormat_BothEmpty_ReturnsUnknown()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        var person = CreateMockPerson("", "", "I1");
        var family = CreateMockFamily(person, null!, Array.Empty<IGenPerson>());

        // Act
        var result = await generator.GenerateAsync(new[] { family });

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Unknown", result[0].Name);
    }

    [TestMethod]
    public async Task GenerateAsync_NameFormat_NullGivenName_UsesSurnameOnly()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        var person = Substitute.For<IGenPerson>();
        person.Surname.Returns("Müller");
        person.GivenName.Returns((string?)null!);
        person.Name.Returns("Müller");
        person.IndRefID.Returns("I1");

        var family = CreateMockFamily(person, null!, Array.Empty<IGenPerson>());

        // Act
        var result = await generator.GenerateAsync(new[] { family });

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Müller", result[0].Name);
    }

    #endregion

    #region Life event tests

    [TestMethod]
    public async Task GenerateAsync_BirthDate_Preserved()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        var birthDate = Substitute.For<IGenDate>();
        var person = CreateMockPerson("Müller", "Hans", "I1");
        person.BirthDate.Returns(birthDate);

        var wife = CreateMockPerson("Schmidt", "Anna", "I2");
        var family = new OFBFamilyModel
        {
            GlobalNumber = "00001",
            FamilyName = "Müller",
            Husband = person,
            Wife = wife,
            Children = Array.Empty<IGenPerson>().AsReadOnly(),
            SourceRefId = "F1"
        };

        // Act
        var result = await generator.GenerateAsync(new[] { family });

        // Assert
        var hansEntry = result.First(e => e.Name == "Müller, Hans");
        Assert.AreSame(birthDate, hansEntry.BirthDate);
    }

    [TestMethod]
    public async Task GenerateAsync_DeathDate_Preserved()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        var deathDate = Substitute.For<IGenDate>();
        var person = CreateMockPerson("Müller", "Hans", "I1");
        person.DeathDate.Returns(deathDate);

        var wife = CreateMockPerson("Schmidt", "Anna", "I2");
        var family = new OFBFamilyModel
        {
            GlobalNumber = "00001",
            FamilyName = "Müller",
            Husband = person,
            Wife = wife,
            Children = Array.Empty<IGenPerson>().AsReadOnly(),
            SourceRefId = "F1"
        };

        // Act
        var result = await generator.GenerateAsync(new[] { family });

        // Assert
        var hansEntry = result.First(e => e.Name == "Müller, Hans");
        Assert.AreSame(deathDate, hansEntry.DeathDate);
    }

    #endregion

    #region Source reference tests

    [TestMethod]
    public async Task GenerateAsync_SourceRefId_UsesIndRefID()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        var person = CreateMockPerson("Müller", "Hans", "I42");
        var family = CreateMockFamily(person, null!, Array.Empty<IGenPerson>());

        // Act
        var result = await generator.GenerateAsync(new[] { family });

        // Assert
        var hansEntry = result.First(e => e.Name == "Müller, Hans");
        Assert.AreEqual("I42", hansEntry.SourceRefId);
    }

    #endregion

    #region Cancellation tests

    [TestMethod]
    public void GenerateAsync_WithCancellation_ThrowsOperationCanceledException()
    {
        // Arrange
        var generator = new PersonIndexGenerator(_logger);
        
        // Create a cancellation token source and cancel immediately
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Create minimal family data (not needed since it's already cancelled)
        var families = new OFBFamilyModel[] { CreateMockEmptyFamily() };

        // Act & Assert
        var ex = Assert.Throws<System.AggregateException>(
            () => { _ = generator.GenerateAsync(families, cts.Token).Result; });
        
        Assert.IsInstanceOfType(ex.InnerException ?? ex, typeof(OperationCanceledException));
    }

    #endregion

    #region Helper methods

    private static IGenPerson CreateMockPerson(string surname, string givenName, string xid)
    {
        var person = Substitute.For<IGenPerson>();
        person.Surname.Returns(surname);
        person.GivenName.Returns(givenName);
        person.Name.Returns(!string.IsNullOrEmpty(surname) ? $"{surname} {givenName}" : givenName);

        // Set IndRefID (GEDCOM individual pointer like "I1")
        person.IndRefID.Returns(xid);

        // Return null for life event dates by default
        person.BirthDate.Returns((IGenDate?)null!);
        person.DeathDate.Returns((IGenDate?)null!);

        return person;
    }

    private static OFBFamilyModel CreateMockFamily(
        IGenPerson? husband,
        IGenPerson? wife,
        IReadOnlyList<IGenPerson>? children = null)
    {
        return new OFBFamilyModel
        {
            GlobalNumber = "00001",
            FamilyName = !string.IsNullOrEmpty(husband?.Surname) ? husband!.Surname : (wife?.Surname ?? "Unknown"),
            Husband = husband,
            Wife = wife,
            Children = children != null ? (IReadOnlyList<IGenPerson>)children : CreateEmptyChildren(),
            SourceRefId = "F1"
        };
    }

    private static OFBFamilyModel CreateMockEmptyFamily()
    {
        return new OFBFamilyModel
        {
            GlobalNumber = "00001",
            FamilyName = "Unknown",
            Husband = null,
            Wife = null,
            Children = Array.Empty<IGenPerson>().AsReadOnly(),
            SourceRefId = "F1"
        };
    }

    private static IReadOnlyList<IGenPerson> CreateEmptyChildren()
    {
        var list = new List<IGenPerson>(0);
        return list.AsReadOnly();
    }

    #endregion
}
