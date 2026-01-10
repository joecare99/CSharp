using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using System.Drawing;
using NSubstitute;
using ConsoleLib.Interfaces;
using ConsoleLib.CommonControls.Tests;

namespace ConsoleLibTests.CommonControls;

[TestClass]
public class MenuItemAdvancedTests : TestBase
{
    [TestMethod]
    [DataRow("&File", 'F')]
    [DataRow("Edit", 'E')]
    [DataRow("&&Amp", '\0')]
    public void SetText_Sets_Accelerator(string text, char expected)
    {
        var mi = new MenuItem();
        mi.SetText(text);
        Assert.AreEqual(expected, mi.Accelerator);
    }

    [TestMethod]
    public void Separator_Draw_Fills_Line()
    {
        var mi = new MenuItem
        {
            IsSeparator = true,
            Parent = new Panel { Dimension = new Rectangle(0, 0, 20, 3) }
        };
        mi.SetText("---");
        mi.Draw();
        Assert.AreEqual(1, mi.size.Height);
    }

    [TestMethod]
    public void Click_Toggles_SubMenu()
    {
        var popup = new MenuPopup();
        var mi = new MenuItem{ SubMenu = popup };
        mi.Click();
        Assert.IsTrue(popup.Visible);
        mi.Click();
        Assert.IsFalse(popup.Visible);
    }

    [TestMethod]
    public void Click_Without_SubMenu_Dispatches_Hide()
    {
        // create fake app extended console using test console for both IConsole and IExtendedConsole not allowed -> use substitute
        var ext = Substitute.For<IExtendedConsole>();
        var app = new Application(_tstCon, ext);
        var bar = new MenuBar{ Parent = app };
        var mi = new MenuItem{ Parent=bar};
        bar.AddRootItem(mi);
        mi.Click();
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void HandlePressKeyEvents_Fires_Click_When_Matching()
    {
        var mi = new MenuItem();
        mi.SetText("Exit");
        bool clicked=false;
        mi.OnClick += (_,_)=>clicked=true;
        var ke = Substitute.For<IKeyEvent>();
        ke.KeyChar.Returns(mi.Accelerator);
        ke.bKeyDown.Returns(true);
        mi.HandlePressKeyEvents(ke);
        Assert.IsTrue(clicked);
        Assert.IsTrue(ke.Handled);
    }

    [TestMethod]
    public void Disabled_Ignores_Click_And_Key()
    {
        var mi = new MenuItem
        {
            Enabled = false
        };
        bool clicked=false;
        mi.OnClick += (_,_)=>clicked=true;
        mi.Click();
        var ke = Substitute.For<IKeyEvent>();
        ke.KeyChar.Returns('X'); ke.bKeyDown.Returns(true);
        mi.HandlePressKeyEvents(ke);
        Assert.IsFalse(clicked);
        Assert.IsFalse(ke.Handled);
    }
}