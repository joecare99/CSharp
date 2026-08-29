using System;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ConsoleLib.Tests;

[TestClass]
public class ModalHostTests
{
    [TestMethod]
    public void ModalHost_ShowsAndClosesDialog()
    {
        var host = new ModalHost();
        var dialog = new Dialog();

        host.Show(dialog);
        Assert.AreSame(dialog, host.ActivePopup);
        Assert.IsTrue(dialog.Visible);

        host.Close();
        Assert.IsNull(host.ActivePopup);
        Assert.IsFalse(dialog.Visible);
    }

    [TestMethod]
    public void ModalHost_RoutesKeyboardInputToPopupAndConsumesIt()
    {
        var host = new ModalHost();
        var dialog = new Dialog();
        var checkBox = new CheckBox();
        dialog.Add(checkBox);
        host.Show(dialog);
        checkBox.Active = true;
        var key = Substitute.For<IKeyEvent>();
        key.bKeyDown.Returns(true);
        key.KeyChar.Returns(' ');

        host.HandlePressKeyEvents(key);

        Assert.IsTrue(checkBox.IsChecked);
        Assert.IsTrue(key.Handled);
    }

    [TestMethod]
    public void ModalHost_ClosesPopupOnEscape()
    {
        var host = new ModalHost();
        host.Show(new Dialog());
        var key = Substitute.For<IKeyEvent>();
        key.bKeyDown.Returns(true);
        key.usKeyCode.Returns((ushort)ConsoleKey.Escape);

        host.HandlePressKeyEvents(key);

        Assert.IsNull(host.ActivePopup);
        Assert.IsTrue(key.Handled);
    }
}
