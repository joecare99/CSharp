using SharpHack.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using SharpHack.Persist;

namespace SharpHack.WPF2D.Services;

/// <summary>
/// Factory helpers for creating game sessions/view models.
/// </summary>
public static class GameSessionFactory
{
    /// <summary>
    /// Creates a new <see cref="LayeredGameViewModel"/> instance.
    /// </summary>
    /// <returns>The created view model.</returns>
    public static LayeredGameViewModel CreateLayeredGameViewModel(IServiceProvider _)
    {
        var random = new BaseLib.Models.CRandom();
        var mapGenerator = new SharpHack.LevelGen.BSP.BSPMapGenerator(random);
        var combatSystem = new SharpHack.Combat.SimpleCombatSystem();
        var enemyAI = new SharpHack.AI.SimpleEnemyAI();
        var gamePersist = _.GetRequiredService<GamePersist>();
        var session = SharpHack.ViewModel.GameSessionFactory.CreateSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        return new LayeredGameViewModel(session);
    }

    public static LayeredGameViewModel CreateRestoredLayeredGameViewModel(IServiceProvider _, RestoreGameState restoreGameState)
    {
        var random = new BaseLib.Models.CRandom();
        var mapGenerator = new SharpHack.LevelGen.BSP.BSPMapGenerator(random);
        var combatSystem = new SharpHack.Combat.SimpleCombatSystem();
        var enemyAI = new SharpHack.AI.SimpleEnemyAI();
        var gamePersist = _.GetRequiredService<GamePersist>();
        var session = SharpHack.ViewModel.GameSessionFactory.CreateSession(mapGenerator, gamePersist, random, combatSystem, enemyAI, restoreGameState);
        return new LayeredGameViewModel(session);
    }
}
