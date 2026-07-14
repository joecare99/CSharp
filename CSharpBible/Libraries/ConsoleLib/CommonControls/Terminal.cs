// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 07-21-2022
// ***********************************************************************
// <copyright file="Terminal.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Interfaces;
using ConsoleLib.Data;
using ConsoleLib.Interfaces;
using System;
using System.Drawing;

namespace ConsoleLib.CommonControls;

/// <summary>
/// Class Terminal.
/// Implements the <see cref="ConsoleLib.Control" />
/// </summary>
/// <seealso cref="ConsoleLib.Control" />
public class Terminal : Control, IConsole
{
    private IConsoleWidgetHost? ConsoleHost => GetWidgetSetCapability<IConsoleWidgetHost>();

    /// <summary>
    /// The boarder
    /// </summary>
    public IBorderDefinition BorderDefinition { get; set; } = new BorderDef { Style = BorderStyle.Single };

    public BorderStyle BorderStyle
    {
        get => BorderDefinition.Style;
        set => BorderDefinition = new BorderDef
        {
            Style = value,
            CustomChars = value == BorderStyle.Custom ? BorderDefinition.CustomChars : null
        };
    }

    public char[] Border
    {
        get => BorderDefinition.CustomChars ?? Array.Empty<char>();
        set => BorderDefinition = new BorderDef
        {
            Style = BorderStyle.Custom,
            CustomChars = value
        };
    }
    /// <summary>
    /// The boarder color
    /// </summary>
    public ConsoleColor BorderColor
    {
        get => BorderDefinition.BorderColor;
        set => BorderDefinition.BorderColor = value;
    }

    protected override void SetSize(Size size)
    {
        base.SetSize(size);
        var _old = ScBuffer;
        ScBuffer = new ScreenCell[size.Width - 2, size.Height - 2];
        for (var y = 0; y < size.Height - 2; y++)
            for (var x = 0; x < size.Width - 2; x++)
            {
                if (_old != null && x < _old.GetLength(0) && y < _old.GetLength(1))
                    ScBuffer[x, y] = _old[x, y];
                else
                    ScBuffer[x, y] = (' ', (ConsoleColor.White, ConsoleColor.Black));
            }
    }

    private ScreenCell[,] ScBuffer = new ScreenCell[0, 0];
    private ConsoleColor _ForegroundColor = ConsoleColor.DarkGray;
    private ConsoleColor _BackgroundColor;
    private Point _cursorPos;
    #region IConsole Implementation
    ConsoleColor IConsole.ForegroundColor { get => _ForegroundColor; set => _ForegroundColor = value; }
    ConsoleColor IConsole.BackgroundColor { get => _BackgroundColor; set => _BackgroundColor = value; }
    public bool IsOutputRedirected => ConsoleHost?.IsOutputRedirected ?? false;
    public bool KeyAvailable => false;  // Todo: Implement keybuffer
    public int LargestWindowHeight => size.Height - 2;
    public string Title { get => Text; set { } }
    public int WindowHeight { get => size.Height - 2; set { } }
    public int WindowWidth { get => size.Width - 2; set { } }

    public bool CursorVisible { get; set; }
    public int BufferWidth => WindowWidth;
    public int BufferHeight => WindowHeight;

    public int LargestWindowWidth => throw new NotImplementedException();

    public int WindowLeft { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int WindowTop { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    #endregion
    /// <summary>
    /// Draws this instance.
    /// </summary>
    public override void Draw()
    {
        WidgetSet?.DrawTerminal(this);
        Valid = true;
    }

    /// <summary>
    /// Res the draw.
    /// </summary>
    /// <param name="dimension">The Dimension.</param>
    public override void ReDraw(Rectangle dimension)
    {
        WidgetSet?.RedrawTerminal(this, dimension);
    }

    public ScreenCell GetScreenCell(int x, int y) => ScBuffer[x, y];

    public void Beep(int freq, int len) => ConsoleHost?.Beep(freq, len);

    public void Clear()
    {
        for (var y = 0; y < size.Height - 2; y++)
            for (var x = 0; x < size.Width - 2; x++)
            {
                ScBuffer[x, y] = (' ', (ConsoleColor.White, ConsoleColor.Black));
                ConsoleHost?.ClearTerminalCell(this, x, y, ScBuffer[x, y].c, ScBuffer[x, y].fc.Fg, ScBuffer[x, y].fc.Bg);
            }
    }

    public (int Left, int Top) GetCursorPosition()
    {
        return (_cursorPos.X, _cursorPos.Y);
    }

    public ConsoleKeyInfo? ReadKey()
    {
        // Todo: Implement KeyBuffer
        return null;
    }

    public string ReadLine()
    {
        return "";
    }

    public void SetCursorPosition(int left, int top)
    {
        (_cursorPos.X, _cursorPos.Y) = (left, top);
    }

    private void _write(char ch, bool xUpdate)
    {
        switch (ch)
        {
            case '\n':
                _cursorPos.X = 0;
                _cursorPos.Y++;
                if (_cursorPos.Y >= size.Height - 2)
                {
                    _cursorPos.Y = size.Height - 3;
                    //Scroll up
                    ScrollUp();
                }
                break;
            case '\r':
                _cursorPos.X = 0;
                break;
            case '\t':
                _cursorPos.X = _cursorPos.X & -4 + 4;
                if (_cursorPos.X >= size.Width - 2)
                {
                    _cursorPos.X = 0;
                    _cursorPos.Y++;
                    if (_cursorPos.Y >= size.Height - 2)
                    {
                        _cursorPos.Y = size.Height - 3;
                    }
                }
                break;
            default:
                if (_cursorPos.X >= size.Width - 2)
                {
                    _cursorPos.X = 0;
                    _cursorPos.Y++;
                    if (_cursorPos.Y >= size.Height - 2)
                    {
                        _cursorPos.Y = size.Height - 3;
                        ScrollUp();
                    }
                }
                ScBuffer[_cursorPos.X, _cursorPos.Y] = (ch, (_ForegroundColor, _BackgroundColor));
                if (xUpdate)
                    ConsoleHost?.WriteTerminalCell(this, _cursorPos.X, _cursorPos.Y, ch, _ForegroundColor, _BackgroundColor);
                _cursorPos.X++;
                break;
        }
        ;
        if (Valid && !xUpdate)
            Invalidate();
    }

    private void ScrollUp()
    {
        for (var y = 0; y < size.Height - 3; y++)
            for (var x = 0; x < size.Width - 2; x++)
            {
                ScBuffer[x, y] = ScBuffer[x, y + 1];
            }
        for (var x = 0; x < size.Width - 2; x++)
        {
            ScBuffer[x, size.Height - 3] = ScreenCell.Blank;
        }
        if (Valid)
            Invalidate();
    }

    public void Write(char ch) => _write(ch, true);
    public void Write(string? st)
    {
        if (st == null)
            return;
        foreach (var ch in st)
        {
            _write(ch, st.Length < size.Width - 2);
        }
    }

    public void WriteLine(string? st = "")
    {
        Write(st + "\r\n");
    }

    public void ResetColor()
    {
        ForeColor = ConsoleColor.Gray;
        BackColor = ConsoleColor.Black;
    }

    public void SetWindowPosition(int left, int top)
    {
        throw new NotImplementedException();
    }

    public void SetWindowSize(int width, int height)
    {
        throw new NotImplementedException();
    }
}

public record struct ScreenCell(char c, FullColor fc)
{
    public static ScreenCell Blank => new ScreenCell(' ', (ConsoleColor.White, ConsoleColor.Black));

    public static implicit operator (char c, FullColor fc)(ScreenCell value)
    {
        return (value.c, value.fc);
    }

    public override string ToString()
    {
        return $"{c} ({fc.Fg}/{fc.Bg})";
    }

    public static implicit operator ScreenCell((char c, FullColor fc) value)
    {
        return new ScreenCell(value.c, value.fc);
    }
}

public record struct FullColor(ConsoleColor Fg, ConsoleColor Bg)
{
    public static implicit operator (ConsoleColor Fg, ConsoleColor Bg)(FullColor value)
    {
        return (value.Fg, value.Bg);
    }

    public static implicit operator FullColor((ConsoleColor Fg, ConsoleColor Bg) value)
    {
        return new FullColor(value.Fg, value.Bg);
    }
}
