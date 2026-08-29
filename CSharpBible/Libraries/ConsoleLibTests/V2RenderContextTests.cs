using System;
using System.Drawing;
using ConsoleLib;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Tests;

[TestClass]
public class V2RenderContextTests
{
    [TestMethod]
    public void InMemoryContext_WritesAndFillsCells()
    {
        var context = new InMemoryRenderContext(new Size(4, 2));
        var cell = new TerminalCell('#', ConsoleColor.Yellow, ConsoleColor.Blue);

        context.SetCell(1, 0, cell);
        context.Fill(new Rectangle(2, 0, 2, 1), cell);

        Assert.AreEqual(cell, context.GetCell(1, 0));
        Assert.AreEqual(cell, context.GetCell(3, 0));
    }

    [TestMethod]
    public void InMemoryContext_ClipsInvalidation()
    {
        var context = new InMemoryRenderContext(new Size(4, 2));

        context.Invalidate(new Rectangle(-2, -2, 3, 3));

        Assert.IsTrue(context.IsInvalidated(new Rectangle(0, 0, 1, 1)));
        Assert.IsFalse(context.IsInvalidated(new Rectangle(3, 1, 1, 1)));
    }
}
