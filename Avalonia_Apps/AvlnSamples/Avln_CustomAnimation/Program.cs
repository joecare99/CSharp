// ***********************************************************************
// Assembly    : Avln_CustomAnimation
// Author     : Mir
// Created     : 01-15-2025
// ***********************************************************************
using Avalonia;
using System;

namespace Avln_CustomAnimation;

class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
      .UsePlatformDetect()
   .WithInterFont()
        .LogToTrace();
}
