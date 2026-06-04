using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avln_TerminalHost.Services;

namespace Avln_TerminalHostTests;

/// <summary>
/// Provides a controllable hosted-process test double for terminal host tests.
/// </summary>
public sealed class TestHostedProcess : IHostedProcess
{
    /// <inheritdoc/>
    public event EventHandler<string>? StandardOutputReceived;

    /// <inheritdoc/>
    public event EventHandler<string>? StandardErrorReceived;

    /// <inheritdoc/>
    public event EventHandler<int>? Exited;

    /// <summary>
    /// Gets the lines written to standard input.
    /// </summary>
    public List<string> WrittenLines { get; } = [];

    /// <summary>
    /// Gets a value indicating whether stop was requested.
    /// </summary>
    public bool StopRequested { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the process was disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc/>
    public Task WriteLineAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        WrittenLines.Add(text);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        StopRequested = true;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        IsDisposed = true;
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Emits a standard output line.
    /// </summary>
    /// <param name="text">The output text.</param>
    public void EmitStandardOutput(string text)
        => StandardOutputReceived?.Invoke(this, text);

    /// <summary>
    /// Emits a standard error line.
    /// </summary>
    /// <param name="text">The error text.</param>
    public void EmitStandardError(string text)
        => StandardErrorReceived?.Invoke(this, text);

    /// <summary>
    /// Emits a process exit event.
    /// </summary>
    /// <param name="exitCode">The exit code.</param>
    public void EmitExited(int exitCode)
        => Exited?.Invoke(this, exitCode);
}
