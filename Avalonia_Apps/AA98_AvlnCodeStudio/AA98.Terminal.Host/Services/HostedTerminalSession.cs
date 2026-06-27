using AA98_AvlnCodeStudio.Base.OS.Models;
using AA98_AvlnCodeStudio.Base.OS.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.Terminal.Host.Services;

/// <summary>
/// Adapts a host-level redirected process to the AA98 terminal session abstraction.
/// </summary>
public sealed class HostedTerminalSession : ITerminalSession
{
    private readonly IHostedTerminalProcess _hostedProcess;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostedTerminalSession"/> class.
    /// </summary>
    /// <param name="shell">The resolved shell descriptor.</param>
    /// <param name="hostedProcess">The underlying hosted process.</param>
    public HostedTerminalSession(TerminalShellDescriptor shell, IHostedTerminalProcess hostedProcess)
    {
        Shell = shell;
        _hostedProcess = hostedProcess;
        State = TerminalSessionState.Running;
        SessionId = Guid.NewGuid().ToString("N");

        _hostedProcess.StandardOutputReceived += OnStandardOutputReceived;
        _hostedProcess.StandardOutputPartialReceived += OnStandardOutputPartialReceived;
        _hostedProcess.StandardErrorReceived += OnStandardErrorReceived;
        _hostedProcess.StandardErrorPartialReceived += OnStandardErrorPartialReceived;
        _hostedProcess.Exited += OnExited;
    }

    /// <inheritdoc/>
    public string SessionId { get; }

    /// <inheritdoc/>
    public TerminalShellDescriptor Shell { get; }

    /// <inheritdoc/>
    public TerminalSessionState State { get; private set; }

    /// <inheritdoc/>
    public event EventHandler<string>? OutputReceived;

    /// <inheritdoc/>
    public event EventHandler<string>? OutputPartialReceived;

    /// <inheritdoc/>
    public event EventHandler<string>? ErrorReceived;

    /// <inheritdoc/>
    public event EventHandler<string>? ErrorPartialReceived;

    /// <inheritdoc/>
    public event EventHandler<int>? Exited;

    /// <inheritdoc/>
    public Task WriteInputAsync(string text, CancellationToken cancellationToken = default)
        => _hostedProcess.WriteLineAsync(text, cancellationToken);

    /// <inheritdoc/>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await _hostedProcess.StopAsync(cancellationToken).ConfigureAwait(false);
        State = TerminalSessionState.Stopped;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        DetachEvents();
        State = TerminalSessionState.Stopped;
        await _hostedProcess.DisposeAsync().ConfigureAwait(false);
    }

    private void OnStandardOutputReceived(object? sender, string text)
        => OutputReceived?.Invoke(this, text);

    private void OnStandardOutputPartialReceived(object? sender, string text)
        => OutputPartialReceived?.Invoke(this, text);

    private void OnStandardErrorReceived(object? sender, string text)
        => ErrorReceived?.Invoke(this, text);

    private void OnStandardErrorPartialReceived(object? sender, string text)
        => ErrorPartialReceived?.Invoke(this, text);

    private void OnExited(object? sender, int exitCode)
    {
        State = TerminalSessionState.Stopped;
        Exited?.Invoke(this, exitCode);
    }

    private void DetachEvents()
    {
        _hostedProcess.StandardOutputReceived -= OnStandardOutputReceived;
        _hostedProcess.StandardOutputPartialReceived -= OnStandardOutputPartialReceived;
        _hostedProcess.StandardErrorReceived -= OnStandardErrorReceived;
        _hostedProcess.StandardErrorPartialReceived -= OnStandardErrorPartialReceived;
        _hostedProcess.Exited -= OnExited;
    }
}