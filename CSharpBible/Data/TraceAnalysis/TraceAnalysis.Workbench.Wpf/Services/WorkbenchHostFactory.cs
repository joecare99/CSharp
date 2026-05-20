using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TraceAnalysis.Workbench.Core.Services;
using TraceAnalysis.Workbench.Wpf.Services;
using TraceAnalysis.Workbench.Wpf.ViewModels;

namespace TraceAnalysis.Workbench.Wpf.Services;

/// <summary>
/// Creates the host used by the WPF workbench bootstrap.
/// </summary>
public static class WorkbenchHostFactory
{
    /// <summary>
    /// Creates a new host instance for the workbench application.
    /// </summary>
    /// <returns>The configured host.</returns>
    public static IHost CreateHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices(static services =>
            {
                services.AddSingleton<IWorkbenchSessionService, WorkbenchSessionService>();
                services.AddSingleton<TraceSeriesProjector>();
                services.AddSingleton<ITraceSourceLoader, TraceSourceLoader>();
                services.AddSingleton<IProcessingConfigurationStorage, JsonProcessingConfigurationStorage>();
                services.AddSingleton<IFileDialogService, FileDialogService>();
                services.AddSingleton<IWorkbenchMenuService, WorkbenchMenuService>();
                services.AddSingleton(static provider =>
                    provider.GetRequiredService<IWorkbenchSessionService>().CreateInitialSession());
                services.AddSingleton<MainWorkbenchViewModel>();
            })
            .Build();
    }
}
