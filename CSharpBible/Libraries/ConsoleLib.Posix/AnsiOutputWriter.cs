using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Posix;

/// <summary>ANSI escape sequence writer backed by an abstract terminal transport.</summary>
public sealed class AnsiOutputWriter : IAnsiOutput
{
    private readonly ITerminalTransport _transport;

    public AnsiOutputWriter(ITerminalTransport transport) =>
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));

    public Task WriteAsync(string text, CancellationToken cancellationToken = default) =>
        _transport.WriteAsync(text, cancellationToken);

    public Task ClearAsync(CancellationToken cancellationToken = default) =>
        WriteAsync("\u001b[2J\u001b[H", cancellationToken);

    public Task MoveCursorAsync(int column, int row, CancellationToken cancellationToken = default) =>
        WriteAsync($"\u001b[{Math.Max(1, row)};{Math.Max(1, column)}H", cancellationToken);

    public Task SetForegroundAsync(ConsoleColor color, CancellationToken cancellationToken = default) =>
        WriteAsync($"\u001b[{AnsiColor(color, false)}m", cancellationToken);

    public Task SetBackgroundAsync(ConsoleColor color, CancellationToken cancellationToken = default) =>
        WriteAsync($"\u001b[{AnsiColor(color, true)}m", cancellationToken);

    public Task ResetAsync(CancellationToken cancellationToken = default) =>
        WriteAsync("\u001b[0m", cancellationToken);

    public Task EnableMouseTrackingAsync(CancellationToken cancellationToken = default) =>
        WriteAsync("\u001b[?1000h\u001b[?1006h", cancellationToken);

    public Task DisableMouseTrackingAsync(CancellationToken cancellationToken = default) =>
        WriteAsync("\u001b[?1006l\u001b[?1000l", cancellationToken);

    private static int AnsiColor(ConsoleColor color, bool background)
    {
        var value = (int)color;
        var bright = value >= 8;
        var baseColor = value % 8;
        return (background ? 40 : 30) + baseColor + (bright ? 60 : 0);
    }
}
