// ***********************************************************************
// Assembly         : ListBinding
// Author           : Mir
// Created          : 12-23-2021
//
// Last Modified By : Mir
// Last Modified On : 12-23-2021
// ***********************************************************************
// <copyright file="App.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using AA19_FilterLists.Model;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA19_FilterLists;

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
            var mainWindow = Services.GetRequiredService<Views.MainWindow>();
            desktop.MainWindow = mainWindow;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = Services.GetRequiredService<Views.MainView>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // ViewModels
        services.AddTransient<ViewModels.MainWindowViewModel>();
        services.AddSingleton<ViewModels.PersonViewViewModel>();

        // Models
        services.AddSingleton<Model.IPersonDataService, Model.PersonDataService>();
        services.AddSingleton<IEnumerable<Person>>((o)=> Model.PersonDataService.CreateDefaultPersons());

        // Views
        services.AddTransient<Views.MainView>();
        services.AddTransient<Views.MainWindow>();
        services.AddTransient<Views.PersonView>();
    }
}
