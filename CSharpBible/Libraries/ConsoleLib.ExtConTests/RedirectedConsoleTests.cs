using ConsoleLib;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;

namespace ConsoleLib.ExtCon.Tests;

[TestClass]
public sealed class RedirectedConsoleTests
{
    [TestMethod]
    public void ParsesKeyboardMouseResizeAndEofFromStdio()
    {
        using var input = new StringReader("a\u001b[B\u001b[<0;12;7M\u001b[8;40;120t");
        using var console = new RedirectedConsole(input);
        var keys = new List<IKeyEvent>();
        var mice = new List<IMouseEvent>();
        var sizes = new List<Point>();
        using var finished = new ManualResetEventSlim();

        console.KeyEvent += (_, key) => keys.Add(key);
        console.MouseEvent += (_, mouse) => mice.Add(mouse);
        console.WindowBufferSizeEvent += (_, size) => sizes.Add(size);
        console.EndOfInput += (_, _) => finished.Set();

        Assert.IsTrue(finished.Wait(TimeSpan.FromSeconds(2)));
        Assert.AreEqual((ushort)ConsoleKey.A, keys[0].usKeyCode);
        Assert.AreEqual((ushort)ConsoleKey.DownArrow, keys[1].usKeyCode);
        Assert.AreEqual(new Point(11, 6), mice[0].MousePos);
        Assert.IsTrue(mice[0].MouseButtonLeft);
        Assert.AreEqual(new Point(120, 40), sizes[0]);
    }

    [TestMethod]
    public void UsesDeterministicDimensionsAndStopsWithoutTouchingConsoleWindow()
    {
        using var console = new RedirectedConsole(new StringReader(string.Empty), 120, 40);
        using var finished = new ManualResetEventSlim();
        console.EndOfInput += (_, _) => finished.Set();

        Assert.AreEqual(120, console.Width);
        Assert.AreEqual(40, console.Height);
        Assert.IsTrue(finished.Wait(TimeSpan.FromSeconds(2)));
    }
}
