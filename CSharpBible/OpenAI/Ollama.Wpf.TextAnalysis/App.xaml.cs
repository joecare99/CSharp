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

    internal static Action<MainWindow> MainWindowPresenter { get; set; } = ShowMainWindow;

    internal void OnStartup(object sender, StartupEventArgs e)
    {
        ServiceCollection services = new();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindowPresenter(mainWindow);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        DisposeServices();
        base.OnExit(e);
    }

    internal static void ShowMainWindow(MainWindow mainWindow)
    {
        ArgumentNullException.ThrowIfNull(mainWindow);
        mainWindow.Show();
    }

    internal void DisposeServices()
    {
        _serviceProvider?.Dispose();
        _serviceProvider = null;
    }

    internal static void ConfigureServices(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<TextAnalysisTool>();
        services.AddSingleton<CSharpCodeAnalysisTool>();
        services.AddSingleton<ContentAnalysisRouter>();
        services.AddSingleton<IContentAnalysisService, ContentAnalysisService>();
        services.AddSingleton<ITextFilePicker, OpenFileDialogTextFilePicker>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<MainWindow>();
    }
}
