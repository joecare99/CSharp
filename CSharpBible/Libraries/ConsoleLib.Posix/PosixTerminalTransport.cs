using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Posix;

/// <summary>Stream-backed transport for POSIX terminals and redirected processes.</summary>
public sealed class PosixTerminalTransport : ITerminalTransport
{
    private readonly Stream _input;
    private readonly Stream _output;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private readonly IPosixTerminalMode _terminalMode;
    private bool _rawModeEntered;

    public PosixTerminalTransport(Stream input, Stream output, ITerminalCapabilities? capabilities = null, IPosixTerminalMode? terminalMode = null)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _output = output ?? throw new ArgumentNullException(nameof(output));
        _reader = new StreamReader(_input, System.Text.Encoding.UTF8, true, 1024, true);
        _writer = new StreamWriter(_output, new System.Text.UTF8Encoding(false), 1024, true) { AutoFlush = true };
        Capabilities = capabilities ?? TerminalCapabilitiesDetector.Detect(false, false);
        _terminalMode = terminalMode ?? NoOpPosixTerminalMode.Instance;
    }

    public ITerminalCapabilities Capabilities { get; }
    public bool IsOpen { get; private set; }
    public Size Size { get; private set; } = new(80, 25);

    public async Task OpenAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (IsOpen)
            return;
        await _terminalMode.EnterRawModeAsync(cancellationToken).ConfigureAwait(false);
        _rawModeEntered = true;
        IsOpen = true;
    }

    public async Task<string?> ReadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!IsOpen) return null;
        var buffer = new char[1024];
        var count = await _reader.ReadAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false);
        return count == 0 ? null : new string(buffer, 0, count);
    }

    public async Task WriteAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (IsOpen && !string.IsNullOrEmpty(text))
            await _writer.WriteAsync(text.AsMemory(), cancellationToken).ConfigureAwait(false);
    }

    public Task ResizeAsync(Size size, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Size = new Size(Math.Max(1, size.Width), Math.Max(1, size.Height));
        return Task.CompletedTask;
    }

    public async Task CloseAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_rawModeEntered)
        {
            await _terminalMode.RestoreAsync(cancellationToken).ConfigureAwait(false);
            _rawModeEntered = false;
        }
        IsOpen = false;
    }

    public async ValueTask DisposeAsync()
    {
        await CloseAsync().ConfigureAwait(false);
        _writer.Dispose();
        _reader.Dispose();
    }
}
