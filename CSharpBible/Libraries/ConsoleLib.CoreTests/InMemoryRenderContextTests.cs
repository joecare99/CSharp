using ConsoleLib;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;

namespace ConsoleLibTests;

[TestClass]
public class InMemoryRenderContextTests
{
    [TestMethod]
    public void Constructor_WithNegativeWidth_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new InMemoryRenderContext(new Size(-1, 2)));
    }

    [TestMethod]
    public void Constructor_WithNegativeHeight_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new InMemoryRenderContext(new Size(2, -1)));
    }

    [TestMethod]
    public void SetCellAndGetCell_RoundTripsCell()
    {
        var context = new InMemoryRenderContext(new Size(2, 2));
        var expected = new TerminalCell('X', ConsoleColor.Yellow, ConsoleColor.Blue);

        context.SetCell(1, 1, expected);

        Assert.AreEqual(expected, context.GetCell(1, 1));
    }

    [TestMethod]
    public void Fill_ClipsAreaToContext()
    {
        var initial = new TerminalCell('.', ConsoleColor.Gray, ConsoleColor.Black);
        var filled = new TerminalCell('#', ConsoleColor.White, ConsoleColor.DarkBlue);
        var context = new InMemoryRenderContext(new Size(3, 2), initial);

        context.Fill(new Rectangle(-1, -1, 3, 2), filled);

        Assert.AreEqual(filled, context.GetCell(0, 0));
        Assert.AreEqual(filled, context.GetCell(1, 0));
        Assert.AreEqual(initial, context.GetCell(2, 0));
        Assert.AreEqual(initial, context.GetCell(0, 1));
    }

    [TestMethod]
    public void Invalidate_ReportsUnionArea()
    {
        var context = new InMemoryRenderContext(new Size(4, 3));

        context.Invalidate(new Rectangle(1, 1, 2, 1));

        Assert.IsTrue(context.IsInvalidated(new Rectangle(2, 1, 1, 1)));
        Assert.IsTrue(context.IsInvalidated(new Rectangle(0, 0, 1, 1)));
        Assert.IsFalse(context.IsInvalidated(new Rectangle(4, 0, 1, 1)));
    }

    [TestMethod]
    public void SetCell_WithCoordinatesOutsideContext_Throws()
    {
        var context = new InMemoryRenderContext(new Size(2, 2));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => context.SetCell(2, 0, new TerminalCell('X', ConsoleColor.White, ConsoleColor.Black)));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => context.GetCell(0, -1));
    }
}
