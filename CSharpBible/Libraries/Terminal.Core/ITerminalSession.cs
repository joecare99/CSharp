using System;
using System.Threading;
using System.Threading.Tasks;

namespace Terminal.Core;

/// <summary>
/// Defines an interactive terminal session connected to a shell or console process.
/// </summary>
public interface ITerminalSession : IAsyncDisposable
{
    /// <summary>
    /// Occurs when terminal output was received.
    /// </summary>
    event EventHandler<string>? OutputReceived;

    /// <summary>
    /// Gets a value indicating whether the session is running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Gets the current terminal size.
    /// </summary>
    TerminalSize Size { get; }

    /// <summary>
    /// Starts the terminal session.
    /// </summary>
    Task StartAsync(TerminalSessionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes user input to the terminal session.
    /// </summary>
    Task WriteAsync(string input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resizes the terminal session.
    /// </summary>
    Task ResizeAsync(TerminalSize size, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the terminal session.
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);
}
