using Microsoft.Extensions.DependencyInjection;
using RnzTrauer.Core;
using RnzTrauer.Core.Services.Interfaces;

namespace RnzTrauer.WebDriver.Edge;

/// <summary>
/// Provides dependency injection registration for Edge selenium drivers.
/// </summary>
public static class EdgeWebDriverServiceCollectionExtensions
{
    /// <summary>
    /// Registers Edge web driver factory services.
    /// </summary>
    public static IServiceCollection AddEdgeWebDriver(this IServiceCollection xServices)
    {
        return xServices.AddSingleton<IEdgeWebDriverFactory, EdgeWebDriverFactory>();
    }
}
