using System.Drawing;
using ConsoleLib.CommonControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Tests;

[TestClass]
public class NavigationWidgetTests
{
    [TestMethod]
    public void ScrollViewer_ClampsContentOffset()
    {
        var viewer = new ScrollViewer { size = new Size(10, 5) };
        viewer.SetContent(new Panel { size = new Size(30, 20) });

        viewer.ScrollBy(100, 100);

        Assert.AreEqual(new Point(20, 15), viewer.Offset);
    }

    [TestMethod]
    public void StatusBar_UsesTextAsStatus()
    {
        var status = new StatusBar { Status = "Ready" };

        Assert.AreEqual("Ready", status.Text);
    }
}
