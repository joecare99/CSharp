using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.OS.Models;

namespace AA98_AvlnCodeStudio.Base.OS.Services;

/// <summary>
/// Defines provider-neutral operations for starting line-oriented shell sessions.
/// </summary>
public interface ITerminalSessionService
{
    /// <summary>
    /// Starts a line-oriented shell session for the requested configuration.
    /// </summary>
    /// <param name="request">The terminal session request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The started terminal session abstraction.</returns>
    Task<ITerminalSession> StartSessionAsync(TerminalSessionStartRequest request, CancellationToken cancellationToken = default);
}
