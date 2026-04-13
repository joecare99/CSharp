using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia_App_01;

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
        => AppBuilder.Configure<App>();
}