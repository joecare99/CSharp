using Microsoft.Extensions.DependencyInjection;
using RnzTrauer.Core;
using RnzTrauer.Core.Services.Interfaces;

namespace RnzTrauer.WebDriver.Firefox;

/// <summary>
/// Provides dependency injection registration for Firefox selenium drivers.
/// </summary>
public static class FirefoxWebDriverServiceCollectionExtensions
{
    /// <summary>
    /// Registers Firefox web driver factory services.
    /// </summary>
    public static IServiceCollection AddFirefoxWebDriver(this IServiceCollection xServices)
    {
        return xServices.AddSingleton<IFirefoxWebDriverFactory, FirefoxWebDriverFactory>();
    }
}
