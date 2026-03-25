using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpHack.Base.Model;

namespace SharpHack.BaseTests.Model;

[TestClass]
public class PointTests
{
    [TestMethod]
    public void Zero_ReturnsZeroPoint()
    {
        var p = Point.Zero;
        Assert.AreEqual(0, p.X);
        Assert.AreEqual(0, p.Y);
    }

    [TestMethod]
    public void OperatorPlus_AddsCoordinates()
    {
        var p1 = new Point(1, 2);
        var p2 = new Point(3, 4);
        var result = p1 + p2;
        Assert.AreEqual(4, result.X);
        Assert.AreEqual(6, result.Y);
    }

    [TestMethod]
    public void OperatorMinus_SubtractsCoordinates()
    {
        var p1 = new Point(5, 5);
        var p2 = new Point(2, 3);
        var result = p1 - p2;
        Assert.AreEqual(3, result.X);
        Assert.AreEqual(2, result.Y);
    }

    [TestMethod]
    public void Equality_WorksCorrectly()
    {
        var p1 = new Point(1, 2);
        var p2 = new Point(1, 2);
        var p3 = new Point(3, 4);

        Assert.AreEqual(p1, p2);
        Assert.AreNotEqual(p1, p3);
    }
}
