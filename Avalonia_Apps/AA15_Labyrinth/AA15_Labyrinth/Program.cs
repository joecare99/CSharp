using Avalonia;
using System;

namespace AA15_Labyrinth;

public static class Program
{
 [STAThread]
 public static void Main(string[] args) => BuildAvaloniaApp()
 .StartWithClassicDesktopLifetime(args);

 public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
 .UsePlatformDetect()
 .WithInterFont()
 .LogToTrace();
}
