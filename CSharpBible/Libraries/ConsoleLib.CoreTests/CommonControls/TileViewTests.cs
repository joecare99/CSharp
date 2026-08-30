using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace ConsoleLib.Tests;

[TestClass]
public class TileViewTests
{
    [TestMethod]
    public void TileView_SelectsItemsInOrder()
    {
        var view = new TileView();
        view.Items.Add(new TileItem("One"));
        view.Items.Add(new TileItem("Two"));

        Assert.IsTrue(view.SelectNext());
        Assert.AreEqual("One", view.SelectedItem!.Text);
        Assert.IsTrue(view.SelectNext());
        Assert.AreEqual("Two", view.SelectedItem!.Text);
    }

    [TestMethod]
    public void TileView_SetItemsAndViewport_ReturnsVisibleItemsAndScrollsSelection()
    {
        var view = new TileView { size = new System.Drawing.Size(4, 2), TileWidth = 2, TileHeight = 1 };
        view.SetItems(Enumerable.Range(1, 5).Select(i => new TileItem(i.ToString())));

        Assert.AreEqual(4, view.GetVisibleItems().Count);
        Assert.AreEqual("1", view.GetVisibleItems()[0].Text);
        Assert.IsTrue(view.SelectNext());
        Assert.IsTrue(view.SelectNext());
        Assert.IsTrue(view.SelectNext());
        Assert.IsTrue(view.SelectNext());
        Assert.AreEqual(1, view.FirstVisibleIndex);
        Assert.AreEqual("5", view.SelectedItem!.Text);
        Assert.AreEqual("5", view.GetVisibleItems()[3].Text);
    }

    [TestMethod]
    public void TileView_EmptyItemsResetSelectionAndRejectInvalidSource()
    {
        var view = new TileView();
        view.SetItems(new[] { new TileItem("One") });
        view.SetItems(Array.Empty<TileItem>());

        Assert.AreEqual(-1, view.SelectedIndex);
        Assert.IsNull(view.SelectedItem);
        Assert.AreEqual(0, view.GetVisibleItems().Count);
        Assert.Throws<ArgumentNullException>(() => view.SetItems(null!));
    }

    [TestMethod]
    public void TileView_KeyboardNavigationMarksHandledOnlyWhenSelectionChanges()
    {
        var view = new TileView();
        view.SetItems(new[] { new TileItem("One"), new TileItem("Two") });
        view.Active = true;

        var down = new KeyEventStub((ushort)ConsoleKey.DownArrow);
        view.HandlePressKeyEvents(down);
        Assert.IsTrue(down.Handled);
        Assert.AreEqual(1, view.SelectedIndex);

        var atEnd = new KeyEventStub((ushort)ConsoleKey.DownArrow);
        view.HandlePressKeyEvents(atEnd);
        Assert.IsFalse(atEnd.Handled);
        Assert.AreEqual(1, view.SelectedIndex);
    }

    private sealed class KeyEventStub : IKeyEvent
    {
        public KeyEventStub(ushort keyCode) => usKeyCode = keyCode;
        public bool bKeyDown => true;
        public char KeyChar => '\0';
        public ushort usKeyCode { get; }
        public ushort usScanCode => 0;
        public uint dwControlKeyState => 0;
        public bool Handled { get; set; }
    }
}
