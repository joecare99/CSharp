using System.Text;

namespace SharpHack.Server.Ansi;

internal sealed class AnsiWriter
{
    private readonly StreamWriter _writer;

    public AnsiWriter(Stream stream)
    {
        _writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
        {
            AutoFlush = true,
            NewLine = "\n"
        };
    }

    public void Write(string value) => _writer.Write(value);

    public void WriteLine(string? value = "") => _writer.WriteLine(value ?? string.Empty);

    public void Clear() => _writer.Write("\x1b[2J\x1b[H");

    public void HideCursor() => _writer.Write("\x1b[?25l");

    public void ShowCursor() => _writer.Write("\x1b[?25h");

    public void MoveCursor(int row1Based, int col1Based) => _writer.Write($"\x1b[{row1Based};{col1Based}H");

    public void Reset() => _writer.Write("\x1b[0m");

    public void SetFg256(int color) => _writer.Write($"\x1b[38;5;{color}m");

    public void SetBg256(int color) => _writer.Write($"\x1b[48;5;{color}m");

    public void Flush() => _writer.Flush();
}
