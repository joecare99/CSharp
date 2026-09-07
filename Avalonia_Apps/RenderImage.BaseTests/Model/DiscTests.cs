using MathLibrary.RenderImage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RenderImage.Base.Model;

namespace RenderImage.BaseTests.Model;

[TestClass]
public class DiscTests
{
    private static readonly RenderColor BaseColor = new() { Red = 0.7, Green = 0.3, Blue = 0.1 };
    private static readonly TFTriple Surface = new() { X = 0.2, Y = 0.4, Z = 0.1 };

    [TestMethod]
    public void Constructor_SetsPositionAndBaseColor()
    {
        var disc = new Disc(Point(1, 2, 3), Vector(0, 0, 2), 4, BaseColor, Surface);

        Assert.AreEqual(1d, disc.Position.Value.X, 1e-10);
        Assert.AreEqual(2d, disc.Position.Value.Y, 1e-10);
        Assert.AreEqual(3d, disc.Position.Value.Z, 1e-10);
        AssertColor(BaseColor, disc[disc.Position]);
    }

    [TestMethod]
    public void ConstructorWithoutSurface_UsesDefaultSurfaceValues()
    {
        var disc = new Disc(Point(0, 0, 0), Vector(0, 0, 1), 2, BaseColor);

        Assert.IsTrue(disc.HitTest(Ray(Point(0, 0, -3), Vector(0, 0, 1)), out var hitData));
        Assert.AreEqual(0.6, hitData.AmbientVal, 1e-10);
        Assert.AreEqual(0.4, hitData.ReflectionVal, 1e-10);
        Assert.AreEqual(0d, hitData.Refraction, 1e-10);
    }

    [TestMethod]
    public void HitTest_HitsDiscAndSetsSurfaceData()
    {
        var disc = new Disc(Point(0, 0, 0), Vector(0, 0, 2), 2, BaseColor, Surface);

        var hit = disc.HitTest(Ray(Point(0.5, 0.5, -3), Vector(0, 0, 1)), out var hitData);

        Assert.IsTrue(hit);
        Assert.AreEqual(3d, hitData.Distance, 1e-10);
        Assert.AreEqual(0.5, hitData.HitPoint.Value.X, 1e-10);
        Assert.AreEqual(0.5, hitData.HitPoint.Value.Y, 1e-10);
        Assert.AreEqual(0d, hitData.HitPoint.Value.Z, 1e-10);
        Assert.AreEqual(0d, hitData.Normalvec.Value.X, 1e-10);
        Assert.AreEqual(0d, hitData.Normalvec.Value.Y, 1e-10);
        Assert.AreEqual(-1d, hitData.Normalvec.Value.Z, 1e-10);
        Assert.AreEqual(Surface.X, hitData.AmbientVal, 1e-10);
        Assert.AreEqual(Surface.Y, hitData.ReflectionVal, 1e-10);
        Assert.AreEqual(Surface.Z, hitData.Refraction, 1e-10);
    }

    [TestMethod]
    public void HitTest_ReturnsFalseForRayStartingOnWrongSide()
    {
        var disc = new Disc(Point(0, 0, 0), Vector(0, 0, 1), 2, BaseColor, Surface);

        var hit = disc.HitTest(Ray(Point(0, 0, 3), Vector(0, 0, 1)), out var hitData);

        Assert.IsFalse(hit);
        Assert.AreEqual(0d, hitData.Distance, 1e-10);
    }

    [TestMethod]
    public void HitTest_ReturnsFalseOutsideRadius()
    {
        var disc = new Disc(Point(0, 0, 0), Vector(0, 0, 1), 2, BaseColor, Surface);

        var hit = disc.HitTest(Ray(Point(2.1, 0, -3), Vector(0, 0, 1)), out var hitData);

        Assert.IsFalse(hit);
        Assert.AreEqual(-1d, hitData.Distance, 1e-10);
    }

    [TestMethod]
    public void BoundaryTest_ReturnsDistanceForHit()
    {
        var disc = new Disc(Point(0, 0, 0), Vector(0, 0, 1), 2, BaseColor, Surface);

        Assert.IsTrue(disc.BoundaryTest(Ray(Point(0, 0, -3), Vector(0, 0, 1)), out var distance));
        Assert.AreEqual(3d, distance, 1e-10);
    }

    [TestMethod]
    public void BoundaryTest_ReturnsFalseOutsideRadiusOrWrongSide()
    {
        var disc = new Disc(Point(0, 0, 0), Vector(0, 0, 1), 2, BaseColor, Surface);

        Assert.IsFalse(disc.BoundaryTest(Ray(Point(2.1, 0, -3), Vector(0, 0, 1)), out var outsideDistance));
        Assert.AreEqual(-1d, outsideDistance, 1e-10);

        Assert.IsFalse(disc.BoundaryTest(Ray(Point(0, 0, 3), Vector(0, 0, 1)), out var wrongSideDistance));
        Assert.AreEqual(-1d, wrongSideDistance, 1e-10);
    }

    private static RenderRay Ray(RenderPoint start, RenderVector direction) => RenderRay.Init(start, direction);

    private static RenderPoint Point(double x, double y, double z) => new()
    {
        Value = new TFTriple { X = x, Y = y, Z = z }
    };

    private static RenderVector Vector(double x, double y, double z) => new()
    {
        Value = new TFTriple { X = x, Y = y, Z = z }
    };

    private static void AssertColor(RenderColor expected, RenderColor actual)
    {
        Assert.AreEqual(expected.Red, actual.Red, 1e-10);
        Assert.AreEqual(expected.Green, actual.Green, 1e-10);
        Assert.AreEqual(expected.Blue, actual.Blue, 1e-10);
    }
}
