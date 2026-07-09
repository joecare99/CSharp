using AA98.Terminal.Host.Services;
using AA98_AvlnCodeStudio.Base.OS.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.Terminal.Host.Tests;

internal sealed class TestHostedTerminalProcessFactory : IHostedTerminalProcessFactory
{
    public List<(TerminalSessionStartRequest Request, TerminalShellDescriptor Shell)> Requests { get; } = [];

    public TestHostedTerminalProcess Process { get; } = new();

    public Task<IHostedTerminalProcess> StartAsync(TerminalSessionStartRequest request, TerminalShellDescriptor shell, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Requests.Add((request, shell));
        return Task.FromResult<IHostedTerminalProcess>(Process);
    }
}