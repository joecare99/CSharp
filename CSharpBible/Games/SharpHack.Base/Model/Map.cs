using System.Collections.Generic;

namespace SharpHack.Base.Model;

public class Map
{
    public int Width { get; }
    public int Height { get; }
    private readonly Tile[,] _tiles;

    public Map(int width, int height)
    {
        Width = width;
        Height = height;
        _tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _tiles[x, y] = new Tile { Position = new Point(x, y), Type = TileType.Empty };
            }
        }
    }

    public Tile this[int x, int y]
    {
        get
        {
            if (IsValid(x, y))
                return _tiles[x, y];
            return new Tile { Position = new Point(x, y), Type = TileType.Empty };
        }
    }
    
    public Tile this[Point p] => this[p.X, p.Y];

    public bool IsValid(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
    public bool IsValid(Point p) => IsValid(p.X, p.Y);
}
