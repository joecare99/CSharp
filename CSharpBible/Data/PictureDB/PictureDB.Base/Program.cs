using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PictureDB.Base.Models;
using PictureDB.Base.Models.Interfaces;
using PictureDB.Base.Services;
using PictureDB.Base.Services.Interfaces;

namespace PictureDB.Base;

// Hauptprogramm
class Program
{
    static async Task Main(string[] args)
    {
        string folderPath = args.Length > 0 ? args[0] : @"C:\Images";
        // allow override via environment variable
        var model = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "mistral";
        var timeoutSecondsString = Environment.GetEnvironmentVariable("OLLAMA_TIMEOUT") ?? "60";
        if (!int.TryParse(timeoutSecondsString, out var timeoutSeconds)) timeoutSeconds = 60;

        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                // Services
                services.AddSingleton<IImageLoader, ImageLoader>();
                services.AddSingleton<IImageProcessor, ImageProcessor>();
                services.AddSingleton<ICategorizer, Categorizer>();
                services.AddSingleton<IEvaluator, Evaluator>();
                services.AddSingleton<ISorter, Sorter>();

                // LLM client (local ollama CLI) - registered with configured model/timeout
                services.AddSingleton<ILLMClient>(sp => new LLMClient(model, timeoutSeconds));

                // Result persistence
                services.AddSingleton<IResultStore, JsonResultStore>();

                // App entrypoint
                services.AddTransient<App>();
            })
            .Build();

        // Resolve and run
        var app = host.Services.GetRequiredService<App>();
        await app.RunAsync(folderPath);
    }
}
