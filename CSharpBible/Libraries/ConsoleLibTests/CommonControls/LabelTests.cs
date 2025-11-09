using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using System.Drawing;
using System;
using ConsoleLibTests.CommonControls; // for ConsoleColor

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class LabelTests : TestBase
{
    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Draw_With_Or_Without_ParentBackground(bool useParent)
    {
        var p = new Panel{ Dimension=new Rectangle(0,0,20,3)}; p.BackColor = ConsoleColor.DarkBlue;
        var l = new Label{ Parent=p, Dimension=new Rectangle(1,1,10,1), ParentBackground=useParent};
        l.Text = "Hello";
        l.Draw();
        Assert.AreEqual("Hello", l.Text);
    }
}