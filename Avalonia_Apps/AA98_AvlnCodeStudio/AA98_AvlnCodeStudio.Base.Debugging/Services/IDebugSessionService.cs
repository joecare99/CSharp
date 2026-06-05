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
    /// Starts or attaches a debugging session.
    /// </summary>
    /// <param name="request">The launch or attach request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created or attached session information.</returns>
    Task<DebugSessionInfo> StartSessionAsync(DebugLaunchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops a running debugging session.
    /// </summary>
    /// <param name="sessionId">The provider-specific session identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that completes when the session has stopped.</returns>
    Task StopSessionAsync(string sessionId, CancellationToken cancellationToken = default);
}
