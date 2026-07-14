using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvlnAhnenNew.Controls;
using AvlnAhnenNew.Services;
using AvlnAhnenNew.Views;
using BaseGenClasses.Helper;
using BaseGenClasses.Helper.Interfaces;
using BaseLib.Helper;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using WinAhnenNew.Services;
using WinAhnenNew.ViewModels;

namespace AvlnAhnenNew;

public partial class App : Application
{
    public App()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        services.AddSingleton<IGenealogyModelFactory, BaseGenGenealogyModelFactory>();
        services.AddSingleton<IPersonSelectionService, PersonSelectionService>();
        services.AddSingleton<IApplicationShutdownService, AvaloniaApplicationShutdownService>();
        services.AddTransient<IGenFactBuilder, GenFactBuilder>();
        services.AddTransient<IGenILBuilder, GenILBuilder>();
        services.AddTransient<FrmAhnenWinMainViewModel>();
        services.AddTransient<SelectionPageViewModel>();
        services.AddTransient<EditPageViewModel>();
        services.AddTransient<DetailPageViewModel>();
        services.AddTransient<RelationshipsPageViewModel>();
        services.AddTransient<SiblingsPageViewModel>();
        services.AddTransient<TextPageViewModel>();
        services.AddTransient<AdditionalPageViewModel>();
        services.AddTransient<AddressPageViewModel>();
        services.AddTransient<ImagesPageViewModel>();
        services.AddTransient<SelectionPageView>();
        services.AddTransient<EditPageView>();
        services.AddTransient<DetailPageView>();
        services.AddTransient<RelationshipsPageView>();
        services.AddTransient<SiblingsPageView>();
        services.AddTransient<TextPageView>();
        services.AddTransient<AdditionalPageView>();
        services.AddTransient<AddressPageView>();
        services.AddTransient<ImagesPageView>();
        services.AddGenSecureStore(options =>
        {
            options.RootDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "JC-Soft",
                "AvlnAhnenNew");
        });

        Services = services.BuildServiceProvider();
        IoC.Configure(Services);
    }

    public IServiceProvider Services { get; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime classicDesktop)
        {
            classicDesktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<FrmAhnenWinMainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
