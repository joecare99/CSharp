using SharpHack.Base.Data;
using SharpHack.Base.Model;

namespace SharpHack.LevelGen;

public class SimpleMapGenerator : IMapGenerator
{
    public Map Generate(int width, int height,Point? point = null)
    {
        var map = new Map(width, height);

        // Fill with walls
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y].Type = TileType.Wall;
            }
        }

        // Create a simple room
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                map[x, y].Type = TileType.Floor;
            }
        }

        return map;
    }
}
