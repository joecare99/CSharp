using System;
using Osb.ModernHost.Models;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Osb.ModernHost.Services;
using Osb.ModernHost.ViewModels;
using Osb.ModernHost.Views;

namespace Osb.ModernHost;

internal static class Program
{
    [STAThread]
    private static void Main(string[] arguments)
    {
        ApplicationConfiguration.Initialize();

        using var services = ConfigureServices(arguments);
        Application.Run(services.GetRequiredService<MainForm>());
    }

    private static ServiceProvider ConfigureServices(string[] arguments)
    {
        return new ServiceCollection()
            .AddSingleton(new ModernHostStartupService(arguments))
            .AddSingleton<IHostPageViewModel, OFBMenuViewModel>()
            .AddSingleton<IHostPageViewModel, PlaceSelectionViewModel>()
            .AddSingleton<IHostPageViewModel, PublicationSettingsViewModel>()
            .AddSingleton<IHostPageViewModel, FamilyPublicationViewModel>()
            .AddSingleton<StatisticsDisplayService>()
            .AddSingleton<IHostPageViewModel, StatisticsViewModel>()
            .AddSingleton<IHostNavigationService, ModernHostNavigationService>()
            .AddTransient<ModernHostViewModel>()
            .AddTransient<MainForm>()
            .BuildServiceProvider();
    }
}
