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
        Assert.AreNotEqual(first.GetCell(1, 0), second.GetCell(1, 0));
    }

    [TestMethod]
    public void SnapshotIsNotChangedByLaterRendering()
    {
        var button = new Button { Text = "Old" };
        var service = new AttachedRenderService();
        service.Attach(button, new Size(10, 3));
        var snapshot = service.GetSnapshot();

        button.Text = "New";

        Assert.AreEqual('O', snapshot.GetCell(1, 0).Character);
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

        Assert.AreEqual(new Rectangle(1, 0, 3, 1), delta.DirtyRegion);
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

    [TestMethod]
    public void ResizeSynchronizesRootAndPublishesNewViewport()
    {
        var panel = new Panel { size = new Size(4, 2) };
        var service = new AttachedRenderService();
        service.Attach(panel, new Size(4, 2));
        var revision = service.Revision;

        service.Resize(new Size(8, 5));

        Assert.AreEqual(new Size(8, 5), service.Size);
        Assert.AreEqual(new Size(8, 5), panel.size);
        Assert.IsTrue(service.Revision > revision);
        Assert.AreEqual(new Size(8, 5), service.GetSnapshot().Size);
    }

    [TestMethod]
    public void RefreshTreeSubscribesControlsAddedAfterAttach()
    {
        var panel = new Panel { size = new Size(8, 2) };
        var service = new AttachedRenderService();
        service.Attach(panel, new Size(8, 2));
        var label = new Label { Text = "Later", Position = new Point(0, 1), size = new Size(8, 1) };
        panel.Add(label);
        var beforeRefresh = service.Revision;

        service.RefreshTree();
        label.Text = "Updated";

        Assert.IsTrue(service.Revision > beforeRefresh);
        Assert.AreEqual('U', service.GetSnapshot().GetCell(0, 1).Character);
    }

    [TestMethod]
    public void DetachStopsUpdatesAndRejectsSnapshotRequests()
    {
        var label = new Label { Text = "Live" };
        var service = new AttachedRenderService();
        service.Attach(label, new Size(8, 1));
        var changes = 0;
        service.FrameChanged += (_, _) => changes++;
        service.Detach();
        label.Text = "Detached";

        Assert.IsFalse(service.IsAttached);
        Assert.ThrowsExactly<InvalidOperationException>(() => service.GetSnapshot());
        Assert.AreEqual(0, changes);
    }

    [TestMethod]
    public void TrackerReturnsNoDirtyRegionForIdenticalFrame()
    {
        var label = new Label { Text = "Same", size = new Size(8, 1) };
        var service = new AttachedRenderService();
        service.Attach(label, new Size(8, 1));
        var tracker = new FrameOutputTracker();
        tracker.Acquire(service.GetSnapshot());

        var delta = tracker.Acquire(service.GetSnapshot());

        Assert.IsFalse(delta.IsDirty);
        Assert.AreEqual(Rectangle.Empty, delta.DirtyRegion);
    }

    [TestMethod]
    public void TrackerMarksResizeAsFullFrame()
    {
        var label = new Label { Text = "Resize", size = new Size(4, 1) };
        var service = new AttachedRenderService();
        service.Attach(label, new Size(4, 1));
        var tracker = new FrameOutputTracker();
        tracker.Acquire(service.GetSnapshot());

        service.Resize(new Size(6, 2));
        var delta = tracker.Acquire(service.GetSnapshot());

        Assert.AreEqual(new Rectangle(0, 0, 6, 2), delta.DirtyRegion);
    }

    [TestMethod]
    public void TrackerRecoversWithFullFrameAfterOutputFailure()
    {
        var label = new Label { Text = "Recover", size = new Size(8, 1) };
        var service = new AttachedRenderService();
        service.Attach(label, new Size(8, 1));
        var tracker = new FrameOutputTracker();
        tracker.Acquire(service.GetSnapshot());

        tracker.ResetAfterOutputFailure();
        var delta = tracker.Acquire(service.GetSnapshot());

        Assert.IsTrue(delta.IsDirty);
        Assert.AreEqual(new Rectangle(0, 0, 8, 1), delta.DirtyRegion);
    }

    [TestMethod]
    public void RendererCentersButtonTextInsideItsAvailableWidth()
    {
        var button = new Button { Text = "Go", size = new Size(8, 1) };
        var service = new AttachedRenderService();
        service.Attach(button, new Size(8, 1));
        var frame = service.GetSnapshot();

        Assert.AreEqual(' ', frame.GetCell(0, 0).Character);
        Assert.AreEqual(' ', frame.GetCell(2, 0).Character);
        Assert.AreEqual('G', frame.GetCell(3, 0).Character);
        Assert.AreEqual('o', frame.GetCell(4, 0).Character);
    }

    [TestMethod]
    public void RendererDisplaysCheckedAndUncheckedCheckboxMarkers()
    {
        var checkBox = new CheckBox { Text = "Ready", size = new Size(9, 1) };
        var service = new AttachedRenderService();
        service.Attach(checkBox, new Size(9, 1));
        var uncheckedFrame = service.GetSnapshot();

        checkBox.IsChecked = true;
        service.Render();
        var checkedFrame = service.GetSnapshot();

        Assert.AreEqual('[', uncheckedFrame.GetCell(0, 0).Character);
        Assert.AreEqual(' ', uncheckedFrame.GetCell(1, 0).Character);
        Assert.AreEqual(']', uncheckedFrame.GetCell(2, 0).Character);
        Assert.AreEqual('x', checkedFrame.GetCell(1, 0).Character);
        Assert.AreEqual('R', checkedFrame.GetCell(4, 0).Character);
    }

    [TestMethod]
    public void RendererTruncatesCheckboxTextWithoutWritingOutsideFrame()
    {
        var checkBox = new CheckBox { Text = "Very long", size = new Size(5, 1) };
        var service = new AttachedRenderService();
        service.Attach(checkBox, new Size(5, 1));

        var row = service.GetSnapshot();

        Assert.AreEqual('[', row.GetCell(0, 0).Character);
        Assert.AreEqual('…', row.GetCell(4, 0).Character);
    }

    [TestMethod]
    public void RendererWrapsMultilineTextBoxAtWordBoundaries()
    {
        var textBox = new TextBox { Text = "one two three", MultiLine = true, size = new Size(7, 3) };
        var service = new AttachedRenderService();
        service.Attach(textBox, new Size(7, 3));
        var frame = service.GetSnapshot();

        Assert.AreEqual("one two", ReadRow(frame, 0, 7));
        Assert.AreEqual("three  ", ReadRow(frame, 1, 7));
    }

    [TestMethod]
    public void RendererSplitsOverlongWordsWithoutExceedingHeight()
    {
        var textBox = new TextBox { Text = "abcdefgh", MultiLine = true, size = new Size(3, 2) };
        var service = new AttachedRenderService();
        service.Attach(textBox, new Size(3, 2));
        var frame = service.GetSnapshot();

        Assert.AreEqual("abc", ReadRow(frame, 0, 3));
        Assert.AreEqual("def", ReadRow(frame, 1, 3));
    }

    private static string ReadRow(IRenderSnapshot snapshot, int y, int width)
    {
        var characters = new char[width];
        for (var x = 0; x < width; x++)
            characters[x] = snapshot.GetCell(x, y).Character;
        return new string(characters);
    }

    [TestMethod]
    public void RendererUsesDisabledButtonColors()
    {
        var button = new Button
        {
            Text = "Off",
            Enabled = false,
            DisabledFrontColor = ConsoleColor.DarkYellow,
            DisabledBackColor = ConsoleColor.DarkRed,
            size = new Size(5, 1)
        };
        var service = new AttachedRenderService();
        service.Attach(button, new Size(5, 1));
        var cell = service.GetSnapshot().GetCell(1, 0);

        Assert.AreEqual('O', cell.Character);
        Assert.AreEqual(ConsoleColor.DarkYellow, cell.Foreground);
        Assert.AreEqual(ConsoleColor.DarkRed, cell.Background);
    }

    [TestMethod]
    public void RendererCompositesFirstChildAsFrontMost()
    {
        var panel = new Panel { size = new Size(4, 1) };
        var back = new Label { Text = "Back", size = new Size(4, 1) };
        var front = new Label { Text = "Front", size = new Size(4, 1) };
        panel.Add(back);
        panel.Add(front);
        panel.BringToFront(back);

        var service = new AttachedRenderService();
        service.Attach(panel, new Size(4, 1));

        Assert.AreEqual('B', service.GetSnapshot().GetCell(0, 0).Character);
    }
}
