// ***********************************************************************
// Assembly         : MVVM_21_Buttons
// Author           : Mir
// Created          : 08-12-2022
//
// Last Modified By : Mir
// Last Modified On : 08-14-2022
// ***********************************************************************
// <copyright file="App.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using AA21_Buttons.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA21_Buttons;

/// <summary>
/// Avalonia Application mit Dependency Injection.
/// </summary>
public partial class App : Application
{
    private static ServiceProvider? _serviceProvider;
    public static IServiceProvider Services => _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        _serviceProvider = services.BuildServiceProvider();


        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            desktop.MainWindow = mainWindow;
        }


        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Konfiguriert die Dependency Injection Container.
    /// </summary>
    private static void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<ButtonsViewViewModel>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<Views.ButtonsView>();
        services.AddSingleton<MainWindow>();
    }
}
