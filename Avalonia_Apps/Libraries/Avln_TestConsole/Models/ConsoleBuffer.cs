using System;
using System.Collections.Generic;
using System.Text;

namespace Avln_TestConsole.Models;

/// <summary>
/// Stores console state and implements console-style buffer behavior.
/// </summary>
public sealed class ConsoleBuffer
{
    private const int DefaultWidth = 80;
    private const int DefaultHeight = 25;
    private const ConsoleColor DefaultForegroundColor = ConsoleColor.Gray;
    private const ConsoleColor DefaultBackgroundColor = ConsoleColor.Black;
    private readonly Queue<ConsoleKeyInfo> _keyBuffer = new();
    private ConsoleCharacterInfo[] _screenBuffer;
    private int _windowWidth;
    private int _windowHeight;
    private int _cursorLeft;
    private int _cursorTop;
    private ConsoleColor _foregroundColor = DefaultForegroundColor;
    private ConsoleColor _backgroundColor = DefaultBackgroundColor;
    private string _title = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleBuffer"/> class.
    /// </summary>
    public ConsoleBuffer()
    {
        _windowWidth = DefaultWidth;
        _windowHeight = DefaultHeight;
        _screenBuffer = CreateBuffer(_windowWidth, _windowHeight);
    }

    /// <summary>
    /// Occurs when the buffer content has changed.
    /// </summary>
    public event EventHandler? BufferChanged;

    /// <summary>
    /// Gets or sets the console title.
    /// </summary>
    public string Title
    {
        get => _title;
        set => _title = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the current foreground color.
    /// </summary>
    public ConsoleColor ForegroundColor
    {
        get => _foregroundColor;
        set => _foregroundColor = value;
    }

    /// <summary>
    /// Gets or sets the current background color.
    /// </summary>
    public ConsoleColor BackgroundColor
    {
        get => _backgroundColor;
        set => _backgroundColor = value;
    }

    /// <summary>
    /// Gets or sets the window width in characters.
    /// </summary>
    public int WindowWidth
    {
        get => _windowWidth;
        set => Resize(Math.Max(1, value), _windowHeight);
    }

    /// <summary>
    /// Gets or sets the window height in characters.
    /// </summary>
    public int WindowHeight
    {
        get => _windowHeight;
        set => Resize(_windowWidth, Math.Max(1, value));
    }

    /// <summary>
    /// Gets a value indicating whether a key is available.
    /// </summary>
    public bool KeyAvailable => _keyBuffer.Count > 0;

    /// <summary>
    /// Gets the internal screen buffer.
    /// </summary>
    public IReadOnlyList<ConsoleCharacterInfo> ScreenBuffer => _screenBuffer;

    /// <summary>
    /// Gets the current cursor position.
    /// </summary>
    public (int Left, int Top) CursorPosition => (_cursorLeft, _cursorTop);

    /// <summary>
    /// Clears the console buffer.
    /// </summary>
    public void Clear()
    {
        _screenBuffer = CreateBuffer(_windowWidth, _windowHeight);
        _cursorLeft = 0;
        _cursorTop = 0;
        RaiseBufferChanged();
    }

    /// <summary>
    /// Sets the current cursor position.
    /// </summary>
    /// <param name="left">The target cursor column.</param>
    /// <param name="top">The target cursor row.</param>
    public void SetCursorPosition(int left, int top)
    {
        _cursorLeft = Math.Clamp(left, 0, _windowWidth - 1);
        _cursorTop = Math.Clamp(top, 0, _windowHeight - 1);
        RaiseBufferChanged();
    }

    /// <summary>
    /// Writes a character to the buffer.
    /// </summary>
    /// <param name="character">The character to write.</param>
    public void Write(char character)
    {
        CheckLineBreak();

        switch (character)
        {
            case '\r':
                _cursorLeft = 0;
                break;
            case '\n':
                _cursorTop++;
                ScrollIfNeeded();
                break;
            case '\t':
                var nextTabStop = (((_cursorLeft + 8) / 8) * 8) - 1;
                for (var index = _cursorLeft; index < nextTabStop; index++)
                {
                    Write(' ');
                }

                WriteVisibleCharacter('\t');
                break;
            case '\b':
                _cursorLeft = Math.Max(_cursorLeft - 1, 0);
                break;
            default:
                WriteVisibleCharacter(character);
                break;
        }

        RaiseBufferChanged();
    }

    /// <summary>
    /// Writes a string to the buffer.
    /// </summary>
    /// <param name="text">The string to write.</param>
    public void Write(string? text)
    {
        foreach (var character in text ?? string.Empty)
        {
            Write(character);
        }
    }

    /// <summary>
    /// Exports the buffer in the encoded content format used by the existing test console.
    /// </summary>
    /// <returns>The encoded console content.</returns>
    public string ExportContent()
    {
        var builder = new StringBuilder();
        var lastForeground = DefaultForegroundColor;
        var lastBackground = DefaultBackgroundColor;

        for (var y = 0; y < _windowHeight; y++)
        {
            for (var x = 0; x < _windowWidth; x++)
            {
                var cell = _screenBuffer[x + (y * _windowWidth)];
                if (cell.ForegroundColor != lastForeground || cell.BackgroundColor != lastBackground)
                {
                    builder.Append($"\\c{(int)cell.BackgroundColor:X}{(int)cell.ForegroundColor:X}");
                    lastForeground = cell.ForegroundColor;
                    lastBackground = cell.BackgroundColor;
                }

                switch (cell.Character)
                {
                    case '\\':
                        builder.Append(@"\\");
                        break;
                    case '\t':
                        TrimTrailingWhitespace(builder);
                        builder.Append("\\t");
                        break;
                    case '\0':
                        if (IsEndOfLineFill(x, y, lastBackground))
                        {
                            x = _windowWidth - 1;
                        }
                        else
                        {
                            builder.Append(@"\x00");
                        }

                        break;
                    default:
                        builder.Append(cell.Character);
                        break;
                }
            }

            if (y < _windowHeight - 1)
            {
                builder.Append("\r\n");
            }
        }

        return builder.ToString().TrimEnd();
    }

    /// <summary>
    /// Enqueues a key for subsequent reads.
    /// </summary>
    /// <param name="keyInfo">The key information to enqueue.</param>
    public void EnqueueKey(ConsoleKeyInfo keyInfo)
    {
        _keyBuffer.Enqueue(keyInfo);
    }

    /// <summary>
    /// Reads the next queued key if present.
    /// </summary>
    /// <returns>The queued key or <c>null</c>.</returns>
    public ConsoleKeyInfo? ReadKey()
    {
        if (_keyBuffer.Count == 0)
        {
            return null;
        }

        return _keyBuffer.Dequeue();
    }

    private static ConsoleCharacterInfo[] CreateBuffer(int width, int height)
    {
        var result = new ConsoleCharacterInfo[width * height];
        for (var index = 0; index < result.Length; index++)
        {
            result[index] = ConsoleCharacterInfo.CreateDefault();
        }

        return result;
    }

    private static void TrimTrailingWhitespace(StringBuilder builder)
    {
        while (builder.Length > 0 && char.IsWhiteSpace(builder[builder.Length - 1]) && builder[^1] != '\r' && builder[^1] != '\n')
        {
            builder.Length--;
        }
    }

    private void Resize(int width, int height)
    {
        var oldBuffer = _screenBuffer;
        var oldWidth = _windowWidth;
        var oldHeight = _windowHeight;

        _windowWidth = width;
        _windowHeight = height;
        _screenBuffer = CreateBuffer(width, height);

        var copyWidth = Math.Min(oldWidth, width);
        var copyHeight = Math.Min(oldHeight, height);
        for (var y = 0; y < copyHeight; y++)
        {
            Array.Copy(oldBuffer, y * oldWidth, _screenBuffer, y * width, copyWidth);
        }

        _cursorLeft = Math.Clamp(_cursorLeft, 0, _windowWidth - 1);
        _cursorTop = Math.Clamp(_cursorTop, 0, _windowHeight - 1);
        RaiseBufferChanged();
    }

    private void CheckLineBreak()
    {
        if (_cursorLeft < _windowWidth)
        {
            return;
        }

        _cursorLeft = 0;
        _cursorTop++;
        ScrollIfNeeded();
    }

    private void ScrollIfNeeded()
    {
        if (_cursorTop < _windowHeight)
        {
            return;
        }

        for (var index = 0; index < _screenBuffer.Length; index++)
        {
            _screenBuffer[index] = index < _screenBuffer.Length - _windowWidth
                ? _screenBuffer[index + _windowWidth]
                : ConsoleCharacterInfo.CreateDefault();
        }

        _cursorTop = _windowHeight - 1;
    }

    private bool IsEndOfLineFill(int x, int y, ConsoleColor currentBackground)
    {
        for (var xx = x + 1; xx < _windowWidth; xx++)
        {
            var cell = _screenBuffer[xx + (y * _windowWidth)];
            if (cell.Character != '\0' || cell.BackgroundColor != currentBackground)
            {
                return false;
            }
        }

        return true;
    }

    private void WriteVisibleCharacter(char character)
    {
        var bufferIndex = _cursorLeft + (_cursorTop * _windowWidth);
        _screenBuffer[bufferIndex] = new ConsoleCharacterInfo
        {
            Character = character,
            ForegroundColor = _foregroundColor,
            BackgroundColor = _backgroundColor,
        };

        _cursorLeft++;
    }

    private void RaiseBufferChanged()
        => BufferChanged?.Invoke(this, EventArgs.Empty);
}
