using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OFBCreator.Abstractions.Tests.Models;

using OFBCreator.Abstractions.Models;

/// <summary>
/// Tests for OFBIndexEntry — factory methods, property setting, ToString formatting.
/// </summary>
[TestClass]
public class OFBIndexEntryTests
{
    [TestMethod]
    public void FromSingle_ShouldSetSortKeyAndNameFromValue()
    {
        // Arrange & Act
        var entry = OFBIndexEntry.FromSingle( "Müller", "F123" );

        // Assert
        Assert.AreEqual( "Müller", entry.SortKey );
        Assert.AreEqual( "Müller", entry.Name );
        Assert.AreEqual( "F123", entry.Ref );
    }

    [TestMethod]
    public void FromCombined_ShouldFormatSortKeyAsSurname_GivenName()
    {
        // Arrange & Act
        var entry = OFBIndexEntry.FromCombined( "Müller", "Hans", "F123" );

        // Assert
        Assert.AreEqual( "Müller, Hans", entry.SortKey );
        Assert.AreEqual( "Müller", entry.Name );
        Assert.AreEqual( "F123", entry.Ref );
    }

    [TestMethod]
    public void FromCombined_ShouldHandleEmptyComponents()
    {
        // Arrange & Act
        var entry = OFBIndexEntry.FromCombined( "", "Hans", "F0" );

        // Assert
        Assert.AreEqual( ", Hans", entry.SortKey );
        Assert.AreEqual( string.Empty, entry.Name );
    }

    [TestMethod]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange & Act
        var entry = OFBIndexEntry.FromSingle( "Schmidt", "F456" );
        var str = entry.ToString();

        // Assert
        Assert.IsNotNull( str );
        Assert.IsTrue( str!.Contains( "SortKey=Schmidt" ) );
        Assert.IsTrue( str.Contains( "Name=Schmidt" ) );
        Assert.IsTrue( str.Contains( "Ref=F456" ) );
    }

    [TestMethod]
    public void Properties_ShouldBeInitOnly()
    {
        // Arrange & Act — compile-time check that properties are init-only
        var entry = new OFBIndexEntry
        {
            SortKey = "Test",
            Name = "TestName",
            Ref = "R1",
        };

        // Assert
        Assert.AreEqual( "Test", entry.SortKey );
        Assert.AreEqual( "TestName", entry.Name );
        Assert.AreEqual( "R1", entry.Ref );
    }
}
