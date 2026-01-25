using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SharpHack.ViewModel;
using SharpHack.Engine;
using SharpHack.LevelGen.BSP;
using SharpHack.Combat;
using SharpHack.AI;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using SharpHack.Base.Interfaces;
using SharpHack.Persist;

namespace SharpHack.View.FirstPerson;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        ServiceCollection services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    private void ConfigureServices(ServiceCollection services)
    {
        // Core Services
        services.AddSingleton<IRandom, CRandom>();
        services.AddSingleton<IMapGenerator, BSPRoomMazeMapGenerator>();
        services.AddSingleton<ICombatSystem, SimpleCombatSystem>();
        services.AddSingleton<IEnemyAI, SimpleEnemyAI>();
        services.AddSingleton<IGamePersist, InMemoryGamePersist>();
        
        // Game Session
        services.AddSingleton<GameSession>();

        // ViewModel
        services.AddSingleton<FirstPersonGameViewModel>();

        // View
        services.AddSingleton<MainWindow>();
    }

    /// <summary>
    /// Raises the <see cref="Application.Startup"/> event.
    /// </summary>
    /// <param name="e">A <see cref="StartupEventArgs"/> that contains the event data.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
