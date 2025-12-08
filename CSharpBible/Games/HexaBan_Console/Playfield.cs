using HexaBan.Models.Interfaces;
using System;

public class Playfield
{
    public int Width => Tiles.GetLength(1);
    public int Height => Tiles.GetLength(0);
    public TileType[,] Tiles { get; private set; }
    public (int X, int Y) PlayerPosition { get; set; }

    public Playfield()
    {  }

    public void LoadLevel((TileType[,] tiles, (int X, int Y) playerStart) level)
    {
        Tiles = level.tiles;
        PlayerPosition = level.playerStart;
    }
}
