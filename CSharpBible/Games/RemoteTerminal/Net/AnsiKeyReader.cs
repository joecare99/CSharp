using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteTerminal.Net;

/// <summary>
/// Reads keys from an ANSI terminal stream.
/// </summary>
public sealed class AnsiKeyReader
{
    private readonly Stream _stream;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiKeyReader"/> class.
    /// </summary>
    /// <param name="stream">The underlying stream.</param>
    public AnsiKeyReader(Stream stream)
    {
        _stream = stream;
    }

    /// <summary>
    /// Reads a key asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The read key info.</returns>
    public async Task<AnsiKeyInfo> ReadAsync(CancellationToken cancellationToken)
    {
        var b = new byte[1];

        while (true)
        {
            int read = await _stream.ReadAsync(b, 0, 1, cancellationToken).ConfigureAwait(false);
            if (read == 0)
            {
                return new AnsiKeyInfo(AnsiKey.None, '\0');
            }

            char ch = (char)b[0];

            if (ch != '\x1b')
            {
                if (ch == '\n' || ch == '\r')
                {
                    return new AnsiKeyInfo(AnsiKey.Enter, '\n');
                }

                if (ch == '\b' || ch == '\x7f')
                {
                    return new AnsiKeyInfo(AnsiKey.Backspace, '\b');
                }

                return new AnsiKeyInfo(AnsiKey.Char, ch);
            }

            // ESC sequence. Try parse CSI arrows. Typical: ESC [ A/B/C/D
            int nextRead = await _stream.ReadAsync(b, 0, 1, cancellationToken).ConfigureAwait(false);
            if (nextRead == 0)
            {
                return new AnsiKeyInfo(AnsiKey.Escape, '\x1b');
            }

            char ch2 = (char)b[0];
            if (ch2 != '[')
            {
                return new AnsiKeyInfo(AnsiKey.Escape, '\x1b');
            }

            int thirdRead = await _stream.ReadAsync(b, 0, 1, cancellationToken).ConfigureAwait(false);
            if (thirdRead == 0)
            {
                return new AnsiKeyInfo(AnsiKey.Escape, '\x1b');
            }

            return ((char)b[0]) switch
            {
                'A' => new AnsiKeyInfo(AnsiKey.Up, '\0'),
                'B' => new AnsiKeyInfo(AnsiKey.Down, '\0'),
                'C' => new AnsiKeyInfo(AnsiKey.Right, '\0'),
                'D' => new AnsiKeyInfo(AnsiKey.Left, '\0'),
                _ => new AnsiKeyInfo(AnsiKey.Escape, '\x1b'),
            };
        }
    }
}

/// <summary>
/// Supported ANSI keys.
/// </summary>
public enum AnsiKey
{
    None = 0,
    Up,
    Down,
    Left,
    Right,
    Enter,
    Escape,
    Backspace,
    Char,
}

/// <summary>
/// Represents one read key.
/// </summary>
public readonly struct AnsiKeyInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiKeyInfo"/> struct.
    /// </summary>
    /// <param name="key">The key kind.</param>
    /// <param name="ch">The read character (if any).</param>
    public AnsiKeyInfo(AnsiKey key, char ch)
    {
        Key = key;
        Char = ch;
    }

    /// <summary>
    /// Gets the key kind.
    /// </summary>
    public AnsiKey Key { get; }

    /// <summary>
    /// Gets the character.
    /// </summary>
    public char Char { get; }
}
