using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Reflection;

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class TextBoxTests : TestBase
{
    private sealed class BindModel : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (!string.Equals(_name, value, StringComparison.Ordinal))
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    private sealed class ReadOnlyBindModel : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (!string.Equals(_name, value, StringComparison.Ordinal))
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    private sealed class KeyEventStub : IKeyEvent
    {
        public bool bKeyDown { get; set; } = true;
        public char KeyChar { get; set; }
        public ushort usKeyCode { get; set; }
        public ushort usScanCode { get; set; }
        public uint dwControlKeyState { get; set; }
        public bool Handled { get; set; }

        public static KeyEventStub Char(char ch) => new() { bKeyDown = true, KeyChar = ch, usKeyCode = 0, usScanCode = 0, dwControlKeyState = 0 };
        public static KeyEventStub Nav(ushort keyCode) => new() { bKeyDown = true, KeyChar = '\0', usKeyCode = keyCode, usScanCode = 0, dwControlKeyState = 0 };
    }

    private static void SetActive(TextBox tb, bool active)
    {
        var prop = typeof(Control).GetProperty("Active", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.IsNotNull(prop, "Expected Control.Active property to exist.");
        prop.SetValue(tb, active);
    }

    [TestMethod]
    public void SetText_MultiLine_SplitsLines_AndNormalizesCarriageReturns()
    {
        var tb = new TextBox { MultiLine = true };
        tb.SetText("a\r\nb\n\rc");
        Assert.AreEqual("a\nb\n\rc".Replace("\r", ""), tb.Text.Replace("\r", ""));
        Assert.AreEqual((1, 2), tb.Caret); // last line "c" length == 1, line index 2
    }

    [TestMethod]
    public void SetText_SingleLine_ReplacesNewlinesWithSpaces()
    {
        var tb = new TextBox { MultiLine = false };
        tb.SetText("a\r\nb\n\rc");
        Assert.AreEqual("a  b  c", tb.Text);
        Assert.AreEqual((7, 0), tb.Caret);
    }

    [TestMethod]
    public void MultiLine_SetFalse_NormalizesExistingMultipleLines()
    {
        var tb = new TextBox { MultiLine = true };
        tb.SetText("a\nb\nc");
        tb.MultiLine = false;
        Assert.AreEqual("a b c", tb.Text);
        Assert.AreEqual(0, tb.Caret.Line);
    }

    [TestMethod]
    public void Caret_Set_ClampsToBounds()
    {
        var tb = new TextBox { MultiLine = true };
        tb.SetText("abc\nxy");

        tb.Caret = (999, 999);
        Assert.AreEqual((2, 1), tb.Caret);

        tb.Caret = (-5, -5);
        Assert.AreEqual((0, 0), tb.Caret);
    }

    [TestMethod]
    public void HandlePressKeyEvents_InsertsCharacters_AndUpdatesText()
    {
        var tb = new TextBox { MultiLine = false };
        SetActive(tb, true);

        tb.SetText(string.Empty);
        tb.HandlePressKeyEvents(KeyEventStub.Char('a'));
        tb.HandlePressKeyEvents(KeyEventStub.Char('b'));
        Assert.AreEqual("ab", tb.Text);
        Assert.AreEqual((2, 0), tb.Caret);
    }

    [TestMethod]
    public void HandlePressKeyEvents_Backspace_RemovesCharacter()
    {
        var tb = new TextBox { MultiLine = false };
        SetActive(tb, true);

        tb.SetText("ab");
        tb.HandlePressKeyEvents(KeyEventStub.Char((char)8));
        Assert.AreEqual("a", tb.Text);
        Assert.AreEqual((1, 0), tb.Caret);
    }

    [TestMethod]
    public void HandlePressKeyEvents_Delete_RemovesCharacterAtCaret()
    {
        var tb = new TextBox { MultiLine = false };
        SetActive(tb, true);

        tb.SetText("ab");
        tb.Caret = (0, 0);

        var e = KeyEventStub.Nav(ConsoleFramework.VK_DELETE);
        tb.HandlePressKeyEvents(e);

        Assert.IsTrue(e.Handled);
        Assert.AreEqual("b", tb.Text);
        Assert.AreEqual((0, 0), tb.Caret);
    }

    [TestMethod]
    public void HandlePressKeyEvents_Enter_InMultiLine_InsertsNewLine()
    {
        var tb = new TextBox { MultiLine = true };
        SetActive(tb, true);

        tb.SetText("ab");
        tb.Caret = (1, 0);
        tb.HandlePressKeyEvents(KeyEventStub.Char('\n'));

        Assert.AreEqual("a\nb", tb.Text);
        Assert.AreEqual((0, 1), tb.Caret);
    }

    [TestMethod]
    public void Binding_ModelToTextBox_InitialSyncAndOnPropertyChanged()
    {
        var tb = new TextBox();
        var model = new BindModel { Name = "Hello" };

        // call protected SetBinding via reflection
        var mi = typeof(TextBox).GetMethod("SetBinding", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(mi);
        mi.Invoke(tb, new object[] { model, "Name" });

        Assert.AreEqual("Hello", tb.Text);

        model.Name = "World";
        Assert.AreEqual("World", tb.Text);
    }

    [TestMethod]
    public void Binding_TextBoxToModel_TwoWay_OnEdit()
    {
        var tb = new TextBox { MultiLine = false };
        SetActive(tb, true);

        var model = new BindModel { Name = string.Empty };

        var mi = typeof(TextBox).GetMethod("SetBinding", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(mi);
        mi.Invoke(tb, new object[] { model, "Name" });

        tb.HandlePressKeyEvents(KeyEventStub.Char('x'));
        tb.HandlePressKeyEvents(KeyEventStub.Char('y'));

        Assert.AreEqual("xy", tb.Text);
        Assert.AreEqual("xy", model.Name);
    }

    [TestMethod]
    public void Binding_ReadOnlyProperty_DoesNotThrow_OnEdit()
    {
        var tb = new TextBox { MultiLine = false };
        SetActive(tb, true);

        // Create model with a read-only public property via runtime type
        var model = new ReadOnlyObject();

        var mi = typeof(TextBox).GetMethod("SetBinding", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(mi);
        mi.Invoke(tb, new object[] { model, "Name" });

        tb.HandlePressKeyEvents(KeyEventStub.Char('x'));
        Assert.AreEqual("x", tb.Text);
    }

    private sealed class ReadOnlyObject : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        public string Name => _name;

        public void SetName(string value)
        {
            _name = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
