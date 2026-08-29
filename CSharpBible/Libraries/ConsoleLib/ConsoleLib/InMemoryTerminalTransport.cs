using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib.Interfaces;

namespace ConsoleLib;

/// <summary>Deterministic transport for tests and render previews.</summary>
public sealed class InMemoryTerminalTransport : ITerminalTransport
{
    private readonly StringBuilder _output = new();

    public InMemoryTerminalTransport(ITerminalCapabilities? capabilities = null)
    {
        Capabilities = capabilities ?? TerminalCapabilitiesDetector.Detect(false, false, _ => "xterm-256color");
    }

    public ITerminalCapabilities Capabilities { get; }
    public bool IsOpen { get; private set; }
    public Size Size { get; private set; } = new(80, 25);
    public string Output => _output.ToString();

    public Task OpenAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IsOpen = true;
        return Task.CompletedTask;
    }

    public Task<string?> ReadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<string?>(null);
    }

    public Task WriteAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (IsOpen && !string.IsNullOrEmpty(text))
            _output.Append(text);
        return Task.CompletedTask;
    }

    public Task ResizeAsync(Size size, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Size = new Size(Math.Max(1, size.Width), Math.Max(1, size.Height));
        return Task.CompletedTask;
    }

    public Task CloseAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IsOpen = false;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync() => new(CloseAsync());
}
