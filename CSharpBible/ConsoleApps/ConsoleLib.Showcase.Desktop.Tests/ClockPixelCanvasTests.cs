using System;
using ConsoleLib.Showcase.Desktop.Controls;
using ConsoleLib.Showcase.Desktop.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Showcase.Desktop.Tests;

[TestClass]
public sealed class ClockPixelCanvasTests
{
    [TestMethod]
    public void ColorFramePreservesAllSixteenConsoleColors()
    {
        var frame = HalfBlockClockRenderer.RenderColorMap(new DateTime(2026, 9, 1, 3, 15, 30));
        var canvas = new ClockPixelCanvas();

        canvas.SetText(frame);

        Assert.IsTrue(frame.Contains('F'));
        Assert.IsTrue(frame.Contains('A'));
        Assert.IsTrue(frame.Contains('C'));
        var hasGold = false;
        for (var y = 0; y < 14; y++)
            for (var x = 0; x < 28; x++)
                hasGold |= canvas.GetCellForeground(x, y) == ConsoleColor.Yellow
                    || canvas.GetCellBackground(x, y) == ConsoleColor.Yellow;
        Assert.IsTrue(hasGold);
    }
}
