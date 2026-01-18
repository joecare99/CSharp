using BaseLib.Interfaces;
using RemoteTerminal.Ansi;
using RemoteTerminal.Net;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace RemoteTerminal.Remote;

/// <summary>
/// Implements <see cref="IConsole"/> on top of a remote ANSI-compatible terminal.
/// </summary>
public sealed class RemoteAnsiConsole : IConsole
{
    private readonly Stream _stream;
    private readonly AnsiWriter _ansi;
    private readonly AnsiKeyReader _keys;

    private readonly Action<string>? _log;

    private ConsoleColor _fg = ConsoleColor.Gray;
    private ConsoleColor _bg = ConsoleColor.Black;
    private bool _cursorVisible;

    private readonly int _bufferWidth;
    private readonly int _bufferHeight;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteAnsiConsole"/> class.
    /// </summary>
    /// <param name="stream">The underlying bidirectional stream.</param>
    /// <param name="bufferWidth">The assumed buffer width in columns.</param>
    /// <param name="bufferHeight">The assumed buffer height in rows.</param>
    /// <param name="log">Optional log callback.</param>
    public RemoteAnsiConsole(Stream stream, int bufferWidth = 120, int bufferHeight = 40, Action<string>? log = null)
    {
        _stream = stream;
        _ansi = new AnsiWriter(stream);
        _keys = new AnsiKeyReader(stream);
        _bufferWidth = bufferWidth;
        _bufferHeight = bufferHeight;
        _log = log;
       
        try
        {
            // Best-effort: if user connects via PuTTY TELNET, this helps to disable line mode / echo.
            TelnetNegotiation.SendBasicServerNegotiation(stream);
        }
        catch (Exception ex)
        {
            _log?.Invoke($"Telnet negotiation failed: {ex.Message}");
        }

        _ansi.Write("\x1b[0m\x1b[2J\x1b[H\x1b[?25l\x1b[?7l");
        _ansi.Flush();
        _log?.Invoke("RemoteAnsiConsole initialized (ANSI reset/clear + hide cursor)");
    }

    /// <inheritdoc />
    public ConsoleColor ForegroundColor
    {
        get => _fg;
        set
        {
            _fg = value;
            _ansi.SetFg256(MapColor256(value));
        }
    }

    /// <inheritdoc />
    public ConsoleColor BackgroundColor
    {
        get => _bg;
        set
        {
            _bg = value;
            _ansi.SetBg256(MapColor256(value));
        }
    }

    /// <inheritdoc />
    public bool IsOutputRedirected => false;

    /// <inheritdoc />
    public bool KeyAvailable
    {
        get
        {
            try
            {
                // Cooperative scheduling: Visuals uses short Thread.Sleep loops and then polls KeyAvailable.
                // Yielding here avoids starving background I/O / socket processing on some runtimes.
                Thread.Yield();
                _ansi.Flush();
                return _stream is NetworkStream ns && ns.DataAvailable;
            }
            catch
            {
                return true;
            }
        }
    }

    /// <inheritdoc />
    public int LargestWindowHeight => _bufferHeight;

    /// <inheritdoc />
    public string Title
    {
        get => string.Empty;
        set => _ansi.Write($"\x1b]0;{value}\x07");
    }

    /// <inheritdoc />
    public int WindowHeight
    {
        get => _bufferHeight;
        set { }
    }

    /// <inheritdoc />
    public int WindowWidth
    {
        get => _bufferWidth;
        set { }
    }

    /// <inheritdoc />
    public bool CursorVisible
    {
        get => _cursorVisible;
        set
        {
            _cursorVisible = value;
            if (value)
            {
                _ansi.ShowCursor();
            }
            else
            {
                _ansi.HideCursor();
            }
        }
    }

    /// <inheritdoc />
    public int BufferWidth => _bufferWidth;

    /// <inheritdoc />
    public int BufferHeight => _bufferHeight;

    /// <inheritdoc />
    public void Beep(int freq, int len) => _ansi.Write("\x07");

    /// <inheritdoc />
    public void Clear() => _ansi.Clear();

    /// <inheritdoc />
    public (int Left, int Top) GetCursorPosition() => (0, 0);

    /// <inheritdoc />
    public ConsoleKeyInfo? ReadKey()
    {
        try
        {
            // Cooperative scheduling: allow I/O/other work to run before potentially blocking.
            _ansi.Flush();
            Thread.Yield();
            var info = _keys.ReadAsync(CancellationToken.None).GetAwaiter().GetResult();
            return info.Key switch
            {
                AnsiKey.Up => new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false),
                AnsiKey.Down => new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false),
                AnsiKey.Left => new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false),
                AnsiKey.Right => new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false),
                AnsiKey.Enter => new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
                AnsiKey.Escape => new ConsoleKeyInfo('\x1b', ConsoleKey.Escape, false, false, false),
                AnsiKey.Backspace => new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false),
                AnsiKey.Char => new ConsoleKeyInfo(info.Char, MapCharKey(info.Char), false, false, false),
                _ => null,
            };
        }
        catch (Exception ex)
        {
            _log?.Invoke($"ReadKey exception: {ex.GetType().Name}: {ex.Message}");
            return null;
        }
    }

    /// <inheritdoc />
    public string ReadLine()
    {
        var sb = new System.Text.StringBuilder();

        while (true)
        {
            var k = ReadKey();
            if (k is null)
            {
                continue;
            }

            switch (k.Value.Key)
            {
                case ConsoleKey.Enter:
                    WriteLine();
                    return sb.ToString();

                case ConsoleKey.Backspace:
                    if (sb.Length > 0)
                    {
                        sb.Length -= 1;
                        Write("\b \b");
                    }
                    break;

                default:
                    var ch = k.Value.KeyChar;
                    if (ch == '\0' || char.IsControl(ch))
                    {
                        break;
                    }

                    sb.Append(ch);
                    Write(ch);
                    break;
            }
        }
    }

    /// <inheritdoc />
    public void SetCursorPosition(int left, int top) => _ansi.MoveCursor(top + 1, left + 1);

    /// <inheritdoc />
    public void Write(char ch) => _ansi.Write(ch.ToString());

    /// <inheritdoc />
    public void Write(string? st)
    {
        if (st is null)
        {
            return;
        }

        _ansi.Write(st);
    }

    /// <inheritdoc />
    public void WriteLine(string? st = "")
    {
        if (!string.IsNullOrEmpty(st))
        {
            Write(st);
        }

        _ansi.Write("\n");
    }

    private static int MapColor256(ConsoleColor color)
        => color switch
        {
            ConsoleColor.Black => 16,
            ConsoleColor.DarkBlue => 18,
            ConsoleColor.DarkGreen => 22,
            ConsoleColor.DarkCyan => 30,
            ConsoleColor.DarkRed => 88,
            ConsoleColor.DarkMagenta => 90,
            ConsoleColor.DarkYellow => 94,
            ConsoleColor.Gray => 245,
            ConsoleColor.DarkGray => 240,
            ConsoleColor.Blue => 21,
            ConsoleColor.Green => 82,
            ConsoleColor.Cyan => 51,
            ConsoleColor.Red => 196,
            ConsoleColor.Magenta => 201,
            ConsoleColor.Yellow => 226,
            ConsoleColor.White => 15,
            _ => 15
        };

    private static ConsoleKey MapCharKey(char ch)
    {
        if (ch >= 'a' && ch <= 'z')
        {
            return (ConsoleKey)(ConsoleKey.A + (ch - 'a'));
        }

        if (ch >= 'A' && ch <= 'Z')
        {
            return (ConsoleKey)(ConsoleKey.A + (ch - 'A'));
        }

        if (ch >= '0' && ch <= '9')
        {
            return (ConsoleKey)(ConsoleKey.D0 + (ch - '0'));
        }

        return ConsoleKey.NoName;
    }
}
