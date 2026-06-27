using AA98_AvlnCodeStudio.Base.OS.Models;
using AA98_AvlnCodeStudio.Base.OS.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AA98.Terminal.Host.Tests;

internal sealed class TestTerminalSessionService : ITerminalSessionService
{
    private readonly Queue<ITerminalSession> _sessions = new();

    public List<TerminalSessionStartRequest> Requests { get; } = [];

    public void EnqueueSession(ITerminalSession session)
        => _sessions.Enqueue(session);

    public Task<ITerminalSession> StartSessionAsync(TerminalSessionStartRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Requests.Add(request);
        return Task.FromResult(_sessions.Count > 0 ? _sessions.Dequeue() : new TestTerminalSession
        {
            Shell = new TerminalShellDescriptor
            {
                DisplayName = "test-shell",
                ExecutablePath = "test-shell",
            },
        });
    }
}