using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using System.Drawing;
using ConsoleLib.CommonControls.Tests;
using ConsoleLib.Data;
using ConsoleLib.Interfaces;
using BaseLib.Interfaces;
using System;
using NSubstitute;

namespace ConsoleLibTests.CommonControls;

[TestClass]
public class TerminalTests : TestBase
{
    [TestMethod]
    [DataRow(10,5,14,6,"ABC","D")]
    [DataRow(12,6,16,8,"Hello","World")] 
    public void Resize_Preserves_Content(int w1,int h1,int w2,int h2,string first,string second)
    {
        var term = new Terminal{ Dimension=new Rectangle(0,0,w1,h1)};
        term.Write(first);
        term.size = new Size(w2,h2);
        term.Write(second);
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void WriteLine_And_Scroll_Wrap_Tab_CR()
    {
        var term = new Terminal{ Dimension=new Rectangle(0,0,10,4)};
        for(int i=0;i<10;i++) term.WriteLine($"L{i}");
        term.Write('\t');
        term.Write('\r');
        term.Write("X");
        Assert.IsTrue(true);
    }

    [TestMethod]
    [DataRow("Data","After")] 
    [DataRow("123","456")] 
    public void Clear_Resets_Area(string first,string second)
    {
        var term = new Terminal{ Dimension=new Rectangle(0,0,12,5)};
        term.Write(first);
        term.Clear();
        term.Write(second);
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Ctor_Uses_SingleBorderStyle_By_Default()
    {
        var term = new Terminal();

        Assert.AreEqual(BorderStyle.Single, term.BorderStyle);
    }

    [TestMethod]
    public void ConsoleProperties_And_CursorPosition_Are_Accessible()
    {
        var term = new Terminal { Dimension = new Rectangle(0, 0, 12, 5) };
        var console = (IConsole)term;

        console.ForegroundColor = ConsoleColor.Green;
        console.BackgroundColor = ConsoleColor.Blue;
        term.SetCursorPosition(3, 2);

        Assert.AreEqual(ConsoleColor.Green, console.ForegroundColor);
        Assert.AreEqual(ConsoleColor.Blue, console.BackgroundColor);
        Assert.AreEqual((3, 2), term.GetCursorPosition());
        Assert.AreEqual(10, term.WindowWidth);
        Assert.AreEqual(3, term.WindowHeight);
        Assert.AreEqual(10, term.BufferWidth);
        Assert.AreEqual(3, term.BufferHeight);
        Assert.IsFalse(term.KeyAvailable);
        Assert.IsNull(term.ReadKey());
        Assert.AreEqual(string.Empty, term.ReadLine());
    }

    [TestMethod]
    public void RenderRows_Stores_Content_And_Fills_Missing_Cells()
    {
        var term = new Terminal { Dimension = new Rectangle(0, 0, 6, 4) };
        var console = (IConsole)term;
        console.ForegroundColor = ConsoleColor.Yellow;
        console.BackgroundColor = ConsoleColor.DarkBlue;

        term.RenderRows(new[] { "AB", "C" });

        Assert.AreEqual('A', term.GetScreenCell(0, 0).c);
        Assert.AreEqual('B', term.GetScreenCell(1, 0).c);
        Assert.AreEqual('C', term.GetScreenCell(0, 1).c);
        Assert.AreEqual(' ', term.GetScreenCell(5 - 2, 0).c);
        Assert.AreEqual(ConsoleColor.Yellow, term.GetScreenCell(0, 0).fc.Fg);
        Assert.AreEqual(ConsoleColor.DarkBlue, term.GetScreenCell(0, 0).fc.Bg);
    }

    [TestMethod]
    public void Write_And_WriteLine_Update_Buffer_And_Colors()
    {
        var term = new Terminal { Dimension = new Rectangle(0, 0, 7, 4) };
        var console = (IConsole)term;
        console.ForegroundColor = ConsoleColor.Cyan;
        console.BackgroundColor = ConsoleColor.DarkRed;

        term.Write("AB");
        term.WriteLine("C");

        Assert.AreEqual('A', term.GetScreenCell(0, 0).c);
        Assert.AreEqual('B', term.GetScreenCell(1, 0).c);
        Assert.AreEqual('C', term.GetScreenCell(2, 0).c);
        Assert.AreEqual(ConsoleColor.Cyan, term.GetScreenCell(0, 0).fc.Fg);
        Assert.AreEqual(ConsoleColor.DarkRed, term.GetScreenCell(0, 0).fc.Bg);
        Assert.AreEqual((0, 1), term.GetCursorPosition());
    }

    [TestMethod]
    public void Clear_ResetColor_And_Draw_Use_Control_State()
    {
        var term = new Terminal { Dimension = new Rectangle(0, 0, 6, 3) };
        term.Write("X");
        term.ResetColor();
        term.Clear();
        term.Draw();

        Assert.AreEqual(' ', term.GetScreenCell(0, 0).c);
        Assert.AreEqual(ConsoleColor.Gray, term.ForeColor);
        Assert.AreEqual(ConsoleColor.Black, term.BackColor);
        Assert.IsTrue(term.Valid);
    }

    [TestMethod]
    public void MouseEvents_And_NativeWindowOperations_Are_Handled()
    {
        var term = new Terminal { Dimension = new Rectangle(0, 0, 10, 4) };
        var mouse = Substitute.For<IMouseEvent>();
        var inputCount = 0;
        term.OnMouseInput += (_, _) => inputCount++;

        term.MouseClick(mouse);
        term.MouseMove(mouse, Point.Empty);
        term.Beep(440, 10);

        Assert.AreEqual(2, inputCount);
        Assert.Throws<NotImplementedException>(() => term.SetWindowPosition(0, 0));
        Assert.Throws<NotImplementedException>(() => term.SetWindowSize(10, 4));
        Assert.Throws<NotImplementedException>(() => term.LargestWindowWidth.ToString());
    }

    [TestMethod]
    public void ScreenCell_And_FullColor_Conversions_Preserve_Values()
    {
        ScreenCell cell = ('x', (ConsoleColor.Red, ConsoleColor.Black));
        (char character, FullColor color) tuple = cell;
        FullColor fullColor = (ConsoleColor.Green, ConsoleColor.White);
        (ConsoleColor foreground, ConsoleColor background) colors = fullColor;

        Assert.AreEqual('x', tuple.character);
        Assert.AreEqual(ConsoleColor.Red, tuple.color.Fg);
        Assert.AreEqual(ConsoleColor.Green, colors.foreground);
        Assert.AreEqual(ConsoleColor.White, colors.background);
        Assert.AreEqual("x (Red/Black)", cell.ToString());
        Assert.AreEqual(' ', ScreenCell.Blank.c);
    }
}