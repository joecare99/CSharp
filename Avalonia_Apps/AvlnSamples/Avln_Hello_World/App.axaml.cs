// ***********************************************************************
// Assembly    : Avln_Hello_World
// Author     : Mir
// Created     : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="App.axaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avln_Hello_World.Models;
using Avln_Hello_World.Models.Interfaces;
using Avln_Hello_World.ViewModels;
using Avln_Hello_World.Views;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Avln_Hello_World;

/// <summary>
/// Interaction logic for App.axaml
/// </summary>
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Configure Dependency Injection
   var sc = new ServiceCollection()
      .AddSingleton<IHelloWorldModel, HelloWorldModel>()
            .AddTransient<ICyclTimer, TimerProxy>()
       .AddTransient<HelloWorldViewModel, HelloWorldViewModel>()
            .AddTransient<MainWindowViewModel, MainWindowViewModel>();

        var sp = sc.BuildServiceProvider();
        Ioc.Default.ConfigureServices(sp);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
 {
            desktop.MainWindow = new MainWindow
       {
       DataContext = Ioc.Default.GetService<MainWindowViewModel>()
      };
   }

        base.OnFrameworkInitializationCompleted();
    }
}
