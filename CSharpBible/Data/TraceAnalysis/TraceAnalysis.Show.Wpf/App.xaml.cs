using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Windows;
using TraceAnalysis.Show.Wpf.Services;
using TraceAnalysis.Show.Wpf.ViewModels;

namespace TraceAnalysis.Show.Wpf;

/// <summary>
/// Interaction logic for the trace quick viewer application.
/// </summary>
public partial class App : Application
{
    private IHost? _host;

    /// <inheritdoc/>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = TraceViewerHostFactory.CreateHost();
        _host.Start();

        var viewModel = _host.Services.GetRequiredService<TraceViewerViewModel>();
        viewModel.LoadInitialTrace(e.Args.FirstOrDefault());

        var mainWindow = new MainWindow
        {
            DataContext = viewModel
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
