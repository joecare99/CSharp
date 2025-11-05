// <copyright file="Program.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>

using Avalonia;
using System;

namespace AA21_Buttons;

internal sealed class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
