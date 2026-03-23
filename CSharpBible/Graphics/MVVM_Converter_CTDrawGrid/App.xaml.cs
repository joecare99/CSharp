using BaseLib.Helper;
using Microsoft.Extensions.DependencyInjection;
using MVVM_Converter_CTDrawGrid.Model;
using MVVM_Converter_CTDrawGrid.Models.Interfaces;
using System;
using System.Windows;

namespace MVVM_Converter_CTDrawGrid;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        var ServiceProvider = services.BuildServiceProvider();
        IoC.Configure(ServiceProvider);
        base.OnStartup(e);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // IoT-Service registrieren
        services.AddSingleton<IDrawGridModel, DrawGridModel>();

        // ViewModels registrieren
//        services.AddTransielt<>();
        // services.AddTransient<MainViewModel>();
    }
}
