using System;
using AA40_Wizzard.Model;
using AA40_Wizzard.ViewModels;
using AA40_Wizzard.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace AA40_Wizzard;

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
        => AvaloniaXamlLoader.Load(this);

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        var services = CreateServices();
        Services = services;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = services.GetRequiredService<MainWindow>();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = services.GetRequiredService<MainView>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider CreateServices()
    {
        var services = new ServiceCollection();

        services
            .AddSingleton<IMessenger, WeakReferenceMessenger>()
            .AddSingleton<ISystemClock, SystemClock>()
            .AddSingleton<ILogSink, SimpleLog>()
            .AddSingleton<IWizzardModel, WizzardModel>()
            .AddSingleton<IWizardContentService, WizardContentService>()
            .AddSingleton<Page1ViewModel>()
            .AddSingleton<Page2ViewModel>()
            .AddSingleton<Page3ViewModel>()
            .AddSingleton<Page4ViewModel>()
            .AddSingleton<WizzardViewModel>()
            .AddSingleton<MainWindowViewModel>()
            .AddTransient<MainWindow>()
            .AddTransient<MainView>();

        return services.BuildServiceProvider();
    }
}
