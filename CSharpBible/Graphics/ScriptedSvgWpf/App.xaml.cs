using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ScriptedSvgWpf.Dsl;
using ScriptedSvgWpf.Rendering;
using ScriptedSvgWpf.Services;
using ScriptedSvgWpf.ViewModels;

namespace ScriptedSvgWpf;

public partial class App : Application
{
    private ServiceProvider? _services;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _services = new ServiceCollection()
            .AddSingleton<ScriptInterpreter>()
            .AddSingleton<SvgExporter>()
            .AddSingleton<IDocumentFileService, WpfDocumentFileService>()
            .AddSingleton<MainViewModel>()
            .BuildServiceProvider();

        var window = new MainWindow
        {
            DataContext = _services.GetRequiredService<MainViewModel>()
        };
        MainWindow = window;
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _services?.Dispose();
        base.OnExit(e);
    }
}
