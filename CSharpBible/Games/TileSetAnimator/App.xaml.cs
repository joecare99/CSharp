using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TileSetAnimator.Services;
using TileSetAnimator.ViewModels;
using TileSetAnimator.Views;

namespace TileSetAnimator;

/// <summary>
/// Provides application entry logic and bootstraps the dependency injection container.
/// </summary>
public partial class App : Application
{
    private readonly IHost host;

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        host = Host.CreateDefaultBuilder()
            .ConfigureServices(static services =>
            {
                services.AddSingleton<IFileDialogService, FileDialogService>();
                services.AddSingleton<ITileSetService, TileSetService>();
                services.AddSingleton<ITileCutoutService, TileCutoutService>();
                services.AddSingleton<IAnimationDetectionService, ContiguousAnimationDetectionService>();
                services.AddSingleton<IAnimationPreviewService, DispatcherAnimationPreviewService>();
                services.AddSingleton<ITileSetPersistence, FileTileSetPersistence>();
                services.AddSingleton<ITileEnumSerializer, CSharpEnumTileSerializer>();
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    /// <inheritdoc />
    /// <remarks>Starts the dependency injection host before showing the main window.</remarks>
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        await host.StartAsync().ConfigureAwait(true);
        var mainWindow = host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    /// <inheritdoc />
    /// <remarks>Disposes the dependency injection host to free resources.</remarks>
    protected override async void OnExit(ExitEventArgs e)
    {
        await host.StopAsync().ConfigureAwait(true);
        host.Dispose();
        base.OnExit(e);
    }
}
