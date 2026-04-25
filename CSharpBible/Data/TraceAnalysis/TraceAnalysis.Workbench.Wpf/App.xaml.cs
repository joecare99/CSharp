using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using TraceAnalysis.Workbench.Wpf.Services;
using TraceAnalysis.Workbench.Wpf.ViewModels;

namespace TraceAnalysis.Workbench.Wpf;

/// <summary>
/// Interaction logic for the trace analysis workbench application.
/// </summary>
public partial class App : Application
{
    private IHost? _host;

    /// <inheritdoc/>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = WorkbenchHostFactory.CreateHost();
        _host.Start();

        var mainWindow = new MainWindow
        {
            DataContext = _host.Services.GetRequiredService<MainWorkbenchViewModel>()
        };

        MainWindow = mainWindow;
        mainWindow.Show();
    }

    /// <inheritdoc/>
    protected override void OnExit(ExitEventArgs e)
    {
        if (_host != null)
        {
            _host.StopAsync().GetAwaiter().GetResult();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
