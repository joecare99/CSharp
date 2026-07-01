using System;
using GenSecure.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace GenSecure.Windows;

/// <summary>
/// Registers the Windows-specific GenSecure platform services.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers the Windows-specific GenSecure platform services.
    /// </summary>
    /// <param name="services">The service collection to update.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddGenSecureWindowsPlatform(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ILocalKeyProtector, DpapiLocalKeyProtector>();
        services.AddSingleton<ICurrentPrincipalProvider, WindowsSidPrincipalProvider>();

        return services;
    }
}
