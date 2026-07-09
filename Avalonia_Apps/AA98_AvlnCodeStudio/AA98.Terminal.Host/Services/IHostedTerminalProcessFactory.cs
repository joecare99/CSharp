using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.OS.Models;

namespace AA98.Terminal.Host.Services;

/// <summary>
/// Creates host-level terminal processes for resolved shell descriptors.
/// </summary>
public interface IHostedTerminalProcessFactory
{
    /// <summary>
    /// Starts a host-level terminal process.
    /// </summary>
    /// <param name="request">The terminal session request.</param>
    /// <param name="shell">The resolved shell descriptor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The started process wrapper.</returns>
    Task<IHostedTerminalProcess> StartAsync(TerminalSessionStartRequest request, TerminalShellDescriptor shell, CancellationToken cancellationToken = default);
}