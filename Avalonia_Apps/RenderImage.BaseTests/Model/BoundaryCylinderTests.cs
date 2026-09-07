using MathLibrary.RenderImage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RenderImage.Base.Model;

namespace RenderImage.BaseTests.Model;

[TestClass]
public class BoundaryCylinderTests
{
    [TestMethod]
    public void BoundaryTest_ReturnsZeroForStartInsideCylinder()
    {
        var cylinder = CreateCylinder();

        var hit = cylinder.BoundaryTest(Ray(Point(0, 0, 0), Vector(1, 0, 0)), out var distance);

        Assert.IsTrue(hit);
        Assert.AreEqual(0d, distance, 1e-10);
    }

    [TestMethod]
    public void BoundaryTest_HitsAxialEndCap()
    {
        var cylinder = CreateCylinder();

        var hit = cylinder.BoundaryTest(Ray(Point(0, 0, -5), Vector(0, 0, 1)), out var distance);

        Assert.IsTrue(hit);
        Assert.AreEqual(1d, distance, 1e-10);
    }

    [TestMethod]
    public void BoundaryTest_HitsLateralSurface()
    {
        var cylinder = CreateCylinder();

        var hit = cylinder.BoundaryTest(Ray(Point(3, 0, 0), Vector(-1, 0, 0)), out var distance);

        Assert.IsTrue(hit);
        Assert.AreEqual(2d, distance, 1e-10);
    }

    [TestMethod]
    public void BoundaryTest_ReturnsFalseForRayMissingCylinder()
    {
        var cylinder = CreateCylinder();

        var hit = cylinder.BoundaryTest(Ray(Point(3, 3, 0), Vector(0, 0, 1)), out var distance);

        Assert.IsFalse(hit);
        Assert.AreEqual(-1d, distance, 1e-10);
    }

    [TestMethod]
    public void BoundaryTest_ReturnsFalseForParallelRayOutsideHeight()
    {
        var cylinder = CreateCylinder();

        var hit = cylinder.BoundaryTest(Ray(Point(0, 0, 5), Vector(0, 0, 1)), out var distance);

        Assert.IsFalse(hit);
        Assert.AreEqual(-1d, distance, 1e-10);
    }

    [TestMethod]
    public void BoundaryTest_NormalizesCylinderAxis()
    {
        var cylinder = new BoundaryCylinder(Point(0, 0, 0), Point(0, 0, 2), 4, 1);

        var hit = cylinder.BoundaryTest(Ray(Point(0, 0, -5), Vector(0, 0, 1)), out var distance);

        Assert.IsTrue(hit);
        Assert.AreEqual(1d, distance, 1e-10);
    }

    private static BoundaryCylinder CreateCylinder()
        => new(Point(0, 0, 0), Point(0, 0, 1), 4, 1);

    private static RenderRay Ray(RenderPoint start, RenderVector direction) => RenderRay.Init(start, direction);

    private static RenderPoint Point(double x, double y, double z) => new()
    {
        Value = new TFTriple { X = x, Y = y, Z = z }
    };

    private static RenderVector Vector(double x, double y, double z) => new()
    {
        Value = new TFTriple { X = x, Y = y, Z = z }
    };
}
