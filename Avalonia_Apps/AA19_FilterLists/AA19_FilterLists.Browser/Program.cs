using AA19_FilterLists;
using Avalonia.Browser;
using System.Runtime.Versioning;
using System.Threading.Tasks;

[assembly: SupportedOSPlatform("browser")]

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
