using System;
using System.Collections.Generic;
using System.Drawing;
using ConsoleDisplay.View;
using SharpHack.ViewModel;

namespace SharpHack.Console;

public sealed class SharpHackTileDef : TileDefBase
{
    private static readonly (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) _empty = MakeDef(" ", ConsoleColor.Black, ConsoleColor.Black);

    private static readonly Dictionary<int, (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors)> _defs = new()
    {
        [(int)DisplayTile.Archaeologist] = MakeDef("@", ConsoleColor.Yellow),
        [(int)DisplayTile.Goblin] = MakeDef("g", ConsoleColor.Green),
        [(int)DisplayTile.Wall_EW] = MakeDef("=", ConsoleColor.Gray),
        [(int)DisplayTile.Wall_NS] = MakeDef("|", ConsoleColor.Gray),
        [(int)DisplayTile.Wall_ES] = MakeDef("+", ConsoleColor.Gray),
        [(int)DisplayTile.Wall_WS] = MakeDef("+", ConsoleColor.Gray),
        [(int)DisplayTile.Wall_EN] = MakeDef("+", ConsoleColor.Gray),
        [(int)DisplayTile.Wall_NW] = MakeDef("+", ConsoleColor.Gray),
        [(int)DisplayTile.Wall_ENWS] = MakeDef("+", ConsoleColor.Gray),
        [(int)DisplayTile.Wall_ENW] = MakeDef("+", ConsoleColor.Gray),
        [(int)DisplayTile.Wall_EWS] = MakeDef("+", ConsoleColor.Gray),
        [(int)DisplayTile.Wall_NWS] = MakeDef("+", ConsoleColor.Gray),
        [(int)DisplayTile.Wall_ENS] = MakeDef("+", ConsoleColor.Gray),
        [(int)DisplayTile.Floor_Lit] = MakeDef(".", ConsoleColor.DarkGray),
        [(int)DisplayTile.Floor_Dark] = MakeDef(".", ConsoleColor.DarkGray),
        [(int)DisplayTile.Door_Closed_NS] = MakeDef("+", ConsoleColor.DarkYellow),
        [(int)DisplayTile.Door_Open_NS] = MakeDef("/", ConsoleColor.DarkYellow),
        [(int)DisplayTile.Stairs_Up] = MakeDef("<", ConsoleColor.White),
        [(int)DisplayTile.Stairs_Down] = MakeDef(">", ConsoleColor.White),
        [(int)DisplayTile.Sword] = MakeDef("/", ConsoleColor.Cyan),
        [(int)DisplayTile.Armor] = MakeDef("[", ConsoleColor.Cyan)
    };

    public SharpHackTileDef()
    {
        TileSize = new Size(4, 2);
    }

    public override (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) GetTileDef(Enum? tile)
    {
        if (tile == null)
            return _empty;

        var key = Tile2Int(tile);
        return _defs.TryGetValue(key, out var def) ? def : _empty;
    }

    private static (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) MakeDef(string symbol, ConsoleColor foreground, ConsoleColor? background = null)
    {
        var bg = background ?? ConsoleColor.Black;
        var lines = new[] { new string(symbol[0], 4), new string(symbol[0], 4) };
        var colors = new (ConsoleColor fgr, ConsoleColor bgr)[lines.Length * lines[0].Length];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = (foreground, bg);
        return (lines, colors);
    }
}
