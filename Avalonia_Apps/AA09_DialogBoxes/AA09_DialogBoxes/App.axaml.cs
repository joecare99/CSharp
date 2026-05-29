// ***********************************************************************
// Assembly         : AA09_DialogBoxes
// Author           : Mir
// Created          : 12-29-2021
//
// Last Modified By : Mir
// Last Modified On : 12-29-2021
// ***********************************************************************
// <copyright file="App.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using AA09_DialogBoxes.ViewModels;
using AA09_DialogBoxes.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA09_DialogBoxes;

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
                DataContext = Services.GetRequiredService<MainWindowViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new BrowserMainView
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider CreateServices()
    {
        var services = new ServiceCollection();

        services
        .AddTransient<MainWindowViewModel>()
        .AddTransient<ISysTime, SysTime>()
        .AddTransient<ICyclTimer, TimerProxy>();

        return services.BuildServiceProvider();
    }
}
