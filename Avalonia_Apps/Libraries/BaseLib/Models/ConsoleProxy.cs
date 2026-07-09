using BaseLib.Interfaces;
using System;

namespace BaseLib.Models;

public class ConsoleProxy : IConsole
{
    private const string HideCursorSequence = "\u001b[?25l";
    private const string ShowCursorSequence = "\u001b[?25h";
    private const string ResetColorSequence = "\u001b[0m";
    private const string ClearScreenSequence = "\u001b[2J\u001b[H";
    private const string BellSequence = "\a";

    private ConsoleColor _backgroundColor = ConsoleColor.Black;
    private (int Left, int Top) _cursorPosition;
    private bool _cursorVisible;
    private ConsoleColor _foregroundColor = ConsoleColor.White;
    private string _title = string.Empty;
    private int _windowHeight;
    private int _windowLeft;
    private int _windowTop;
    private int _windowWidth;

    public ConsoleColor ForegroundColor
    {
        get => GetValue(() => Console.ForegroundColor, _foregroundColor);
        set
        {
            _foregroundColor = value;
            TryInvoke(() => Console.ForegroundColor = value);
        }
    }

    public ConsoleColor BackgroundColor
    {
        get => GetValue(() => Console.BackgroundColor, _backgroundColor);
        set
        {
            _backgroundColor = value;
            TryInvoke(() => Console.BackgroundColor = value);
        }
    }

    public bool IsOutputRedirected => GetValue(() => Console.IsOutputRedirected, false);

    public bool KeyAvailable => GetValue(() => Console.KeyAvailable, false);

    public int LargestWindowHeight => GetValue(() => Console.LargestWindowHeight, 0);

    public int LargestWindowWidth => GetValue(() => Console.LargestWindowWidth, 0);

    public string Title
    {
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
        get => GetValue(() => Console.Title, _title);
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
        set
        {
            _title = value ?? string.Empty;
            TryInvoke(() => Console.Title = value ?? string.Empty);
        }
    }

    public int WindowHeight
    {
        get => GetValue(() => Console.WindowHeight, _windowHeight);
        set
        {
            _windowHeight = value;
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
            TryInvoke(() => Console.WindowHeight = value);
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
        }
    }

    public int WindowWidth
    {
        get => GetValue(() => Console.WindowWidth, _windowWidth);
        set
        {
            _windowWidth = value;
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
            TryInvoke(() => Console.WindowWidth = value);
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
        }
    }

    public int WindowLeft
    {
        get => GetValue(() => Console.WindowLeft, _windowLeft);
        set
        {
            _windowLeft = value;
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
            TryInvoke(() => Console.WindowLeft = value);
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
        }
    }

    public int WindowTop
    {
        get => GetValue(() => Console.WindowTop, _windowTop);
        set
        {
            _windowTop = value;
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
            TryInvoke(() => Console.WindowTop = value);
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
        }
    }

    public bool CursorVisible
    {
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
        get => GetValue(() => Console.CursorVisible, _cursorVisible);
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
        set
        {
            _cursorVisible = value;
            if (!TryInvoke(() => Console.CursorVisible = value))
            {
                WriteAnsi(value ? ShowCursorSequence : HideCursorSequence);
            }
        }
    }

    public int BufferWidth => GetValue(() => Console.BufferWidth, 0);

    public int BufferHeight => GetValue(() => Console.BufferHeight, 0);

    public void Beep(int freq, int len)
    {
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
        if (!TryInvoke(() => Console.Beep(freq, len)))
        {
            WriteAnsi(BellSequence);
        }
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
    }

    public void Clear()
    {
        if (!TryInvoke(Console.Clear))
        {
            WriteAnsi(ClearScreenSequence);
        }
    }

    public (int Left, int Top) GetCursorPosition()
    {
#if NET5_0_OR_GREATER
        return GetValue(Console.GetCursorPosition, _cursorPosition);
#else
        try
        {
            _cursorPosition = (Console.CursorLeft, Console.CursorTop);
            return _cursorPosition;
        }
        catch
        {
            return _cursorPosition;
        }
#endif
    }

    public ConsoleKeyInfo? ReadKey()
    {
        try
        {
            return Console.ReadKey();
        }
        catch
        {
            return null;
        }
    }

    public string ReadLine()
    {
        try
        {
            return Console.ReadLine() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public void ResetColor()
    {
        if (!TryInvoke(Console.ResetColor))
        {
            WriteAnsi(ResetColorSequence);
        }
    }

    public void SetCursorPosition(int left, int top)
    {
        _cursorPosition = (left, top);
        if (!TryInvoke(() => Console.SetCursorPosition(left, top)))
        {
            WriteAnsi($"\u001b[{top + 1};{left + 1}H");
        }
    }

    public void SetWindowPosition(int left, int top)
    {
        _windowLeft = left;
        _windowTop = top;
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
        TryInvoke(() => Console.SetWindowPosition(left, top));
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
    }

    public void SetWindowSize(int width, int height)
    {
        _windowWidth = width;
        _windowHeight = height;
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
        TryInvoke(() => Console.SetWindowSize(width, height));
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
    }

    public void Write(char ch) => TryInvoke(() => Console.Write(ch));

    public void Write(string? st) => TryInvoke(() => Console.Write(st));

    public void WriteLine(string? st = "") => TryInvoke(() => Console.WriteLine(st));

    private static T GetValue<T>(Func<T> getter, T fallback)
    {
        try
        {
            return getter();
        }
        catch
        {
            return fallback;
        }
    }

    private static bool TryInvoke(Action action)
    {
        try
        {
            action();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void WriteAnsi(string sequence)
    {
        if (string.IsNullOrWhiteSpace(sequence) || Console.IsOutputRedirected)
        {
            return;
        }

        try
        {
            Console.Write(sequence);
        }
        catch
        {
        }
    }
}
