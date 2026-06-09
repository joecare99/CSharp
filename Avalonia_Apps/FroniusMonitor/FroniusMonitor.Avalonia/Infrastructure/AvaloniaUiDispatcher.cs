using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using FroniusMonitor.Core.Contracts;

namespace FroniusMonitor.Avalonia.Infrastructure;

/// <summary>
/// Marshals UI updates onto the Avalonia UI thread dispatcher.
/// </summary>
public sealed class AvaloniaUiDispatcher : IUiDispatcher
{
    /// <inheritdoc />
    public async Task InvokeAsync(Action action, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Dispatcher.UIThread.CheckAccess())
        {
            action();
            return;
        }

        await Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Background);
    }
}
