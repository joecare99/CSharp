// ***********************************************************************
// Assembly         : AA06_Converters_4
// Author      : Mir
// Created   : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 07-04-2022
// ***********************************************************************
// <copyright file="App.axaml.cs" company="AA06_Converters_4">
//  Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using AA06_Converters_4.Model;
using AA06_Converters_4.ViewModels;
using AA06_Converters_4.View;

namespace AA06_Converters_4;

/// <summary>
/// Interaction logic for App.axaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = default!;

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
    desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Models
        services.AddSingleton<IAGVModel, AGV_Model>();

        // ViewModels
     services.AddSingleton<VehicleViewModel>();
        services.AddTransient<PlotFrameViewModel>();

        // Views
        services.AddTransient<MainWindow>();
        services.AddTransient<VehicleView1>();
        services.AddTransient<PlotFrame>();
    }
}
