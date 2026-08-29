using System.Threading;
using System.Threading.Tasks;

namespace ConsoleLib.Posix;

/// <summary>Controls the input mode of a POSIX terminal.</summary>
public interface IPosixTerminalMode
{
    Task EnterRawModeAsync(CancellationToken cancellationToken = default);
    Task RestoreAsync(CancellationToken cancellationToken = default);
}
