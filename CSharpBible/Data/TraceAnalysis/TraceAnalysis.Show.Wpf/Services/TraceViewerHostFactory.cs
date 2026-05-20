using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TraceAnalysis.Show.Wpf.ViewModels;
using TraceAnalysis.Workbench.Core.Services;
using TraceAnalysis.Widgets.Wpf.ViewModels;

namespace TraceAnalysis.Show.Wpf.Services;

/// <summary>
/// Creates the host used by the quick trace viewer bootstrap.
/// </summary>
public static class TraceViewerHostFactory
{
    /// <summary>
    /// Creates a new host instance for the quick trace viewer.
    /// </summary>
    public static IHost CreateHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices(static services =>
            {
                services.AddSingleton<TraceSeriesProjector>();
                services.AddSingleton<ITraceSourceLoader, TraceSourceLoader>();
                services.AddSingleton<ITraceViewerFileDialogService, TraceViewerFileDialogService>();
                services.AddSingleton<TraceChartViewModel>();
                services.AddSingleton<TraceViewerViewModel>();
            })
            .Build();
    }
}
