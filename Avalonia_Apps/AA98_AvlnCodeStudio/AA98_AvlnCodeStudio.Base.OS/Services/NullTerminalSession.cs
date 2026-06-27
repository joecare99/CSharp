using System;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.OS.Models;

namespace AA98_AvlnCodeStudio.Base.OS.Services;

/// <summary>
/// Provides a provider-neutral fallback terminal session without process integration.
/// </summary>
public sealed class NullTerminalSession : ITerminalSession
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NullTerminalSession"/> class.
    /// </summary>
    /// <param name="shell">The resolved shell descriptor.</param>
    public NullTerminalSession(TerminalShellDescriptor shell)
    {
        Shell = shell ?? new TerminalShellDescriptor();
    }

    /// <inheritdoc/>
    public string SessionId { get; } = string.Empty;

    /// <inheritdoc/>
    public TerminalShellDescriptor Shell { get; }

    /// <inheritdoc/>
    public TerminalSessionState State { get; private set; } = TerminalSessionState.Stopped;

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
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (State != TerminalSessionState.Stopped)
        {
            State = TerminalSessionState.Stopped;
            Exited?.Invoke(this, 0);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        State = TerminalSessionState.Stopped;
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Marks the session as running for provider-neutral startup flows.
    /// </summary>
    public void MarkStarted()
    {
        State = TerminalSessionState.Running;
    }
}
