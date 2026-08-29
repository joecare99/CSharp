// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : AI Assistant
// Created          : 10-01-2025
// Last Modified    : 10-01-2025
// ***********************************************************************
// <copyright file="TextBox.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// Simple (optional multi-line) text input control for ConsoleLib (mit Cursor-Steuerung)
// ***********************************************************************
using ConsoleLib.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel; // Added for binding
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleLib.CommonControls;

/// <summary>
/// Text input control supporting single or multi-line editing incl. basic cursor navigation.
/// </summary>
public class TextBox : Control
{
    private sealed class DefaultConsoleKeyMap : IHasConsoleKeyMap
    {
        public static DefaultConsoleKeyMap Instance { get; } = new();

        public ushort KeyEnter => (ushort)ConsoleKey.Enter;
        public ushort KeyEsc => (ushort)ConsoleKey.Escape;
        public ushort KeyTab => (ushort)ConsoleKey.Tab;
        public ushort KeyLeft => 0x25;
        public ushort KeyUp => 0x26;
        public ushort KeyRight => 0x27;
        public ushort KeyDown => 0x28;
        public ushort KeyHome => 0x24;
        public ushort KeyEnd => 0x23;
        public ushort KeyDelete => 0x2E;
        public ushort KeyPageUp => 0x21;
        public ushort KeyPageDown => 0x22;
    }

    private readonly List<string> _lines = new();
    private int _caretLine;
    private int _caretCol;
    private int _firstVisibleLine;
    private bool _multiLine = true;
    private DateTime _lastBlink = DateTime.Now;
    private bool _showCaret = true;
    private int? _selectionAnchor;

    // Two-way binding backing fields
    private INotifyPropertyChanged? _boundModel;
    private string? _boundProperty;
    private PropertyInfo? _boundPropInfo;
    private bool _suppressModelUpdate;

    public bool MultiLine
    {
        get => _multiLine;
        set
        {
            _multiLine = value;
            if (!value)
            {
                NormalizeSingleLine();
                UpdateTextProperty();
            }
            NotifyWidgetStateChanged();
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets caret (column,line)
    /// </summary>
    public (int Column, int Line) Caret
    {
        get => (_caretCol, _caretLine);
        set
        {
            _caretLine = Math.Max(0, Math.Min(value.Line, _lines.Count - 1));
            _caretCol = Math.Max(0, Math.Min(value.Column, _lines[_caretLine].Length));
            EnsureCaretVisible();
            NotifyWidgetStateChanged();
            Invalidate();
        }
    }

    public ConsoleColor CaretColor { get; set; } = ConsoleColor.Yellow;
    public ConsoleColor DisabledForeColor { get; set; } = ConsoleColor.DarkGray;
    public ITextLayoutService TextLayoutService { get; set; } = new UnicodeTextLayoutService();
    public IClipboardService? ClipboardService { get; set; }
    public event EventHandler<IKeyEvent>? OnEnterKey;

    public TextBox()
    {
        _lines.Add(string.Empty);
        BackColor = ConsoleColor.DarkBlue;
        ForeColor = ConsoleColor.White;
    }

    /// <summary>
    /// Establish or change a two-way binding between this TextBox and a property of a model implementing INotifyPropertyChanged.
    /// Changing the binding updates the TextBox text from the model immediately.
    /// </summary>
    protected override void SetBinding(INotifyPropertyChanged model, string propertyName)
    {
        if (_boundModel != null)
        {
            _boundModel.PropertyChanged -= OnModelPropertyChanged;
        }
        _boundModel = model;
        _boundProperty = propertyName;
        _boundPropInfo = model.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (_boundPropInfo == null || !_boundPropInfo.CanRead)
        {
            _boundModel = null;
            _boundProperty = null;
            _boundPropInfo = null;
            return;
        }
        if (!_boundPropInfo.CanWrite)
        {
            // Still allow one-way (model -> TextBox)
        }
        _boundModel.PropertyChanged += OnModelPropertyChanged;
        // Initial sync from model
        SyncFromModel();
    }

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_boundProperty == null || !string.Equals(e.PropertyName, _boundProperty, StringComparison.OrdinalIgnoreCase))
            return;
        SyncFromModel();
    }

    private void SyncFromModel()
    {
        if (_boundModel == null || _boundPropInfo == null)
            return;
        var val = _boundPropInfo.GetValue(_boundModel);
        var str = val?.ToString() ?? string.Empty;
        if (!string.Equals(str, Text, StringComparison.Ordinal))
        {
            _suppressModelUpdate = true;
            try
            {
                SetText(str);
            }
            finally
            {
                _suppressModelUpdate = false;
            }
        }
    }

    public override void SetText(string value)
    {
        if (value == null)
            value = string.Empty;

        var normalizedValue = _multiLine
            ? value.Replace("\r", "")
            : value.Replace("\r", " ").Replace("\n", " ");

        base.SetText(normalizedValue);
        _lines.Clear();
        if (_multiLine)
        {
            foreach (var l in normalizedValue.Split('\n'))
                _lines.Add(l);
        }
        else
        {
            _lines.Add(normalizedValue);
        }
        if (_lines.Count == 0)
            _lines.Add(string.Empty);
        _caretLine = _lines.Count - 1;
        _caretCol = _lines[_caretLine].Length;
        _firstVisibleLine = Math.Max(0, _caretLine - size.Height + 1);
        UpdateBoundProperty(normalizedValue);
        NotifyWidgetStateChanged();
    }

    private void NormalizeSingleLine()
    {
        if (_lines.Count <= 1)
            return;
        var all = string.Join(" ", _lines);
        _lines.Clear();
        _lines.Add(all);
        _caretLine = 0;
        _caretCol = Math.Min(_caretCol, _lines[0].Length);
        _firstVisibleLine = 0;
    }

    public override void Draw()
    {
        WidgetSet?.DrawTextBox(this);
        Valid = true;
    }

    public void UpdateBlinkState()
    {
        if ((DateTime.Now - _lastBlink).TotalMilliseconds > 500)
        {
            _showCaret = !_showCaret;
            _lastBlink = DateTime.Now;
        }
    }

    public int GetFirstVisibleLine() => _firstVisibleLine;
    public int GetCaretLine() => _caretLine;
    public int GetCaretColumn() => _caretCol;
    public int GetCaretCellColumn() => TextLayoutService.GetCellWidth(_lines[_caretLine].Substring(0, _caretCol));

    public Task<bool> CopyAsync(CancellationToken cancellationToken = default) =>
        ClipboardService is null ? Task.FromResult(false) : ClipboardService.CopyAsync(GetSelectionLength() == 0 ? Text : GetSelectedText(), cancellationToken);

    public async Task<bool> PasteAsync(CancellationToken cancellationToken = default)
    {
        if (ClipboardService is null)
            return false;
        var text = await ClipboardService.PasteAsync(cancellationToken).ConfigureAwait(false);
        if (text is null)
            return false;
        var start = GetSelectionStart();
        var length = GetSelectionLength();
        var newText = Text.Remove(start, length).Insert(start, text);
        SetText(newText);
        SetCaretFromAbsolute(start + text.Length);
        ClearSelection();
        return true;
    }

    public string SelectedText => GetSelectedText();

    public void SelectAll()
    {
        _selectionAnchor = 0;
        SetCaretFromAbsolute(Text.Length);
        Invalidate();
    }
    public bool ShouldShowCaret() => _showCaret;
    public string GetDisplayLine(int index) => GetLineForDisplay(index);
    public void ApplyNativeText(string value)
    {
        SetText(value);
    }

    private string GetLineForDisplay(int idx)
    {
        if (idx < 0 || idx >= _lines.Count)
            return string.Empty;
        return _lines[idx];
    }

    public override void HandlePressKeyEvents(IKeyEvent e)
    {
        if (!Enabled || !Active)
        { base.HandlePressKeyEvents(e); return; }

        if (e.bKeyDown && IsControlPressed(e))
        {
            var shortcut = char.ToUpperInvariant(e.KeyChar);
            if (shortcut == 'A' || e.usKeyCode == (ushort)ConsoleKey.A)
            {
                SelectAll();
                e.Handled = true;
                return;
            }
            if (shortcut == 'C' || e.usKeyCode == (ushort)ConsoleKey.C)
            {
                _ = CopyAsync().GetAwaiter().GetResult();
                e.Handled = true;
                return;
            }
            if (shortcut == 'X' || e.usKeyCode == (ushort)ConsoleKey.X)
            {
                if (GetSelectionLength() > 0)
                {
                    _ = CopyAsync().GetAwaiter().GetResult();
                    ReplaceSelection(string.Empty);
                }
                e.Handled = true;
                return;
            }
            if (shortcut == 'V' || e.usKeyCode == (ushort)ConsoleKey.V)
            {
                _ = PasteAsync().GetAwaiter().GetResult();
                e.Handled = true;
                return;
            }
        }

        // Navigation keys (check usKeyCode)
        if (e.bKeyDown && e.KeyChar == '\0') // typical for non-char keys
        {
            var selectionBeforeNavigation = GetCaretAbsoluteIndex();
            bool navHandled = HandleNavigationKey(e.usKeyCode, IsControlPressed(e));
            if (navHandled)
            {
                if (IsShiftPressed(e))
                    _selectionAnchor ??= selectionBeforeNavigation;
                else
                    ClearSelection();
                e.Handled = true;
                return; // swallow
            }
        }

        bool handled = false;
        char ch = e.KeyChar;
        if (e.bKeyDown && IsControlPressed(e) && GetSelectionLength() == 0)
        {
            if (ch == (char)8)
            {
                handled = DeleteWordBackward();
            }
            else if (ch == (char)127)
            {
                handled = DeleteWordForward();
            }
            if (handled)
            {
                UpdateTextProperty();
                e.Handled = true;
                return;
            }
        }

        // Basic editing
        if (GetSelectionLength() > 0 && (ch == (char)8 || ch == (char)127 || ch == '\r' || ch == '\n' || !char.IsControl(ch)))
        {
            if (ch == '\r' || ch == '\n')
                handled = MultiLine ? ReplaceSelection("\n") : false;
            else if (ch == (char)8 || ch == (char)127)
                handled = ReplaceSelection(string.Empty);
            else
                handled = ReplaceSelection(ch.ToString());
            if (handled)
            {
                UpdateTextProperty();
                e.Handled = true;
                return;
            }
        }
        switch (ch)
        {
            case (char)8: // Backspace
                handled = Backspace();
                break;
            case '\r': // Enter
            case '\n':
                if (MultiLine)
                {
                    handled = NewLine();
                }
                else if (OnEnterKey is not null)
                {
                    OnEnterKey.Invoke(this, e);
                    handled = e.Handled;
                    if (!handled)
                        e.Handled = true;
                    return;
                }
                else
                {
                    if (Parent is Control parent)
                        parent.HandleUnhandledChildKeyEvent(e, this);
                    return;
                }
                break;
            case (char)27: // ESC ignore
                handled = false;
                break;
            default:
                if (!char.IsControl(ch))
                {
                    handled = InsertChar(ch);
                }
                break;
        }
        if (handled)
        {
            UpdateTextProperty();
            e.Handled = true;
        }
        if (!e.Handled)
            base.HandlePressKeyEvents(e);
    }

    private bool HandleNavigationKey(ushort keyCode, bool controlPressed)
    {
        var keyMap = GetWidgetSetCapability<IHasConsoleKeyMap>() ?? DefaultConsoleKeyMap.Instance;

        switch (keyCode)
        {
            case var _ when keyCode == keyMap.KeyLeft:
                return controlPressed ? CaretByWord(-1) : CaretLeft();
            case var _ when keyCode == keyMap.KeyRight:
                return controlPressed ? CaretByWord(1) : CaretRight();
            case var _ when keyCode == keyMap.KeyUp:
                return CaretUp();
            case var _ when keyCode == keyMap.KeyDown:
                return CaretDown();
            case var _ when keyCode == keyMap.KeyHome:
                return CaretHome();
            case var _ when keyCode == keyMap.KeyEnd:
                return CaretEnd();
            case var _ when keyCode == keyMap.KeyDelete:
                return Delete();
            case var _ when keyCode == keyMap.KeyPageUp:
                return PageUp();
            case var _ when keyCode == keyMap.KeyPageDown:
                return PageDown();
        }
        return false;
    }

    private static bool IsControlPressed(IKeyEvent e) =>
            (e.dwControlKeyState & (0x0004u | 0x0008u)) != 0;

    private static bool IsShiftPressed(IKeyEvent e) => (e.dwControlKeyState & 0x0010u) != 0;

        private int GetCaretAbsoluteIndex()
        {
            var index = 0;
            for (var i = 0; i < _caretLine; i++)
                index += _lines[i].Length + 1;
            return index + _caretCol;
        }

        private int GetSelectionStart() =>
            Math.Min(_selectionAnchor ?? GetCaretAbsoluteIndex(), GetCaretAbsoluteIndex());

        private int GetSelectionLength() =>
            Math.Abs((_selectionAnchor ?? GetCaretAbsoluteIndex()) - GetCaretAbsoluteIndex());

        private string GetSelectedText() =>
            GetSelectionLength() == 0 ? string.Empty : Text.Substring(GetSelectionStart(), GetSelectionLength());

        private void ClearSelection() => _selectionAnchor = null;

        private bool ReplaceSelection(string replacement)
        {
            var start = GetSelectionStart();
            SetText(Text.Remove(start, GetSelectionLength()).Insert(start, replacement));
            SetCaretFromAbsolute(start + replacement.Length);
            ClearSelection();
            return true;
        }

        private void SetCaretFromAbsolute(int index)
        {
            index = Math.Max(0, Math.Min(index, Text.Length));
            var remaining = index;
            for (var line = 0; line < _lines.Count; line++)
            {
                if (remaining <= _lines[line].Length)
                {
                    _caretLine = line;
                    _caretCol = remaining;
                    EnsureCaretVisible();
                    return;
                }
                remaining -= _lines[line].Length;
                if (line < _lines.Count - 1)
                    remaining--;
            }
            _caretLine = _lines.Count - 1;
            _caretCol = _lines[_caretLine].Length;
            EnsureCaretVisible();
    }

    private bool CaretLeft()
    {
        if (_caretCol > 0)
        {
            _caretCol--;
            Invalidate();
            return true;
        }
        if (MultiLine && _caretLine > 0)
        {
            _caretLine--;
            _caretCol = _lines[_caretLine].Length;
            EnsureCaretVisible();
            Invalidate();
            return true;
        }
        return false;
    }
    private bool CaretRight()
    {
        var lineLen = _lines[_caretLine].Length;
        if (_caretCol < lineLen)
        {
            _caretCol++;
            Invalidate();
            return true;
        }
        if (MultiLine && _caretLine < _lines.Count - 1)
        {
            _caretLine++;
            _caretCol = 0;
            EnsureCaretVisible();
            Invalidate();
            return true;
        }
        return false;
    }

    private bool CaretByWord(int direction)
    {
        var current = GetCaretAbsoluteIndex();
        var target = current;
        if (direction < 0)
        {
            while (target > 0 && char.IsWhiteSpace(Text[target - 1]))
                target--;
            while (target > 0 && !char.IsWhiteSpace(Text[target - 1]))
                target--;
        }
        else
        {
            while (target < Text.Length && !char.IsWhiteSpace(Text[target]))
                target++;
            while (target < Text.Length && char.IsWhiteSpace(Text[target]))
                target++;
        }

        if (target == current)
            return false;
        SetCaretFromAbsolute(target);
        Invalidate();
        return true;
    }
    private bool CaretUp()
    {
        if (!MultiLine || _caretLine == 0)
            return false;
        _caretLine--;
        _caretCol = Math.Min(_caretCol, _lines[_caretLine].Length);
        EnsureCaretVisible();
        Invalidate();
        return true;
    }
    private bool CaretDown()
    {
        if (!MultiLine || _caretLine >= _lines.Count - 1)
            return false;
        _caretLine++;
        _caretCol = Math.Min(_caretCol, _lines[_caretLine].Length);
        EnsureCaretVisible();
        Invalidate();
        return true;
    }
    private bool CaretHome()
    {
        if (_caretCol == 0)
            return false;
        _caretCol = 0;
        Invalidate();
        return true;
    }
    private bool CaretEnd()
    {
        var len = _lines[_caretLine].Length;
        if (_caretCol == len)
            return false;
        _caretCol = len;
        Invalidate();
        return true;
    }
    private bool PageUp()
    {
        if (!MultiLine)
            return false;
        int newLine = Math.Max(0, _caretLine - Math.Max(1, size.Height - 1));
        if (newLine == _caretLine)
            return false;
        _caretLine = newLine;
        _caretCol = Math.Min(_caretCol, _lines[_caretLine].Length);
        EnsureCaretVisible();
        Invalidate();
        return true;
    }
    private bool PageDown()
    {
        if (!MultiLine)
            return false;
        int newLine = Math.Min(_lines.Count - 1, _caretLine + Math.Max(1, size.Height - 1));
        if (newLine == _caretLine)
            return false;
        _caretLine = newLine;
        _caretCol = Math.Min(_caretCol, _lines[_caretLine].Length);
        EnsureCaretVisible();
        Invalidate();
        return true;
    }

    private bool InsertChar(char ch)
    {
        if (GetSelectionLength() > 0)
            return ReplaceSelection(ch.ToString());
        var line = _lines[_caretLine];
        if (line.Length >= 2000)
            return false; // simple guard
        line = line.Insert(_caretCol, ch.ToString());
        _lines[_caretLine] = line;
        _caretCol++;
        EnsureCaretVisible();
        Invalidate();
        return true;
    }

    private bool Backspace()
    {
        if (GetSelectionLength() > 0)
            return ReplaceSelection(string.Empty);
        if (_caretCol > 0)
        {
            var line = _lines[_caretLine];
            line = line.Remove(_caretCol - 1, 1);
            _lines[_caretLine] = line;
            _caretCol--;
            EnsureCaretVisible();
            Invalidate();
            return true;
        }
        if (_caretLine > 0)
        {
            // Merge with previous line
            var prevLen = _lines[_caretLine - 1].Length;
            _lines[_caretLine - 1] += _lines[_caretLine];
            _lines.RemoveAt(_caretLine);
            _caretLine--;
            _caretCol = prevLen;
            EnsureCaretVisible();
            Invalidate();
            return true;
        }
        return false;
    }

    private bool Delete()
    {
        if (GetSelectionLength() > 0)
            return ReplaceSelection(string.Empty);
        var line = _lines[_caretLine];
        if (_caretCol < line.Length)
        {
            _lines[_caretLine] = line.Remove(_caretCol, 1);
            Invalidate();
            UpdateTextProperty();
            return true;
        }
        if (MultiLine && _caretLine < _lines.Count - 1)
        {
            // Merge with next line
            _lines[_caretLine] += _lines[_caretLine + 1];
            _lines.RemoveAt(_caretLine + 1);
            EnsureCaretVisible();
            Invalidate();
            UpdateTextProperty();
            return true;
        }
        return false;
    }

    private bool DeleteWordBackward()
    {
        var caret = GetCaretAbsoluteIndex();
        var start = caret;
        while (start > 0 && char.IsWhiteSpace(Text[start - 1]))
            start--;
        while (start > 0 && !char.IsWhiteSpace(Text[start - 1]))
            start--;
        if (start == caret)
            return false;

        SetText(Text.Remove(start, caret - start));
        SetCaretFromAbsolute(start);
        Invalidate();
        return true;
    }

    private bool DeleteWordForward()
    {
        var caret = GetCaretAbsoluteIndex();
        var end = caret;
        while (end < Text.Length && char.IsWhiteSpace(Text[end]))
            end++;
        while (end < Text.Length && !char.IsWhiteSpace(Text[end]))
            end++;
        if (end == caret)
            return false;

        SetText(Text.Remove(caret, end - caret));
        SetCaretFromAbsolute(caret);
        Invalidate();
        return true;
    }

    private bool NewLine()
    {
        if (!_multiLine)
            return false;
        var line = _lines[_caretLine];
        string newLine = (_caretCol < line.Length) ? line.Substring(_caretCol) : string.Empty;
        _lines[_caretLine] = (_caretCol > 0) ? line.Substring(0, _caretCol) : string.Empty;
        _lines.Insert(_caretLine + 1, newLine);
        _caretLine++;
        _caretCol = 0;
        EnsureCaretVisible();
        Invalidate();
        return true;
    }

    private void EnsureCaretVisible()
    {
        if (_caretLine < _firstVisibleLine)
            _firstVisibleLine = _caretLine;
        else if (_caretLine >= _firstVisibleLine + size.Height)
            _firstVisibleLine = _caretLine - size.Height + 1;
        NotifyWidgetStateChanged();
    }

    private void UpdateTextProperty()
    {
        // Avoid triggering SetText recursion; set backing field directly
        var newText = _multiLine ? string.Join("\n", _lines) : _lines[0];
        if (Text != newText)
        {
            base.SetText(newText); // base handles OnChange + Invalidate
            UpdateBoundProperty(newText);
        }
    }

    private void UpdateBoundProperty(string value)
    {
        if (!_suppressModelUpdate && _boundModel != null && _boundPropInfo != null && _boundPropInfo.CanWrite)
        {
            _boundPropInfo.SetValue(_boundModel, value);
        }
    }
}
