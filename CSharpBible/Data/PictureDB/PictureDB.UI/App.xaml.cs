using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PictureDB.Base.Services;
using PictureDB.Base.Services.Interfaces;
using PictureDB.Base.Models.Interfaces;
using PictureDB.Base.Models;
using PictureDB.UI.ViewModels;

namespace PictureDB.UI;

public partial class App : Application
{
    private IHost? _host;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // register base services from PictureDB.Base
                services.AddSingleton<IImageLoader, ImageLoader>();
                services.AddSingleton<IImageProcessor, ImageProcessor>();
                services.AddSingleton<ICategorizer, Categorizer>();
                services.AddSingleton<IEvaluator, Evaluator>();
                services.AddSingleton<ISorter, Sorter>();

                // LLM client using default constructor (model via env var)
                services.AddSingleton<ILLMClient>(sp => new LLMClient());

                services.AddSingleton<IResultStore, JsonResultStore>();

                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();

        var main = _host.Services.GetRequiredService<MainWindow>();
        main.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null) await _host.StopAsync();
        _host?.Dispose();
        base.OnExit(e);
    }
}
