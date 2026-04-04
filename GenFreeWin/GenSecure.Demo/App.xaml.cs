using System;
using System.IO;
using System.Windows;
using GenSecure.Core;
using GenSecure.Demo.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace GenSecure.Demo;

/// <summary>
/// WPF application entry point. Bootstraps the DI container and opens the main window.
/// </summary>
public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    /// <inheritdoc />
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        services.AddGenSecureStore(o =>
            o.RootDirectory = Path.Combine(Path.GetTempPath(), "GenSecureDemo"));

        services.AddSingleton<MainViewModel>();

        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = new MainWindow(_serviceProvider.GetRequiredService<MainViewModel>());
        mainWindow.Show();
    }

    /// <inheritdoc />
    protected override void OnExit(ExitEventArgs e)
    {
        (_serviceProvider as IDisposable)?.Dispose();
        base.OnExit(e);
    }
}
