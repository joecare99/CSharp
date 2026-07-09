using BaseLib.Interfaces;
using BaseLib.Models;
using BaseLib.Show.Demos;
using Microsoft.Extensions.DependencyInjection;

namespace BaseLib.Show;

/// <summary>
/// Registers the services required by the BaseLib showcase application.
/// </summary>
internal static class ShowcaseServiceCollectionExtensions
{
    /// <summary>
    /// Adds the showcase services and demo modules to the service collection.
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddBaseLibShowcase(this IServiceCollection services)
    {
        services.AddSingleton<IConsole, ConsoleProxy>();
        services.AddSingleton<ShowcaseText>();
        services.AddSingleton<ShowcaseConsole>();
        services.AddSingleton<ShowcaseApplication>();

        services.AddSingleton<IDemoModule, StringUtilsDemo>();
        services.AddSingleton<IDemoModule, MathUtilitiesDemo>();
        services.AddSingleton<IDemoModule, ByteUtilsDemo>();
        services.AddSingleton<IDemoModule, ObjectHelperDemo>();
        services.AddSingleton<IDemoModule, ListHelperDemo>();
        services.AddSingleton<IDemoModule, TypeUtilsDemo>();
        services.AddSingleton<IDemoModule, ConsoleProxyDemo>();
        services.AddSingleton<IDemoModule, IoCDemo>();
        services.AddSingleton<IDemoModule, SysTimeAndRandomDemo>();

        return services;
    }
}
