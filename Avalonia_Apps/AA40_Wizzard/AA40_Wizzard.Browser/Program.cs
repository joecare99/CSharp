using System.Threading.Tasks;
using AA40_Wizzard;
using Avalonia;
using Avalonia.Browser;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        await BrowserAppBuilder.StartBrowserAppAsync(
            AppBuilderFactory.BuildAvaloniaApp(),
            "out",
            new BrowserPlatformOptions());
    }
}
