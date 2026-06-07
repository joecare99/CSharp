using Avalonia;
using Avln_Bubbles.View;
using System;

namespace Avln_Bubbles.Desktop;

/// <summary>
/// Desktop entry point for the Avalonia Bubbles application.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Starts the desktop application.
    /// </summary>
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    /// <summary>
    /// Builds the Avalonia app.
    /// </summary>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
