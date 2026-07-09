using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.OS.Models;

namespace AA98_AvlnCodeStudio.Base.OS.Services;

/// <summary>
/// Provides a provider-neutral fallback terminal session service without process integration.
/// </summary>
public sealed class NullTerminalSessionService(ITerminalShellResolver terminalShellResolver) : ITerminalSessionService
{
    private readonly ITerminalShellResolver _terminalShellResolver = terminalShellResolver;

    /// <inheritdoc/>
    public async Task<ITerminalSession> StartSessionAsync(TerminalSessionStartRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var shell = await _terminalShellResolver.ResolveShellAsync(request, cancellationToken).ConfigureAwait(false);
        var session = new NullTerminalSession(shell);
        session.MarkStarted();
        return session;
    }
}
