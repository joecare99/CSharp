using Avalonia;
using System;

namespace AA19_FilterLists.Desktop;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
        => AppBuilderFactory.BuildAvaloniaApp()
            .UsePlatformDetect()
            .StartWithClassicDesktopLifetime(args);
}
