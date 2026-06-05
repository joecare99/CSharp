// ***********************************************************************
// Assembly         : ConsoleLib.ExtCon
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : AI Assistant
// Last Modified On : 06-05-2026
// ***********************************************************************
using BaseLib.Interfaces;
using BaseLib.Models;
using ConsoleLib.Interfaces;
using System;
using System.Drawing;

namespace ConsoleLib.ExtCon;

/// <summary>
/// Provides the concrete ExtendedConsole-backed console state and drawing helpers for the extracted console widget set.
/// </summary>
public static class ConsoleFramework
{
    public static readonly char[] chars = { '█', '▓', '▒', '░', ' ' };
    public static readonly char[] singleBorder = { '─', '│', '┌', '┐', '└', '┘', '├', '┤', '┬', '┴', '┼' };
    public static readonly char[] doubleBorder = { '═', '║', '╔', '╗', '╚', '╝', '╠', '╣', '╦', '╩', '╬' };
    public static readonly char[] simpleBorder = { '-', '|', ',', ',', '\'', '\'', '+', '+', '+', '+', '+' };

    public const ushort VK_ENTER = (ushort)ConsoleKey.Enter;
    public const ushort VK_ESC = (ushort)ConsoleKey.Escape;
    public const ushort VK_TAB = (ushort)ConsoleKey.Tab;
    public const ushort VK_LEFT = 0x25;
    public const ushort VK_UP = 0x26;
    public const ushort VK_RIGHT = 0x27;
    public const ushort VK_DOWN = 0x28;
    public const ushort VK_HOME = 0x24;
    public const ushort VK_END = 0x23;
    public const ushort VK_DELETE = 0x2E;
    public const ushort VK_PRIOR = 0x21;
    public const ushort VK_NEXT = 0x22;

    public static Point MousePos { get; private set; }

    public static IConsole console { get; set; } = new ConsoleProxy();

    public static IExtendedConsole? ExtendedConsole
    {
        get => extendedConsole;
        set
        {
            if (extendedConsole != null)
            {
                extendedConsole.MouseEvent -= OnMouseEvent;
                extendedConsole.WindowBufferSizeEvent -= OnWindowSizeEvent;
            }

            extendedConsole = value;
            if (extendedConsole != null)
            {
                extendedConsole.MouseEvent += OnMouseEvent;
                extendedConsole.WindowBufferSizeEvent += OnWindowSizeEvent;
            }
        }
    }

    public static TextCanvas Canvas => _canvas ??= new TextCanvas(console, new Rectangle(0, 0, console.WindowWidth, Math.Min(50, console.LargestWindowHeight)));

    private static IExtendedConsole? extendedConsole;
    private static TextCanvas? _canvas;

    private static void OnWindowSizeEvent(object? sender, Point e)
    {
        Canvas.SetDimension(e.X, e.Y);
    }

    private static void OnMouseEvent(object? sender, IMouseEvent e)
    {
        MousePos = e.MousePos;
    }

    public static void SetPixel(int x, int y, ConsoleColor color)
    {
        console.SetCursorPosition(x, y);
        console.BackgroundColor = color;
        console.Write(" ");
        console.BackgroundColor = ConsoleColor.Black;
    }
}
