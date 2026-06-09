using System;
using Avalonia;

namespace FroniusMonitor.Avalonia.Desktop;

internal sealed class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<FroniusMonitor.Avalonia.App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
