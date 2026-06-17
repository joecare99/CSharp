using System;
using System.Windows;
using ActionTest.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ActionTestWPF;

/// <summary>
/// Interaktionslogik für "App.xaml"
/// </summary>
public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _services = serviceCollection.BuildServiceProvider();
    }

    public IServiceProvider Services => _services;

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddTransient<IActionMainViewModel, ActionMainViewModel>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

}
