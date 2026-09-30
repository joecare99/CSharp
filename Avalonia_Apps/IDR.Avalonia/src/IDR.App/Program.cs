using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace IDR.App;

internal static class Program
{
    [STAThread]
    public static async Task<int> Main(string[] args)
    {
        if (args.Length > 0)
        {
            using ServiceProvider services = App.CreateServices();
            CommandLineHost commandLineHost = new(
                services.GetRequiredService<IDR.Core.Services.IPeImageLoader>(),
                services.GetRequiredService<IDR.Core.Services.IDelphiVersionDetector>(),
                services.GetRequiredService<IDR.Core.Services.IKnowledgeBaseProvider>(),
                services.GetRequiredService<IDR.Core.Services.IInstructionDecoder>(),
                services.GetRequiredService<IDR.Core.Services.IAnalysisService>());
            return await commandLineHost.RunAsync(args).ConfigureAwait(false);
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        return 0;
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
