using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Ollama.CodingAgent.Desktop.Host;
using Ollama.CodingAgent.Desktop.Models;

namespace Ollama.CodingAgent.Desktop;

/// <summary>
/// Starts the Avalonia desktop adapter.
/// </summary>
internal static class Program
{
    private static Action<AppBuilder, string[]> StartDesktopLifetime =
        static (builder, args) => builder.StartWithClassicDesktopLifetime(args, ConfigureDesktopLifetime);

    private static Action<IClassicDesktopStyleApplicationLifetime>? ConfigureDesktopLifetime;

    [STAThread]
    private static void Main(string[] args)
    {
        DesktopComposition.Initialize(DesktopOptions.Parse(args));
        StartDesktopLifetime(BuildAvaloniaApp(), args);
    }

    /// <summary>
    /// Creates the configured Avalonia application builder.
    /// </summary>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
