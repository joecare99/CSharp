using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using Avln_TestConsole.Controls;
using Avln_TestConsole.Interfaces;
using Avln_TestConsole.Models;
using BaseLib.Interfaces;

namespace Avln_TestConsole;

/// <summary>
/// Provides an Avalonia-based in-memory console implementation for tests and hosted UI scenarios.
/// </summary>
public sealed class AvaloniaTestConsole : IAvaloniaConsole
{
    private readonly ConsoleBuffer _buffer;
    private readonly ConcurrentQueue<string> _scriptedLines = new();
    private bool _cursorVisible = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaTestConsole"/> class.
    /// </summary>
    public AvaloniaTestConsole()
    {
        _buffer = new ConsoleBuffer();
        Control = new AvaloniaConsoleControl
        {
            Buffer = _buffer,
            Focusable = true,
        };
        Write("Testconsole Ver. 1.0\r\n");
    }

    /// <inheritdoc/>
    public AvaloniaConsoleControl Control { get; }

    /// <inheritdoc/>
    public ConsoleColor ForegroundColor
    {
        get => _buffer.ForegroundColor;
        set => _buffer.ForegroundColor = value;
    }

    /// <inheritdoc/>
    public ConsoleColor BackgroundColor
    {
        get => _buffer.BackgroundColor;
        set => _buffer.BackgroundColor = value;
    }

    /// <inheritdoc/>
    public bool IsOutputRedirected => false;

    /// <inheritdoc/>
    public bool KeyAvailable => _buffer.KeyAvailable;

    /// <inheritdoc/>
    public int LargestWindowHeight => WindowHeight;

    /// <inheritdoc/>
    public string Title
    {
        get => _buffer.Title;
        set => _buffer.Title = value;
    }

    /// <inheritdoc/>
    public int WindowHeight
    {
        get => _buffer.WindowHeight;
        set => _buffer.WindowHeight = value;
    }

    /// <inheritdoc/>
    public int WindowWidth
    {
        get => _buffer.WindowWidth;
        set => _buffer.WindowWidth = value;
    }

    /// <inheritdoc/>
    public bool CursorVisible
    {
        get => _cursorVisible;
        set => _cursorVisible = value;
    }

    /// <inheritdoc/>
    public int BufferWidth => WindowWidth;

    /// <inheritdoc/>
    public int BufferHeight => WindowHeight;

    /// <inheritdoc/>
    public string Content => _buffer.ExportContent();

    /// <inheritdoc/>
    public TimeSpan? ReadLineTimeout { get; set; }

    /// <inheritdoc/>
    public void Beep(int freq, int len)
    {
    }

    /// <inheritdoc/>
    public void Clear() => _buffer.Clear();

    /// <inheritdoc/>
    public (int Left, int Top) GetCursorPosition() => _buffer.CursorPosition;

    /// <inheritdoc/>
    public ConsoleKeyInfo? ReadKey() => _buffer.ReadKey();

    /// <inheritdoc/>
    public string ReadLine()
    {
        if (_scriptedLines.TryDequeue(out var scripted))
        {
            Write(scripted);
            WriteLine();
            return scripted;
        }

        var timeout = ReadLineTimeout;
        var start = DateTime.UtcNow;
        var builder = new StringBuilder();

        while (true)
        {
            if (timeout.HasValue && DateTime.UtcNow - start > timeout.Value)
            {
                throw new TimeoutException("ReadLine timed out waiting for input.");
            }

            if (!KeyAvailable)
            {
                Thread.Sleep(10);
                continue;
            }

            var keyInfo = ReadKey();
            if (keyInfo is null)
            {
                continue;
            }

            switch (keyInfo.Value.Key)
            {
                case ConsoleKey.Enter:
                    WriteLine();
                    return builder.ToString();
                case ConsoleKey.Backspace:
                    if (builder.Length > 0)
                    {
                        builder.Length -= 1;
                        Write("\b \b");
                    }

                    break;
                default:
                    var character = keyInfo.Value.KeyChar;
                    if (character == '\0' || char.IsControl(character))
                    {
                        break;
                    }

                    builder.Append(character);
                    Write(character);
                    break;
            }
        }
    }

    /// <inheritdoc/>
    public void ResetColor()
    {
        ForegroundColor = ConsoleColor.Gray;
        BackgroundColor = ConsoleColor.Black;
    }

    /// <inheritdoc/>
    public void SetCursorPosition(int left, int top) => _buffer.SetCursorPosition(left, top);

    /// <inheritdoc/>
    public void Write(char ch) => _buffer.Write(ch);

    /// <inheritdoc/>
    public void Write(string? st) => _buffer.Write(st);

    /// <inheritdoc/>
    public void WriteLine(string? st = "") => Write((st ?? string.Empty) + "\r\n");

    /// <inheritdoc/>
    public void WriteLine(string? text, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
    {
        var previousForegroundColor = ForegroundColor;
        var previousBackgroundColor = BackgroundColor;

        ForegroundColor = foregroundColor;
        BackgroundColor = backgroundColor;
        WriteLine(text);
        ForegroundColor = previousForegroundColor;
        BackgroundColor = previousBackgroundColor;
    }

    /// <inheritdoc/>
    public void EnqueueKey(ConsoleKeyInfo keyInfo) => _buffer.EnqueueKey(keyInfo);

    /// <inheritdoc/>
    public void EnqueueLine(string line) => _scriptedLines.Enqueue(line);
}
