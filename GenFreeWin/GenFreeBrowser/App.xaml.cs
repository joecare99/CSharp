using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using GenFreeBrowser.ViewModels.Interfaces;
using GenFreeBrowser.ViewModels;
using GenFreeBrowser.Map.DI;

namespace GenFreeBrowser;

/// <summary>
/// WPF App mit einfachem DI (ServiceCollection -> ServiceProvider).
/// </summary>
public partial class App : Application
{
    public static ServiceProvider? ServiceProvider { get; private set; }

    public static T GetRequiredService<T>() where T : class =>
        ServiceProvider!.GetRequiredService<T>();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var services = new ServiceCollection();

        // Windows
        services.AddSingleton<MainWindow>();

        // ViewModels
        services.AddSingleton<IPersonenListViewModel, FraPersonenListViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        // Domain Services
        services.AddSingleton<IPersonenService, PersonenService>();

        // Module
        services.AddMapViewer();

        ServiceProvider = services.BuildServiceProvider();

        var mainWindow = GetRequiredService<MainWindow>();
        mainWindow.DataContext = GetRequiredService<MainWindowViewModel>();
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        ServiceProvider?.Dispose();
        base.OnExit(e);
    }
}
