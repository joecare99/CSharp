using System;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.Terminal.Host.Services;

/// <summary>
/// Represents a running host-level terminal process with redirected streams.
/// </summary>
public interface IHostedTerminalProcess : IAsyncDisposable
{
    /// <summary>
    /// Occurs when standard output data is received.
    /// </summary>
    event EventHandler<string>? StandardOutputReceived;

    /// <summary>
    /// Occurs when partial standard output data is received.
    /// </summary>
    event EventHandler<string>? StandardOutputPartialReceived;

    /// <summary>
    /// Occurs when standard error data is received.
    /// </summary>
    event EventHandler<string>? StandardErrorReceived;

    /// <summary>
    /// Occurs when partial standard error data is received.
    /// </summary>
    event EventHandler<string>? StandardErrorPartialReceived;

    /// <summary>
    /// Occurs when the process exits.
    /// </summary>
    event EventHandler<int>? Exited;

    /// <summary>
    /// Writes a line of input to the process.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task WriteLineAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the process if it is still running.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task StopAsync(CancellationToken cancellationToken = default);
}