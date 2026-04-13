using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia_App02.Models;
using Avalonia_App02.Models.Interfaces;
using Avalonia_App02.ViewModels;
using Avalonia_App02.ViewModels.Interfaces;
using Avalonia_App02.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia_App02;

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

        services.AddTransient<ISomeTemplateViewModel, SomeTemplateViewModel>()
        .AddTransient<ISysTime, SysTime>()
        .AddTransient<ICyclTimer, TimerProxy>()
        .AddSingleton<IPlatformHandle>((s)=>null!)
        .AddSingleton<ISomeTemplateModel, SomeTemplateModel>();

        Services = services.BuildServiceProvider();

        // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
        // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
        desktop.MainWindow = new MainWindow
        {
            DataContext = Services.GetRequiredService<ISomeTemplateViewModel>()
        };
    }

    public IServiceProvider? Services { get; private set; }
}