using System.Threading;
using System.Threading.Tasks;

namespace ConsoleLib.Posix;

internal sealed class NoOpPosixTerminalMode : IPosixTerminalMode
{
    public static NoOpPosixTerminalMode Instance { get; } = new();

    public Task EnterRawModeAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    public Task RestoreAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
