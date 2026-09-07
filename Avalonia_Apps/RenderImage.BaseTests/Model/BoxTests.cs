using MathLibrary.RenderImage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RenderImage.Base.Model;

namespace RenderImage.BaseTests.Model;

[TestClass]
public class BoxTests
{
    private static readonly RenderColor BaseColor = new() { Red = 0.7, Green = 0.3, Blue = 0.1 };
    private static readonly TFTriple Surface = new() { X = 0.2, Y = 0.4, Z = 0.1 };

    [TestMethod]
    public void Constructors_SetPositionColorAndSurface()
    {
        var box = new Box(Point(1, 2, 3), Vector(4, 6, 8), BaseColor, Surface);

        Assert.AreEqual(1d, box.Position.Value.X, 1e-10);
        Assert.AreEqual(2d, box.Position.Value.Y, 1e-10);
        Assert.AreEqual(3d, box.Position.Value.Z, 1e-10);
        AssertColor(BaseColor, box[box.Position]);

        var hit = box.HitTest(Ray(Point(1, 2, -2), Vector(0, 0, 1)), out var hitData);

        Assert.IsTrue(hit);
        Assert.AreEqual(Surface.X, hitData.AmbientVal, 1e-10);
        Assert.AreEqual(Surface.Y, hitData.ReflectionVal, 1e-10);
        Assert.AreEqual(Surface.Z, hitData.Refraction, 1e-10);
    }

    [TestMethod]
    public void ConstructorWithoutSurface_UsesDefaultSurfaceValues()
    {
        var box = new Box(Point(0, 0, 0), Vector(4, 4, 4), BaseColor);

        Assert.IsTrue(box.HitTest(Ray(Point(0, 0, -3), Vector(0, 0, 1)), out var hitData));
        Assert.AreEqual(0.6, hitData.AmbientVal, 1e-10);
        Assert.AreEqual(0.4, hitData.ReflectionVal, 1e-10);
        Assert.AreEqual(0d, hitData.Refraction, 1e-10);
    }

    [TestMethod]
    public void HitTest_HitsAllSixAxisAlignedFaces()
    {
        var box = new Box(Point(0, 0, 0), Vector(4, 4, 4), BaseColor, Surface);

        AssertHit(box, Point(0, 0, -6), Vector(0, 0, 1), 4, 0, 0, -1);
        AssertHit(box, Point(0, 0, 6), Vector(0, 0, -1), 4, 0, 0, 1);
        AssertHit(box, Point(0, -6, 0), Vector(0, 1, 0), 4, 0, -1, 0);
        AssertHit(box, Point(0, 6, 0), Vector(0, -1, 0), 4, 0, 1, 0);
        AssertHit(box, Point(-6, 0, 0), Vector(1, 0, 0), 4, -1, 0, 0);
        AssertHit(box, Point(6, 0, 0), Vector(-1, 0, 0), 4, 1, 0, 0);
    }

    [TestMethod]
    public void HitTest_FromInsideBoxHitsExitFace()
    {
        var box = new Box(Point(0, 0, 0), Vector(4, 4, 4), BaseColor, Surface);

        var hit = box.HitTest(Ray(Point(0, 0, 0), Vector(1, 0, 0)), out var hitData);

        Assert.IsTrue(hit);
        Assert.AreEqual(2d, hitData.Distance, 1e-10);
        Assert.AreEqual(2d, hitData.HitPoint.Value.X, 1e-10);
        Assert.AreEqual(-1d, hitData.Normalvec.Value.X, 1e-10);
    }

    [TestMethod]
    public void HitTest_ReturnsFalseForRayMovingAwayOrMissingFace()
    {
        var box = new Box(Point(0, 0, 0), Vector(4, 4, 4), BaseColor, Surface);

        Assert.IsFalse(box.HitTest(Ray(Point(6, 0, 0), Vector(1, 0, 0)), out var awayHit));
        Assert.AreEqual(0d, awayHit.Distance, 1e-10);

        Assert.IsFalse(box.HitTest(Ray(Point(0, 3, -6), Vector(0, 0, 1)), out var offsetHit));
        Assert.AreEqual(0d, offsetHit.Distance, 1e-10);
    }

    [TestMethod]
    public void BoundaryTest_UsesBoxExtents()
    {
        var box = new Box(Point(0, 0, 0), Vector(4, 4, 4), BaseColor, Surface);

        Assert.IsTrue(box.BoundaryTest(Ray(Point(0, 0, 0), Vector(1, 0, 0)), out var insideDistance));
        Assert.AreEqual(0d, insideDistance, 1e-10);

        Assert.IsTrue(box.BoundaryTest(Ray(Point(0, 0, -6), Vector(0, 0, 1)), out var frontDistance));
        Assert.AreEqual(4d, frontDistance, 1e-10);

        Assert.IsFalse(box.BoundaryTest(Ray(Point(6, 0, 0), Vector(1, 0, 0)), out var missingDistance));
        Assert.AreEqual(-1d, missingDistance, 1e-10);
    }

    private static void AssertHit(
        Box box,
        RenderPoint start,
        RenderVector direction,
        double expectedDistance,
        double normalX,
        double normalY,
        double normalZ)
    {
        Assert.IsTrue(box.HitTest(Ray(start, direction), out var hitData));
        Assert.AreEqual(expectedDistance, hitData.Distance, 1e-10);
        Assert.AreEqual(normalX, hitData.Normalvec.Value.X, 1e-10);
        Assert.AreEqual(normalY, hitData.Normalvec.Value.Y, 1e-10);
        Assert.AreEqual(normalZ, hitData.Normalvec.Value.Z, 1e-10);
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
