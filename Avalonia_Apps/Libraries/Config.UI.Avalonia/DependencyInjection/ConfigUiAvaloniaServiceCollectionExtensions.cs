using Config.UI.Avalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Property.Editor.Avalonia.DependencyInjection;
using System;

namespace Config.UI.Avalonia.DependencyInjection;

/// <summary>
/// Registers reusable Avalonia configuration UI services.
/// </summary>
public static class ConfigUiAvaloniaServiceCollectionExtensions
{
    /// <summary>
    /// Adds configuration UI view-model services. A host must register
    /// <see cref="IConfigUiSectionRegistry"/> before resolving the view model.
    /// </summary>
    public static IServiceCollection AddConfigUiAvalonia(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddPropertyEditorAvalonia();
        services.AddTransient<ConfigUiViewModel>();
        return services;
    }
}
