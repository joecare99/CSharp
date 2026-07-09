using Microsoft.Extensions.DependencyInjection;
using Workbench.Builder.Cli;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Services.Compilation;
using Workbench.Builder.Core.Services.Inspection;
using Workbench.Builder.Core.Services.Loading;
using Workbench.Builder.Core.Services.References;

namespace Workbench.Builder.Host;

/// <summary>
/// Registers services required by the V1.2 compile host.
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
        services.AddSingleton<IProjectEmitSupportEvaluator, ProjectEmitSupportEvaluator>();
        services.AddSingleton<IProjectCompilationService, ProjectCompilationService>();
        services.AddSingleton<IProjectInspectionService, ProjectInspectionService>();
        services.AddSingleton<HostCommandLineParser>();
        services.AddSingleton<IHostConsole, SystemConsoleAdapter>();
        services.AddSingleton<HostApplication>();
        return services.BuildServiceProvider();
    }
}
