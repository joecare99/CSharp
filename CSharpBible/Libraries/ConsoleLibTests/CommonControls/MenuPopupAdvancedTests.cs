using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using System.Drawing;
using ConsoleLibTests.CommonControls;

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class MenuPopupAdvancedTests : TestBase
{
    [DataTestMethod]
    [DataRow(true)]  // popup visible before adding second item
    [DataRow(false)] // popup hidden before adding second item
    public void LayoutItems_Recalculates_Size_For_Visibility(bool makeVisibleFirst)
    {
        var popup = new MenuPopup();
        var i1 = new MenuItem(); i1.SetText("One"); popup.AddItem(i1);
        if (makeVisibleFirst) popup.Show(); else popup.Hide();

        var i2 = new MenuItem(); i2.SetText("Second"); popup.AddItem(i2); // triggers LayoutItems via AddItem
        popup.LayoutItems(); // explicit re-layout

        // Assertions common
        Assert.AreEqual(new Point(1,1), i1.Position);
        Assert.AreEqual(new Point(1,2), i2.Position);
        Assert.IsTrue(popup.size.Width >= i2.size.Width + 2);
        if (makeVisibleFirst)
        {
            Assert.IsTrue(popup.Visible); // remained visible and invalidated
        }
        else
        {
            Assert.IsFalse(popup.Visible); // stayed hidden
            popup.Show();
            Assert.IsTrue(popup.Visible);
        }
    }
}