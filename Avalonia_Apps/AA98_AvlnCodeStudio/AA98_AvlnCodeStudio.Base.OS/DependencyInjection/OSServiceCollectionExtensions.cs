using AA98_AvlnCodeStudio.Base.OS.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.Base.OS.DependencyInjection;

/// <summary>
/// Provides registration helpers for OS-scoped services.
/// </summary>
public static class OSServiceCollectionExtensions
{
    /// <summary>
    /// Adds a specific text document storage service implementation.
    /// </summary>
    /// <typeparam name="TTextDocumentStorageService">The storage service implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddCodeStudioOS<TTextDocumentStorageService>(this IServiceCollection services)
        where TTextDocumentStorageService : class, ITextDocumentStorageService
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ITextDocumentStorageService, TTextDocumentStorageService>();
        return services;
    }
}
