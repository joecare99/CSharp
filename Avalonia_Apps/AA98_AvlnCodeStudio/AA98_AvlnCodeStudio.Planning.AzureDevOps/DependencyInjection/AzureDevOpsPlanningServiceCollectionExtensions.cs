using AA98_AvlnCodeStudio.Planning.AzureDevOps.Services;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.Planning.AzureDevOps.DependencyInjection;

/// <summary>
/// Provides registration helpers for Azure DevOps planning adapter services.
/// </summary>
public static class AzureDevOpsPlanningServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Azure DevOps planning adapter skeleton.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddAzureDevOpsPlanningAdapter(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IPlanningAdapter, AzureDevOpsPlanningAdapter>();
        return services;
    }
}
