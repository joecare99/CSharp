using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Core.Services;
using OFBCreator.Core.Tests.Helpers;
using OFBCreator.Abstractions.Models;

namespace OFBCreator.Core.Tests.Services;

/// <summary>
/// Tests for FamilyNameGroupBuilder — name clustering from parent/family data.
/// </summary>
[TestClass]
public class FamilyNameGroupBuilderTests
{
    [TestMethod]
    public void BuildGroups_WithNullSource_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new FamilyNameGroupBuilder();

        // Act & Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Throws<System.ArgumentNullException>(
            () => builder.BuildGroups((IEnumerable<IGenFamily>)null!) );
    }

    [TestMethod]
    public void BuildGroups_WithEmptySource_ShouldReturnEmptyGroupKeys()
    {
        // Arrange
        var builder = new FamilyNameGroupBuilder();
        var families = Array.Empty<IGenFamily>();

        // Act
        builder.BuildGroups(families);

        // Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, builder.GroupKeys.Count);
    }

    [TestMethod]
    public void BuildGroups_WithSameSurname_ShouldGroupTogether()
    {
        // Arrange
        var builder = new FamilyNameGroupBuilder();
        
        var family1 = CreateFamilyWithSurname("Müller");
        var family2 = CreateFamilyWithSurname("Müller");
        var families = new List<IGenFamily> { family1, family2 };

        // Act
        builder.BuildGroups(families);

        // Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, builder.GroupKeys.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(builder.GroupKeys.Contains("Müller/Müller"));
        var group = builder.GetGroup("Müller/Müller");
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(group);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, group!.Count);
    }

    [TestMethod]
    public void BuildGroups_WithDifferentSurnames_ShouldCreateSeparateGroups()
    {
        // Arrange
        var builder = new FamilyNameGroupBuilder();
        
        var familyMueller = CreateFamilyWithSurname("Müller");
        var familySchmidt = CreateFamilyWithSurname("Schmidt");
        var families = new List<IGenFamily> { familyMueller, familySchmidt };

        // Act
        builder.BuildGroups(families);

        // Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, builder.GroupKeys.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(builder.GroupKeys.Contains("Müller/Müller"));
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(builder.GroupKeys.Contains("Schmidt/Schmidt"));
    }

    [TestMethod]
    public void BuildGroups_WithNullHusband_ShouldUseWifeSurname()
    {
        // Arrange
        var builder = new FamilyNameGroupBuilder();
        
        var wife = Substitute.For<IGenPerson>();
        wife.Surname.Returns("Weber");

        var child = Substitute.For<IGenPerson>();
        child.Surname.Returns("Weber");

        var family = Substitute.For<IGenFamily>();
        family.Husband.Returns((IGenPerson)null!);
        family.Wife.Returns(wife);
        family.Children.Returns(new TestIndexedList<IGenPerson> { child });
        family.ChildCount.Returns(1);

        var families = new List<IGenFamily> { family };

        // Act
        builder.BuildGroups(families);

        // Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, builder.GroupKeys.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(builder.GroupKeys.Contains("Weber"));
    }

    [TestMethod]
    public void BuildGroups_WithNullSpouses_ShouldUseChildSurname()
    {
        // Arrange
        var builder = new FamilyNameGroupBuilder();
        
        var child = Substitute.For<IGenPerson>();
        child.Surname.Returns("Bauer");

        var family = Substitute.For<IGenFamily>();
        family.Husband.Returns((IGenPerson)null!);
        family.Wife.Returns((IGenPerson)null!);
        family.Children.Returns(new TestIndexedList<IGenPerson> { child });
        family.ChildCount.Returns(1);

        var families = new List<IGenFamily> { family };

        // Act
        builder.BuildGroups(families);

        // Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, builder.GroupKeys.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(builder.GroupKeys.Contains("Bauer"));
    }

    [TestMethod]
    public void BuildGroups_WithAllNulls_ShouldUseUnknownGroup()
    {
        // Arrange
        var builder = new FamilyNameGroupBuilder();
        
        var family = Substitute.For<IGenFamily>();
        family.Husband.Returns((IGenPerson)null!);
        family.Wife.Returns((IGenPerson)null!);
        family.Children.Returns(new TestIndexedList<IGenPerson>());
        family.ChildCount.Returns(0);

        var families = new List<IGenFamily> { family };

        // Act
        builder.BuildGroups(families);

        // Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, builder.GroupKeys.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(builder.GroupKeys.Contains("Unbekannt"));
    }

    // --- Helpers ---

    private static IGenFamily CreateFamilyWithSurname(string surname)
    {
        var husband = Substitute.For<IGenPerson>();
        husband.Surname.Returns(surname);
        husband.ChildCount.Returns(1);

        var wife = Substitute.For<IGenPerson>();
        wife.Surname.Returns(surname);
        wife.ChildCount.Returns(0);

        var child = Substitute.For<IGenPerson>();
        child.Surname.Returns(surname);
        child.ChildCount.Returns(0);

        var childrenList = new TestIndexedList<IGenPerson> { child };

        var family = Substitute.For<IGenFamily>();
        family.Husband.Returns(husband);
        family.Wife.Returns(wife);
        family.Children.Returns(childrenList);
        family.ChildCount.Returns(1);

        return family;
    }
}
