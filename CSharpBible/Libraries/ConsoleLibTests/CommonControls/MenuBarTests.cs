using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using System.Drawing;
using NSubstitute;
using ConsoleLib.Interfaces;
using ConsoleLibTests.CommonControls;

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class MenuBarTests: TestBase
{
    [TestMethod]
    public void Ctor_Sets_Defaults()
    {
        var mb = new MenuBar();
        Assert.AreEqual(ConsoleColor.DarkGray, mb.BackColor);
        Assert.AreEqual(ConsoleColor.Black, mb.ForeColor);
        Assert.AreEqual(ConsoleColor.DarkGray, mb.BoarderColor);
        Assert.AreEqual(new Size(_tstCon.WindowWidth,1), mb.size);
        Assert.AreEqual(new Point(0,0), mb.Position);
    }

    [TestMethod]
    public void AddRootItem_Adds_Item_And_Layouts()
    {
        var mb = new MenuBar();
        var item = new MenuItem();
        var popup = new MenuPopup();
        mb.AddRootItem(item, popup);
        Assert.AreSame(mb, item.Parent);
        Assert.AreSame(popup, item.SubMenu);
        Assert.AreSame(mb.Parent, popup.Parent); // popup parent is menubar parent (likely null in test)
        Assert.IsFalse(popup.Visible);
    }

    [TestMethod]
    public void LayoutItems_Positions_Items_And_SubMenus()
    {
        var mb = new MenuBar();
        var i1 = new MenuItem(); i1.SetText("File");
        var i2 = new MenuItem(); i2.SetText("Edit");
        var p2 = new MenuPopup();
        mb.AddRootItem(i1);
        mb.AddRootItem(i2, p2);

        Assert.AreEqual(new Point(0,0), i1.Position);
        Assert.AreEqual(new Point(i1.size.Width,0), i2.Position);
        Assert.AreEqual(new Point(i2.Position.X + 1,2), p2.Position);
    }

    [TestMethod]
    public void ShowSubMenuFor_Shows_Only_Target_SubMenu_When_Others_Open()
    {
        var mb = new MenuBar();
        var i1 = new MenuItem(); i1.SetText("File");
        var p1 = new MenuPopup();
        var i2 = new MenuItem(); i2.SetText("Edit");
        var p2 = new MenuPopup();
        mb.AddRootItem(i1, p1);
        mb.AddRootItem(i2, p2);
        p1.Show();
        p2.Hide();

        mb.ShowSubMenuFor(i2);

        Assert.IsFalse(p1.Visible);
        Assert.IsTrue(p2.Visible);
        Assert.AreEqual(new Point(i2.Position.X + 1,2), p2.Position);
    }

    [TestMethod]
    public void ShowSubMenuFor_Does_Not_Show_When_No_Others_Open()
    {
        var mb = new MenuBar();
        var i1 = new MenuItem(); i1.SetText("File");
        var p1 = new MenuPopup();
        mb.AddRootItem(i1, p1);
        p1.Hide();
        mb.ShowSubMenuFor(i1);
        Assert.IsFalse(p1.Visible); // remains hidden because _flag false
    }

    [TestMethod]
    public void HideAllPopups_Hides_All()
    {
        var mb = new MenuBar();
        var i1 = new MenuItem(); var p1 = new MenuPopup(); i1.SetText("File");
        var i2 = new MenuItem(); var p2 = new MenuPopup(); i2.SetText("Edit");
        mb.AddRootItem(i1, p1);
        mb.AddRootItem(i2, p2);
        p1.Show(); p2.Show();
        mb.HideAllPopups();
        Assert.IsFalse(p1.Visible);
        Assert.IsFalse(p2.Visible);
    }

    [TestMethod]
    public void MouseClick_Outside_Hides_Popups()
    {
        var mb = new MenuBar();
        var i1 = new MenuItem(); var p1 = new MenuPopup(); i1.SetText("File");
        mb.AddRootItem(i1, p1);
        p1.Show();
        var me = Substitute.For<IMouseEvent>();
        me.MousePos.Returns(new Point(100,10)); // outside
        me.MouseButtonLeft.Returns(true);
        mb.MouseClick(me);
        Assert.IsFalse(p1.Visible);
    }
}
