using System;
using System.Drawing;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using ConsoleLib.Rendering;
using ConsoleLib.Data;
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

    [TestMethod]
    public void RendererUsesCanonicalSingleBorderAndEllipsis()
    {
        var panel = new Panel
        {
            Text = "Long text",
            BorderStyle = BorderStyle.Single,
            size = new Size(6, 3)
        };
        var service = new AttachedRenderService();
        service.Attach(panel, new Size(6, 3));
        var frame = service.GetSnapshot();

        Assert.AreEqual('┌', frame.GetCell(0, 0).Character);
        Assert.AreEqual('┐', frame.GetCell(5, 0).Character);
        Assert.AreEqual('│', frame.GetCell(0, 1).Character);
        Assert.AreEqual('L', frame.GetCell(1, 1).Character);
        Assert.AreEqual('…', frame.GetCell(4, 1).Character);
        Assert.AreEqual('┘', frame.GetCell(5, 2).Character);
    }

    [TestMethod]
    public void RendererClipsControlsOutsideFrameBounds()
    {
        var label = new Label { Text = "Visible", Position = new Point(-2, 1), size = new Size(10, 1) };
        var service = new AttachedRenderService();
        service.Attach(label, new Size(4, 3));
        var frame = service.GetSnapshot();

        Assert.AreEqual('s', frame.GetCell(0, 1).Character);
        Assert.AreEqual('i', frame.GetCell(1, 1).Character);
        Assert.AreEqual('b', frame.GetCell(2, 1).Character);
        Assert.AreEqual('l', frame.GetCell(3, 1).Character);
    }

    [TestMethod]
    public void RendererPreservesGridChildPlacement()
    {
        var grid = new Grid { size = new Size(8, 4) };
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4) });
        var label = new Label { Text = "X" };
        Grid.SetRow(label, 1);
        Grid.SetColumn(label, 1);
        grid.Add(label);

        var service = new AttachedRenderService();
        service.Attach(grid, new Size(8, 4));

        Assert.AreEqual('X', service.GetSnapshot().GetCell(4, 2).Character);
    }
}
