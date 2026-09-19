using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OFBCreator.Abstractions.Tests.Models;

using OFBCreator.Abstractions.Models;
using GenInterfaces.Interfaces.Genealogic;
using NSubstitute;

/// <summary>
/// Tests for OFBSourceSelection model — construction, empty state, HasData logic.
/// </summary>
[TestClass]
public class OFBSourceSelectionTests
{
    [TestMethod]
    public void Empty_ShouldReturnStaticWithZeroCounter()
    {
        // Arrange & Act
        var selection = OFBSourceSelection.Empty;

        // Assert
        Assert.AreEqual( 0, selection.LastGlobalNumber );
        Assert.IsFalse( selection.HasData );
        Assert.IsTrue( selection.SelectedFamilies.Count == 0 );
        Assert.IsTrue( selection.VirtualPersons.Count == 0 );
        Assert.IsTrue( selection.Exclusions.Count == 0 );
    }

    [TestMethod]
    public void HasData_ShouldBeTrueWhenFamiliesPresent()
    {
        // Arrange
        var family = Substitute.For<IGenFamily>();
        var families = new List<IGenFamily> { family };

        // Act
        var selection = new OFBSourceSelection
        {
            SelectedFamilies = families.AsReadOnly(),
            LastGlobalNumber = 5,
        };

        // Assert
        Assert.IsTrue( selection.HasData );
    }

    [TestMethod]
    public void HasData_ShouldBeTrueWhenVirtualPersonsPresent()
    {
        // Arrange
        var person = Substitute.For<IGenPerson>();
        var persons = new List<IGenPerson> { person };

        // Act
        var selection = new OFBSourceSelection
        {
            VirtualPersons = persons.AsReadOnly(),
        };

        // Assert
        Assert.IsTrue( selection.HasData );
    }

    [TestMethod]
    public void HasData_ShouldBeFalseWhenEmpty()
    {
        // Arrange & Act
        var selection = new OFBSourceSelection();

        // Assert
        Assert.IsFalse( selection.HasData );
    }

    [TestMethod]
    public void SelectedFamilies_ShouldDefaultToEmptyReadOnlyList()
    {
        // Arrange & Act
        var selection = new OFBSourceSelection();

        // Assert
        Assert.IsTrue( selection.SelectedFamilies.Count == 0 );
    }

    [TestMethod]
    public void Exclusions_ShouldDefaultToEmptyReadOnlyList()
    {
        // Arrange & Act
        var selection = new OFBSourceSelection();

        // Assert
        Assert.IsTrue( selection.Exclusions.Count == 0 );
    }

    [TestMethod]
    public void VirtualPersons_ShouldDefaultToEmptyReadOnlyList()
    {
        // Arrange & Act
        var selection = new OFBSourceSelection();

        // Assert
        Assert.IsTrue( selection.VirtualPersons.Count == 0 );
    }
}
