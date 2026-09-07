using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using ConsoleLib.Showcase.Services;
using ConsoleLib.Showcase.ViewModels;
using ConsoleLib.Showcase.Views;
using ConsoleLib.Showcase.Terminal.Core;
using ConsoleLib.Interfaces;
using ConsoleLib;
using ConsoleLib.ExtCon;
using BaseLib.Interfaces;
using BaseLib.Models;
using Terminal.Core;
using ConsoleLib.Showcase.Desktop;
using ConsoleLib.Showcase.Desktop.Capabilities;
using ConsoleLib.Showcase.DesktopHost;

namespace ConsoleLib.Showcase;

/// <summary>Composes and starts the native ConsoleLib showcase.</summary>
public static class Program
{
    public static void Main(string[] args)
    {
        if (!OperatingSystem.IsWindows())
        {
            Console.WriteLine("ConsoleLib Showcase requires Windows and the native ExtendedConsole host.");
            return;
        }

        ConfigureUnicodeConsole();

        using var provider = ConfigureServices().BuildServiceProvider();
        using var app = provider.GetRequiredService<DesktopShell>();
        app.Run();
    }

    private static void ConfigureUnicodeConsole()
    {
        Console.OutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        Console.InputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    }

    internal static IServiceCollection ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton<IExtendedConsole, ExtendedConsole>()
            .AddSingleton<IConsole, ConsoleProxy>()
            .AddSingleton<IWidgetSet, ConsoleWidgetSet>()
            .AddSingleton<IShowcaseHostCapabilities, ExtConShowcaseHostCapabilities>()
            .AddSingleton<DesktopViewModel>()
            .AddSingleton<DesktopShell>()
            .AddSingleton<ITerminalSessionBackendFactory, WindowsConPtyTerminalSessionFactory>()
            .AddSingleton<ITerminalSessionFactory, TerminalSessionFactory>()
            .AddSingleton<TerminalSnapshotRenderer>()
            .AddSingleton<TerminalInputRouter>()
            .AddSingleton<TerminalMouseNegotiator>()
            .AddSingleton<IShowcaseTerminalService, ShowcaseTerminalService>()
            .AddSingleton<VisualEffects>()
            .AddSingleton<ShowcaseViewModel>()
            .AddSingleton<ShowcaseView>();
    }
}
