using AA98.DevOpsPlanning.Host.Commands;
using AA98.DevOpsPlanning.Host.Services;
using AA98.DevOpsPlanning.Host.ViewModels;
using AA98.DevOpsPlanning.Host.Views;
using AA98_AvlnCodeStudio.Base.Components.Commands;
using AA98_AvlnCodeStudio.Diagnostics.Debug.DependencyInjection;
using AA98_AvlnCodeStudio.Diagnostics.UI.DependencyInjection;
using AA98_AvlnCodeStudio.Planning.Local.Services;
using AA98_AvlnCodeStudio.Planning.UI.Extensions;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98.DevOpsPlanning.Host;

/// <summary>
/// Represents the Avalonia application for the AA98 DevOps planning host.
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

    /// <summary>
    /// Initializes the desktop host and main window composition.
    /// </summary>
    /// <param name="desktop">The desktop lifetime.</param>
    /// <returns>The created main window.</returns>
    public MainWindow InitializeDesktop(IClassicDesktopStyleApplicationLifetime desktop)
    {
        ArgumentNullException.ThrowIfNull(desktop);

        ServiceProvider = CreateServiceProvider();
        MainWindow mainWindow = CreateMainWindow(ServiceProvider);
        desktop.MainWindow = mainWindow;
        return mainWindow;
    }

    /// <summary>
    /// Creates the host service provider.
    /// </summary>
    /// <returns>The built service provider.</returns>
    public static ServiceProvider CreateServiceProvider()
    {
        ServiceCollection services = new();
        ConfigureServices(services);
        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });
    }

    /// <summary>
    /// Creates the host main window from the service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The main window.</returns>
    public static MainWindow CreateMainWindow(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>();
        return mainWindow;
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddPlanningUi();
        services.AddDiagnosticsUi();
        services.AddDebugDiagnostics();

        services.AddSingleton<IDevOpsFolderPickerService, AvaloniaDevOpsFolderPickerService>();
        services.AddSingleton<ILocalPlanningProjectScaffolder, LocalPlanningProjectScaffolder>();

        services.AddSingleton<IWorkbenchCommandContribution, OpenSolutionRootCommandContribution>();
        services.AddSingleton<IWorkbenchCommandContribution, OpenPlanningProjectCommandContribution>();
        services.AddSingleton<IWorkbenchCommandContribution, NewPlanningProjectCommandContribution>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();
    }
}
