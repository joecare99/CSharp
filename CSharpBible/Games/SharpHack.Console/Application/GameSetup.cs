using BaseLib.Interfaces;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using ConsoleDisplay.View;
using SharpHack.Base.Data;
using SharpHack.AI;
using SharpHack.Combat;
using SharpHack.Engine;
using SharpHack.LevelGen.BSP;
using SharpHack.Persist;
using SharpHack.ViewModel;
using DrawingPoint = System.Drawing.Point;
using DrawingSize = System.Drawing.Size;

namespace SharpHack.Console;

/// <summary>
/// Creates the objects required to run the console game.
/// </summary>
internal sealed class GameSetup
{
    private readonly GamePersist _gamePersist = new();

    public GamePersist DurablePersist => _gamePersist;

    /// <summary>
    /// Builds the view model, tile display and mini map instances.
    /// </summary>
    public GameContext Create(IConsole console, Func<GameSession, IGameSaveLoadService?>? createSaveLoadService = null)
    {
        ArgumentNullException.ThrowIfNull(console);

        return CreateCore(console, createSaveLoadService, restoreGameState: null);
    }

    public GameContext CreateFromRestore(IConsole console, RestoreGameState restoreGameState, Func<GameSession, IGameSaveLoadService?>? createSaveLoadService = null)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(restoreGameState);

        return CreateCore(console, createSaveLoadService, restoreGameState);
    }

    private GameContext CreateCore(IConsole console, Func<GameSession, IGameSaveLoadService?>? createSaveLoadService, RestoreGameState? restoreGameState)
    {
        ArgumentNullException.ThrowIfNull(console);

        var random = new CRandom();
        var mapGenerator = new BSPMapGenerator(random);
        var combatSystem = new SimpleCombatSystem();
        var enemyAI = new SimpleEnemyAI();
        var session = restoreGameState == null
            ? GameSessionFactory.CreateSession(mapGenerator, _gamePersist, random, combatSystem, enemyAI)
            : GameSessionFactory.CreateSession(mapGenerator, _gamePersist, random, combatSystem, enemyAI, restoreGameState);
        var saveLoadService = createSaveLoadService?.Invoke(session);
        var viewModel = GameSessionFactory.CreateGameViewModel(session, saveLoadService);

        var tileDef = new TileDefRes(".\\Resources\\Tiles4x2.tdj");
        var viewWidth = 70 / tileDef.TileSize.Width;
        var viewHeight = 20 / tileDef.TileSize.Height;
        viewModel.SetViewSize(viewWidth, viewHeight);

        var tileDisplay = new TileDisplay<DisplayTile>(console, tileDef, DrawingPoint.Empty,
            new DrawingSize(viewWidth, viewHeight), tileDef.TileSize);
        TileDisplay<DisplayTile>.defaultTile = DisplayTile.Empty;

        var miniMap = new Display(console, 60, 21, 20, 12);

        return new GameContext(viewModel, tileDisplay, tileDef, miniMap);
    }
}

/// <summary>
/// Bundles the objects produced by <see cref="GameSetup"/>.
/// </summary>
internal sealed record GameContext(
    GameViewModel ViewModel,
    TileDisplay<DisplayTile> TileDisplay,
    ITileDef TileDefinition,
    Display MiniMap);
