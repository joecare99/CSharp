using System.Net.Sockets;
using System.Text;

namespace SharpHack.Server.Net;

internal enum AnsiKey
{
    None,
    Up,
    Down,
    Left,
    Right,
    Enter,
    Escape,
    Backspace,
    Char,
}

internal readonly record struct AnsiKeyInfo(AnsiKey Key, char Char);

internal sealed class AnsiKeyReader
{
    private readonly Stream _stream;

    public AnsiKeyReader(Stream stream)
    {
        _stream = stream;
    }

    public async Task<AnsiKeyInfo> ReadAsync(CancellationToken ct)
    {
        int b = await ReadByteAsync(ct).ConfigureAwait(false);
        if (b < 0)
        {
            return new AnsiKeyInfo(AnsiKey.None, '\0');
        }

        if (b == '\r')
        {
            // Consume optional LF to avoid the renderer accidentally treating it as another Enter.
            if (_stream is NetworkStream ns)
            {
                await Task.Delay(1, ct).ConfigureAwait(false);
                if (ns.DataAvailable)
                {
                    int peek = await ReadByteAsync(ct).ConfigureAwait(false);
                    if (peek != '\n')
                    {
                        // Not LF; we cannot unread. Treat as normal char by mapping it to None.
                    }
                }
            }

            return new AnsiKeyInfo(AnsiKey.Enter, '\n');
        }

        if (b == '\n')
        {
            return new AnsiKeyInfo(AnsiKey.Enter, '\n');
        }

        if (b == 0x7F || b == '\b')
        {
            return new AnsiKeyInfo(AnsiKey.Backspace, '\b');
        }

        if (b == 0x1B)
        {
            int b2 = await ReadByteAsync(ct).ConfigureAwait(false);
            if (b2 < 0)
            {
                return new AnsiKeyInfo(AnsiKey.Escape, '\0');
            }

            if (b2 == '[')
            {
                int b3 = await ReadByteAsync(ct).ConfigureAwait(false);
                return b3 switch
                {
                    'A' => new AnsiKeyInfo(AnsiKey.Up, '\0'),
                    'B' => new AnsiKeyInfo(AnsiKey.Down, '\0'),
                    'C' => new AnsiKeyInfo(AnsiKey.Right, '\0'),
                    'D' => new AnsiKeyInfo(AnsiKey.Left, '\0'),
                    _ => new AnsiKeyInfo(AnsiKey.Escape, '\0'),
                };
            }

            // Some clients use SS3 (ESC O) for cursor keys.
            if (b2 == 'O')
            {
                int b3 = await ReadByteAsync(ct).ConfigureAwait(false);
                return b3 switch
                {
                    'A' => new AnsiKeyInfo(AnsiKey.Up, '\0'),
                    'B' => new AnsiKeyInfo(AnsiKey.Down, '\0'),
                    'C' => new AnsiKeyInfo(AnsiKey.Right, '\0'),
                    'D' => new AnsiKeyInfo(AnsiKey.Left, '\0'),
                    _ => new AnsiKeyInfo(AnsiKey.Escape, '\0'),
                };
            }

            return new AnsiKeyInfo(AnsiKey.Escape, '\0');
        }

        char ch = (char)b;
        if (char.IsControl(ch))
        {
            return new AnsiKeyInfo(AnsiKey.None, '\0');
        }

        return new AnsiKeyInfo(AnsiKey.Char, ch);
    }

    private async Task<int> ReadByteAsync(CancellationToken ct)
    {
        var buf = new byte[1];
        int read = await _stream.ReadAsync(buf, 0, 1, ct).ConfigureAwait(false);
        return read == 0 ? -1 : buf[0];
    }
}
