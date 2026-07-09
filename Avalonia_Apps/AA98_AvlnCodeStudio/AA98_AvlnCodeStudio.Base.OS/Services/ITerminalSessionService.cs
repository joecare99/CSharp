using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.OS.Models;

namespace AA98_AvlnCodeStudio.Base.OS.Services;

/// <summary>
/// Defines provider-neutral terminal session operations for studio components.
/// </summary>
public interface ITerminalSessionService
{
    /// <summary>
    /// Starts a terminal session for the requested configuration.
    /// </summary>
    /// <param name="request">The terminal session request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The started terminal session abstraction.</returns>
    Task<ITerminalSession> StartSessionAsync(TerminalSessionStartRequest request, CancellationToken cancellationToken = default);
}
