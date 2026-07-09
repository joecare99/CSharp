using Avalonia;
using System;

namespace AA98.Terminal.Host;

/// <summary>
/// Defines the entry point for the AA98 terminal micro host.
/// </summary>
public sealed class Program
{
    /// <summary>
    /// Starts the Avalonia desktop application.
    /// </summary>
    /// <param name="args">The startup arguments.</param>
    [STAThread]
    public static void Main(string[] args)
        => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    /// <summary>
    /// Builds the Avalonia application.
    /// </summary>
    /// <returns>The configured application builder.</returns>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}