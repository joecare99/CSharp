using ConsoleLib.Showcase.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Showcase.Tests;

[TestClass]
public sealed class DesktopWindowManagerTests
{
    [TestMethod]
    public void OpenCreatesWindowAndAssignsActiveZOrder()
    {
        var manager = new DesktopWindowManager();

        Assert.IsTrue(manager.Open("calendar"));

        var window = manager.Find("calendar");
        Assert.IsNotNull(window);
        Assert.IsTrue(window.IsOpen);
        Assert.AreSame(window, manager.Active);
        Assert.IsTrue(window.ZIndex > 0);
    }

    [TestMethod]
    public void ToggleMinimizesAndRestoresSameWindow()
    {
        var manager = new DesktopWindowManager();
        manager.Open("calendar");

        manager.Toggle("calendar");
        Assert.IsTrue(manager.Find("calendar")!.IsMinimized);
        Assert.IsNull(manager.Active);

        manager.Toggle("calendar");
        Assert.IsFalse(manager.Find("calendar")!.IsMinimized);
        Assert.AreEqual("calendar", manager.Active!.Id);
    }

    [TestMethod]
    public void CloseThenOpen_ReopensExistingWindowInstance()
    {
        var manager = new DesktopWindowManager();
        manager.Open("calendar");
        var first = manager.Find("calendar");

        Assert.IsTrue(manager.Close("calendar"));
        Assert.IsFalse(first!.IsOpen);

        Assert.IsTrue(manager.Open("calendar"));
        Assert.AreSame(first, manager.Find("calendar"));
        Assert.IsTrue(first.IsOpen);
    }

    [TestMethod]
    public void BringToFront_ChangesActiveWindow()
    {
        var manager = new DesktopWindowManager();
        manager.Open("calendar");
        manager.Open("calculator");

        manager.BringToFront("calendar");

        Assert.AreEqual("calendar", manager.Active!.Id);
        Assert.IsTrue(manager.Find("calendar")!.ZIndex > manager.Find("calculator")!.ZIndex);
    }
}
