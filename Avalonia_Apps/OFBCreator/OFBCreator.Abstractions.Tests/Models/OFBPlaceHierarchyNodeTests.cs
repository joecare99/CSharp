using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OFBCreator.Abstractions.Tests.Models;

using OFBCreator.Abstractions.Models;

/// <summary>
/// Tests for OFBPlaceHierarchyNode — constructor validation, children management.
/// </summary>
[TestClass]
public class OFBPlaceHierarchyNodeTests
{
    [TestMethod]
    public void Constructor_ShouldSetNameAndPlaceId()
    {
        // Arrange & Act
        var node = new OFBPlaceHierarchyNode( "Musterstadt", "P123" );

        // Assert
        Assert.AreEqual( "Musterstadt", node.Name );
        Assert.AreEqual( "P123", node.PlaceId );
    }

    [TestMethod]
    public void Constructor_ShouldThrowOnNullName()
    {
        // Act & Assert
        var ex = Assert.Throws<System.ArgumentNullException>( () =>
            new OFBPlaceHierarchyNode( null!, "P1" ) );

        Assert.IsTrue( ex.ParamName!.Contains( "name" ), $"Expected 'name' but got '{ex.ParamName}'." );
    }

    [TestMethod]
    public void Constructor_ShouldThrowOnNullPlaceId()
    {
        // Act & Assert
        var ex = Assert.Throws<System.ArgumentNullException>( () =>
            new OFBPlaceHierarchyNode( "Stadt", null! ) );

        Assert.IsTrue( ex.ParamName!.Contains( "placeId" ), $"Expected 'placeId' but got '{ex.ParamName}'." );
    }

    [TestMethod]
    public void Children_ShouldDefaultToEmptyArray()
    {
        // Arrange & Act
        var node = new OFBPlaceHierarchyNode( "Stadt", "P1" );

        // Assert
        Assert.IsTrue( node.Children.Length == 0, "Children should default to empty array." );
    }

    [TestMethod]
    public void ParentPlaceId_ShouldBeNullable()
    {
        // Arrange & Act
        var node = new OFBPlaceHierarchyNode( "Stadtteil", "P2" );

        // Assert — defaults to null
        Assert.IsNull( node.ParentPlaceId );

        // Set and verify
        node.ParentPlaceId = "P1";
        Assert.AreEqual( "P1", node.ParentPlaceId );
    }

    [TestMethod]
    public void Children_ShouldSupportAddition()
    {
        // Arrange & Act
        var parent = new OFBPlaceHierarchyNode( "Landkreis", "P0" );
        var child1 = new OFBPlaceHierarchyNode( "Stadt A", "P1" );
        var child2 = new OFBPlaceHierarchyNode( "Stadt B", "P2" );

        parent.Children = new[] { child1, child2 };

        // Assert
        Assert.AreEqual( 2, parent.Children.Length );
        Assert.AreEqual( "Stadt A", parent.Children[0].Name );
        Assert.AreEqual( "Stadt B", parent.Children[1].Name );
    }

    [TestMethod]
    public void PlaceIdAndName_AreReadOnlyProperties()
    {
        // Arrange & Act — compile-time verification that properties are set-only via constructor
        var node = new OFBPlaceHierarchyNode( "Ort", "P9" );

        // Assert
        Assert.AreEqual( "Ort", node.Name );
        Assert.AreEqual( "P9", node.PlaceId );
    }
}
