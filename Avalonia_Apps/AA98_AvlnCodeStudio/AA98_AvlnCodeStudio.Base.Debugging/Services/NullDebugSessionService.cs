using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Debugging.Models;

namespace AA98_AvlnCodeStudio.Base.Debugging.Services;

/// <summary>
/// Provides a provider-neutral fallback debugging service without debugger integration.
/// </summary>
public sealed class NullDebugSessionService : IDebugSessionService
{
    /// <inheritdoc/>
    public event System.EventHandler<DebugSessionEventArgs>? SessionEventReceived
    {
        add { }
        remove { }
    }

    /// <inheritdoc/>
    public Task<DebugSessionInfo> StartSessionAsync(DebugLaunchRequest request, CancellationToken cancellationToken = default)
    {
        var session = new DebugSessionInfo
        {
            DisplayName = request.Target,
            State = DebugSessionState.Stopped,
        };

        return Task.FromResult(session);
    }

    /// <inheritdoc/>
    public Task StopSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task SetBreakpointsAsync(
        string sessionId,
        string sourcePath,
        IReadOnlyList<int> lines,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task ContinueSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task PauseSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StepSessionAsync(
        string sessionId,
        DebugStepKind stepKind,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<DebugStackFrame>> GetStackTraceAsync(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<DebugStackFrame>>([]);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<DebugVariable>> GetVariablesAsync(
        string sessionId,
        int frameId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<DebugVariable>>([]);
    }
}
