using System.Threading;
using System.Threading.Tasks;

namespace ConsoleLib.Interfaces;

/// <summary>
/// Provides optional clipboard operations for a ConsoleLib host.
/// </summary>
public interface IClipboardService
{
    Task<bool> CopyAsync(string text, CancellationToken cancellationToken = default);
    Task<string?> PasteAsync(CancellationToken cancellationToken = default);
}
