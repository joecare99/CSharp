using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avln_ControlsAndLayout.ViewModels;
using Avln_ControlsAndLayout.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Avln_ControlsAndLayout;

/// <summary>
/// Provides the Avalonia application bootstrap for the controls and layout sample.
/// </summary>
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection()
            .AddTransient<ControlsAndLayoutViewModel>()
            .AddTransient<MainWindowViewModel>()
            .BuildServiceProvider();

        Ioc.Default.ConfigureServices(services);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Ioc.Default.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
