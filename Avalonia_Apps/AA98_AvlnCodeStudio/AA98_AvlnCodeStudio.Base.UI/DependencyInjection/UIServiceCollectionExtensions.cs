using AA98_AvlnCodeStudio.Base.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.Base.UI.DependencyInjection;

/// <summary>
/// Provides registration helpers for UI-scoped services.
/// </summary>
public static class UIServiceCollectionExtensions
{
    /// <summary>
    /// Adds a specific editor file dialog service implementation.
    /// </summary>
    /// <typeparam name="TEditorFileDialogService">The dialog service implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddCodeStudioUI<TEditorFileDialogService>(this IServiceCollection services)
        where TEditorFileDialogService : class, IEditorFileDialogService
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IEditorFileDialogService, TEditorFileDialogService>();
        return services;
    }
}
