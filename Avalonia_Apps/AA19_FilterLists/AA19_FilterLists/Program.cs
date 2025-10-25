using System;
using Avalonia;

namespace AA19_FilterLists;

internal class Program
{
 [STAThread]
 public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

 public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
 .UsePlatformDetect()
 .WithInterFont()
 .LogToTrace();
}
