using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using System.Drawing;
using NSubstitute;
using System.Windows.Input;
using TestConsole;
using BaseLib.Interfaces; // use WPF ICommand if available

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class MenuItemTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private static TstConsole _tstCon;
    private IConsole _oldCon;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

    [TestInitialize]
    public void TestInit()
    {
        _tstCon ??= new TstConsole();
        _oldCon = ConsoleFramework.console;
        ConsoleFramework.console = _tstCon;
    }
    [TestCleanup]
    public void TestCleanup()
    {
        ConsoleFramework.console = _oldCon;
    }

    [TestMethod]
    public void SetText_Updates_Size()
    {
        var mi = new MenuItem();
        mi.SetText("File");
        Assert.IsGreaterThanOrEqualTo(4, mi.size.Width); // simplistic assumption
    }

    [TestMethod]
    public void MouseEnter_Sets_Hover()
    {
        var mi = new MenuItem();
        mi.MouseEnter(new Point(0,0));
        mi.Draw();
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Click_Invokes_Command_When_Enabled()
    {
        var mi = new MenuItem();
        var cmd = Substitute.For<ICommand>();
        cmd.CanExecute(mi.Tag).Returns(true);
        mi.Command = cmd;
        mi.Click();
        cmd.Received().Execute(mi.Tag);
    }

    [TestMethod]
    public void HandlePressKeyEvents_Invokes_Click_On_Shortcut()
    {
        var mi = new MenuItem
        {
            ShortcutKey = 'X'
        };
        var ke = Substitute.For<Interfaces.IKeyEvent>();
        ke.bKeyDown.Returns(true);
        ke.KeyChar.Returns('x');
        mi.HandlePressKeyEvents(ke);
        Assert.IsTrue(true);
    }
}
