using AA28_DataGridExt;
using Avalonia;
using Avalonia.Browser;
using System.Runtime.Versioning;
using System.Threading.Tasks;

[assembly: SupportedOSPlatform("browser")]

internal static class Program
{
    private static async Task Main(string[] args)
    {
        await BrowserAppBuilder.StartBrowserAppAsync(
            BuildAvaloniaApp().WithInterFont(),
            "out",
            new BrowserPlatformOptions());
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}
