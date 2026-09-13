using Microsoft.Extensions.DependencyInjection;
using System;

namespace Property.Editor.Avalonia.DependencyInjection;

/// <summary>
/// Registers reusable Property.Editor Avalonia view models.
/// </summary>
public static class PropertyEditorServiceCollectionExtensions
{
    /// <summary>
    /// Adds the reusable Property.Editor view model factory services.
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddPropertyEditorAvalonia(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<PropertyEditorViewModel>();
        return services;
    }
}
