using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Posix;

/// <summary>Capability- and policy-gated OSC-52 clipboard integration.</summary>
public sealed class Osc52ClipboardService : IClipboardService
{
    private readonly ITerminalTransport _transport;
    private readonly ClipboardPolicy _policy;
    private readonly Func<Task<string?>>? _pasteFallback;

    public Osc52ClipboardService(
        ITerminalTransport transport,
        ClipboardPolicy? policy = null,
        Func<Task<string?>>? pasteFallback = null)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        _policy = policy ?? new ClipboardPolicy();
        _pasteFallback = pasteFallback;
    }

    public async Task<bool> CopyAsync(string text, CancellationToken cancellationToken = default)
    {
        if (!_policy.CanCopy(_transport.Capabilities, text))
            return false;

        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        await _transport.WriteAsync($"\u001b]52;c;{encoded}\a", cancellationToken).ConfigureAwait(false);
        return true;
    }

    public Task<string?> PasteAsync(CancellationToken cancellationToken = default)
    {
        if (!_policy.CanPaste(_transport.Capabilities))
            return _pasteFallback is null ? Task.FromResult<string?>(null) : _pasteFallback();

        return _transport.ReadAsync(cancellationToken);
    }
}
