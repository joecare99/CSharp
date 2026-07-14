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
        try
        {
            var val = _boundPropInfo.GetValue(_boundModel);
            var str = val?.ToString() ?? string.Empty;
            if (!string.Equals(str, Text, StringComparison.Ordinal))
            {
                _suppressModelUpdate = true;
                SetText(str);
                _suppressModelUpdate = false;
            }
        }
        catch { /* ignore */ }
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
    public bool ShouldShowCaret() => _showCaret;
    public string GetDisplayLine(int index) => GetLineForDisplay(index);
    public void ApplyNativeText(string value)
    {
        _suppressModelUpdate = true;
        SetText(value);
        _suppressModelUpdate = false;
        UpdateTextProperty();
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

        // Navigation keys (check usKeyCode)
        if (e.bKeyDown && e.KeyChar == '\0') // typical for non-char keys
        {
            bool navHandled = HandleNavigationKey(e.usKeyCode);
            if (navHandled)
            {
                e.Handled = true;
                return; // swallow
            }
        }

        // Basic editing
        bool handled = false;
        char ch = e.KeyChar;
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
                else
                {
                    handled = false; // allow propagate maybe as click
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

    private bool HandleNavigationKey(ushort keyCode)
    {
        var keyMap = GetWidgetSetCapability<IHasConsoleKeyMap>() ?? DefaultConsoleKeyMap.Instance;

        switch (keyCode)
        {
            case var _ when keyCode == keyMap.KeyLeft:
                return CaretLeft();
            case var _ when keyCode == keyMap.KeyRight:
                return CaretRight();
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
            // push to model if bound (two-way)
            if (!_suppressModelUpdate && _boundModel != null && _boundPropInfo != null && _boundPropInfo.CanWrite)
            {
                try
                { _boundPropInfo.SetValue(_boundModel, newText); }
                catch { /* ignore */ }
            }
        }
    }
}
