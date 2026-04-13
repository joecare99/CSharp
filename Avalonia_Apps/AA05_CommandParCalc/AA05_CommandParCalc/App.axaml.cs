using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using AA05_CommandParCalc.Models;
using AA05_CommandParCalc.Models.Interfaces;
using AA05_CommandParCalc.ViewModels;
using AA05_CommandParCalc.ViewModels.Interfaces;
using AA05_CommandParCalc.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AA05_CommandParCalc;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            InitDesktopApp(desktop);
        }

        base.OnFrameworkInitializationCompleted();
    }

    protected void InitDesktopApp(IClassicDesktopStyleApplicationLifetime desktop)
    {
        var services = new ServiceCollection();

        services.AddTransient<ICommandParCalcViewModel, CommandParCalcViewModel>()
        .AddTransient<ISysTime, SysTime>()
        .AddTransient<ICyclTimer, TimerProxy>()
        .AddSingleton<IPlatformHandle>((s)=>null!)
        .AddSingleton<ICommandParCalcModel, CommandParCalcModel>();

        Services = services.BuildServiceProvider();

        desktop.MainWindow = new MainWindow
        {
            DataContext = Services.GetRequiredService<ICommandParCalcViewModel>()
        };
    }

    public IServiceProvider? Services { get; private set; }
}
