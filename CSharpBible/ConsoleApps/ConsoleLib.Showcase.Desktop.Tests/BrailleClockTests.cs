using System;
using System.Linq;
using ConsoleLib.Showcase.Desktop.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Showcase.Desktop.Tests;

[TestClass]
public sealed class BrailleClockTests
{
    [TestMethod]
    public void ClockFaceUsesBrailleCellsAtTwoByFourPixelResolution()
    {
        var face = BrailleClockRenderer.Render(new DateTime(2026, 9, 1, 3, 0, 0));
        var rows = face.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

        Assert.AreEqual(6, rows.Length);
        Assert.IsTrue(rows.All(row => row.Length == 18));
        Assert.IsTrue(face.Any(character => character > '\u2800'));
    }

    [TestMethod]
    public void ClockFaceChangesWhenTheSecondHandMoves()
    {
        var first = BrailleClockRenderer.Render(new DateTime(2026, 9, 1, 12, 0, 0));
        var second = BrailleClockRenderer.Render(new DateTime(2026, 9, 1, 12, 0, 30));

        Assert.AreNotEqual(first, second);
    }

    [TestMethod]
    public void HalfBlockClockUsesPortableTwoPixelCells()
    {
        var face = HalfBlockClockRenderer.Render(new DateTime(2026, 9, 1, 3, 0, 0));
        var rows = face.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

        Assert.AreEqual(12, rows.Length);
        Assert.IsTrue(rows.All(row => row.Length == 48));
        Assert.IsTrue(face.Contains('░') || face.Contains('▒') || face.Contains('▓'));
    }
}
