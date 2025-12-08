using HexaBan.Models.Interfaces;

namespace HexaBan.Models;

public class HexTile
{
    public TileType Type { get; set; }
    public bool HasBox { get; set; }
    public bool HasPlayer { get; set; }
}
