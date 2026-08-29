using BaseLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Drawing;

namespace ConsoleLib.ExtCon.Tests;

[TestClass]
public class TextCanvasTests
{
    private static readonly char[] Border = { '-', '|', '+', '+', '+', '+', '+', '+', '+', '+', '+' };

    [TestMethod]
    public void DrawRect_WithZeroDimension_DoesNotWrite()
    {
        var console = CreateConsole();
        var canvas = new TextCanvas(console, new Rectangle(10, 20, 5, 4));

        canvas.DrawRect(new Rectangle(0, 0, 0, 3), ConsoleColor.White, ConsoleColor.Black, Border);

        console.DidNotReceive().SetCursorPosition(Arg.Any<int>(), Arg.Any<int>());
        console.DidNotReceive().Write(Arg.Any<char>());
    }

    [TestMethod]
    public void DrawRect_WithOneColumn_WritesVerticalBorder()
    {
        var console = CreateConsole();
        var canvas = new TextCanvas(console, new Rectangle(10, 20, 5, 4));

        canvas.DrawRect(new Rectangle(1, 0, 1, 3), ConsoleColor.White, ConsoleColor.Black, Border);

        console.Received().SetCursorPosition(11, 20);
        console.Received().SetCursorPosition(11, 21);
        console.Received().SetCursorPosition(11, 22);
        console.Received(3).Write('|');
    }

    [TestMethod]
    public void DrawRect_WithOneRow_WritesHorizontalBorder()
    {
        var console = CreateConsole();
        var canvas = new TextCanvas(console, new Rectangle(10, 20, 5, 4));

        canvas.DrawRect(new Rectangle(0, 1, 4, 1), ConsoleColor.White, ConsoleColor.Black, Border);

        console.Received().SetCursorPosition(10, 21);
        console.Received(1).Write("----");
    }

    [TestMethod]
    public void DrawRect_WithNormalRectangle_WritesCorners()
    {
        var console = CreateConsole();
        var canvas = new TextCanvas(console, new Rectangle(10, 20, 5, 4));

        canvas.DrawRect(new Rectangle(1, 1, 3, 2), ConsoleColor.White, ConsoleColor.Black, Border);

        console.Received().SetCursorPosition(11, 21);
        console.Received().Write('+');
        console.Received().SetCursorPosition(13, 21);
        console.Received().SetCursorPosition(11, 22);
        console.Received().SetCursorPosition(13, 22);
    }

    [TestMethod]
    public void OutTextXY_WithColors_AppliesCanvasOffsetAndColors()
    {
        var console = CreateConsole();
        var canvas = new TextCanvas(console, new Rectangle(10, 20, 5, 4));

        canvas.OutTextXY(1, 2, 'X', ConsoleColor.Yellow, ConsoleColor.Blue);

        Assert.AreEqual(ConsoleColor.Yellow, console.ForegroundColor);
        Assert.AreEqual(ConsoleColor.Blue, console.BackgroundColor);
        console.Received().SetCursorPosition(11, 22);
        console.Received().Write('X');
    }

    [TestMethod]
    public void SetDimension_ChangesOnlyCanvasSize()
    {
        var console = CreateConsole();
        var canvas = new TextCanvas(console, new Rectangle(10, 20, 5, 4));

        canvas.SetDimension(8, 6);

        Assert.AreEqual(new Rectangle(10, 20, 8, 6), canvas.Dimension);
    }

    private static IConsole CreateConsole()
    {
        var console = Substitute.For<IConsole>();
        console.BufferHeight.Returns(50);
        return console;
    }
}
