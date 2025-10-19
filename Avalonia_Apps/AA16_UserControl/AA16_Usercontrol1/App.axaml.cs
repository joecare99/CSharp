using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA16_UserControl1;

public partial class App : Application
{
    public static IServiceProvider Services { get; set; } = null!;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        ConfigureServices(services);

        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.DataContext = Services.GetRequiredService<ViewModels.MainWindowViewModel>();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // ViewModels
        services.AddTransient<ViewModels.MainWindowViewModel>();
        services.AddTransient<ViewModels.UserControlViewModel>();
        services.AddTransient<ViewModels.CurrencyViewViewModel>();

        // Views
        services.AddTransient<MainWindow>();
        services.AddTransient<Views.UserControlView>();
        services.AddTransient<Views.LabeldMaxLengthTextbox>();
        // services.AddTransient<Views.DoubleButtonUC>();
    }
}
