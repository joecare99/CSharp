using ConsoleLib.CommonControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Tests;

[TestClass]
public class TabControlTests
{
    [TestMethod]
    public void TabControl_SelectsAdjacentTabs()
    {
        var tabs = new TabControl();
        tabs.Items.Add(new TabItem("First"));
        tabs.Items.Add(new TabItem("Second"));

        Assert.IsTrue(tabs.SelectNext());
        Assert.AreEqual("First", tabs.SelectedItem!.Header);
        Assert.IsTrue(tabs.SelectNext());
        Assert.AreEqual("Second", tabs.SelectedItem!.Header);
    }
}
