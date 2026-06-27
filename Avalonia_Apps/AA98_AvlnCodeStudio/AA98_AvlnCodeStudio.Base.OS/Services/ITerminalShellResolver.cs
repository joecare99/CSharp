using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.OS.Models;

namespace AA98_AvlnCodeStudio.Base.OS.Services;

/// <summary>
/// Resolves the shell configuration used to start terminal sessions.
/// </summary>
public interface ITerminalShellResolver
{
    /// <summary>
    /// Resolves the shell to use for the requested terminal session.
    /// </summary>
    /// <param name="request">The terminal session request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The resolved shell descriptor.</returns>
    Task<TerminalShellDescriptor> ResolveShellAsync(TerminalSessionStartRequest request, CancellationToken cancellationToken = default);
}
