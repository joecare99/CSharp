using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Debugging.Models;

namespace AA98_AvlnCodeStudio.Base.Debugging.Services;

/// <summary>
/// Defines provider-neutral debugging session operations for studio components.
/// </summary>
public interface IDebugSessionService
{
    /// <summary>
    /// Raised when a debug adapter reports a session state or output event.
    /// </summary>
    event EventHandler<DebugSessionEventArgs>? SessionEventReceived;

    /// <summary>
    /// Starts or attaches a debugging session.
    /// </summary>
    /// <param name="request">The launch or attach request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created or attached session information.</returns>
    Task<DebugSessionInfo> StartSessionAsync(DebugLaunchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces the breakpoints for a source file in a running debug session.
    /// </summary>
    Task SetBreakpointsAsync(
        string sessionId,
        string sourcePath,
        IReadOnlyList<int> lines,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Continues execution of a paused debug session.
    /// </summary>
    Task ContinueSessionAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pauses execution of a running debug session.
    /// </summary>
    Task PauseSessionAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Steps through the current paused debug session.
    /// </summary>
    Task StepSessionAsync(
        string sessionId,
        DebugStepKind stepKind,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current stack frames for the active thread in a debug session.
    /// </summary>
    Task<IReadOnlyList<DebugStackFrame>> GetStackTraceAsync(
        string sessionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the variables visible from a stack frame.
    /// </summary>
    Task<IReadOnlyList<DebugVariable>> GetVariablesAsync(
        string sessionId,
        int frameId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops a running debugging session.
    /// </summary>
    /// <param name="sessionId">The provider-specific session identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that completes when the session has stopped.</returns>
    Task StopSessionAsync(string sessionId, CancellationToken cancellationToken = default);
}
