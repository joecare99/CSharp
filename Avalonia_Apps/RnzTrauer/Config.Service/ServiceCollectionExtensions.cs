using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Config.Service;

/// <summary>
/// DI extension methods for configuring the configuration service with sections.
/// Use <see cref="ServiceCollectionExtensions.AddConfigService(IServiceCollection, string, string)"/> as the starting point.
/// Then chain <see cref="ServiceCollectionExtensions.AddConfigSection{TModel}(IServiceCollection, IConfigSectionProvider)"/> for each part.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds configuration service infrastructure for a specific vendor and application.</summary>
    public static IServiceCollection AddConfigService(
        this IServiceCollection services,
        string vendorName,
        string applicationName)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrEmpty(vendorName);
        ArgumentException.ThrowIfNullOrEmpty(applicationName);

        var registry = new ConfigSectionRegistry();
        var store = new JsonConfigStore(registry, vendorName, applicationName);
        var baseKey = $"{vendorName}.{applicationName}";

        services.AddSingleton<IConfigSectionRegistry>(registry);
        services.AddSingleton<IConfigStore>(store);
        services.AddSingleton(new ConfigService(store, registry, baseKey, vendorName, applicationName));

        return services;
    }

    /// <summary>Adds configuration service infrastructure using a base key legacy format.</summary>
    public static IServiceCollection AddConfigService(
        this IServiceCollection services,
        string baseKey)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrEmpty(baseKey);

        var parts = baseKey.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length >= 2)
        {
            return services.AddConfigService(parts[0], string.Join('.', parts.Skip(1)));
        }

        return services.AddConfigService("Vendor", parts[0]);
    }

    /// <summary>Registers a configuration section provider and its model type.</summary>
    public static IServiceCollection AddConfigSection<TModel>(this IServiceCollection services, IConfigSectionProvider section) where TModel : notnull
    {
        return services.AddConfigSection<TModel>(section, section.Description);
    }

    /// <summary>Registers a configuration section provider and lets the UI/app layer provide a localized description.</summary>
    public static IServiceCollection AddConfigSection<TModel>(this IServiceCollection services, IConfigSectionProvider section, string? description) where TModel : notnull
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(section);

        var registry = services
            .FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IConfigSectionRegistry))
            ?.ImplementationInstance as IConfigSectionRegistry;

        if (registry is null)
        {
            registry = new ConfigSectionRegistry();
            services.AddSingleton<IConfigSectionRegistry>(registry);
        }

        registry.AddSection(section);
        services.AddSingleton<IConfigSectionProvider>(section);
        services.AddSingleton(new ConfigSectionRegistration(section, description));

        return services;
    }
}
