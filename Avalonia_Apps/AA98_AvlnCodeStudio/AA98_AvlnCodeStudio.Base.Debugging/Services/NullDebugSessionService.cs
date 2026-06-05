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
}
