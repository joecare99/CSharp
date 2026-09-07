using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace ConsoleLib.Tests;

[TestClass]
public class DockPanelTests
{
    [TestMethod]
    public void DockPanel_DefaultsToFillingTheLastChild()
    {
        var panel = new DockPanel();

        Assert.IsTrue(panel.LastChildFill);
    }

    [TestMethod]
    public void DockPanel_DocksChildrenInAllDirectionsAndConsumesAvailableArea()
    {
        var panel = CreatePanel(20, 10);
        var left = CreateChild(2, 2);
        var top = CreateChild(3, 3);
        var right = CreateChild(4, 4);
        var bottom = CreateChild(5, 1);
        panel.LastChildFill = false;
        DockPanel.SetDock(left, Dock.Left);
        DockPanel.SetDock(top, Dock.Top);
        DockPanel.SetDock(right, Dock.Right);
        DockPanel.SetDock(bottom, Dock.Bottom);

        panel.Add(left);
        panel.Add(top);
        panel.Add(right);
        panel.Add(bottom);

        Assert.AreEqual(new Rectangle(0, 0, 2, 10), left.Dimension);
        Assert.AreEqual(new Rectangle(2, 0, 18, 3), top.Dimension);
        Assert.AreEqual(new Rectangle(16, 3, 4, 7), right.Dimension);
        Assert.AreEqual(new Rectangle(2, 9, 14, 1), bottom.Dimension);
    }

    [TestMethod]
    public void DockPanel_FillsLastChildWithTheRemainingAreaByDefault()
    {
        var panel = CreatePanel(20, 10);
        var left = CreateChild(4, 2);
        var top = CreateChild(3, 3);
        var fill = CreateChild(1, 1);
        DockPanel.SetDock(left, Dock.Left);
        DockPanel.SetDock(top, Dock.Top);
        DockPanel.SetDock(fill, Dock.Right);

        panel.Add(left);
        panel.Add(top);
        panel.Add(fill);

        Assert.AreEqual(new Rectangle(0, 0, 4, 10), left.Dimension);
        Assert.AreEqual(new Rectangle(4, 0, 16, 3), top.Dimension);
        Assert.AreEqual(new Rectangle(4, 3, 16, 7), fill.Dimension);
    }

    [TestMethod]
    public void DockPanel_DoesNotFillLastChildWhenLastChildFillIsDisabled()
    {
        var panel = CreatePanel(20, 10);
        var first = CreateChild(4, 2);
        var last = CreateChild(6, 3);
        panel.LastChildFill = false;
        DockPanel.SetDock(first, Dock.Left);
        DockPanel.SetDock(last, Dock.Top);

        panel.Add(first);
        panel.Add(last);

        Assert.AreEqual(new Rectangle(0, 0, 4, 10), first.Dimension);
        Assert.AreEqual(new Rectangle(4, 0, 16, 3), last.Dimension);
    }

    [TestMethod]
    public void DockPanel_SetDockBeforeAddIsAppliedWhenChildIsAdded()
    {
        var panel = CreatePanel(12, 6);
        var child = CreateChild(3, 2);
        DockPanel.SetDock(child, Dock.Right);

        Assert.AreEqual(Dock.Right, DockPanel.GetDock(child));

        panel.LastChildFill = false;
        panel.Add(child);

        Assert.AreEqual(Dock.Right, DockPanel.GetDock(child));
        Assert.AreEqual(new Rectangle(9, 0, 3, 6), child.Dimension);
    }

    [TestMethod]
    public void DockPanel_GetDockDefaultsToLeftForUnconfiguredChild()
    {
        var child = new Control();

        Assert.AreEqual(Dock.Left, DockPanel.GetDock(child));
    }

    [TestMethod]
    public void DockPanel_SetDockAfterAddRearrangesChildren()
    {
        var panel = CreatePanel(20, 10);
        var first = CreateChild(4, 2);
        var fill = CreateChild(2, 2);
        panel.Add(first);
        panel.Add(fill);

        DockPanel.SetDock(first, Dock.Right);

        Assert.AreEqual(new Rectangle(16, 0, 4, 10), first.Dimension);
        Assert.AreEqual(new Rectangle(0, 0, 16, 10), fill.Dimension);
    }

    [TestMethod]
    public void DockPanel_ResizingPanelRearrangesChildren()
    {
        var panel = CreatePanel(10, 6);
        var left = CreateChild(3, 2);
        var fill = CreateChild(1, 1);
        DockPanel.SetDock(left, Dock.Left);
        panel.Add(left);
        panel.Add(fill);

        panel.Dimension = new Rectangle(0, 0, 16, 9);

        Assert.AreEqual(new Rectangle(0, 0, 3, 9), left.Dimension);
        Assert.AreEqual(new Rectangle(3, 0, 13, 9), fill.Dimension);
    }

    [TestMethod]
    public void DockPanel_ResizingChildRearrangesFollowingChildren()
    {
        var panel = CreatePanel(20, 10);
        var left = CreateChild(3, 2);
        var top = CreateChild(2, 4);
        panel.LastChildFill = false;
        DockPanel.SetDock(left, Dock.Left);
        DockPanel.SetDock(top, Dock.Top);
        panel.Add(left);
        panel.Add(top);

        left.size = new Size(7, 2);

        Assert.AreEqual(new Rectangle(0, 0, 7, 10), left.Dimension);
        Assert.AreEqual(new Rectangle(7, 0, 13, 4), top.Dimension);
    }

    [TestMethod]
    public void DockPanel_RemovingChildRearrangesRemainingChildren()
    {
        var panel = CreatePanel(20, 10);
        var left = CreateChild(4, 2);
        var removed = CreateChild(3, 2);
        var fill = CreateChild(1, 1);
        DockPanel.SetDock(left, Dock.Left);
        DockPanel.SetDock(removed, Dock.Top);
        panel.Add(left);
        panel.Add(removed);
        panel.Add(fill);

        panel.Remove(removed);

        Assert.AreEqual(new Rectangle(0, 0, 4, 10), left.Dimension);
        Assert.AreEqual(new Rectangle(4, 0, 16, 10), fill.Dimension);
        Assert.IsNull(removed.Parent);
    }

    [TestMethod]
    public void DockPanel_RemovedChildNoLongerTriggersLayoutChanges()
    {
        var panel = CreatePanel(20, 10);
        var left = CreateChild(4, 2);
        var removed = CreateChild(3, 2);
        var fill = CreateChild(1, 1);
        DockPanel.SetDock(left, Dock.Left);
        DockPanel.SetDock(removed, Dock.Top);
        panel.Add(left);
        panel.Add(removed);
        panel.Add(fill);
        panel.Remove(removed);
        var layoutAfterRemove = fill.Dimension;

        removed.size = new Size(15, 15);

        Assert.AreEqual(layoutAfterRemove, fill.Dimension);
    }

    [TestMethod]
    public void DockPanel_BringToFrontMovesExistingChildToTheBeginning()
    {
        var panel = CreatePanel(20, 10);
        var first = CreateChild(2, 2);
        var second = CreateChild(3, 3);
        panel.Add(first);
        panel.Add(second);

        ((IGroupControl)panel).BringToFront(second);

        Assert.AreSame(second, panel.Children[0]);
        Assert.AreSame(first, panel.Children[1]);
    }

    [TestMethod]
    public void DockPanel_BringToFrontIgnoresChildThatIsNotInPanel()
    {
        var panel = CreatePanel(20, 10);
        var child = CreateChild(2, 2);
        var other = CreateChild(3, 3);
        panel.Add(child);

        ((IGroupControl)panel).BringToFront(other);

        Assert.AreEqual(1, panel.Children.Count);
        Assert.AreSame(child, panel.Children[0]);
    }

    private static DockPanel CreatePanel(int width, int height)
    {
        return new DockPanel { Dimension = new Rectangle(0, 0, width, height) };
    }

    private static Control CreateChild(int width, int height)
    {
        return new Control { size = new Size(width, height) };
    }
}
