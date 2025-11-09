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
        var services = new ServiceCollection()

        // Windows
        .AddSingleton<MainWindow>()

        // ViewModels
        .AddSingleton<IPersonenListViewModel, FraPersonenListViewModel>()
        .AddSingleton<KernFamilieViewModel>()
        .AddSingleton<MainWindowViewModel>()

        // Domain Services
        .AddSingleton<IPersonenService, PersonenService>()

        // Module
        .AddMapViewer();

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
