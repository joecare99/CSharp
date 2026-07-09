using System;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.OS.Models;

namespace AA98_AvlnCodeStudio.Base.OS.Services;

/// <summary>
/// Represents a started terminal session.
/// </summary>
public interface ITerminalSession : IAsyncDisposable
{
    /// <summary>
    /// Gets the provider-specific or synthetic session identifier.
    /// </summary>
    string SessionId { get; }

    /// <summary>
    /// Gets the resolved shell descriptor for the session.
    /// </summary>
    TerminalShellDescriptor Shell { get; }

    /// <summary>
    /// Gets the high-level session state.
    /// </summary>
    TerminalSessionState State { get; }

    /// <summary>
    /// Occurs when standard output data is received.
    /// </summary>
    event EventHandler<string>? OutputReceived;

    /// <summary>
    /// Occurs when partial standard output data is received.
    /// </summary>
    event EventHandler<string>? OutputPartialReceived;

    /// <summary>
    /// Occurs when standard error data is received.
    /// </summary>
    event EventHandler<string>? ErrorReceived;

    /// <summary>
    /// Occurs when partial standard error data is received.
    /// </summary>
    event EventHandler<string>? ErrorPartialReceived;

    /// <summary>
    /// Occurs when the session exits.
    /// </summary>
    event EventHandler<int>? Exited;

    /// <summary>
    /// Writes input to the session.
    /// </summary>
    /// <param name="text">The input text to write.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task WriteInputAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the session.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task StopAsync(CancellationToken cancellationToken = default);
}
