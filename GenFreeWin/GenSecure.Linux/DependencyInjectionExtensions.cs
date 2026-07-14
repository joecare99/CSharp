using GenSecure.Contracts;
using GenSecure.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GenSecure.Linux;

/// <summary>
/// Registers the Linux/non-Windows-specific GenSecure platform services.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers the Linux/non-Windows-specific GenSecure platform services.
    /// </summary>
    /// <param name="services">The service collection to update.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddGenSecureLinuxPlatform(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ILocalKeyProtector>(provider => new DataProtectionLocalKeyProtector(provider.GetRequiredService<GenSecureStoreOptions>()));
        services.AddSingleton<ICurrentPrincipalProvider, EnvironmentPrincipalProvider>();

        return services;
    }
}
