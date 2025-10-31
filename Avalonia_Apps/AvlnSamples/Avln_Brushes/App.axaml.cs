using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Avln_Brushes.ViewModels;
using Avln_Brushes.Views;
using Avln_Brushes.ViewModels.Interfaces;

namespace Avln_Brushes;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Configure Dependency Injection
        var services = new ServiceCollection();

        // Register ViewModels
        services.AddSingleton<ISampleViewerViewModel, SampleViewerViewModel>();
        services.AddTransient<GradientBrushesViewModel>();
        services.AddTransient<InteractiveLinearGradientViewModel>();
        services.AddTransient<BrushOpacityViewModel>();
        services.AddTransient<DashExampleViewModel>();
        services.AddTransient<PredefinedBrushesViewModel>();
        services.AddTransient<BrushTransformViewModel>();

        // Register Views
        services.AddTransient<GradientBrushesView>();
        services.AddTransient<InteractiveLinearGradientView>();
        services.AddTransient<DashExampleView>();
        services.AddTransient<PredefinedBrushesView>();
        services.AddTransient<BrushTransformView>();

        var serviceProvider = services.BuildServiceProvider();
        Ioc.Default.ConfigureServices(serviceProvider);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Create main window with DI
            var mainViewModel = serviceProvider.GetRequiredService<ISampleViewerViewModel>();
            desktop.MainWindow = new SampleViewer(mainViewModel);
        }

        base.OnFrameworkInitializationCompleted();
    }
}
