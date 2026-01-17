using BaseLib.Interfaces;
using SharpHack.Server.Ansi;
using SharpHack.Server.Net;
using System.Net.Sockets;

namespace SharpHack.Server.Remote;

internal sealed class RemoteAnsiConsole : IConsole
{
    private readonly Stream _stream;
    private readonly AnsiWriter _ansi;
    private readonly AnsiKeyReader _keys;

    private readonly Action<string>? _log;

    private ConsoleColor _fg = ConsoleColor.Gray;
    private ConsoleColor _bg = ConsoleColor.Black;
    private bool _cursorVisible;

    // We can't reliably know the remote terminal size in RAW mode.
    // Use a sane default and allow server-side configuration later.
    private readonly int _bufferWidth;
    private readonly int _bufferHeight;

    public RemoteAnsiConsole(Stream stream, int bufferWidth = 120, int bufferHeight = 40, Action<string>? log = null)
    {
        _stream = stream;
        _ansi = new AnsiWriter(stream);
        _keys = new AnsiKeyReader(stream);
        _bufferWidth = bufferWidth;
        _bufferHeight = bufferHeight;
        _log = log;

        // Basic ANSI handshake / reset.
        // - reset attributes
        // - clear screen
        // - hide cursor initially
        // - disable line wrap (some terminals)
        _ansi.Write("\x1b[0m\x1b[2J\x1b[H\x1b[?25l\x1b[?7l");
        _ansi.Flush();
        _log?.Invoke("RemoteAnsiConsole initialized (ANSI reset/clear + hide cursor)");
    }

    public ConsoleColor ForegroundColor
    {
        get => _fg;
        set
        {
            _fg = value;
            _ansi.SetFg256(MapColor256(value));
        }
    }

    public ConsoleColor BackgroundColor
    {
        get => _bg;
        set
        {
            _bg = value;
            _ansi.SetBg256(MapColor256(value));
        }
    }

    public bool IsOutputRedirected => false;

    public bool KeyAvailable
    {
        get
        {
            try
            {
                return _stream is NetworkStream ns ? ns.DataAvailable : true;
            }
            catch
            {
                return true;
            }
        }
    }

    public int LargestWindowHeight => _bufferHeight;

    public string Title
    {
        get => string.Empty;
        set
        {
            // OSC title: ESC ] 0 ; title BEL
            _ansi.Write($"\x1b]0;{value}\x07");
        }
    }

    public int WindowHeight
    {
        get => _bufferHeight;
        set { }
    }

    public int WindowWidth
    {
        get => _bufferWidth;
        set { }
    }

    public bool CursorVisible
    {
        get => _cursorVisible;
        set
        {
            _cursorVisible = value;
            if (value) _ansi.ShowCursor(); else _ansi.HideCursor();
        }
    }

    public int BufferWidth => _bufferWidth;

    public int BufferHeight => _bufferHeight;

    public void Beep(int freq, int len)
    {
        // BEL
        _ansi.Write("\x07");
    }

    public void Clear() => _ansi.Clear();

    public (int Left, int Top) GetCursorPosition()
    {
        // Not tracked. Return 0,0.
        return (0, 0);
    }

    public ConsoleKeyInfo? ReadKey()
    {
        try
        {
            var info = _keys.ReadAsync(CancellationToken.None).GetAwaiter().GetResult();
            _log?.Invoke($"ReadKey: {info.Key} '{(info.Char == '\0' ? "\\0" : info.Char.ToString())}'");

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

    public string ReadLine()
    {
        var sb = new System.Text.StringBuilder();

        while (true)
        {
            var k = ReadKey();
            if (k == null)
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

    public void SetCursorPosition(int left, int top)
    {
        // Console coordinates are 0-based; ANSI uses 1-based.
        _ansi.MoveCursor(top + 1, left + 1);
    }

    public void Write(char ch)
    {
        // Preserve \n semantics: keep it as \n (StreamWriter.NewLine is \n).
        _ansi.Write(ch.ToString());
    }

    public void Write(string? st)
    {
        if (st == null)
        {
            return;
        }

        _ansi.Write(st);
    }

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
