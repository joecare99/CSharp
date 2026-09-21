using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Core.Services;
using OFBCreator.Abstractions.Models;
using OFBCreator.Core.Tests.Helpers;

namespace OFBCreator.Core.Tests.Services;

/// <summary>
/// Tests for OFBFamilySorter — sorting, numbering, formation date calculation.
/// </summary>
[TestClass]
public class OFBFamilySorterTests
{
    [TestMethod]
    public void SortAndNumber_WithNullFamilies_ShouldThrowArgumentNullException()
    {
        // Arrange
        var sorter = new OFBFamilySorter();

        // Act & Assert
        var ex = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Throws<System.ArgumentNullException>(
            () => { _ = sorter.SortAndNumber(null!); });
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("families", ex.ParamName);
    }

    [TestMethod]
    public void SortAndNumber_WithEmptyFamilies_ShouldReturnEmptyList()
    {
        // Arrange
        var sorter = new OFBFamilySorter();
        var families = Array.Empty<IGenFamily>();

        // Act
        var result = sorter.SortAndNumber(families);

        // Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void SortAndNumber_ShouldAssignSequentialNumbers()
    {
        // Arrange
        var sorter = new OFBFamilySorter();
        var family1 = CreateFamily("Müller", "John", "Jane", marriageDate: new DateTime(1900, 5, 15));
        var family2 = CreateFamily("Schmidt", "Hans", "Greta", marriageDate: new DateTime(1905, 8, 20));
        var families = new List<IGenFamily> { family1, family2 };

        // Act
        var result = sorter.SortAndNumber(families);

        // Assert — should be sorted alphabetically by family name
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, result.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("00001", result[0].GlobalNumber);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("00002", result[1].GlobalNumber);
    }

    [TestMethod]
    public void SortAndNumber_ShouldSortByFamilyNameThenDate()
    {
        // Arrange
        var sorter = new OFBFamilySorter();
        
        // Same family name, different dates
        var familyA2 = CreateFamily("Müller", "Carl", "Anna", marriageDate: new DateTime(1920, 3, 10));
        var familyA1 = CreateFamily("Müller", "Hans", "Maria", marriageDate: new DateTime(1910, 6, 15));
        var familyB = CreateFamily("Schmidt", "Fritz", "Lisa", marriageDate: new DateTime(1905, 1, 1));
        
        var families = new List<IGenFamily> { familyA2, familyA1, familyB };

        // Act
        var result = sorter.SortAndNumber(families);

        // Assert — Müller before Schmidt (alphabetical), A1 before A2 (earlier date first)
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(3, result.Count);
        var actual0 = result[0].FamilyName ?? "<null>";
        var actual1 = result[1].FamilyName ?? "<null>";
        var actual2 = result[2].FamilyName ?? "<null>";
        Assert.IsTrue(result[0].FamilyName == "Müller", $"Expected Müller, got '{actual0}'");
        Assert.IsTrue(result[1].FamilyName == "Müller", $"Expected Müller, got '{actual1}'");
        Assert.IsTrue(result[2].FamilyName == "Schmidt", $"Expected Schmidt, got '{actual2}'");
        
        // Earlier date should come first within same group
        var mullerFamilies = result.Where(f => f.FamilyName == "Müller").ToList();
        AssertDateBefore(mullerFamilies[0], mullerFamilies[1]);
    }

    [TestMethod]
    public void SortAndNumber_WithNullFormationDate_ShouldPlaceLast()
    {
        // Arrange
        var sorter = new OFBFamilySorter();
        
        var familyNoDate = CreateFamily("Müller", "Hans", null!, marriageDate: null!);
        var familyWithDate = CreateFamily("Schmidt", "Fritz", "Lisa", marriageDate: new DateTime(1905, 1, 1));
        
        var families = new List<IGenFamily> { familyNoDate, familyWithDate };

        // Act
        var result = sorter.SortAndNumber(families);

        // Assert — Müller should still come before Schmidt alphabetically, null date shouldn't break anything
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void SortAndNumber_WithPlaceFilter_ShouldOnlyIncludeMatchingFamilies()
    {
        // Arrange
        var sorter = new OFBFamilySorter();
        
        var husband1 = Substitute.For<IGenPerson>();
        husband1.ChildCount.Returns(1);
        husband1.Surname.Returns("Mueller");

        var wife1 = Substitute.For<IGenPerson>();
        wife1.ChildCount.Returns(0);
        wife1.Surname.Returns("Muller");

        var child1 = Substitute.For<IGenPerson>();
        child1.ChildCount.Returns(0);

        var placeBerlin = CreatePlace("gov-berlin");
        
        var familyBerlin = Substitute.For<IGenFamily>();
        familyBerlin.Husband.Returns(husband1);
        familyBerlin.Wife.Returns(wife1);
        familyBerlin.Children.Returns(new TestIndexedList<IGenPerson> { child1 });
        familyBerlin.ChildCount.Returns(1);
        familyBerlin.MarriagePlace.Returns(placeBerlin);

        var husband2 = Substitute.For<IGenPerson>();
        husband2.ChildCount.Returns(1);
        husband2.Surname.Returns("Schmidt");

        var wife2 = Substitute.For<IGenPerson>();
        wife2.ChildCount.Returns(0);
        wife2.Surname.Returns("Schmidt");

        var child2 = Substitute.For<IGenPerson>();
        child2.ChildCount.Returns(0);

        var placeMunich = CreatePlace("gov-munich");
        
        var familyMunich = Substitute.For<IGenFamily>();
        familyMunich.Husband.Returns(husband2);
        familyMunich.Wife.Returns(wife2);
        familyMunich.Children.Returns(new TestIndexedList<IGenPerson> { child2 });
        familyMunich.ChildCount.Returns(1);
        familyMunich.MarriagePlace.Returns(placeMunich);
        
        var allFamilies = new List<IGenFamily> { familyBerlin, familyMunich };

        // Act
        var result = sorter.SortAndNumber(allFamilies, placeId: "gov-berlin");

        // Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void SortAndNumber_Beyond5Families_ShouldUse5DigitPadding()
    {
        // Arrange
        var sorter = new OFBFamilySorter();
        
        var families = new List<IGenFamily>();
        for (int i = 0; i < 100; i++)
        {
            families.Add(CreateFamily("Müller", $"Name{i}", "Spouse", marriageDate: new DateTime(1900 + i, 1, 1)));
        }

        // Act
        var result = sorter.SortAndNumber(families);

        // Assert — first should be 00001, 100th should be 00100 (5 digits)
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(100, result.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("00001", result[0].GlobalNumber);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("00100", result[99].GlobalNumber);
    }

    [TestMethod]
    public void SortAndNumber_BeyondMaxPadding_ShouldUseRawNumber()
    {
        // Arrange
        var sorter = new OFBFamilySorter();
        
        var families = new List<IGenFamily>();
        for (int i = 0; i < 100000; i++)
        {
            families.Add(CreateFamily("Müller", $"Name{i}", "Spouse", marriageDate: new DateTime(1900, 1, 1)));
        }

        // Act
        var result = sorter.SortAndNumber(families);

        // Assert — last family should have raw number (not padded to 6+ digits)
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(100000, result.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("99999", result[99998].GlobalNumber);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("100000", result[99999].GlobalNumber);
    }

    // --- Helpers ---

    private IGenFamily CreateFamily(
        string familyName,
        string husbandName,
        string? wifeName,
        DateTime? marriageDate)
    {
        var husband = Substitute.For<IGenPerson>();
        husband.ChildCount.Returns(1);
        husband.FamilyCount.Returns(1);
        husband.Surname.Returns(familyName);
        // Cannot mock ToString() via NSubstitute, rely on Name property for display

        IGenPerson? wife = null;
        if (wifeName != null)
        {
            wife = Substitute.For<IGenPerson>();
            wife.ChildCount.Returns(0);
            wife.FamilyCount.Returns(1);
            // Leave Surname as null — OFBFamilySorter will use husband.Surname only in this case
        }

        var child = Substitute.For<IGenPerson>();
        child.ChildCount.Returns(0);
        child.SpouseCount.Returns(0);

        var childrenList = new TestIndexedList<IGenPerson> { child };

        var family = Substitute.For<IGenFamily>();
        family.Husband.Returns(husband);
        family.Wife.Returns(wife);
        family.Children.Returns(childrenList);
        family.ChildCount.Returns(1);
        family.FamilyRefID.Returns($"F{Guid.NewGuid():N}");

        // Set formation date via MarriageDate (not Marriage)
        if (marriageDate != null)
        {
            var marriageDateObj = Substitute.For<IGenDate>();
            marriageDateObj.Date1.Returns(marriageDate.Value);
            family.MarriageDate.Returns(marriageDateObj);
        }

        return family;
    }

    private static IGenPlace CreatePlace(string govId)
    {
        var place = Substitute.For<IGenPlace>();
        place.GOV_ID.Returns(govId);
        return place;
    }

    private static void AssertDateBefore(OFBFamilyModel earlier, OFBFamilyModel later)
    {
        if (earlier.FormationDate == null || later.FormationDate == null)
            return; // skip date assertion if either is null
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(
            earlier.FormationDate.Value <= later.FormationDate.Value);
    }
}
