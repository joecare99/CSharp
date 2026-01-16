using System;
using BaseLib.Interfaces;
using BaseLib.Models;
using ConsoleDisplay.View;
using SharpHack.AI;
using SharpHack.Base.Data;
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
    /// <summary>
    /// Builds the view model, tile display and mini map instances.
    /// </summary>
    public GameContext Create(IConsole console)
    {
        if (console == null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        var random = new CRandom();
        var mapGenerator = new BSPMapGenerator(random);
        var combatSystem = new SimpleCombatSystem();
        var enemyAI = new SimpleEnemyAI();
        var gamePersist = new InMemoryGamePersist();

        var session = new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        var viewModel = new GameViewModel(session);

        var tileDef = new TileDefRes(".\\Resources\\Tiles1x1.tdj");
        var viewWidth = 70 / tileDef.TileSize.Width;
        var viewHeight = 20 / tileDef.TileSize.Height;
        viewModel.SetViewSize(viewWidth, viewHeight);

        var tileDisplay = new TileDisplay<DisplayTile>(console, tileDef, DrawingPoint.Empty,
            new DrawingSize(viewWidth, viewHeight), tileDef.TileSize);
        TileDisplay<DisplayTile>.defaultTile = DisplayTile.Empty;

        var miniMap = new Display(60, 21, 20, 12);

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
