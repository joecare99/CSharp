using System;
using Avalonia;

namespace Avln_ImageEditor.Host;

/// <summary>
/// Desktop entry point for the image editor host application.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Starts the desktop host application.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    /// <summary>
    /// Builds the Avalonia app.
    /// </summary>
    /// <returns>The configured app builder.</returns>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
