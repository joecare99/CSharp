using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Core.Services;
using OFBCreator.Abstractions.Models;
using OFBCreator.Core.Tests.Helpers;

namespace OFBCreator.Core.Tests.Services;

/// <summary>
/// Tests for OFBFamilySelectionProvider — full-family detection, virtual family creation, and exclusion diagnostics.
/// </summary>
[TestClass]
public class OFBFamilySelectionProviderTests
{
    [TestMethod]
    public void SelectAsync_WithNullSource_ShouldThrowArgumentNullException()
    {
        // Arrange
        var provider = new OFBFamilySelectionProvider();

        // Act & Assert
        var ex = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Throws<System.AggregateException>(
            () => { _ = provider.SelectAsync((IGenealogy)null!).Result; });
        // Unwrap AggregateException to check the inner ArgumentNullException
        var innerEx = ex.InnerException ?? ex;
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(innerEx, typeof(System.ArgumentNullException));
    }

    [TestMethod]
    public void SelectAsync_WithEmptyEntities_ShouldReturnEmptySelection()
    {
        // Arrange
        var provider = new OFBFamilySelectionProvider();
        var source = Substitute.For<IGenealogy>();
        source.Entitys.Returns(new List<IGenEntity>());

        // Act
        var result = provider.SelectAsync(source).Result;

        // Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, result.SelectedFamilies.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, result.VirtualPersons.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, result.Exclusions.Count);
    }

    [TestMethod]
    public void SelectAsync_WithFullFamily_ShouldSelectFamily()
    {
        // Arrange
        var provider = new OFBFamilySelectionProvider();
        var husband = Substitute.For<IGenPerson>();
        husband.ChildCount.Returns(2); // > 1 triggers full-family via husband
        
        var wife = Substitute.For<IGenPerson>();
        wife.ChildCount.Returns(1);

        var child1 = Substitute.For<IGenPerson>();
        var child2 = Substitute.For<IGenPerson>();

        var childrenList = new TestIndexedList<IGenPerson> { child1, child2 };
        
        var family = Substitute.For<IGenFamily>();
        family.Husband.Returns(husband);
        family.Wife.Returns(wife);
        family.Children.Returns(childrenList);
        family.ChildCount.Returns(2);

        var source = Substitute.For<IGenealogy>();
        source.Entitys.Returns(new List<IGenEntity> { family });

        // Act
        var result = provider.SelectAsync(source).Result;

        // Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, result.SelectedFamilies.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(family, result.SelectedFamilies[0]);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, result.VirtualPersons.Count);
    }

    [TestMethod]
    public void SelectAsync_WithWifeHasChildren_ShouldSelectFamily()
    {
        // Arrange
        var provider = new OFBFamilySelectionProvider();
        var husband = Substitute.For<IGenPerson>();
        husband.ChildCount.Returns(0);
        
        var wife = Substitute.For<IGenPerson>();
        wife.ChildCount.Returns(3); // > 1 triggers full-family via wife

        var child = Substitute.For<IGenPerson>();
        var childrenList = new TestIndexedList<IGenPerson> { child };

        var family = Substitute.For<IGenFamily>();
        family.Husband.Returns(husband);
        family.Wife.Returns(wife);
        family.Children.Returns(childrenList);
        family.ChildCount.Returns(1);

        var source = Substitute.For<IGenealogy>();
        source.Entitys.Returns(new List<IGenEntity> { family });

        // Act
        var result = provider.SelectAsync(source).Result;

        // Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, result.SelectedFamilies.Count);
    }

    [TestMethod]
    public void SelectAsync_WithParentFamily_ShouldSelectIncompleteFamily()
    {
        // Arrange
        var provider = new OFBFamilySelectionProvider();
        var husband = Substitute.For<IGenPerson>();
        husband.ChildCount.Returns(0);
        husband.ParentFamily.Returns(Substitute.For<IGenFamily>()); // has parent family
        
        var wife = Substitute.For<IGenPerson>();
        wife.ChildCount.Returns(0);

        var family = Substitute.For<IGenFamily>();
        family.Husband.Returns(husband);
        family.Wife.Returns(wife);
        family.Children.Returns(new TestIndexedList<IGenPerson>());
        family.ChildCount.Returns(0);

        var source = Substitute.For<IGenealogy>();
        source.Entitys.Returns(new List<IGenEntity> { family });

        // Act
        var result = provider.SelectAsync(source).Result;

        // Assert — childless couple should be selected
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, result.SelectedFamilies.Count);
    }

    [TestMethod]
    public void SelectAsync_WithIncompleteFamily_ShouldExcludeFromSelection()
    {
        // Arrange
        var provider = new OFBFamilySelectionProvider();
        var husband = Substitute.For<IGenPerson>();
        husband.ChildCount.Returns(0);
        husband.ParentFamily.Returns((IGenFamily?)null!); // no parent family
        
        var wife = Substitute.For<IGenPerson>();
        wife.ChildCount.Returns(0);
        wife.ParentFamily.Returns((IGenFamily?)null!); // NSubstitute auto-generates substitutes for interface types — must explicitly set null

        var child = Substitute.For<IGenPerson>();
        child.ChildCount.Returns(0);
        child.SpouseCount.Returns(0); // no children or spouses

        var childrenList = new TestIndexedList<IGenPerson> { child };

        var family = Substitute.For<IGenFamily>();
        family.Husband.Returns(husband);
        family.Wife.Returns(wife);
        family.Children.Returns(childrenList);
        family.ChildCount.Returns(1);

        var source = Substitute.For<IGenealogy>();
        source.Entitys.Returns(new List<IGenEntity> { family });

        // Act
        var result = provider.SelectAsync(source).Result;

        // Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, result.SelectedFamilies.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, result.Exclusions.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("IncompleteFamily", result.Exclusions[0].ReasonCode);
    }

    [TestMethod]
    public void SelectAsync_WithChildHavingChildren_ShouldSelectFamily()
    {
        // Arrange
        var provider = new OFBFamilySelectionProvider();
        var husband = Substitute.For<IGenPerson>();
        husband.ChildCount.Returns(0);
        
        var wife = Substitute.For<IGenPerson>();
        wife.ChildCount.Returns(0);

        var child = Substitute.For<IGenPerson>();
        child.ChildCount.Returns(1); // has children — triggers full-family
        child.SpouseCount.Returns(0);

        var childrenList = new TestIndexedList<IGenPerson> { child };

        var family = Substitute.For<IGenFamily>();
        family.Husband.Returns(husband);
        family.Wife.Returns(wife);
        family.Children.Returns(childrenList);
        family.ChildCount.Returns(1);

        var source = Substitute.For<IGenealogy>();
        source.Entitys.Returns(new List<IGenEntity> { family });

        // Act
        var result = provider.SelectAsync(source).Result;

        // Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, result.SelectedFamilies.Count);
    }

    [TestMethod]
    public void SelectAsync_WithIsolatedIndividual_ShouldCreateVirtualPerson()
    {
        // Arrange
        var provider = new OFBFamilySelectionProvider();
        var person = Substitute.For<IGenPerson>();
        person.FamilyCount.Returns(0);
        person.ParentFamily.Returns((IGenFamily?)null!);

        var source = Substitute.For<IGenealogy>();
        source.Entitys.Returns(new List<IGenEntity> { person });

        // Act
        var result = provider.SelectAsync(source).Result;

        // Assert
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, result.SelectedFamilies.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, result.VirtualPersons.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(person, result.VirtualPersons[0]);
    }

    [TestMethod]
    public void SelectAsync_WithPersonInFamily_ShouldNotCreateVirtualPerson()
    {
        // Arrange
        var provider = new OFBFamilySelectionProvider();
        var husband = Substitute.For<IGenPerson>();
        husband.ChildCount.Returns(2);
        husband.FamilyCount.Returns(1);
        husband.ParentFamily.Returns(Substitute.For<IGenFamily>());

        var wife = Substitute.For<IGenPerson>();
        wife.ChildCount.Returns(1);
        wife.FamilyCount.Returns(1);

        var child = Substitute.For<IGenPerson>();
        child.ChildCount.Returns(0);
        child.SpouseCount.Returns(0);

        var childrenList = new TestIndexedList<IGenPerson> { child }; // Count is inherent from list initialization

        var family = Substitute.For<IGenFamily>();
        family.Husband.Returns(husband);
        family.Wife.Returns(wife);
        family.Children.Returns(childrenList);
        family.ChildCount.Returns(1);

        var source = Substitute.For<IGenealogy>();
        source.Entitys.Returns(new List<IGenEntity> { husband, wife, child, family });

        // Act
        var result = provider.SelectAsync(source).Result;

        // Assert — all should be tracked as part of real family
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, result.SelectedFamilies.Count);
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, result.VirtualPersons.Count);
    }

    [TestMethod]
    public void SelectAsync_WithChildlessCouple_ShouldSelectFamily()
    {
        // Arrange
        var provider = new OFBFamilySelectionProvider();
        var husband = Substitute.For<IGenPerson>();
        husband.ChildCount.Returns(0);
        
        var wife = Substitute.For<IGenPerson>();
        wife.ChildCount.Returns(0);

        var childrenList = new TestIndexedList<IGenPerson>();

        var family = Substitute.For<IGenFamily>();
        family.Husband.Returns(husband);
        family.Wife.Returns(wife);
        family.Children.Returns(childrenList);
        family.ChildCount.Returns(0); // childless

        var source = Substitute.For<IGenealogy>();
        source.Entitys.Returns(new List<IGenEntity> { family });

        // Act
        var result = provider.SelectAsync(source).Result;

        // Assert — childless couples are included
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, result.SelectedFamilies.Count);
    }
}
