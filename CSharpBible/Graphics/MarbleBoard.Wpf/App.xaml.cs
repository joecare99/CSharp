using MarbleBoard.Engine.Models;
using MarbleBoard.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace MarbleBoard.Wpf;

/// <summary>
/// Provides application startup and dependency injection configuration.
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    /// <summary>
    /// Starts the application and shows the main window.
    /// </summary>
    /// <param name="e">The startup arguments.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        ServiceCollection services = new();
        services
            .AddSingleton(MarbleBoardModel.CreateSampleBoard())
            .AddSingleton<BoardViewModel>()
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindow = mainWindow;
        mainWindow.Show();
        base.OnStartup(e);
    }

    /// <summary>
    /// Disposes managed resources on application shutdown.
    /// </summary>
    /// <param name="e">The exit event arguments.</param>
    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
