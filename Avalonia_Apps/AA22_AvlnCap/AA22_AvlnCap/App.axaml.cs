// ***********************************************************************
// Assembly         : AA22_AvlnCap
// Author           : Mir
// Created          : 08-14-2022
//
// Last Modified By : Mir
// Last Modified On : 08-14-2022
// ***********************************************************************
// <copyright file="App.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using AA22_AvlnCap.ViewModels;
using AA22_AvlnCap.Model;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using BaseLib.Helper;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using AA22_AvlnCap.ViewModels.Interfaces;

namespace AA22_AvlnCap;

/// <summary>
/// Avalonia App
/// </summary>
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        services.AddTransient<IWpfCapModel, CWpfCapModel>();
        services.AddTransient<IRandom, CRandom>();
        services.AddTransient<IWpfCapViewModel, WpfCapViewModel>();

        ServiceProvider container = services.BuildServiceProvider();
        IoC.GetReqSrv = (type) => container.GetRequiredService(type);
        IoC.GetSrv = (type) => container.GetService(type);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

}
