using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Code.Navigation.Tests;

[TestClass]
public sealed class CodeLocationTests
{
    [TestMethod]
    public void Constructor_WithPathOnly_NormalizesPathAndLeavesCoordinatesUnspecified()
    {
        var location = new CodeLocation("  source\file.cs  ");

        Assert.AreEqual(Path.GetFullPath("source\file.cs"), location.Path);
        Assert.IsNull(location.Line);
        Assert.IsNull(location.Column);
        Assert.IsNull(location.Offset);
    }

    [TestMethod]
    public void Constructor_WithCoordinates_PreservesDocumentPosition()
    {
        var location = new CodeLocation("source\file.cs", 12, 7, 214);

        Assert.AreEqual(12, location.Line);
        Assert.AreEqual(7, location.Column);
        Assert.AreEqual(214, location.Offset);
    }

    [TestMethod]
    public void Constructor_WithEquivalentNormalizedPaths_ProducesEqualValues()
    {
        var first = new CodeLocation("source\file.cs", 12, 7, 214);
        var second = new CodeLocation(Path.GetFullPath("source\file.cs"), 12, 7, 214);

        Assert.AreEqual(first, second);
        Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
    }

    [TestMethod]
    public void Constructor_WithEmptyPath_ThrowsArgumentException()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new CodeLocation("  "));
    }

    [TestMethod]
    public void Constructor_WithInvalidLine_ThrowsArgumentOutOfRangeException()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new CodeLocation("file.cs", 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new CodeLocation("file.cs", -1));
    }

    [TestMethod]
    public void Constructor_WithInvalidColumn_ThrowsArgumentOutOfRangeException()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new CodeLocation("file.cs", column: 0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new CodeLocation("file.cs", column: -1));
    }

    [TestMethod]
    public void Constructor_WithNegativeOffset_ThrowsArgumentOutOfRangeException()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new CodeLocation("file.cs", offset: -1));
    }

    [TestMethod]
    public void Constructor_WithZeroOffset_AllowsStartOfDocument()
    {
        var location = new CodeLocation("file.cs", offset: 0);

        Assert.AreEqual(0, location.Offset);
    }
}
