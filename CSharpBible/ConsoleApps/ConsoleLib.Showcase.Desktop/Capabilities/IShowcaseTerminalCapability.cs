using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleLib.Showcase.Desktop.Capabilities;

/// <summary>Portable terminal-session boundary consumed by the desktop page.</summary>
public interface IShowcaseTerminalCapability : IAsyncDisposable
{
    bool IsAvailable { get; }

    bool IsRunning { get; }

    event EventHandler<string>? OutputChanged;

    Task StartAsync(Size size, CancellationToken cancellationToken = default);

    Task SendInputAsync(string input, CancellationToken cancellationToken = default);

    Task ResizeAsync(Size size, CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}
