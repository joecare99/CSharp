using AA98_AvlnCodeStudio.Base.Profiling.Models;
using AA98_AvlnCodeStudio.Base.Profiling.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace AA98_AvlnCodeStudio.Base.Profiling.DependencyInjection;

/// <summary>
/// Registers the optional .NET profiling component.
/// </summary>
public static class ProfilingServiceCollectionExtensions
{
    /// <summary>
    /// Adds .NET profiling services. Calling this method is the opt-in point for host applications.
    /// </summary>
    public static IServiceCollection AddDotNetProfiling(
        this IServiceCollection services,
        DotNetProfilingToolPaths? toolPaths = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        if (toolPaths is not null)
        {
            services.Replace(ServiceDescriptor.Singleton(toolPaths));
        }
        else
        {
            services.TryAddSingleton<DotNetProfilingToolPaths>();
        }

        services.TryAddSingleton<IDotNetProfilingService, DotNetProfilingService>();
        return services;
    }
}
