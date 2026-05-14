using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Ollama.Tools.ContentAnalysis;
using Ollama.Wpf.TextAnalysis.Services;
using Ollama.Wpf.TextAnalysis.ViewModels;

namespace Ollama.Wpf.TextAnalysis;

/// <summary>
/// Hosts the WPF sample application bootstrap.
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    private void OnStartup(object sender, StartupEventArgs e)
    {
        ServiceCollection services = new();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<TextAnalysisTool>();
        services.AddSingleton<CSharpCodeAnalysisTool>();
        services.AddSingleton<ContentAnalysisRouter>();
        services.AddSingleton<IContentAnalysisService, ContentAnalysisService>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<MainWindow>();
    }
}
