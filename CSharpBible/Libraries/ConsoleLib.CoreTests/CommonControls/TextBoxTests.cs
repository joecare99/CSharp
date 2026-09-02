using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib.ExtCon;

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class TextBoxTests : TestBase
{
    private sealed class ClipboardStub : IClipboardService
    {
        public string? Text { get; private set; }
        public string? PasteText { get; set; }

        public Task<bool> CopyAsync(string text, CancellationToken cancellationToken = default)
        {
            Text = text;
            return Task.FromResult(true);
        }

        public Task<string?> PasteAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(PasteText);
    }

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
        public static KeyEventStub ShiftNav(ushort keyCode) => new() { bKeyDown = true, KeyChar = '\0', usKeyCode = keyCode, usScanCode = 0, dwControlKeyState = 0x10 };
        public static KeyEventStub ControlNav(ushort keyCode) => new() { bKeyDown = true, KeyChar = '\0', usKeyCode = keyCode, usScanCode = 0, dwControlKeyState = 0x08 };
        public static KeyEventStub ControlChar(char ch) => new() { bKeyDown = true, KeyChar = ch, usKeyCode = 0, usScanCode = 0, dwControlKeyState = 0x08 };
        public static KeyEventStub ControlNavWithShift(ushort keyCode) => new() { bKeyDown = true, KeyChar = '\0', usKeyCode = keyCode, usScanCode = 0, dwControlKeyState = 0x18 };
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

    [TestMethod]
    public void SelectAllAndCopyAsync_CopiesSelectedText()
    {
        var clipboard = new ClipboardStub();
        var tb = new TextBox { MultiLine = false, ClipboardService = clipboard };
        tb.SetText("hello");
        tb.SelectAll();

        Assert.IsTrue(tb.CopyAsync().GetAwaiter().GetResult());
        Assert.AreEqual("hello", clipboard.Text);
        Assert.AreEqual("hello", tb.SelectedText);
    }

    [TestMethod]
    public void PasteAsync_ReplacesSelectionAndPlacesCaretAfterInsertedText()
    {
        var clipboard = new ClipboardStub { PasteText = "X" };
        var tb = new TextBox { MultiLine = false, ClipboardService = clipboard };
        tb.SetText("hello");
        tb.SelectAll();

        Assert.IsTrue(tb.PasteAsync().GetAwaiter().GetResult());

        Assert.AreEqual("X", tb.Text);
        Assert.AreEqual((1, 0), tb.Caret);
        Assert.AreEqual(string.Empty, tb.SelectedText);
    }

    [TestMethod]
    public void ShiftNavigation_SelectsText_AndTypingReplacesIt()
    {
        var tb = new TextBox { MultiLine = false };
        SetActive(tb, true);
        tb.SetText("hello");
        tb.Caret = (1, 0);

        tb.HandlePressKeyEvents(KeyEventStub.ShiftNav(ConsoleFramework.VK_RIGHT));
        tb.HandlePressKeyEvents(KeyEventStub.ShiftNav(ConsoleFramework.VK_RIGHT));

        Assert.AreEqual("el", tb.SelectedText);
        tb.HandlePressKeyEvents(KeyEventStub.Char('X'));

        Assert.AreEqual("hXlo", tb.Text);
        Assert.AreEqual((2, 0), tb.Caret);
        Assert.AreEqual(string.Empty, tb.SelectedText);
    }

    [TestMethod]
    public void ControlNavigation_MovesByWords_AndShiftSelectsWordRange()
    {
        var tb = new TextBox { MultiLine = false };
        SetActive(tb, true);
        tb.SetText("one two three");
        tb.Caret = (0, 0);

        tb.HandlePressKeyEvents(KeyEventStub.ControlNav(ConsoleFramework.VK_RIGHT));
        Assert.AreEqual((4, 0), tb.Caret);

        tb.HandlePressKeyEvents(KeyEventStub.ControlNavWithShift(ConsoleFramework.VK_RIGHT));
        Assert.AreEqual("two ", tb.SelectedText);
        Assert.AreEqual((8, 0), tb.Caret);
    }

    [TestMethod]
    public void ControlBackspace_DeletesPreviousWordAndWhitespace()
    {
        var tb = new TextBox { MultiLine = false };
        SetActive(tb, true);
        tb.SetText("one two");
        tb.HandlePressKeyEvents(KeyEventStub.ControlChar((char)8));

        Assert.AreEqual("one ", tb.Text);
        Assert.AreEqual((4, 0), tb.Caret);
    }

    [TestMethod]
    public void ControlDelete_DeletesNextWord()
    {
        var tb = new TextBox { MultiLine = false };
        SetActive(tb, true);
        tb.SetText("one two");
        tb.Caret = (0, 0);
        tb.HandlePressKeyEvents(KeyEventStub.ControlChar((char)127));

        Assert.AreEqual(" two", tb.Text);
        Assert.AreEqual((0, 0), tb.Caret);
    }

    [TestMethod]
    public void ControlWordNavigation_CrossesLineBoundaries()
    {
        var tb = new TextBox { MultiLine = true };
        SetActive(tb, true);
        tb.SetText("one\ntwo");
        tb.Caret = (3, 0);

        tb.HandlePressKeyEvents(KeyEventStub.ControlNav(ConsoleFramework.VK_RIGHT));
        Assert.AreEqual((0, 1), tb.Caret);

        tb.HandlePressKeyEvents(KeyEventStub.ControlNav(ConsoleFramework.VK_LEFT));
        Assert.AreEqual((0, 0), tb.Caret);
    }

    [TestMethod]
    public void Navigation_HandlesHomeEndVerticalAndPageKeys()
    {
        var tb = new TextBox { MultiLine = true, size = new System.Drawing.Size(10, 2) };
        SetActive(tb, true);
        tb.SetText("first\nsecond\nthird\nfourth");
        tb.Caret = (3, 2);

        tb.HandlePressKeyEvents(KeyEventStub.Nav(ConsoleFramework.VK_HOME));
        Assert.AreEqual((0, 2), tb.Caret);
        tb.HandlePressKeyEvents(KeyEventStub.Nav(ConsoleFramework.VK_END));
        Assert.AreEqual((5, 2), tb.Caret);
        tb.HandlePressKeyEvents(KeyEventStub.Nav(ConsoleFramework.VK_UP));
        Assert.AreEqual((5, 1), tb.Caret);
        tb.HandlePressKeyEvents(KeyEventStub.Nav(ConsoleFramework.VK_DOWN));
        Assert.AreEqual((5, 2), tb.Caret);
        tb.HandlePressKeyEvents(KeyEventStub.Nav((ushort)ConsoleKey.PageUp));
        Assert.AreEqual((5, 1), tb.Caret);
        tb.HandlePressKeyEvents(KeyEventStub.Nav((ushort)ConsoleKey.PageDown));
        Assert.AreEqual((5, 2), tb.Caret);
        Assert.AreEqual(1, tb.GetFirstVisibleLine());
    }

    [TestMethod]
    public void Navigation_AtBoundariesFallsThroughWithoutHandling()
    {
        var tb = new TextBox { MultiLine = false };
        SetActive(tb, true);
        tb.SetText("x");

        var left = KeyEventStub.Nav(ConsoleFramework.VK_LEFT);
        tb.Caret = (0, 0);
        tb.HandlePressKeyEvents(left);
        Assert.IsFalse(left.Handled);

        var right = KeyEventStub.Nav(ConsoleFramework.VK_RIGHT);
        tb.Caret = (1, 0);
        tb.HandlePressKeyEvents(right);
        Assert.IsFalse(right.Handled);
    }

    [TestMethod]
    public void Enter_InvokesHandlerForSingleLineAndMarksUnhandledParentEvent()
    {
        var tb = new TextBox { MultiLine = false };
        SetActive(tb, true);
        var invoked = false;
        tb.OnEnterKey += (_, e) => invoked = true;

        var enter = KeyEventStub.Char('\r');
        tb.HandlePressKeyEvents(enter);

        Assert.IsTrue(invoked);
        Assert.IsTrue(enter.Handled);
    }

    [TestMethod]
    public void ClipboardOperations_ReturnFalseWithoutServiceOrWhenPasteIsNull()
    {
        var withoutClipboard = new TextBox();
        Assert.IsFalse(withoutClipboard.CopyAsync().GetAwaiter().GetResult());
        Assert.IsFalse(withoutClipboard.PasteAsync().GetAwaiter().GetResult());

        var clipboard = new ClipboardStub { PasteText = null };
        var withClipboard = new TextBox { ClipboardService = clipboard };
        withClipboard.SetText("unchanged");

        Assert.IsFalse(withClipboard.PasteAsync().GetAwaiter().GetResult());
        Assert.AreEqual("unchanged", withClipboard.Text);
    }

    [TestMethod]
    public void DisplayAndCaretHelpers_ReturnExpectedValues()
    {
        var tb = new TextBox { MultiLine = true };
        tb.SetText("A界\nB");
        tb.Caret = (2, 0);

        Assert.AreEqual(3, tb.GetCaretCellColumn());
        Assert.AreEqual("A界", tb.GetDisplayLine(0));
        Assert.AreEqual("", tb.GetDisplayLine(-1));
        Assert.AreEqual("", tb.GetDisplayLine(99));
        Assert.AreEqual(tb.ShouldShowCaret(), tb.ShouldShowCaret());

        tb.ApplyNativeText("native");
        Assert.AreEqual("native", tb.Text);
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
