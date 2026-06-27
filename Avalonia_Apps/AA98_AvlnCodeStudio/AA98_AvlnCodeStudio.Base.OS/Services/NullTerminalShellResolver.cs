using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.OS.Models;

namespace AA98_AvlnCodeStudio.Base.OS.Services;

/// <summary>
/// Provides a provider-neutral fallback shell resolver without OS-specific discovery.
/// </summary>
public sealed class NullTerminalShellResolver : ITerminalShellResolver
{
    /// <inheritdoc/>
    public Task<TerminalShellDescriptor> ResolveShellAsync(TerminalSessionStartRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var shell = new TerminalShellDescriptor
        {
            DisplayName = request.ShellDisplayName,
            ExecutablePath = request.ShellPath,
            IsFallback = true,
        };

        foreach (var argument in request.Arguments)
        {
            shell.Arguments.Add(argument);
        }

        return Task.FromResult(shell);
    }
}
