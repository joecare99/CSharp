using Microsoft.Extensions.DependencyInjection;
using Workbench.Builder.Cli;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Services.Formatting;
using Workbench.Builder.Core.Services.Inspection;
using Workbench.Builder.Core.Services.Loading;
using Workbench.Builder.Core.Services.References;

namespace Workbench.Builder.Analysis;

/// <summary>
/// Registers services required by the V1.1 analysis host.
/// </summary>
public static class ServiceRegistration
{
    /// <summary>
    /// Creates a service provider for the host.
    /// </summary>
    /// <returns>The configured service provider.</returns>
    public static ServiceProvider CreateServiceProvider()
    {
        ServiceCollection services = new();
        services.AddSingleton<IProjectLoader, MsBuildProjectLoader>();
        services.AddSingleton<IReferenceResolver, ReferenceResolver>();
        services.AddSingleton<ITestProjectDetector, TestProjectDetector>();
        services.AddSingleton<IProjectInspectionService, ProjectInspectionService>();
        services.AddSingleton<IProjectInspectionFormatter, ProjectInspectionFormatter>();
        services.AddSingleton<AnalysisCommandLineParser>();
        services.AddSingleton<IHostConsole, SystemConsoleAdapter>();
        services.AddSingleton<AnalysisApplication>();
        return services.BuildServiceProvider();
    }
}
