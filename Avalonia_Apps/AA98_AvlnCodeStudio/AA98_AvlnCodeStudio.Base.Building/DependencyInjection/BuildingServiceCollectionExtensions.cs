using AA98_AvlnCodeStudio.Base.Building.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.Base.Building.DependencyInjection;

/// <summary>
/// Provides registration helpers for builder services.
/// </summary>
public static class BuildingServiceCollectionExtensions
{
    /// <summary>
    /// Adds the default provider-neutral builder services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddCodeStudioBuilding(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddCodeStudioBuilding<NullCodeStudioBuilderService>();
    }

    /// <summary>
    /// Adds a specific builder service implementation.
    /// </summary>
    /// <typeparam name="TCodeStudioBuilderService">The builder service implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddCodeStudioBuilding<TCodeStudioBuilderService>(this IServiceCollection services)
        where TCodeStudioBuilderService : class, ICodeStudioBuilderService
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ICodeStudioBuilderService, TCodeStudioBuilderService>();
        return services;
    }
}
