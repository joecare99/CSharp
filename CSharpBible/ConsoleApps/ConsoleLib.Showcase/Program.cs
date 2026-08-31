using Microsoft.Extensions.DependencyInjection;
using System;
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

        using var provider = ConfigureServices().BuildServiceProvider();
        using var app = provider.GetRequiredService<ShowcaseView>();
        app.Run();
    }

    internal static IServiceCollection ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton<IExtendedConsole, ExtendedConsole>()
            .AddSingleton<IConsole, ConsoleProxy>()
            .AddSingleton<IWidgetSet, ConsoleWidgetSet>()
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
