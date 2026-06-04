using System;
using Avalonia;

namespace Avln_TerminalHost;

/// <summary>
/// Defines the entry point for the terminal host application.
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
