using Avalonia;
using System;

namespace AvlnAhnenNew;

internal static class Program
{
    [STAThread]
    public static void Main(string[] arrArgs)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(arrArgs);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
