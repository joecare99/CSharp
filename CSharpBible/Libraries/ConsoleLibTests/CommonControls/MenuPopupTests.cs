using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using System.Drawing;
using TestConsole;
using BaseLib.Interfaces;

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class MenuPopupTests : TestBase
{

    [TestMethod]
    public void AddItem_Sets_Parent_And_Layouts()
    {
        var popup = new MenuPopup();
        var mi = new MenuItem();
        popup.AddItem(mi);
        Assert.AreSame(popup, mi.Parent);
    }

    [TestMethod]
    public void Show_Sets_Visible_True()
    {
        var popup = new MenuPopup();
        popup.Show();
        Assert.IsTrue(popup.Visible);
    }

    [TestMethod]
    public void Hide_Sets_Visible_False()
    {
        var popup = new MenuPopup();
        popup.Show();
        popup.Hide();
        Assert.IsFalse(popup.Visible);
    }
}
