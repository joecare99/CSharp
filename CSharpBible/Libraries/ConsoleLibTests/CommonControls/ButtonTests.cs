using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using System.Drawing;
using System.Windows.Input;
using NSubstitute;
using System;
using ConsoleLibTests.CommonControls; // for ConsoleColor

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class ButtonTests : TestBase
{
    [TestMethod]
    [DataRow(5,3,"Ok", ConsoleColor.DarkGreen)]
    [DataRow(0,0,"X", ConsoleColor.Blue)]
    public void Set_Configures_Size_Position_And_Text(int x,int y,string text, ConsoleColor col)
    {
        var b = new Button();
        b.Set(x,y,text,col);
        Assert.AreEqual(new Point(x,y), b.Position);
        Assert.AreEqual(text, b.Text);
        Assert.AreEqual(text.Length+2, b.size.Width);
    }

    [TestMethod]
    public void MouseEnter_Highlights_When_Enabled()
    {
        var b = new Button();
        b.Set(0,0,"X",ConsoleColor.Blue);
        b.MouseEnter(new Point(0,0));
        Assert.IsFalse(b.Valid); // Invalidate ausgelöst
    }

    [TestMethod]
    [DataRow(false,true)]
    [DataRow(true,false)]
    public void Command_Sets_Enabled_From_CanExecute(bool first,bool second)
    {
        var b = new Button();
        var cmd = Substitute.For<ICommand>();
        cmd.CanExecute(Arg.Any<object?>()).Returns(first);
        b.Command = cmd;
        Assert.AreEqual(first, b.Enabled);
        cmd.CanExecute(Arg.Any<object?>()).Returns(second);
        cmd.CanExecuteChanged += Raise.Event();
        Assert.AreEqual(second, b.Enabled);
    }
}