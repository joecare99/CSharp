using AA98_AvlnCodeStudio.Base.Testing.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.Base.Testing.DependencyInjection;

/// <summary>
/// Provides registration helpers for testing services.
/// </summary>
public static class TestingServiceCollectionExtensions
{
    /// <summary>
    /// Adds the default provider-neutral testing services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddCodeStudioTesting(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddCodeStudioTesting<NullTestExecutionService>();
    }

    /// <summary>
    /// Adds a specific testing service implementation.
    /// </summary>
    /// <typeparam name="TTestExecutionService">The testing service implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddCodeStudioTesting<TTestExecutionService>(this IServiceCollection services)
        where TTestExecutionService : class, ITestExecutionService
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ITestExecutionService, TTestExecutionService>();
        return services;
    }
}
