using System.Windows;

namespace WinAhnenNew.Services;

/// <summary>
/// Shuts down the current WPF application.
/// </summary>
public sealed class WpfApplicationShutdownService : IApplicationShutdownService
{
    /// <inheritdoc />
    public void Shutdown()
    {
        Application.Current?.Shutdown();
    }
}
