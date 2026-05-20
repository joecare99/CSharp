using BaseLib.Models.Interfaces;
using SharpHack.Base.Interfaces;
using SharpHack.Engine;
using SharpHack.Persist;

namespace SharpHack.ViewModel;

public static class GameSessionFactory
{
    public static GameSession CreateSession(
        IMapGenerator mapGenerator,
        IGamePersist gamePersist,
        IRandom random,
        ICombatSystem combatSystem,
        IEnemyAI enemyAI)
    {
        return new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
    }

    public static GameSession CreateSession(
        IMapGenerator mapGenerator,
        IGamePersist gamePersist,
        IRandom random,
        ICombatSystem combatSystem,
        IEnemyAI enemyAI,
        RestoreGameState restoreGameState)
    {
        return new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI, restoreGameState);
    }

    public static GameViewModel CreateGameViewModel(GameSession session, IGameSaveLoadService? saveLoadService = null)
    {
        return new GameViewModel(session, saveLoadService);
    }

    public static LayeredGameViewModel CreateLayeredGameViewModel(GameSession session, IGameSaveLoadService? saveLoadService = null)
    {
        return new LayeredGameViewModel(session, saveLoadService);
    }
}
