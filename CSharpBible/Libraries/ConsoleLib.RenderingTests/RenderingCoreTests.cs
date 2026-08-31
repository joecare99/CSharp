using System;
using System.Drawing;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using ConsoleLib.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.RenderingTests;

[TestClass]
public sealed class RenderingCoreTests
{
    [TestMethod]
    public void AttachedServicePublishesCurrentImmutableSnapshot()
    {
        var button = new Button { Text = "Run" };
        var service = new AttachedRenderService();
        var changes = 0;
        service.FrameChanged += (_, _) => changes++;

        service.Attach(button, new Size(10, 3));
        var first = service.GetSnapshot();
        button.Text = "Go";
        var second = service.GetSnapshot();

        Assert.AreEqual(1L, first.Revision);
        Assert.IsTrue(second.Revision > first.Revision);
        Assert.IsTrue(changes > 1);
        Assert.AreNotEqual(first.GetCell(0, 0), second.GetCell(0, 0));
    }

    [TestMethod]
    public void SnapshotIsNotChangedByLaterRendering()
    {
        var button = new Button { Text = "Old" };
        var service = new AttachedRenderService();
        service.Attach(button, new Size(10, 3));
        var snapshot = service.GetSnapshot();

        button.Text = "New";

        Assert.AreEqual('O', snapshot.GetCell(0, 0).Character);
    }

    [TestMethod]
    public void TrackerMarksFirstFrameAndResetAsFull()
    {
        var button = new Button { Text = "Run" };
        var service = new AttachedRenderService();
        service.Attach(button, new Size(4, 2));
        var tracker = new FrameOutputTracker();

        var first = tracker.Acquire(service.GetSnapshot());
        tracker.Reset();
        var afterReset = tracker.Acquire(service.GetSnapshot());

        Assert.AreEqual(new Rectangle(0, 0, 4, 2), first.DirtyRegion);
        Assert.AreEqual(new Rectangle(0, 0, 4, 2), afterReset.DirtyRegion);
    }

    [TestMethod]
    public void TrackerReportsSmallestChangedRegion()
    {
        var button = new Button { Text = "Run" };
        var service = new AttachedRenderService();
        service.Attach(button, new Size(10, 3));
        var tracker = new FrameOutputTracker();
        tracker.Acquire(service.GetSnapshot());

        button.Text = "Go";
        var delta = tracker.Acquire(service.GetSnapshot());

        Assert.AreEqual(new Rectangle(0, 0, 3, 1), delta.DirtyRegion);
    }
}
