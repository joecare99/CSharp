using AA16_UserControl1.ViewModels.Interfaces;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Views.Extension;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA16_UserControl1;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        ConfigureServices(services);

        IoC.Configure(services.BuildServiceProvider());

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = IoC.GetRequiredService<MainWindow>();
            mainWindow.DataContext = IoC.GetRequiredService<ViewModels.MainWindowViewModel>();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // ViewModels
        services.AddTransient<ViewModels.MainWindowViewModel>();
        services.AddTransient<IUserControlViewModel,ViewModels.UserControlViewModel>();

        // Views
        services.AddTransient<MainWindow>();
        services.AddTransient<Views.UserControlView>();
        services.AddTransient<Views.LabeldMaxLengthTextbox>();
        // services.AddTransient<Views.DoubleButtonUC>();
    }
}
