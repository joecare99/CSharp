using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
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

        // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
        // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
        DisableAvaloniaDataAnnotationValidation();
        desktop.MainWindow = new MainWindow
        {
            DataContext = Services.GetRequiredService<ICommandParCalcViewModel>()
        };
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

    public IServiceProvider? Services { get; private set; }
}
