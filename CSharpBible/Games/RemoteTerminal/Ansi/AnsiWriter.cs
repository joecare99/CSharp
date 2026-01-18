using System.IO;
using System.Text;

namespace RemoteTerminal.Ansi;

/// <summary>
/// Writes ANSI escape sequences to a stream.
/// </summary>
public sealed class AnsiWriter
{
    private readonly StreamWriter _writer;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnsiWriter"/> class.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    public AnsiWriter(Stream stream)
    {
        _writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), bufferSize: 1024, leaveOpen: true)
        {
            NewLine = "\n",
            AutoFlush = false,
        };
    }

    /// <summary>
    /// Writes a string to the underlying stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void Write(string value) => _writer.Write(value);

    /// <summary>
    /// Flushes the underlying writer.
    /// </summary>
    public void Flush() => _writer.Flush();

    /// <summary>
    /// Clears the screen.
    /// </summary>
    public void Clear() => Write("\x1b[2J\x1b[H");

    /// <summary>
    /// Shows the cursor.
    /// </summary>
    public void ShowCursor() => Write("\x1b[?25h");

    /// <summary>
    /// Hides the cursor.
    /// </summary>
    public void HideCursor() => Write("\x1b[?25l");

    /// <summary>
    /// Moves the cursor.
    /// </summary>
    /// <param name="row">The 1-based row.</param>
    /// <param name="col">The 1-based column.</param>
    public void MoveCursor(int row, int col) => Write($"\x1b[{row};{col}H");

    /// <summary>
    /// Sets 256-color foreground.
    /// </summary>
    /// <param name="color">The 0-255 color index.</param>
    public void SetFg256(int color) => Write($"\x1b[38;5;{color}m");

    /// <summary>
    /// Sets 256-color background.
    /// </summary>
    /// <param name="color">The 0-255 color index.</param>
    public void SetBg256(int color) => Write($"\x1b[48;5;{color}m");
}
