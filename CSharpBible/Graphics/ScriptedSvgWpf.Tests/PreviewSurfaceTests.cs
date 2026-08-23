using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptedSvgWpf.Models;
using ScriptedSvgWpf.Rendering;

namespace ScriptedSvgWpf.Tests;

[TestClass]
public sealed class PreviewSurfaceTests
{
    [TestMethod]
    public void DocumentPropertyHasExpectedMetadataAndDefaultValue()
    {
        var metadata = (FrameworkPropertyMetadata)PreviewSurface.DocumentProperty.GetMetadata(typeof(PreviewSurface));

        Assert.IsNull(metadata.DefaultValue);
        Assert.IsTrue(metadata.AffectsRender);
    }

    [TestMethod]
    public void DocumentPropertyRoundTripsAssignedDocument()
    {
        RunOnSta(() =>
        {
            var surface = new PreviewSurface();
            var document = new RenderDocument(10, 20, "Red");

            surface.Document = document;

            Assert.AreSame(document, surface.Document);
        });
    }

    [TestMethod]
    public void RenderWithNullDocumentDrawsWhiteSmokeBackground()
    {
        RunOnSta(() =>
        {
            var bitmap = Render(new PreviewSurface(), 8, 8);

            AssertColorNear(Colors.WhiteSmoke, GetPixel(bitmap, 4, 4));
        });
    }

    [TestMethod]
    [DataRow(0d, 10d)]
    [DataRow(10d, 0d)]
    [DataRow(-1d, 10d)]
    [DataRow(10d, -1d)]
    public void RenderWithInvalidDocumentDimensionsDrawsWhiteSmokeBackground(double width, double height)
    {
        RunOnSta(() =>
        {
            var surface = new PreviewSurface
            {
                Document = new RenderDocument(width, height, "Red")
            };
            var bitmap = Render(surface, 8, 8);

            AssertColorNear(Colors.WhiteSmoke, GetPixel(bitmap, 4, 4));
        });
    }

    [TestMethod]
    public void RenderWithZeroActualSizeDoesNotThrow()
    {
        RunOnSta(() =>
        {
            var surface = new PreviewSurface
            {
                Document = new RenderDocument(10, 10, "Red")
            };

            var bitmap = RenderWithArrangeSize(surface, 0, 0);

            Assert.AreEqual(1, bitmap.PixelWidth);
            Assert.AreEqual(1, bitmap.PixelHeight);
        });
    }

    [TestMethod]
    public void RenderCanBeRepeatedAfterSizeChange()
    {
        RunOnSta(() =>
        {
            var surface = new PreviewSurface
            {
                Document = new RenderDocument(10, 10, "Red")
            };

            var first = Render(surface, 10, 20);
            var second = Render(surface, 20, 10);

            Assert.AreEqual(10, first.PixelWidth);
            Assert.AreEqual(20, second.PixelWidth);
            AssertColorNear(Colors.Red, GetPixel(second, 10, 5));
        });
    }

    [TestMethod]
    public void RenderDrawsEverySupportedCommandType()
    {
        RunOnSta(() =>
        {
            var document = new RenderDocument(100, 100, "White");
            document.Commands.Add(new RectangleCommand(5, 5, 20, 20, "Red", 1.1, 15));
            document.Commands.Add(new CircleCommand(50, 20, 8, "Green"));
            document.Commands.Add(new LineCommand(5, 40, 30, 40, "Blue", 2));
            document.Commands.Add(new TextCommand(5, 50, "Text", "Black", 12));
            document.Commands.Add(new PolygonCommand(
                new[] { new ScriptPoint(60, 40), new ScriptPoint(80, 40), new ScriptPoint(70, 60) },
                "Cyan",
                "Black",
                1));
            document.Commands.Add(new PolygonCommand(Array.Empty<ScriptPoint>(), "Yellow"));
            document.Commands.Add(new PathCommand("M 60,70 L 80,70 L 70,90 Z", "Black", 1, "Magenta"));

            var bitmap = Render(new PreviewSurface { Document = document });

            AssertColorNear(Colors.Red, GetPixel(bitmap, 15, 15));
            AssertColorNear(Colors.Green, GetPixel(bitmap, 50, 20));
            AssertColorNear(Colors.Blue, GetPixel(bitmap, 15, 40));
            AssertColorNear(Colors.Cyan, GetPixel(bitmap, 70, 50));
            AssertColorNear(Colors.Magenta, GetPixel(bitmap, 70, 75));
        });
    }

    [TestMethod]
    public void RenderUsesBlackFallbackForInvalidBrushAndIgnoresInvalidPath()
    {
        RunOnSta(() =>
        {
            var document = new RenderDocument(100, 100, "not-a-brush");
            document.Commands.Add(new RectangleCommand(40, 40, 20, 20, "also-not-a-brush"));
            document.Commands.Add(new PathCommand("not valid geometry", "not-a-brush", 1, "not-a-brush"));

            var bitmap = Render(new PreviewSurface { Document = document });

            AssertColorNear(Colors.Black, GetPixel(bitmap, 50, 50));
            AssertColorNear(Colors.Black, GetPixel(bitmap, 5, 5));
        });
    }

    [TestMethod]
    public void RenderSupportsCommandsWithoutOptionalPolygonOrPathStrokes()
    {
        RunOnSta(() =>
        {
            var document = new RenderDocument(100, 100, "White");
            document.Commands.Add(new PolygonCommand(
                new[] { new ScriptPoint(10, 10), new ScriptPoint(30, 10), new ScriptPoint(20, 30) },
                "Orange"));
            document.Commands.Add(new PathCommand("M 40,10 L 60,10 L 50,30 Z", null, 1, "Purple"));

            var bitmap = Render(new PreviewSurface { Document = document });

            AssertColorNear(Colors.Orange, GetPixel(bitmap, 20, 16));
            AssertColorNear(Colors.Purple, GetPixel(bitmap, 50, 16));
        });
    }

    private static RenderTargetBitmap Render(PreviewSurface surface, int width = 100, int height = 100)
    {
        surface.Measure(new Size(width, height));
        surface.Arrange(new Rect(0, 0, width, height));
        surface.UpdateLayout();

        var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(surface);
        return bitmap;
    }

    private static RenderTargetBitmap RenderWithArrangeSize(PreviewSurface surface, double width, double height, int bitmapWidth = 1, int bitmapHeight = 1)
    {
        surface.Measure(new Size(width, height));
        surface.Arrange(new Rect(0, 0, width, height));
        surface.UpdateLayout();

        var bitmap = new RenderTargetBitmap(bitmapWidth, bitmapHeight, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(surface);
        return bitmap;
    }

    private static Color GetPixel(BitmapSource bitmap, int x, int y)
    {
        var pixels = new byte[4];
        bitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);
        return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
    }

    private static void RunOnSta(Action action)
    {
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception caught)
            {
                exception = caught;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (exception is not null)
            throw new AssertFailedException("The STA test action failed.", exception);
    }

    private static void AssertColorNear(Color expected, Color actual, byte tolerance = 3)
    {
        Assert.IsTrue(Math.Abs(expected.R - actual.R) <= tolerance, $"Expected {expected}, actual {actual}.");
        Assert.IsTrue(Math.Abs(expected.G - actual.G) <= tolerance, $"Expected {expected}, actual {actual}.");
        Assert.IsTrue(Math.Abs(expected.B - actual.B) <= tolerance, $"Expected {expected}, actual {actual}.");
    }
}
