using System;
using System.Drawing;
using BaseLib.Interfaces;
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
    public void RendererDisplaysSelectedComboBoxItemWithCanonicalBrackets()
    {
        var comboBox = new ComboBox { size = new Size(12, 1) };
        comboBox.Items.Add("First");
        comboBox.Items.Add("Second");
        Assert.IsTrue(comboBox.SelectNext());
        Assert.IsTrue(comboBox.SelectNext());

        var service = new AttachedRenderService();
        service.Attach(comboBox, new Size(12, 1));
        var frame = service.GetSnapshot();

        Assert.AreEqual("[Second]    ", ReadRow(frame, 0, 12));
    }

    [TestMethod]
    public void RendererDisplaysListBoxItemsAndSelectedColors()
    {
        var listBox = new ListBox
        {
            ItemsSource = new[] { "One", "Two", "Three" },
            SelectedIndex = 1,
            size = new Size(7, 3)
        };
        listBox.BorderDefinition = new BorderDef { Style = BorderStyle.None };
        var service = new AttachedRenderService();
        service.Attach(listBox, new Size(7, 3));
        var frame = service.GetSnapshot();

        Assert.AreEqual("One    ", ReadRow(frame, 0, 7));
        Assert.AreEqual("Two    ", ReadRow(frame, 1, 7));
        Assert.AreEqual(listBox.SelectedForeColor, frame.GetCell(0, 1).Foreground);
        Assert.AreEqual(listBox.SelectedBackColor, frame.GetCell(0, 1).Background);
        Assert.AreEqual("Three  ", ReadRow(frame, 2, 7));
    }

    [TestMethod]
    public void RendererDisplaysTabsWithSelectedHeaderMarkers()
    {
        var tabs = new TabControl { size = new Size(16, 1) };
        var first = new TabItem("One");
        var second = new TabItem("Two");
        tabs.Items.Add(first);
        tabs.Items.Add(second);
        Assert.IsTrue(tabs.SelectNext());
        Assert.IsTrue(tabs.SelectNext());

        var service = new AttachedRenderService();
        service.Attach(tabs, new Size(16, 1));

        Assert.AreEqual(" One [Two]      ", ReadRow(service.GetSnapshot(), 0, 16));
    }

    [TestMethod]
    public void RendererDisplaysTileViewInGridWithSelectedColors()
    {
        var tiles = new TileView
        {
            size = new Size(8, 2),
            TileWidth = 4,
            TileHeight = 1
        };
        tiles.SetItems(new[] { new TileItem("One"), new TileItem("Two") });
        Assert.IsTrue(tiles.SelectNext());

        var service = new AttachedRenderService();
        service.Attach(tiles, new Size(8, 2));
        var frame = service.GetSnapshot();

        Assert.AreEqual("One Two ", ReadRow(frame, 0, 8));
        Assert.AreEqual(ConsoleColor.Yellow, frame.GetCell(4, 0).Foreground);
        Assert.AreEqual(' ', frame.GetCell(0, 1).Character);
    }

    [TestMethod]
    public void RendererClipsTileTextAndRowsToViewport()
    {
        var tiles = new TileView
        {
            Position = new Point(-1, 1),
            size = new Size(8, 3),
            TileWidth = 4,
            TileHeight = 2
        };
        tiles.SetItems(new[] { new TileItem("Long"), new TileItem("Next"), new TileItem("Third") });

        var service = new AttachedRenderService();
        service.Attach(tiles, new Size(4, 3));
        var frame = service.GetSnapshot();

        Assert.AreEqual("ongN", ReadRow(frame, 1, 4));
        Assert.AreEqual("    ", ReadRow(frame, 2, 4));
    }

    [TestMethod]
    public void RendererDisplaysExpandedTreeHierarchyAndSelectedNode()
    {
        var tree = new TreeView { size = new Size(16, 3) };
        var root = new TreeNode("Root") { IsExpanded = true };
        root.Add(new TreeNode("Child"));
        tree.Nodes.Add(root);
        Assert.IsTrue(tree.SelectNext());
        Assert.IsTrue(tree.SelectNext());

        var service = new AttachedRenderService();
        service.Attach(tree, new Size(16, 3));
        var frame = service.GetSnapshot();

        Assert.AreEqual("- Root          ", ReadRow(frame, 0, 16));
        Assert.AreEqual("    Child       ", ReadRow(frame, 1, 16));
        Assert.AreEqual(ConsoleColor.Yellow, frame.GetCell(0, 1).Foreground);
    }

    [TestMethod]
    public void RendererOmitsCollapsedTreeChildrenAndUsesExpandMarker()
    {
        var tree = new TreeView { size = new Size(12, 3) };
        var root = new TreeNode("Root");
        root.Add(new TreeNode("Hidden"));
        tree.Nodes.Add(root);

        var service = new AttachedRenderService();
        service.Attach(tree, new Size(12, 3));
        var frame = service.GetSnapshot();

        Assert.AreEqual("+ Root      ", ReadRow(frame, 0, 12));
        Assert.AreEqual("            ", ReadRow(frame, 1, 12));
    }

    [TestMethod]
    public void RendererClipsIndentedTreeTextToControlAndViewport()
    {
        var tree = new TreeView
        {
            Position = new Point(-2, 1),
            size = new Size(20, 2)
        };
        tree.Nodes.Add(new TreeNode("LongNode"));

        var service = new AttachedRenderService();
        service.Attach(tree, new Size(4, 3));
        var frame = service.GetSnapshot();

        Assert.AreEqual("Long", ReadRow(frame, 1, 4));
        Assert.AreEqual("    ", ReadRow(frame, 2, 4));
    }

    [TestMethod]
    public void RendererDisplaysVerticalScrollBarTrackThumbAndArrows()
    {
        var scrollBar = new ScrollBar
        {
            Vertical = true,
            Minimum = 0,
            Maximum = 100,
            LargeChange = 10,
            Value = 50,
            size = new Size(1, 10)
        };
        var service = new AttachedRenderService();
        service.Attach(scrollBar, new Size(1, 10));
        var frame = service.GetSnapshot();

        Assert.AreEqual('▲', frame.GetCell(0, 0).Character);
        Assert.AreEqual('│', frame.GetCell(0, 1).Character);
        Assert.AreEqual('█', frame.GetCell(0, 5).Character);
        Assert.AreEqual(scrollBar.ThumbColor, frame.GetCell(0, 5).Foreground);
        Assert.AreEqual('▼', frame.GetCell(0, 9).Character);
    }

    [TestMethod]
    public void RendererDisplaysHorizontalScrollBarAndClipsToViewport()
    {
        var scrollBar = new ScrollBar
        {
            Vertical = false,
            Minimum = 0,
            Maximum = 0,
            size = new Size(8, 1),
            Position = new Point(-2, 1)
        };
        var service = new AttachedRenderService();
        service.Attach(scrollBar, new Size(4, 3));
        var frame = service.GetSnapshot();

        Assert.AreEqual('█', frame.GetCell(0, 1).Character);
        Assert.AreEqual('█', frame.GetCell(1, 1).Character);
        Assert.AreEqual('█', frame.GetCell(2, 1).Character);
        Assert.AreEqual('█', frame.GetCell(3, 1).Character);
    }

    [TestMethod]
    public void RendererUsesDisabledScrollBarColorsForEveryPart()
    {
        var scrollBar = new ScrollBar
        {
            Enabled = false,
            Vertical = true,
            LargeChange = 100,
            DisabledColor = ConsoleColor.DarkRed,
            DisabledBackColor = ConsoleColor.Blue,
            DisabledThumbBackColor = ConsoleColor.Green,
            size = new Size(1, 5)
        };
        var service = new AttachedRenderService();
        service.Attach(scrollBar, new Size(1, 5));
        var frame = service.GetSnapshot();

        Assert.AreEqual(ConsoleColor.DarkRed, frame.GetCell(0, 0).Foreground);
        Assert.AreEqual(ConsoleColor.Blue, frame.GetCell(0, 0).Background);
        Assert.AreEqual(ConsoleColor.DarkRed, frame.GetCell(0, 2).Foreground);
        Assert.AreEqual(ConsoleColor.Green, frame.GetCell(0, 2).Background);
    }

    [TestMethod]
    public void RendererClipsListBoxItemsToViewportAndControlWidth()
    {
        var listBox = new ListBox
        {
            ItemsSource = new[] { "VeryLongItem", "Second" },
            SelectedIndex = 0,
            Position = new Point(-2, 1),
            size = new Size(8, 2)
        };
        listBox.BorderDefinition = new BorderDef { Style = BorderStyle.None };
        var service = new AttachedRenderService();
        service.Attach(listBox, new Size(4, 3));
        var frame = service.GetSnapshot();

        Assert.AreEqual("ryLo", ReadRow(frame, 1, 4));
        Assert.AreEqual("cond", ReadRow(frame, 2, 4));
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

    [TestMethod]
    public void RendererUsesTextBoxViewportLines()
    {
        var textBox = new TextBox { size = new Size(6, 2), Text = "top\nmiddle\nbottom", MultiLine = true };
        textBox.Caret = (0, 2);
        var service = new AttachedRenderService();
        service.Attach(textBox, new Size(6, 2));

        var frame = service.GetSnapshot();

        Assert.AreEqual("middle", ReadRow(frame, 0, 6));
        Assert.AreEqual("bottom", ReadRow(frame, 1, 6));
    }

    [TestMethod]
    public void RendererUsesDisabledTextBoxColorAndBackground()
    {
        var textBox = new TextBox { Text = "Input", Enabled = false, size = new Size(5, 1) };
        var service = new AttachedRenderService();
        service.Attach(textBox, new Size(5, 1));

        var cell = service.GetSnapshot().GetCell(0, 0);

        Assert.AreEqual('I', cell.Character);
        Assert.AreEqual(textBox.DisabledForeColor, cell.Foreground);
        Assert.AreEqual(textBox.BackColor, cell.Background);
    }

    [TestMethod]
    public void RendererDrawsActiveTextBoxCaretWithCaretColors()
    {
        var textBox = new TextBox { Text = "abc", MultiLine = false, Active = true, size = new Size(5, 1) };
        textBox.Caret = (1, 0);
        var service = new AttachedRenderService();
        service.Attach(textBox, new Size(5, 1));

        var cell = service.GetSnapshot().GetCell(1, 0);

        Assert.AreEqual('b', cell.Character);
        Assert.AreEqual(textBox.BackColor, cell.Foreground);
        Assert.AreEqual(textBox.CaretColor, cell.Background);
    }

    [TestMethod]
    public void RendererClipsTextBoxViewportAtFrameBounds()
    {
        var textBox = new TextBox { Text = "abcdef", MultiLine = false, Position = new Point(-2, 0), size = new Size(6, 1) };
        var service = new AttachedRenderService();
        service.Attach(textBox, new Size(3, 1));

        Assert.AreEqual("cde", ReadRow(service.GetSnapshot(), 0, 3));
    }

    [TestMethod]
    public void RendererDisplaysTerminalCellsInsideBorder()
    {
        var terminal = new Terminal { size = new Size(7, 4) };
        terminal.RenderRows(new[] { "ABC", "123" });
        var service = new AttachedRenderService();
        service.Attach(terminal, new Size(7, 4));

        var frame = service.GetSnapshot();

        Assert.AreEqual("┌─────┐", ReadRow(frame, 0, 7));
        Assert.AreEqual("│ABC  │", ReadRow(frame, 1, 7));
        Assert.AreEqual("│123  │", ReadRow(frame, 2, 7));
        Assert.AreEqual("└─────┘", ReadRow(frame, 3, 7));
    }

    [TestMethod]
    public void RendererPreservesTerminalCellColorsAndClearedSpaces()
    {
        var terminal = new Terminal { size = new Size(5, 4) };
        ((IConsole)terminal).ForegroundColor = ConsoleColor.Green;
        ((IConsole)terminal).BackgroundColor = ConsoleColor.DarkBlue;
        terminal.RenderRows(new[] { "X" });
        var cells = new TerminalCell[5, 3];
        new ControlFrameRenderer().Render(terminal, cells, new Size(5, 3));

        var content = cells[1, 1];
        var blank = cells[2, 1];

        Assert.AreEqual('X', content.Character);
        Assert.AreEqual(ConsoleColor.Green, content.Foreground);
        Assert.AreEqual(ConsoleColor.DarkBlue, content.Background);
        Assert.AreEqual(' ', blank.Character);
        Assert.AreEqual(ConsoleColor.Green, blank.Foreground);
        Assert.AreEqual(ConsoleColor.DarkBlue, blank.Background);
    }

    [TestMethod]
    public void RendererClipsTerminalAtFrameBounds()
    {
        var terminal = new Terminal { Position = new Point(-1, 0), size = new Size(6, 4) };
        terminal.BorderStyle = BorderStyle.None;
        terminal.RenderRows(new[] { "ABCDE", "FGHIJ" });
        var cells = new TerminalCell[4, 3];
        new ControlFrameRenderer().Render(terminal, cells, new Size(4, 3));

        Assert.AreEqual("BCD ", new string(new[] { cells[0, 0].Character, cells[1, 0].Character, cells[2, 0].Character, cells[3, 0].Character }));
        Assert.AreEqual("GHI ", new string(new[] { cells[0, 1].Character, cells[1, 1].Character, cells[2, 1].Character, cells[3, 1].Character }));
    }

    private static string ReadRow(IRenderSnapshot snapshot, int y, int width)
    {
        return ReadRowAt(snapshot, 0, y, width);
    }

    private static string ReadRowAt(IRenderSnapshot snapshot, int x, int y, int width)
    {
        var characters = new char[width];
        for (var index = 0; index < width; index++)
            characters[index] = snapshot.GetCell(x + index, y).Character;
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

    [TestMethod]
    public void RendererPlacesShadowBehindControlContent()
    {
        var label = new Label { Text = "A", Shadow = true, size = new Size(2, 1) };
        var service = new AttachedRenderService();
        service.Attach(label, new Size(4, 3));
        var frame = service.GetSnapshot();

        Assert.AreEqual('A', frame.GetCell(0, 0).Character);
        Assert.AreEqual('░', frame.GetCell(1, 1).Character);
        Assert.AreEqual(ConsoleColor.Gray, frame.GetCell(1, 1).Foreground);
        Assert.AreEqual(ConsoleColor.Black, frame.GetCell(1, 1).Background);
    }

    [TestMethod]
    public void RendererClipsShadowAtViewportBoundary()
    {
        var label = new Label { Text = "A", Shadow = true, Position = new Point(2, 1), size = new Size(2, 2) };
        var service = new AttachedRenderService();
        service.Attach(label, new Size(4, 3));
        var frame = service.GetSnapshot();

        Assert.AreEqual('░', frame.GetCell(3, 2).Character);
        Assert.AreEqual(' ', frame.GetCell(0, 0).Character);
    }

    [TestMethod]
    public void RendererFillsProgressBarAccordingToFraction()
    {
        var progress = new ProgressBar
        {
            Minimum = 0,
            Maximum = 100,
            Value = 50,
            size = new Size(8, 1)
        };
        var service = new AttachedRenderService();
        service.Attach(progress, new Size(8, 1));

        Assert.AreEqual("####----", ReadRow(service.GetSnapshot(), 0, 8));
    }

    [TestMethod]
    public void RendererTruncatesFractionAndRendersProgressEndpoints()
    {
        var progress = new ProgressBar { Maximum = 3, Value = 1, size = new Size(7, 1) };
        var service = new AttachedRenderService();
        service.Attach(progress, new Size(7, 1));

        Assert.AreEqual("##-----", ReadRow(service.GetSnapshot(), 0, 7));

        progress.Value = 3;
        service.Render();
        Assert.AreEqual("#######", ReadRow(service.GetSnapshot(), 0, 7));

        progress.Value = 0;
        service.Render();
        Assert.AreEqual("-------", ReadRow(service.GetSnapshot(), 0, 7));
    }

    [TestMethod]
    public void RendererHandlesDegenerateProgressRange()
    {
        var progress = new ProgressBar
        {
            Minimum = 5,
            Maximum = 5,
            Value = 5,
            size = new Size(6, 1)
        };
        var service = new AttachedRenderService();
        service.Attach(progress, new Size(6, 1));
        var frame = service.GetSnapshot();

        Assert.AreEqual("------", ReadRow(frame, 0, 6));
    }

    [TestMethod]
    public void RendererClipsProgressBarAndUsesDisabledColors()
    {
        var progress = new ProgressBar
        {
            Value = 50,
            Enabled = false,
            Position = new Point(-2, 0),
            size = new Size(8, 1)
        };
        var service = new AttachedRenderService();
        service.Attach(progress, new Size(4, 1));
        var frame = service.GetSnapshot();

        Assert.AreEqual("##--", ReadRow(frame, 0, 4));
        Assert.AreEqual(ConsoleColor.DarkGray, frame.GetCell(0, 0).Foreground);
        Assert.AreEqual(ConsoleColor.Black, frame.GetCell(0, 0).Background);
    }

    [TestMethod]
    public void RendererUsesStatusTextAndStatusColor()
    {
        var status = new StatusBar
        {
            Status = "Ready",
            StatusColor = ConsoleColor.Green,
            Position = new Point(0, 1),
            size = new Size(8, 1)
        };
        var service = new AttachedRenderService();
        service.Attach(status, new Size(8, 3));
        var frame = service.GetSnapshot();

        Assert.AreEqual("Ready   ", ReadRow(frame, 1, 8));
        Assert.AreEqual(ConsoleColor.Green, frame.GetCell(0, 1).Foreground);
        Assert.AreEqual(ConsoleColor.Black, frame.GetCell(7, 1).Background);
    }

    [TestMethod]
    public void RendererUsesDisabledStatusColorsAndClipsStatusText()
    {
        var status = new StatusBar
        {
            Status = "Loading data",
            StatusColor = ConsoleColor.Green,
            Enabled = false,
            size = new Size(5, 1)
        };
        var service = new AttachedRenderService();
        service.Attach(status, new Size(5, 1));
        var cell = service.GetSnapshot().GetCell(0, 0);

        Assert.AreEqual("Loadi", ReadRow(service.GetSnapshot(), 0, 5));
        Assert.AreEqual(ConsoleColor.DarkGray, cell.Foreground);
    }

    [TestMethod]
    public void RendererRejectsNullRoot()
    {
        var renderer = new ControlFrameRenderer();
        var exceptionThrown = false;

        try
        {
            renderer.Render(null!, new TerminalCell[1, 1], new Size(1, 1));
        }
        catch (ArgumentNullException)
        {
            exceptionThrown = true;
        }

        Assert.IsTrue(exceptionThrown);
    }

    [TestMethod]
    public void RendererSkipsInvisibleControls()
    {
        var label = new Label { Text = "Hidden", Visible = false, size = new Size(6, 1) };
        var service = new AttachedRenderService();
        service.Attach(label, new Size(6, 1));

        Assert.AreEqual("      ", ReadRow(service.GetSnapshot(), 0, 6));
    }

    [TestMethod]
    public void RendererUsesDoubleBorderGlyphs()
    {
        var panel = new Panel { BorderStyle = BorderStyle.Double, size = new Size(3, 2) };
        var service = new AttachedRenderService();
        service.Attach(panel, new Size(3, 2));
        var frame = service.GetSnapshot();

        Assert.AreEqual('╔', frame.GetCell(0, 0).Character);
        Assert.AreEqual('╝', frame.GetCell(2, 1).Character);
    }

    [TestMethod]
    public void RendererClipsShadowWhenShadowStartsOutsideViewport()
    {
        var label = new Label { Text = "A", Shadow = true, Position = new Point(-1, -1), size = new Size(2, 2) };
        var service = new AttachedRenderService();
        service.Attach(label, new Size(2, 2));

        Assert.AreEqual('░', service.GetSnapshot().GetCell(0, 0).Character);
    }

    [TestMethod]
    public void RendererStopsTreeRowsAtControlHeight()
    {
        var tree = new TreeView { size = new Size(8, 1) };
        tree.Nodes.Add(new TreeNode("First"));
        tree.Nodes.Add(new TreeNode("Second"));
        var service = new AttachedRenderService();
        service.Attach(tree, new Size(8, 1));

        Assert.AreEqual("  First ", ReadRow(service.GetSnapshot(), 0, 8));
    }

    [TestMethod]
    public void RendererPreservesEmptyMultilineRows()
    {
        var textBox = new TextBox { Text = "top\n\nbottom", MultiLine = true, size = new Size(6, 3) };
        var service = new AttachedRenderService();
        service.Attach(textBox, new Size(6, 3));

        Assert.AreEqual("top   ", ReadRow(service.GetSnapshot(), 0, 6));
        Assert.AreEqual("      ", ReadRow(service.GetSnapshot(), 1, 6));
        Assert.AreEqual("bottom", ReadRow(service.GetSnapshot(), 2, 6));
    }

    [TestMethod]
    public void RendererBreaksMultilineWordsAtAvailableSpaces()
    {
        var textBox = new TextBox { Text = "one two", MultiLine = true, size = new Size(5, 2) };
        var service = new AttachedRenderService();
        service.Attach(textBox, new Size(5, 2));

        Assert.AreEqual("one  ", ReadRow(service.GetSnapshot(), 0, 5));
        Assert.AreEqual("two  ", ReadRow(service.GetSnapshot(), 1, 5));
    }

    [TestMethod]
    public void RendererLayoutsMenuBarItemsAndHighlightsAccelerator()
    {
        var menuBar = new MenuBar { size = new Size(14, 1) };
        var file = new MenuItem { Text = "&File", HotColor = ConsoleColor.Yellow };
        var edit = new MenuItem { Text = "Edit" };
        menuBar.AddRootItem(file);
        menuBar.AddRootItem(edit);
        menuBar.SetAcceleratorVisibility(true);
        var service = new AttachedRenderService();
        service.Attach(menuBar, new Size(14, 1));
        var frame = service.GetSnapshot();

        Assert.AreEqual("File   Edit   ", ReadRow(frame, 0, 14));
        Assert.AreEqual(ConsoleColor.Yellow, frame.GetCell(0, 0).Foreground);
        Assert.AreEqual(ConsoleColor.Yellow, frame.GetCell(1, 0).Foreground);
    }

    [TestMethod]
    public void RendererHighlightsActiveAndDisabledMenuItems()
    {
        var menuBar = new MenuBar { size = new Size(14, 1) };
        var active = new MenuItem { Text = "Active", HotColor = ConsoleColor.White, HotBackColor = ConsoleColor.Blue };
        var disabled = new MenuItem { Text = "Disabled", Enabled = false, DisabledForeColor = ConsoleColor.DarkGray };
        menuBar.AddRootItem(active);
        menuBar.AddRootItem(disabled);
        active.Active = true;
        var service = new AttachedRenderService();
        service.Attach(menuBar, new Size(14, 1));
        var frame = service.GetSnapshot();

        Assert.AreEqual(ConsoleColor.White, frame.GetCell(0, 0).Foreground);
        Assert.AreEqual(ConsoleColor.Blue, frame.GetCell(0, 0).Background);
        Assert.AreEqual(ConsoleColor.DarkGray, frame.GetCell(8, 0).Foreground);
    }

    [TestMethod]
    public void RendererDrawsPopupBorderItemsAndSelection()
    {
        var root = new Panel { size = new Size(16, 7) };
        var popup = new MenuPopup { Position = new Point(2, 1) };
        popup.AddItem(new MenuItem { Text = "Open" });
        var selected = new MenuItem { Text = "Save" };
        popup.AddItem(selected);
        selected.Active = true;
        popup.Show();
        root.Add(popup);
        var service = new AttachedRenderService();
        service.Attach(root, new Size(16, 7));
        var frame = service.GetSnapshot();

        Assert.AreEqual('┌', frame.GetCell(2, 1).Character);
        Assert.AreEqual('O', frame.GetCell(3, 2).Character);
        Assert.AreEqual('S', frame.GetCell(3, 3).Character);
        Assert.AreEqual(ConsoleColor.Black, frame.GetCell(3, 2).Background);
        Assert.AreEqual(ConsoleColor.DarkBlue, frame.GetCell(3, 3).Background);
    }

    [TestMethod]
    public void RendererDrawsPopupSeparatorsAndClipsItems()
    {
        var popup = new MenuPopup { Position = new Point(-1, 0) };
        popup.AddItem(new MenuItem { Text = "Long entry" });
        popup.AddItem(new MenuItem { IsSeparator = true, Text = string.Empty });
        var service = new AttachedRenderService();
        popup.Show();
        service.Attach(popup, new Size(5, 4));
        var frame = service.GetSnapshot();

        Assert.AreEqual("Long ", ReadRow(frame, 1, 5));
        Assert.AreEqual("─────", ReadRow(frame, 2, 5));
        Assert.AreEqual('─', frame.GetCell(0, 0).Character);
    }

    [TestMethod]
    public void RendererDrawsStandaloneMenuItemAndChildren()
    {
        var item = new MenuItem { Text = "Item", size = new Size(6, 1) };
        item.Add(new Label { Text = "X", Position = new Point(5, 0), size = new Size(1, 1) });
        var service = new AttachedRenderService();
        service.Attach(item, new Size(6, 1));

        Assert.AreEqual("Item X", ReadRow(service.GetSnapshot(), 0, 6));
    }

    [TestMethod]
    public void RendererDisplaysUncheckedRadioButton()
    {
        var radio = new RadioButton { Text = "Choice", size = new Size(10, 1) };
        var service = new AttachedRenderService();
        service.Attach(radio, new Size(10, 1));

        Assert.AreEqual("( ) Choice", ReadRow(service.GetSnapshot(), 0, 10));
    }

    [TestMethod]
    public void RendererDisplaysSelectedRadioButton()
    {
        var panel = new Panel { size = new Size(12, 1) };
        var second = new RadioButton { Text = "Second", Position = new Point(0, 0), size = new Size(10, 1) };
        panel.Add(second);
        second.Select();
        var service = new AttachedRenderService();
        service.Attach(panel, new Size(12, 1));

        Assert.AreEqual("(*) Second  ", ReadRow(service.GetSnapshot(), 0, 12));
    }

    [TestMethod]
    public void RendererUsesDisabledRadioButtonColors()
    {
        var radio = new RadioButton { Text = "Off", Enabled = false, size = new Size(7, 1) };
        var service = new AttachedRenderService();
        service.Attach(radio, new Size(7, 1));
        var cell = service.GetSnapshot().GetCell(0, 0);

        Assert.AreEqual('(', cell.Character);
        Assert.AreEqual(ConsoleColor.DarkGray, cell.Foreground);
    }

    [TestMethod]
    public void RendererClipsRadioButtonMarkerAndText()
    {
        var radio = new RadioButton { Text = "Choice", Position = new Point(-2, 0), size = new Size(10, 1) };
        var service = new AttachedRenderService();
        service.Attach(radio, new Size(5, 1));

        Assert.AreEqual(") Cho", ReadRow(service.GetSnapshot(), 0, 5));
    }
}
