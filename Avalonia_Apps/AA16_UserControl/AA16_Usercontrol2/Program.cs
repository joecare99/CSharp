using Avalonia;
using System;

namespace AA16_UserControl2;

internal static class Program
{
 [STAThread]
 public static void Main(string[] args) =>
 BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

 public static AppBuilder BuildAvaloniaApp()
 => AppBuilder.Configure<App>()
 .UsePlatformDetect()
 .WithInterFont()
 .LogToTrace();
}
