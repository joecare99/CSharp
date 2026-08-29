using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Posix;

/// <summary>Coordinates a POSIX transport with ANSI output and VT input decoders.</summary>
public sealed class PosixTerminalHost : IAsyncDisposable
{
    private readonly ITerminalTransport _transport;
    private readonly AnsiOutputWriter _output;
    private readonly VtInputDecoder _keyboard = new();
    private readonly SgrMouseParser _mouse = new();
    private string _pendingMouseInput = string.Empty;
    private CancellationTokenSource? _runCancellation;

    public PosixTerminalHost(ITerminalTransport transport)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        _output = new AnsiOutputWriter(transport);
    }

    public bool IsRunning => _transport.IsOpen;
    public event EventHandler<KeyInput>? KeyInputReceived;
    public event EventHandler<PointerInput>? PointerInputReceived;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _transport.OpenAsync(cancellationToken).ConfigureAwait(false);
        if (_transport.Capabilities.SupportsMouse)
            await _output.EnableMouseTrackingAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await StartAsync(cancellationToken).ConfigureAwait(false);
        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _runCancellation = linkedCancellation;
        try
        {
            while (!linkedCancellation.IsCancellationRequested && IsRunning)
            {
                var input = await _transport.ReadAsync(linkedCancellation.Token).ConfigureAwait(false);
                if (input is null)
                    break;
                await ProcessInputAsync(input, linkedCancellation.Token).ConfigureAwait(false);
            }
        }
        finally
        {
            _runCancellation = null;
            await StopAsync().ConfigureAwait(false);
        }
    }

    public async Task ProcessInputAsync(string input, CancellationToken cancellationToken = default)
    {
        if (!IsRunning)
            throw new InvalidOperationException("The POSIX terminal host is not running.");
        cancellationToken.ThrowIfCancellationRequested();
        foreach (var pointer in _mouse.Decode(input))
            PointerInputReceived?.Invoke(this, pointer);
        foreach (var key in _keyboard.Decode(RemoveMouseSequences(input)))
            KeyInputReceived?.Invoke(this, key);
        await Task.CompletedTask.ConfigureAwait(false);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _runCancellation?.Cancel();
        if (!IsRunning)
            return;
        if (_transport.Capabilities.SupportsMouse)
            await _output.DisableMouseTrackingAsync(cancellationToken).ConfigureAwait(false);
        await _transport.CloseAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync() => await StopAsync().ConfigureAwait(false);

    private string RemoveMouseSequences(string input)
    {
        const string prefix = "\u001b[<";
        var combined = _pendingMouseInput + input;
        var result = new System.Text.StringBuilder(combined.Length);
        var index = 0;
        _pendingMouseInput = string.Empty;
        while (index < combined.Length)
        {
            var start = combined.IndexOf(prefix, index, StringComparison.Ordinal);
            if (start < 0)
            {
                result.Append(combined, index, combined.Length - index);
                break;
            }

            result.Append(combined, index, start - index);
            var terminator = combined.IndexOfAny(new[] { 'M', 'm' }, start + prefix.Length);
            if (terminator < 0)
            {
                _pendingMouseInput = combined[start..];
                break;
            }
            index = terminator + 1;
        }
        return result.ToString();
    }
}
