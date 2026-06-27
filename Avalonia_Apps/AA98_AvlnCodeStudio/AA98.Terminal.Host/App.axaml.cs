using AA98.Terminal.Host.Services;
using AA98.Terminal.Host.ViewModels;
using AA98.Terminal.Host.Views;
using AA98_AvlnCodeStudio.Base.OS.Services;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98.Terminal.Host;

/// <summary>
/// Represents the Avalonia application for the AA98 terminal host.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets the configured service provider.
    /// </summary>
    public IServiceProvider? ServiceProvider { get; private set; }

    /// <inheritdoc/>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <inheritdoc/>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            InitializeDesktop(desktop);
        }

        base.OnFrameworkInitializationCompleted();
    }

    public MainWindow InitializeDesktop(IClassicDesktopStyleApplicationLifetime desktop)
    {
        ArgumentNullException.ThrowIfNull(desktop);

        ServiceProvider = CreateServiceProvider();
        var mainWindow = CreateMainWindow(ServiceProvider);
        desktop.MainWindow = mainWindow;
        return mainWindow;
    }

    public static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });
    }

    public static MainWindow CreateMainWindow(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        return serviceProvider.GetRequiredService<MainWindow>();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ITerminalShellResolver, DevelopmentTerminalShellResolver>();
        services.AddSingleton<IHostedTerminalProcessFactory, HostedTerminalProcessFactory>();
        services.AddSingleton<ITerminalSessionService, HostedTerminalSessionService>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();
    }
}