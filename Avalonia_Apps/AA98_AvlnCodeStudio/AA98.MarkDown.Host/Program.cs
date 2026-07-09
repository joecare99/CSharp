using Avalonia;
using System;

namespace AA98.MarkDown.Host;

/// <summary>
/// Defines the entry point for the markdown mini host.
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
