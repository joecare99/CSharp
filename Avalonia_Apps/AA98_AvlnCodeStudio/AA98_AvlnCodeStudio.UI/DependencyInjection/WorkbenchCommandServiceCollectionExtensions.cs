using AA98_AvlnCodeStudio.Base.Components.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.UI.DependencyInjection;

/// <summary>
/// Registers in-process workbench command contributions.
/// </summary>
public static class WorkbenchCommandServiceCollectionExtensions
{
    /// <summary>
    /// Registers the command catalog used by a workbench host.
    /// </summary>
    public static IServiceCollection AddWorkbenchCommandCatalog(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<WorkbenchCommandCatalog>();
        services.AddSingleton<IWorkbenchCommandCatalog>(static provider => provider.GetRequiredService<WorkbenchCommandCatalog>());
        return services;
    }

    /// <summary>
    /// Registers an in-process workbench command contribution.
    /// </summary>
    public static IServiceCollection AddWorkbenchCommandContribution<TContribution>(this IServiceCollection services)
        where TContribution : class, IWorkbenchCommandContribution
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IWorkbenchCommandContribution, TContribution>();
        return services;
    }
}
