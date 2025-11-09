using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RenderDemo.ViewModels;

namespace RenderDemo
{
    public class App : Application
    {
        private IHost? _host;
        public override void Initialize()
        {
           base.Initialize();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddTransient<MainWindow>();
                })
                .Build();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = _host.Services.GetRequiredService<MainWindow>();
            }
            base.OnFrameworkInitializationCompleted();
        }

        // TODO: Make this work with GTK/Skia/Cairo depending on command-line args
        // again.
        public static void Main(string[] args)
            => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        // App configuration, used by the entry point and previewer
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .With(new Win32PlatformOptions
                {
                    OverlayPopups = true,
                })
                .UsePlatformDetect()
                .LogToTrace();
    }
}
