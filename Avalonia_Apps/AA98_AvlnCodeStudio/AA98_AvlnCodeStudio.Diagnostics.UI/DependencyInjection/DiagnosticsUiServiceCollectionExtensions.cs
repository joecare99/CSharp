using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AppKomponentBaseLib.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.Diagnostics.UI.DependencyInjection;

/// <summary>
/// Registers reusable diagnostics UI consumers.
/// </summary>
public static class DiagnosticsUiServiceCollectionExtensions
{
    /// <summary>
    /// Adds the reusable diagnostics UI consumer.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddDiagnosticsUi(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<DiagnosticCollectionViewModel>();
        services.AddSingleton<IDiagnosticConsumer>(serviceProvider => serviceProvider.GetRequiredService<DiagnosticCollectionViewModel>());
        return services;
    }
}
