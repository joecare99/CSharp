using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Trnsp.Show.Lfm.Services;
using Trnsp.Show.Lfm.Services.Interfaces;
using Trnsp.Show.Lfm.ViewModels;

namespace Trnsp.Show.Lfm;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static IServiceProvider? _serviceProvider;

    public static IServiceProvider ServiceProvider => _serviceProvider
        ?? throw new InvalidOperationException("ServiceProvider not initialized");

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = new MainWindow
        {
            DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
        };
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Services
        services.AddSingleton<ILfmParserService, LfmParserService>();
        services.AddSingleton<IComponentFactory, ComponentFactory>();
        services.AddSingleton<IComponentRenderer, ComponentRenderer>();
        services.AddSingleton<IXamlExporter, XamlExporter>();

        // ViewModels
        services.AddTransient<MainViewModel>();
    }
}
