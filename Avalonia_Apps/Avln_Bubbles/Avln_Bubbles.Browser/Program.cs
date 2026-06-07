using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avln_Bubbles.View;

[assembly: SupportedOSPlatform("browser")]

namespace Avln_Bubbles.Browser;

/// <summary>
/// Browser entry point for the Avalonia Bubbles application.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Starts the browser application.
    /// </summary>
    private static async Task Main(string[] args)
    {
        await BrowserAppBuilder.StartBrowserAppAsync(
            BuildAvaloniaApp()
                .WithInterFont(),
            "out",
            new BrowserPlatformOptions());
    }

    /// <summary>
    /// Builds the Avalonia application.
    /// </summary>
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>();
}
