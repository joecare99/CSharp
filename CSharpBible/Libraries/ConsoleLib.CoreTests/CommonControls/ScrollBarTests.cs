using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using System;
using System.Drawing;
using NSubstitute;
using ConsoleLib.Interfaces;
using ConsoleLib.CommonControls.Tests; // for TestBase

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class ScrollBarTests : TestBase
{
    [TestMethod]
    public void Ctor_Defaults()
    {
        var sb = new ScrollBar();
        Assert.IsTrue(sb.Vertical);
        Assert.AreEqual(0, sb.Minimum);
        Assert.AreEqual(100, sb.Maximum);
        Assert.AreEqual(0, sb.Value);
    }

    [TestMethod]
    public void Set_Minimum_Adjusts_Max_And_Value()
    {
        var sb = new ScrollBar();
        sb.Value = 50;
        sb.Minimum = 60;
        Assert.AreEqual(60, sb.Minimum);
        Assert.AreEqual(100, sb.Maximum);
        Assert.AreEqual(60, sb.Value);
    }

    [TestMethod]
    public void Set_Maximum_Clamps_Value()
    {
        var sb = new ScrollBar();
        sb.Value = 90;
        sb.Maximum = 80;
        Assert.AreEqual(80, sb.Maximum);
        Assert.AreEqual(80, sb.Value);
    }

    [TestMethod]
    public void LargeChange_Rejects_Zero()
    {
        var sb = new ScrollBar();
        sb.LargeChange = 0;
        Assert.AreEqual(1, sb.LargeChange);
    }

    [TestMethod]
    public void Value_Raises_Event()
    {
        var sb = new ScrollBar();
        int cnt=0; sb.OnValueChanged += (_,_)=>cnt++;
        sb.Value = 10; sb.Value = 10; sb.Value = 15;
        Assert.AreEqual(2,cnt); // initial->10, 10->15
    }

    [TestMethod]
    public void Draw_Validates()
    {
        var sb = new ScrollBar{ Dimension=new Rectangle(0,0,1,8)};
        sb.Value = 30;
        sb.Draw();
        Assert.IsTrue(sb.Valid);
    }

    [TestMethod]
    public void Thumb_Computes_For_Horizontal()
    {
        var sb = new ScrollBar{ Vertical=false, Dimension=new Rectangle(0,0,10,1)};
        sb.Maximum = 200; sb.Minimum = 0; sb.Value = 100; sb.LargeChange=50;
        sb.Draw();
        Assert.IsTrue(sb.Valid);
    }

    [TestMethod]
    public void Keyboard_Small_And_Large_Steps()
    {
        var sb = new ScrollBar();
        var key = Substitute.For<IKeyEvent>();
        key.bKeyDown.Returns(true);
        key.KeyChar.Returns('+'); sb.HandlePressKeyEvents(key); Assert.AreEqual(1,sb.Value);
        key.KeyChar.Returns('-'); sb.HandlePressKeyEvents(key); Assert.AreEqual(0,sb.Value);
        key.KeyChar.Returns('P'); sb.HandlePressKeyEvents(key); Assert.AreEqual(sb.LargeChange,sb.Value);
        key.KeyChar.Returns('O'); sb.HandlePressKeyEvents(key); Assert.AreEqual(0,sb.Value);
    }

    [TestMethod]
    public void Value_Clamped_On_Set()
    {
        var sb = new ScrollBar();
        sb.Value = 500;
        Assert.AreEqual(100, sb.Value);
        sb.Value = -10;
        Assert.AreEqual(0, sb.Value);
    }
}
