using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;

[assembly: SupportedOSPlatform("browser")]

internal sealed partial class Program
{
    private static async Task Main(string[] args)
    {
        await BrowserAppBuilder.StartBrowserAppAsync(
            BuildAvaloniaApp()
                .WithInterFont(),
            "out",
            new BrowserPlatformOptions());
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<FroniusMonitor.Avalonia.App>();
}
