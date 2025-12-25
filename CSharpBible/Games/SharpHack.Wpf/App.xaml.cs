using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SharpHack.ViewModel;
using SharpHack.Engine;
using SharpHack.LevelGen;
using SharpHack.LevelGen.BSP;
using SharpHack.Combat;
using SharpHack.AI;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using SharpHack.Base.Interfaces;
using SharpHack.Wpf.Services;

namespace SharpHack.Wpf;

public partial class App : Application
{
    private ServiceProvider _serviceProvider;

    public App()
    {
        ServiceCollection services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        // Core Services
        services.AddSingleton<IRandom, CRandom>();
        services.AddSingleton<IMapGenerator, BSPMapGenerator>();
        services.AddSingleton<ICombatSystem, SimpleCombatSystem>();
        services.AddSingleton<IEnemyAI, SimpleEnemyAI>();
        services.AddSingleton<ITileService, TileService>(); // Register TileService
        
        // Game Session
        services.AddSingleton<GameSession>();

        // ViewModel
        services.AddSingleton<GameViewModel>();

        // View
        services.AddSingleton<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
