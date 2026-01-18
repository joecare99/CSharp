using BaseLib.Models;
using SharpHack.AI;
using SharpHack.Base.Data;
using SharpHack.Combat;
using SharpHack.Engine;
using SharpHack.LevelGen.BSP;
using SharpHack.Persist;
using SharpHack.ViewModel;

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
        var random = new CRandom();
        var mapGenerator = new BSPMapGenerator(random);
        var combatSystem = new SimpleCombatSystem();
        var enemyAI = new SimpleEnemyAI();
        var gamePersist = new InMemoryGamePersist();

        var session = new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        return new LayeredGameViewModel(session);
    }
}
