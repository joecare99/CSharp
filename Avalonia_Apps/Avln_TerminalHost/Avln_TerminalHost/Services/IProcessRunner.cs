using System.Threading;
using System.Threading.Tasks;

namespace Avln_TerminalHost.Services;

/// <summary>
/// Starts terminal child processes for the host application.
/// </summary>
public interface IProcessRunner
{
    /// <summary>
    /// Starts the configured shell process.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The hosted process abstraction.</returns>
    Task<IHostedProcess> StartAsync(CancellationToken cancellationToken = default);
}
