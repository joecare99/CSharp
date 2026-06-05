using AA98_AvlnCodeStudio.Base.Versioning.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.Base.Versioning.DependencyInjection;

/// <summary>
/// Provides registration helpers for versioning services.
/// </summary>
public static class VersioningServiceCollectionExtensions
{
    /// <summary>
    /// Adds the default provider-neutral versioning services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddCodeStudioVersioning(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddCodeStudioVersioning<NullVersionControlService>();
    }

    /// <summary>
    /// Adds a specific versioning service implementation.
    /// </summary>
    /// <typeparam name="TVersionControlService">The versioning service implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddCodeStudioVersioning<TVersionControlService>(this IServiceCollection services)
        where TVersionControlService : class, IVersionControlService
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IVersionControlService, TVersionControlService>();
        return services;
    }
}
