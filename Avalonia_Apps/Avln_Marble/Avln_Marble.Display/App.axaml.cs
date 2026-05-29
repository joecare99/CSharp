using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avln_Marble.Display.ViewModels;
using Avln_Marble.Display.Views;
using MarbleBoard.Engine.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Avln_Marble.Display;

/// <summary>
/// Provides application startup and dependency injection configuration for the Avalonia marble sample.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets the application service provider.
    /// </summary>
    public IServiceProvider? Services { get; private set; }

    /// <inheritdoc/>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <inheritdoc/>
    public override void OnFrameworkInitializationCompleted()
    {
        Services = CreateServices();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Services.GetRequiredService<MainWindow>();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = Services.GetRequiredService<BrowserMainView>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider CreateServices()
    {
        ServiceCollection services = new();

        services
            .AddSingleton(MarbleBoardModel.CreateSampleBoard())
            .AddSingleton<BoardViewModel>()
            .AddSingleton<MainWindowViewModel>()
            .AddTransient<MainWindow>()
            .AddTransient<BrowserMainView>();

        return services.BuildServiceProvider();
    }
}
