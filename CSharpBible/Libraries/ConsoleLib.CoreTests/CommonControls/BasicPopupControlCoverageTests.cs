using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Drawing;

namespace ConsoleLib.Tests;

[TestClass]
public class BasicPopupControlCoverageTests
{
    [TestMethod]
    public void StatusBar_UsesTextAndRenderer()
    {
        var status = new StatusBar { Status = "Ready", StatusColor = ConsoleColor.Green };
        Assert.AreEqual("Ready", status.Status);
        Assert.AreEqual(ConsoleColor.Green, status.StatusColor);

        status.Status = null!;
        Assert.AreEqual(string.Empty, status.Status);

        var renderer = Substitute.For<IWidgetSet, IFormWidgetRenderer>();
        var app = new Application(renderer);
        status.Parent = app;
        status.Draw();
        ((IFormWidgetRenderer)renderer).Received(1).DrawStatusBar(status);
    }

    [TestMethod]
    public void Dialog_ShowAndHideManageVisibilityAndFocus()
    {
        var dialog = new Dialog();
        Assert.IsFalse(dialog.Visible);
        Assert.IsFalse(dialog.Active);

        dialog.Show();
        Assert.IsTrue(dialog.Visible);
        Assert.IsTrue(dialog.Active);

        dialog.Hide();
        Assert.IsFalse(dialog.Visible);
        Assert.IsFalse(dialog.Active);
    }

    [TestMethod]
    public void Pixel_SetsPositionAndOptionalText()
    {
        var pixel = new Pixel();
        Assert.AreEqual(new Size(1, 1), pixel.size);

        pixel.Set(3, 4, "X");
        Assert.AreEqual(new Point(3, 4), pixel.Position);
        Assert.AreEqual("X", pixel.Text);

        pixel.Set(new Point(5, 6));
        Assert.AreEqual(new Point(5, 6), pixel.Position);
        Assert.AreEqual("X", pixel.Text);
    }

    [TestMethod]
    public void ModalHost_ShowsReplacesAndClosesPopup()
    {
        var host = new ModalHost();
        var previous = new Control { Parent = host };
        previous.Active = true;
        var first = Substitute.For<IPopup>();
        var second = Substitute.For<IPopup>();

        host.Show(first);
        Assert.AreSame(first, host.ActivePopup);
        first.Received(1).Show();
        Assert.AreSame(first, host.ActiveControl);

        host.Show(second);
        first.Received(1).Hide();
        second.Received(1).Show();
        Assert.AreSame(second, host.ActivePopup);

        host.Close();
        second.Received(1).Hide();
        Assert.IsNull(host.ActivePopup);
        Assert.IsTrue(previous.Active);

        host.Close();
        Assert.Throws<ArgumentNullException>(() => host.Show(null!));
    }

    [TestMethod]
    public void ModalHost_ForwardsKeysAndMouseAndClosesOnEscape()
    {
        var host = new ModalHost();
        var popup = Substitute.For<IPopup>();
        host.Show(popup);

        var key = new KeyEventStub((ushort)ConsoleKey.A);
        host.HandlePressKeyEvents(key);
        popup.Received(1).HandlePressKeyEvents(key);
        Assert.IsTrue(key.Handled);

        var mouse = Substitute.For<IMouseEvent>();
        host.MouseClick(mouse);
        popup.Received(1).MouseClick(mouse);

        var escape = new KeyEventStub((ushort)ConsoleKey.Escape);
        host.HandlePressKeyEvents(escape);
        popup.Received(1).Hide();
        Assert.IsTrue(escape.Handled);
        Assert.IsNull(host.ActivePopup);
    }

    [TestMethod]
    public void ModalHost_CloseWithoutPopupIsIdempotent()
    {
        var host = new ModalHost();

        host.Close();

        Assert.IsNull(host.ActivePopup);
        Assert.AreEqual(0, host.Children.Count);
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