using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleLib.Interfaces;

/// <summary>Small ANSI output contract independent of the underlying terminal provider.</summary>
public interface IAnsiOutput
{
    Task WriteAsync(string text, CancellationToken cancellationToken = default);
    Task ClearAsync(CancellationToken cancellationToken = default);
    Task MoveCursorAsync(int column, int row, CancellationToken cancellationToken = default);
    Task SetForegroundAsync(ConsoleColor color, CancellationToken cancellationToken = default);
    Task SetBackgroundAsync(ConsoleColor color, CancellationToken cancellationToken = default);
    Task ResetAsync(CancellationToken cancellationToken = default);
    Task EnableMouseTrackingAsync(CancellationToken cancellationToken = default);
    Task DisableMouseTrackingAsync(CancellationToken cancellationToken = default);
}
