using MathLibrary.RenderImage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RenderImage.Base.Model;

namespace RenderImage.BaseTests.Model;

[TestClass]
public class ConeTests
{
    private static readonly RenderColor BaseColor = new() { Red = 0.7, Green = 0.3, Blue = 0.1 };
    private static readonly TFTriple Surface = new() { X = 0.2, Y = 0.4, Z = 0.1 };

    [TestMethod]
    public void Constructors_CreateConeWithExpectedBaseProperties()
    {
        var position = Point(0, 0, 0);
        var endPosition = Point(0, 0, 4);

        var cone = new Cone(position, endPosition, 2, BaseColor, Surface);

        Assert.IsInstanceOfType(cone, typeof(Cylinder));
        Assert.AreEqual(0d, cone.Position.Value.X, 1e-10);
        Assert.AreEqual(0d, cone.Position.Value.Y, 1e-10);
        Assert.AreEqual(2d, cone.Position.Value.Z, 1e-10);
        AssertColor(BaseColor, cone[cone.Position]);
    }

    [TestMethod]
    public void Constructors_AcceptAllOverloads()
    {
        var position = Point(0, 0, 0);
        var endPosition = Point(0, 0, 4);

        Assert.IsNotNull(new Cone(position, endPosition, 2, BaseColor));
        Assert.IsNotNull(new Cone(position, endPosition, 2, BaseColor, Surface));
        Assert.IsNotNull(new Cone(position, endPosition, 2, 1, BaseColor));
        Assert.IsNotNull(new Cone(position, endPosition, 2, 1, BaseColor, Surface));
    }

    [TestMethod]
    public void HitTest_HitsConicalEndAndSetsHitData()
    {
        var cone = new Cone(
            Point(0, 0, 0),
            Point(0, 0, 4),
            2,
            0,
            BaseColor,
            Surface);
        var ray = Ray(Point(0, 0, -5), Vector(0, 0, 1));

        var hit = cone.HitTest(ray, out var hitData);

        Assert.IsTrue(hit);
        Assert.AreEqual(5d, hitData.Distance, 1e-10);
        Assert.AreEqual(0d, hitData.HitPoint.Value.X, 1e-10);
        Assert.AreEqual(0d, hitData.HitPoint.Value.Z, 1e-10);
        Assert.AreEqual(-1d, hitData.Normalvec.Value.Z, 1e-10);
    }

    [TestMethod]
    public void HitTest_ReturnsFalseForRayParallelToAxisOutsideCone()
    {
        var cone = new Cone(Point(0, 0, 0), Point(0, 0, 4), 2, 0, BaseColor);
        var ray = Ray(Point(3, 0, -1), Vector(0, 0, 1));

        Assert.IsFalse(cone.HitTest(ray, out var hitData));
        Assert.AreEqual(0d, hitData.Distance, 1e-10);
    }

    [TestMethod]
    public void EqualRadii_FallsBackToCylinderHitTest()
    {
        var cone = new Cone(
            Point(0, 0, 0),
            Point(0, 0, 4),
            2,
            2,
            BaseColor,
            Surface);
        var ray = Ray(Point(0, 0, -5), Vector(0, 0, 1));

        var hit = cone.HitTest(ray, out var hitData);

        Assert.IsTrue(hit);
        Assert.AreEqual(5d, hitData.Distance, 1e-10);
        Assert.AreEqual(0d, hitData.HitPoint.Value.Z, 1e-10);
    }

    [TestMethod]
    public void HitTest_ReturnsFalseForRayPointingAwayFromCone()
    {
        var cone = new Cone(Point(0, 0, 0), Point(0, 0, 4), 2, 0, BaseColor);
        var ray = Ray(Point(3, 0, 2), Vector(1, 0, 0));

        Assert.IsFalse(cone.HitTest(ray, out _));
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
