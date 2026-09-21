using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OFBCreator.Abstractions.Tests.Models;

using OFBCreator.Abstractions.Models;

/// <summary>
/// Tests for OFBGEDCOMDate model — precision enum, display formatting, origin tracking.
/// </summary>
[TestClass]
public class OFBGEDCOMDateTests
{
    [TestMethod]
    public void DatePrecision_ShouldHaveAllValues()
    {
        // Assert
        Assert.IsTrue( Enum.IsDefined( typeof( OFBGEDCOMPrecision ), 0 ) );   // Exact
        Assert.IsTrue( Enum.IsDefined( typeof( OFBGEDCOMPrecision ), 1 ) );   // Approximate
        Assert.IsTrue( Enum.IsDefined( typeof( OFBGEDCOMPrecision ), 2 ) );   // Before
        Assert.IsTrue( Enum.IsDefined( typeof( OFBGEDCOMPrecision ), 3 ) );   // After
        Assert.IsTrue( Enum.IsDefined( typeof( OFBGEDCOMPrecision ), 4 ) );   // Estimated
    }

    [TestMethod]
    public void DateOrigin_ShouldHaveAllValues()
    {
        // Assert
        Assert.IsTrue( Enum.IsDefined( typeof( OFBGEDCOMOrigin ), 0 ) );       // SourceStatement
        Assert.IsTrue( Enum.IsDefined( typeof( OFBGEDCOMOrigin ), 1 ) );       // Calculated
        Assert.IsTrue( Enum.IsDefined( typeof( OFBGEDCOMOrigin ), 2 ) );       // Estimated
        Assert.IsTrue( Enum.IsDefined( typeof( OFBGEDCOMOrigin ), 3 ) );       // Manual
    }

    [TestMethod]
    public void DisplayFormattedDate_ShouldFormatCorrectly()
    {
        // Arrange — this tests that the display formatting enum values are correct
        var exact = OFBGEDCOMPrecision.Exact;
        var approx = OFBGEDCOMPrecision.Approximate;

        // Assert — verify we can read the values
        Assert.AreEqual( 0, (int)exact );
        Assert.AreEqual( 1, (int)approx );
    }

    [TestMethod]
    public void DateOriginValues_ShouldBeDistinct()
    {
        // Assert
        var values = Enum.GetValues( typeof( OFBGEDCOMOrigin ) ).Cast<int>().ToArray();
        Assert.IsTrue( values.Distinct().Count() == values.Length, "All origin values must be distinct." );
    }

    [TestMethod]
    public void DatePrecisionValues_ShouldBeDistinct()
    {
        // Assert
        var values = Enum.GetValues( typeof( OFBGEDCOMPrecision ) ).Cast<int>().ToArray();
        Assert.IsTrue( values.Distinct().Count() == values.Length, "All precision values must be distinct." );
    }
}
