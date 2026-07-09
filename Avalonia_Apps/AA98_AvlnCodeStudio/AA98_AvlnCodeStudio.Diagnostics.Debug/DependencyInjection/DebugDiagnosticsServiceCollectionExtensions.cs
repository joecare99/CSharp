using AA98_AvlnCodeStudio.Diagnostics.Debug.Consumers;
using AppKomponentBaseLib.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.Diagnostics.Debug.DependencyInjection;

/// <summary>
/// Registers debug-based diagnostics consumers.
/// </summary>
public static class DebugDiagnosticsServiceCollectionExtensions
{
    /// <summary>
    /// Adds the default debug diagnostics consumer.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddDebugDiagnostics(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IDiagnosticConsumer, DebugDiagnosticConsumer>();
        return services;
    }
}
