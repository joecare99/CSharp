using AA98_AvlnCodeStudio.Base.AI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.Base.AI.DependencyInjection;

/// <summary>
/// Provides registration helpers for AI services.
/// </summary>
public static class AIServiceCollectionExtensions
{
    /// <summary>
    /// Adds the default provider-neutral AI services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddCodeStudioAI(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IAIClientFactory, NullAIClientFactory>();
        services.AddTransient<IAIClient>(serviceProvider => serviceProvider.GetRequiredService<IAIClientFactory>().Create());
        return services;
    }

    /// <summary>
    /// Adds a specific AI client factory implementation.
    /// </summary>
    /// <typeparam name="TAIClientFactory">The AI client factory implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddCodeStudioAI<TAIClientFactory>(this IServiceCollection services)
        where TAIClientFactory : class, IAIClientFactory
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IAIClientFactory, TAIClientFactory>();
        services.AddTransient<IAIClient>(serviceProvider => serviceProvider.GetRequiredService<IAIClientFactory>().Create());
        return services;
    }
}
