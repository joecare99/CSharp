using System;
using AA37_TreeView.Model;
using AA37_TreeView.ViewModels;
using AA37_TreeView.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace AA37_TreeView;

/// <summary>
/// Configures application services and host-specific startup UI.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets the configured service provider.
    /// </summary>
    public IServiceProvider? Services { get; private set; }

    /// <inheritdoc />
    public override void Initialize()
        => AvaloniaXamlLoader.Load(this);

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        var services = CreateServices();
        Services = services;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = services.GetRequiredService<MainWindowViewModel>(),
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = new MainView
            {
                DataContext = services.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider CreateServices()
    {
        var services = new ServiceCollection();

        services
            .AddSingleton<IBooksService, BooksService>()
            .AddTransient<BooksTreeViewModel>()
            .AddTransient<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }
}
