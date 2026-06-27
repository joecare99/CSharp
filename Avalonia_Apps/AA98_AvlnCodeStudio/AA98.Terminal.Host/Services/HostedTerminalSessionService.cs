using AA98_AvlnCodeStudio.Base.OS.Models;
using AA98_AvlnCodeStudio.Base.OS.Services;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.Terminal.Host.Services;

/// <summary>
/// Starts AA98 terminal sessions backed by redirected shell processes.
/// </summary>
public sealed class HostedTerminalSessionService(ITerminalShellResolver terminalShellResolver, IHostedTerminalProcessFactory hostedTerminalProcessFactory) : ITerminalSessionService
{
    private readonly ITerminalShellResolver _terminalShellResolver = terminalShellResolver;
    private readonly IHostedTerminalProcessFactory _hostedTerminalProcessFactory = hostedTerminalProcessFactory;

    /// <inheritdoc/>
    public async Task<ITerminalSession> StartSessionAsync(TerminalSessionStartRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var shell = await _terminalShellResolver.ResolveShellAsync(request, cancellationToken).ConfigureAwait(false);
        var hostedProcess = await _hostedTerminalProcessFactory.StartAsync(request, shell, cancellationToken).ConfigureAwait(false);
        return new HostedTerminalSession(shell, hostedProcess);
    }
}