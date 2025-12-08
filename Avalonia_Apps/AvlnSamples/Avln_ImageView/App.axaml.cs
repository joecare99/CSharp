using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avln_ImageView.ViewModels;
using Avln_ImageView.Views;
using Microsoft.Extensions.DependencyInjection;
using Avln_ImageView.Models;
using Avln_ImageView.Models.Interfaces;

namespace Avln_ImageView;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var sc = new ServiceCollection()
        .AddTransient<MainWindow>()
        .AddSingleton<IImageViewerModel, ImageViewerModel>()
        .AddTransient<ImageViewerViewModel>();

        var sp = sc.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vm = sp.GetRequiredService<ImageViewerViewModel>();
            desktop.MainWindow = sp.GetRequiredService<MainWindow>();
            desktop.MainWindow.DataContext = vm;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
