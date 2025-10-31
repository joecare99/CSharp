using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Avln_Geometry.ViewModels;
using Avln_Geometry.ViewModels.Interfaces;
using Avln_Geometry.Views;
using BaseLib.Helper;

namespace Avln_Geometry;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var sc = new ServiceCollection()
      .AddTransient<SampleViewer>()
            .AddTransient<ISampleViewerViewModel, SampleViewerViewModel>();

        var sp = sc.BuildServiceProvider();
        IoC.Configure(sp);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
