using AA28_DataGridExt;
using Avalonia;
using System;

namespace AA28_DataGridExt.Desktop;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
        => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilderFactory.BuildAvaloniaApp()
            .UsePlatformDetect();
}
