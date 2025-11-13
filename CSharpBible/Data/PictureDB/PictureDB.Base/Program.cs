using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PictureDB.Base.Models;
using PictureDB.Base.Models.Interfaces;
using PictureDB.Base.Services;
using PictureDB.Base.Services.Interfaces;

namespace PictureDB.Base;

// Lädt Bilder aus einem Ordner

// Hauptprogramm
class Program
{
    static async Task Main(string[] args)
    {
        string folderPath = args.Length > 0 ? args[0] : @"C:\Images";

        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                // Services
                services.AddSingleton<IImageLoader, ImageLoader>();
                services.AddSingleton<IImageProcessor, ImageProcessor>();
                services.AddSingleton<ICategorizer, Categorizer>();
                services.AddSingleton<IEvaluator, Evaluator>();
                services.AddSingleton<ISorter, Sorter>();

                // Http client for LLM; Basisadresse kann bei Bedarf angepasst werden
                services.AddHttpClient<ILLMClient, LLMClient>(client =>
                {
                    client.BaseAddress = new Uri("http://localhost:5000/");
                });

                // App entrypoint
                services.AddTransient<App>();
            })
            .Build();

        // Resolve and run
        var app = host.Services.GetRequiredService<App>();
        await app.RunAsync(folderPath);
    }
}
