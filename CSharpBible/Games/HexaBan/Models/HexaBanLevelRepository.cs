using HexaBan.Models.Interfaces;
using System;
using System.Collections.Generic;

/// <summary>
/// Die HexaBanLevelRepository-Klasse stellt 50 vordefinierte HexaBan-Level bereit.
/// Jedes Level ist als 2D-Array von TileType definiert.
/// Die Level sind zickzack-versetzt (hexagonal) angeordnet.
/// 
/// Hinweise:
/// - Ziel ist es, alle Boxen ($) auf die Ziele (.) zu schieben.
/// - Die Level werden mit steigender Schwierigkeit komplexer.
/// - Die ASCII-Art zeigt die Startanordnung.
/// </summary>
public class HexaBanLevelRepository : IHexaBanLevelRepository
{
    // Leveldaten: (Größe, Tiles, Spielerposition, ASCII-Art, Hinweis)
    private readonly List<(TileType[,] tiles, (int, int) player, string g, string h)> _levels = new()
    {
        // Level 1
        (
            new TileType[5,5]
            {
                { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall },
                { TileType.Wall, TileType.Player, TileType.Floor, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Box, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall }
            },
            (1,1),
@"
  # # # # #
   # @   #
  #   $   #
   #   . # 
  # # # # #
",
            "Schiebe die Kiste nach unten auf das Ziel."
        ),
        // Level 2
        (
            new TileType[6,6]
            {
                { TileType.Wall, TileType.Wall,  TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Player, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Box, TileType.Floor, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Box, TileType.Goal, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Wall,  TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall }
            },
            (3,1),
@"
  # # # # # #
   #     @   #
  #   $     #
   #   $   . #
  #   .     #
   # # # # # #
",
            "Bewege zuerst die untere Kiste, um Platz zu schaffen."
        ),
        // Level 3
        (
            new TileType[6,6]
            {
                { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Player, TileType.Floor, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Box, TileType.Box, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall }
            },
            (2,1),
@"
   # # # # # #
  #   @     #
   # $ $     #
  #     .   #
   #   .     #
  # # # # # #
",
            "Achte darauf, keine Kiste in eine Ecke zu schieben."
        ),
        // Level 4
        (
            new TileType[7,7]
            {
                { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Player, TileType.Floor, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Box, TileType.Floor, TileType.Box, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall }
            },
            (3 , 1),
@"
    # # # # # # #
   #     @     #
    #   $   $   #
   #           #
    #   .   .   #
   #           #
    # # # # # # #
",
            "Versuche, beide Kisten nacheinander auf die Ziele zu schieben."
        ),
        // Level 5
        (
            new TileType[7,7]
            {
                { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Box, TileType.Box, TileType.Box, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Player, TileType.Floor, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Goal, TileType.Goal, TileType.Goal, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
                { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall }
            },
            (3,3),
@"
    # # # # # # #
   #           #
    # $ $ $     #
   #     @     #
    # . . .     # 
   #           #
    # # # # # # #
",
            "Plane deine Züge, damit keine Kiste blockiert wird."
        ),
// Level 6
(
    new TileType[7,7]
    {
        { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Box, TileType.Floor, TileType.Box, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall }
    },
    (3,3),
@"
# # # # # # #
 #           #
# $   $     #
 #     @     #
# .   .     #
 #           #
# # # # # # #
",
    "Bewege die Kisten nacheinander auf die Ziele."
),
// Level 7
(
    new TileType[8,8]
    {
        { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Box, TileType.Floor, TileType.Box, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall }
    },
    (3,3),
@"
# # # # # # # #
 #             #
# $   $       #
 #     @       #
# .   .       #
 #             #
#             #
 # # # # # # # #
",
    "Achte auf die Reihenfolge der Kisten."
),
// Level 8
(
    new TileType[8,8]
    {
        { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Box, TileType.Floor, TileType.Floor, TileType.Box, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall }
    },
    (3,3),
@"
# # # # # # # #
 #             #
#   $     $   #
 #     @       #
#   .   .     #
 #             #
#             #
 # # # # # # # #
",
    "Bewege die Kisten so, dass keine blockiert wird."
),
// Level 9
(
    new TileType[9,9]
    {
        { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Box, TileType.Floor, TileType.Box, TileType.Floor, TileType.Box, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall }
    },
    (3,3),
@"
# # # # # # # # #
 #               #
#   $   $   $   #
 #     @         #
#   .   .   .   #
 #               #
#               #
 #               #
# # # # # # # # #
",
    "Drei Kisten, drei Ziele – plane voraus!"
),
// Level 10
(
    new TileType[9,9]
    {
        { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Box, TileType.Box, TileType.Floor, TileType.Box, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Goal, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall },
        { TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall, TileType.Wall }
    },
    (3,3),
@"
# # # # # # # # #
 #               #
# $     $       #
 #     @         #
# .   .   .     #
 #               #
#               #
 #               #
# # # # # # # # #
",
    "Vermeide es, Kisten in die Ecken zu schieben."
),
        
    };

    /// <summary>
    /// Gibt das gewünschte Level als Tupel (Größe, Tiles, Spielerposition) zurück.
    /// </summary>
    /// <param name="level">Levelnummer (1-basiert)</param>
    /// <returns>Leveldaten</returns>
    /// <exception cref="ArgumentOutOfRangeException">Wenn das Level nicht existiert</exception>
    public (TileType[,] tiles, (int, int) player) GetLevel(int level)
    {
        if (level < 1 || level > _levels.Count)
            throw new ArgumentOutOfRangeException(nameof(level), $"Level muss zwischen 1 und {_levels.Count} liegen.");
        var levData = _levels[level - 1];
        levData.tiles[levData.player.Item1, levData.player.Item2] = TileType.Floor; // Setze den Spieler auf die Position
        return ((TileType[,])levData.tiles.Clone(), levData.player);
    }

    /// <summary>
    /// Gibt die ASCII-Art des Levels zurück.
    /// </summary>
    public string GetLevelAsciiArt(int level)
    {
        if (level < 1 || level > _levels.Count)
            throw new ArgumentOutOfRangeException(nameof(level), $"Level muss zwischen 1 und {_levels.Count} liegen.");
        return _levels[level - 1].Item3;
    }

    /// <summary>
    /// Gibt einen Hinweis zum Level zurück.
    /// </summary>
    public string GetLevelHint(int level)
    {
        if (level < 1 || level > _levels.Count)
            throw new ArgumentOutOfRangeException(nameof(level), $"Level muss zwischen 1 und {_levels.Count} liegen.");
        return _levels[level - 1].Item4;
    }
}
