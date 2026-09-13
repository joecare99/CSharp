using Config.Service;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Config.UI.ConfigService.DependencyInjection;

/// <summary>
/// Registers Config.Service adapters for Config.UI.
/// </summary>
public static class ConfigServiceUiServiceCollectionExtensions
{
    /// <summary>
    /// Adds a Config.UI registry backed by the host's Config.Service registrations.
    /// </summary>
    public static IServiceCollection AddConfigServiceUi(this IServiceCollection services, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IConfigUiSectionRegistry>(serviceProvider => new ConfigServiceUiSectionRegistry(
            serviceProvider.GetRequiredService<global::Config.Service.ConfigService>(),
            serviceProvider.GetRequiredService<IConfigSectionRegistry>(),
            isReadOnly));
        return services;
    }
}
