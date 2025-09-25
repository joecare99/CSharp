using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AsteroidsModern.Engine.Services;
using AsteroidsModern.Engine.Abstractions;
using AsteroidsModern.Engine.Model;
using AsteroidsModern.Engine.Services.Interfaces;

namespace AsteroidsModern.UI;

public partial class App : Application
{
    public IHost Host { get; }

    public App()
    {
        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureServices((ctx, services) =>
            {
                services.AddSingleton<IGameWorld, GameWorld>();
                services.AddSingleton<ITimeProvider, WpfTimeProvider>();
                services.AddSingleton<ISound, MidiSound>();
                services.AddSingleton<IRandom, CRandom>();
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await Host.StartAsync();
        var window = Host.Services.GetRequiredService<MainWindow>();
        window.Show();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await Host.StopAsync();
        Host.Dispose();
        base.OnExit(e);
    }
}
