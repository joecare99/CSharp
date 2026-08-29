using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleLib.Interfaces;

/// <summary>Provider-agnostic character transport used by terminal renderers.</summary>
public interface ITerminalTransport : IAsyncDisposable
{
    ITerminalCapabilities Capabilities { get; }
    bool IsOpen { get; }
    Task OpenAsync(CancellationToken cancellationToken = default);
    Task<string?> ReadAsync(CancellationToken cancellationToken = default);
    Task WriteAsync(string text, CancellationToken cancellationToken = default);
    Task ResizeAsync(Size size, CancellationToken cancellationToken = default);
    Task CloseAsync(CancellationToken cancellationToken = default);
}
