using System;
using GenSecure.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace GenSecure.Core;

/// <summary>
/// Registers secure storage services for dependency injection.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds the encrypted person store services.
    /// </summary>
    /// <param name="services">The service collection to update.</param>
    /// <param name="configureOptions">The configuration callback.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddGenSecureStore(this IServiceCollection services, Action<GenSecureStoreOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        var options = new GenSecureStoreOptions();
        configureOptions(options);

        services.AddSingleton(options);
        services.AddSingleton<MasterKeyBackupService>();
        services.AddSingleton<IMasterKeyBackupService>(provider => provider.GetRequiredService<MasterKeyBackupService>());
        services.AddSingleton<IPersonSecureStore, PersonSecureStore>();

        return services;
    }
}
