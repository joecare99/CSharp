using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avln_TerminalHost.Services;
using Avln_TerminalHost.ViewModels;
using Avln_TerminalHost.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Avln_TerminalHost;

/// <summary>
/// Represents the Avalonia application for the terminal host.
/// </summary>
public partial class App : Application
{
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
            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();

            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Gets the configured service provider.
    /// </summary>
    public IServiceProvider? Services { get; private set; }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IComSpecLocator, ComSpecLocator>();
        services.AddSingleton<IProcessRunner, ProcessRunner>();
        services.AddSingleton<MainWindowViewModel>();
    }
}
