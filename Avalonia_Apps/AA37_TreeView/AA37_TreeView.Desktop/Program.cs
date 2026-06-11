using AA37_TreeView;
using Avalonia;
using System;

namespace AA37_TreeView.Desktop;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
        => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilderFactory.BuildAvaloniaApp()
            .UsePlatformDetect();
}
