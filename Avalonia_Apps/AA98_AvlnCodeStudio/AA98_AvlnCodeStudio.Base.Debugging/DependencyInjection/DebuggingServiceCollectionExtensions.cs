using AA98_AvlnCodeStudio.Base.Debugging.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.Base.Debugging.DependencyInjection;

/// <summary>
/// Provides registration helpers for debugging services.
/// </summary>
public static class DebuggingServiceCollectionExtensions
{
    /// <summary>
    /// Adds the default provider-neutral debugging services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddCodeStudioDebugging(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddCodeStudioDebugging<NullDebugSessionService>();
    }

    /// <summary>
    /// Adds a specific debugging service implementation.
    /// </summary>
    /// <typeparam name="TDebugSessionService">The debugging service implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddCodeStudioDebugging<TDebugSessionService>(this IServiceCollection services)
        where TDebugSessionService : class, IDebugSessionService
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IDebugSessionService, TDebugSessionService>();
        return services;
    }
}
