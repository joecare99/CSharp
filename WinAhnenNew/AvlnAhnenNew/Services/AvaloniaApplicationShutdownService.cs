using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using WinAhnenNew.Services;

namespace AvlnAhnenNew.Services;

/// <summary>
/// Shuts down the current Avalonia desktop application.
/// </summary>
public sealed class AvaloniaApplicationShutdownService : IApplicationShutdownService
{
    /// <inheritdoc />
    public void Shutdown()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime classicDesktop)
        {
            classicDesktop.Shutdown();
        }
    }
}
