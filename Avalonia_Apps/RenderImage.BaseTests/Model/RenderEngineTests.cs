using MathLibrary.RenderImage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RenderImage.Base.Model;

namespace RenderImage.BaseTests.Model;

[TestClass]
public class RenderEngineTests
{
    [TestMethod]
    public void MaxDepth_RoundTripsValue()
    {
        var engine = new RenderEngine
        {
            MaxDepth = 7
        };

        Assert.AreEqual(7, engine.MaxDepth);
    }

    [TestMethod]
    public void Append_CanRegisterCameraAndLightSource()
    {
        var engine = new RenderEngine();
        var camera = CreateCamera(2, 2);
        var light = new RenderLightSource(Point(0, 5, -5));

        engine.Append(camera);
        engine.Append(light);

        var result = engine.RenderToArray();

        Assert.AreEqual(2, result.GetLength(1));
        Assert.AreEqual(2, result.GetLength(0));
    }

    [TestMethod]
    public void Trace_ReturnsBlackWhenDepthExceedsMaximum()
    {
        var engine = new RenderEngine
        {
            MaxDepth = 1
        };
        var ray = Ray(Point(0, 0, 0), Vector(0, 0, 1));

        var result = engine.Trace(ray, 1.0, 2);

        AssertColor(Black(), result);
    }

    [TestMethod]
    public void Trace_ReturnsBlackWhenShareIsBelowMinimum()
    {
        var engine = new RenderEngine();
        var ray = Ray(Point(0, 0, 0), Vector(0, 0, 1));

        var result = engine.Trace(ray, 1e-5, 1);

        AssertColor(Black(), result);
    }

    [TestMethod]
    public void Trace_ReturnsBlackWhenRayMissesAllObjects()
    {
        var engine = new RenderEngine();
        engine.Append(CreateCamera(2, 2));
        var ray = Ray(Point(0, 0, 0), Vector(0, 0, 1));

        var result = engine.Trace(ray, 1.0, 1);

        AssertColor(Black(), result);
    }

    [TestMethod]
    public void Trace_ReturnsAmbientColorForHitWithoutLight()
    {
        var engine = new RenderEngine();
        var sphere = new Sphere(
            Point(0, 0, 5),
            1,
            Color(0.8, 0.4, 0.2),
            new TFTriple { X = 1.0, Y = 0.0, Z = 0.0 });
        engine.Append(sphere);

        var result = engine.Trace(Ray(Point(0, 0, 0), Vector(0, 0, 1)), 1.0, 1);

        AssertColor(Color(0.8, 0.4, 0.2), result, 1e-8);
    }

    [TestMethod]
    public void Render_WithoutCameraThrows()
    {
        var engine = new RenderEngine();
        var buffer = new ArrayPixelBuffer(2, 2);

        Assert.ThrowsExactly<InvalidOperationException>(() => engine.Render(buffer));
    }

    [TestMethod]
    public void RenderToArray_WithoutCameraThrows()
    {
        var engine = new RenderEngine();

        Assert.ThrowsExactly<InvalidOperationException>(() => engine.RenderToArray());
    }

    [TestMethod]
    public void Render_WritesEveryPixelToBuffer()
    {
        var engine = new RenderEngine();
        engine.Append(CreateCamera(3, 2));
        engine.Append(new Sphere(
            Point(0, 0, 5),
            1,
            Color(0.8, 0.4, 0.2),
            new TFTriple { X = 1.0, Y = 0.0, Z = 0.0 }));
        var buffer = new ArrayPixelBuffer(3, 2);

        engine.Render(buffer);

        Assert.AreEqual(3, buffer.Width);
        Assert.AreEqual(2, buffer.Height);
        for (var y = 0; y < buffer.Height; y++)
        {
            for (var x = 0; x < buffer.Width; x++)
            {
                Assert.IsFalse(double.IsNaN(buffer.Pixels[y, x].Red));
                Assert.IsFalse(double.IsNaN(buffer.Pixels[y, x].Green));
                Assert.IsFalse(double.IsNaN(buffer.Pixels[y, x].Blue));
            }
        }
    }

    [TestMethod]
    public void RenderToArray_UsesCameraResolution()
    {
        var engine = new RenderEngine();
        engine.Append(CreateCamera(3, 2));

        var result = engine.RenderToArray();

        Assert.AreEqual(2, result.GetLength(0));
        Assert.AreEqual(3, result.GetLength(1));
    }

    [TestMethod]
    public void RenderToArray_UsesAtLeastOnePixelForSubpixelResolution()
    {
        var engine = new RenderEngine();
        engine.Append(CreateCamera(0.4, 0.4));

        var result = engine.RenderToArray();

        Assert.AreEqual(1, result.GetLength(0));
        Assert.AreEqual(1, result.GetLength(1));
    }

    private static RenderSimpleCamera CreateCamera(double width, double height)
    {
        var camera = new RenderSimpleCamera
        {
            Position = Point(0, 0, 0),
            LookAt = Point(0, 0, 1),
            Resolution = new TFTuple { X = width, Y = height }
        };
        return camera;
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

    private static RenderColor Color(double red, double green, double blue) => new()
    {
        Red = red,
        Green = green,
        Blue = blue
    };

    private static RenderColor Black() => Color(0, 0, 0);

    private static void AssertColor(RenderColor expected, RenderColor actual, double delta = 1e-10)
    {
        Assert.AreEqual(expected.Red, actual.Red, delta, "Red");
        Assert.AreEqual(expected.Green, actual.Green, delta, "Green");
        Assert.AreEqual(expected.Blue, actual.Blue, delta, "Blue");
    }
}
