using HexaBan.Models.Interfaces;
using System.Collections.Generic;

namespace HexaBan.Models;

/// <summary>
/// Die HexaBanEngine verwaltet den Spielzustand eines HexaBan-Levels.
/// Sie speichert das Spielfeld als Dictionary, die aktuelle Spielerposition
/// und bietet Methoden zur Spielerbewegung, zur Überprüfung des Levelabschlusses
/// sowie zum Zugriff auf einzelne Spielfelder.
///
/// Die Spiellogik unterstützt das Verschieben von Kisten auf einem hexagonalen Spielfeld.
/// Der Spieler kann sich in sechs Richtungen bewegen. Wenn sich eine Kiste im Weg befindet,
/// wird geprüft, ob sie in die gleiche Richtung verschoben werden kann. Wände und andere Kisten
/// blockieren die Bewegung. Das Level gilt als abgeschlossen, wenn alle Zielfelder (Goal)
/// mit Kisten belegt sind.
/// </summary>
/// <remarks>The HexaBanEngine handles the game board, player position, and game logic for a hexagonal
/// Sokoban-style puzzle. It supports player movement in six directions, box pushing mechanics, and level completion
/// checks. The game board is represented as a dictionary of hexagonal coordinates to tiles, where each tile can have a
/// type (e.g., wall, goal) and state (e.g., contains a box or the player).</remarks>
public class HexaBanEngine : IHexaBanEngine
{
    private readonly Dictionary<HexCoord, HexTile> _board;
    public HexCoord PlayerPosition { get; private set; }

    public int Width => throw new System.NotImplementedException();

    public int Height => throw new System.NotImplementedException();

    public TileType[,] Board => throw new System.NotImplementedException();

    (int X, int Y) IHexaBanEngine.PlayerPosition => throw new System.NotImplementedException();

    public bool LevelBeendet => throw new System.NotImplementedException();

    public int Level => throw new System.NotImplementedException();

    public string Preview => throw new System.NotImplementedException();

    public HexaBanEngine(Dictionary<HexCoord, HexTile> board, HexCoord playerStart)
    {
        _board = board;
        PlayerPosition = playerStart;
        _board[playerStart].HasPlayer = true;
    }

    public bool MovePlayer(HexDirection dir)
    {
        var target = PlayerPosition.Move(dir);
        if (!_board.TryGetValue(target, out var targetTile) || targetTile.Type == TileType.Wall)
            return false;

        if (targetTile.HasBox)
        {
            var boxTarget = target.Move(dir);
            if (!_board.TryGetValue(boxTarget, out var boxTargetTile) ||
                boxTargetTile.Type == TileType.Wall || boxTargetTile.HasBox)
                return false;

            // Move box
            boxTargetTile.HasBox = true;
            targetTile.HasBox = false;
        }

        // Move player
        _board[PlayerPosition].HasPlayer = false;
        targetTile.HasPlayer = true;
        PlayerPosition = target;
        return true;
    }

    public bool IsLevelCompleted()
    {
        foreach (var tile in _board.Values)
        {
            if (tile.Type == TileType.Goal && !tile.HasBox)
                return false;
        }
        return true;
    }

    public HexTile GetTile(HexCoord coord) =>
        _board.TryGetValue(coord, out var tile) ? tile : null;

    public void Move(HexDirection dir)
    {
        throw new System.NotImplementedException();
    }

    public void Reset()
    {
        throw new System.NotImplementedException();
    }

    public void StartNextLevel()
    {
        throw new System.NotImplementedException();
    }
}


