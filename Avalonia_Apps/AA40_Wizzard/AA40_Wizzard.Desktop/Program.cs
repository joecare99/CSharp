using System;
using AA40_Wizzard;
using Avalonia;

namespace AA40_Wizzard.Desktop;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
        => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilderFactory.BuildAvaloniaApp()
            .UsePlatformDetect();
}
