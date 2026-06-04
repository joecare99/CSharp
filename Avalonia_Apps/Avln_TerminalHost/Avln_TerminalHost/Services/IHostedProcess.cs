using System;
using System.Threading;
using System.Threading.Tasks;

namespace Avln_TerminalHost.Services;

/// <summary>
/// Represents a running child process used by the terminal host.
/// </summary>
public interface IHostedProcess : IAsyncDisposable
{
    /// <summary>
    /// Occurs when standard output data is received.
    /// </summary>
    event EventHandler<string>? StandardOutputReceived;

    /// <summary>
    /// Occurs when standard error data is received.
    /// </summary>
    event EventHandler<string>? StandardErrorReceived;

    /// <summary>
    /// Occurs when the process exits.
    /// </summary>
    event EventHandler<int>? Exited;

    /// <summary>
    /// Writes a line to the process standard input.
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
