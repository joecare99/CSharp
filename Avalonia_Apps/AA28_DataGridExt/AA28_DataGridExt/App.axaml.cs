using System;
using AA28_DataGridExt.Model;
using AA28_DataGridExt.ViewModels;
using AA28_DataGridExt.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BaseLib.Helper;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AA28_DataGridExt;

/// <summary>
/// Configures application services and host-specific startup UI.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets the configured service provider.
    /// </summary>
    public IServiceProvider? Services { get; private set; }

    /// <inheritdoc />
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        var services = CreateServices();
        Services = services;
        IoC.Configure(services);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = services.GetRequiredService<MainWindowViewModel>(),
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = new MainView
            {
                DataContext = services.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider CreateServices()
    {
        var services = new ServiceCollection();

        services
            .AddTransient<IRandom, CRandom>()
            .AddSingleton<IPersonService, PersonService>()
            .AddTransient<DataGridViewModel>()
            .AddTransient<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }
}
