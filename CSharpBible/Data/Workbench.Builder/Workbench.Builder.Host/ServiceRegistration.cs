using Microsoft.Extensions.DependencyInjection;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Services.Formatting;
using Workbench.Builder.Core.Services.Inspection;
using Workbench.Builder.Core.Services.Loading;
using Workbench.Builder.Core.Services.References;

namespace Workbench.Builder.Host;

/// <summary>
/// Registers services required by the thin builder inspection host.
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
        services.AddSingleton<HostCommandLineParser>();
        services.AddSingleton<IHostConsole, SystemConsoleAdapter>();
        services.AddSingleton<HostApplication>();
        return services.BuildServiceProvider();
    }
}
