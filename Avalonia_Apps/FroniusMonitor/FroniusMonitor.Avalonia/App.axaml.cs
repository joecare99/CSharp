using System;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FroniusMonitor.Avalonia.Infrastructure;
using FroniusMonitor.Avalonia.Services;
using FroniusMonitor.Avalonia.Views;
using FroniusMonitor.Core.Configuration;
using FroniusMonitor.Core.Contracts;
using FroniusMonitor.Core.Services;
using FroniusMonitor.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FroniusMonitor.Avalonia;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        ServiceCollection services = new();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        FroniusDashboardViewModel viewModel = _serviceProvider.GetRequiredService<FroniusDashboardViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = viewModel;
            desktop.MainWindow = mainWindow;
            desktop.Exit += async (_, _) =>
            {
                await viewModel.StopAsync();
                _serviceProvider?.Dispose();
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            MainView mainView = _serviceProvider.GetRequiredService<MainView>();
            mainView.DataContext = viewModel;
            singleView.MainView = mainView;
        }

        await viewModel.StartAsync();
        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        bool xBrowserHost = OperatingSystem.IsBrowser();
        string[] args = Environment.GetCommandLineArgs();
        FroniusDeviceEndpointOptions endpointOptions = new()
        {
            Host = args.Length > 1 && !string.IsNullOrWhiteSpace(args[1])
                ? args[1]
                : "192.168.0.80",
        };

        services.AddSingleton(endpointOptions);
        services.AddSingleton(xBrowserHost
            ? new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5117/"),
            }
            : new HttpClient());

        if (xBrowserHost)
        {
            services.AddSingleton<IFroniusSnapshotService, BrowserGatewaySnapshotService>();
        }
        else
        {
            services.AddSingleton<IFroniusPowerFlowClient, FroniusPowerFlowClient>();
            services.AddSingleton<FroniusPowerFlowJsonParser>();
            services.AddSingleton<IFroniusSnapshotService, FroniusSnapshotService>();
            services.AddSingleton<IFroniusHostReachabilityProbe, FroniusHostReachabilityProbe>();
        }

        services.AddSingleton<IUiDispatcher, AvaloniaUiDispatcher>();
        services.AddSingleton<FroniusDashboardViewModel>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainView>();
    }
}
