using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using System.Drawing;
using TestConsole;
using BaseLib.Interfaces;

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class MenuPopupTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private static TstConsole _tstCon;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
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
    public void AddItem_Sets_Parent_And_Layouts()
    {
        var popup = new MenuPopup();
        var mi = new MenuItem();
        popup.AddItem(mi);
        Assert.AreSame(popup, mi.Parent);
    }

    [TestMethod]
    public void Show_Sets_Visible_True()
    {
        var popup = new MenuPopup();
        popup.Show();
        Assert.IsTrue(popup.Visible);
    }

    [TestMethod]
    public void Hide_Sets_Visible_False()
    {
        var popup = new MenuPopup();
        popup.Show();
        popup.Hide();
        Assert.IsFalse(popup.Visible);
    }
}
