using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AA05_CommandParCalc.Models;
using AA05_CommandParCalc.Models.Interfaces;
using AA05_CommandParCalc.ViewModels;
using AA05_CommandParCalc.ViewModels.Interfaces;
using AA05_CommandParCalc.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AA05_CommandParCalc;

public partial class App : Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Services = CreateServices();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<ICommandParCalcViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new CommandParCalcView
            {
                DataContext = Services.GetRequiredService<ICommandParCalcViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider CreateServices()
    {
        var services = new ServiceCollection();

        services.AddTransient<ICommandParCalcViewModel, CommandParCalcViewModel>()
        .AddTransient<ISysTime, SysTime>()
        .AddTransient<ICyclTimer, TimerProxy>()
        .AddSingleton<ICommandParCalcModel, CommandParCalcModel>();

        return services.BuildServiceProvider();
    }
}
