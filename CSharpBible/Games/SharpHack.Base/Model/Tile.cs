using SharpHack.Base.Data;
using SharpHack.Base.Interfaces;
using System.Collections.Generic;

namespace SharpHack.Base.Model;

public class Tile : ITile
{
    public Point Position { get; set; }
    public TileType Type { get; set; }
    public IList<IItem> Items { get; } = [];
    public ICreature? Creature { get; set; }

    public bool IsVisible { get; set; }
    public bool IsExplored { get; set; }

    public bool IsWalkable => Type is not TileType.Wall and not TileType.Empty and not TileType.DoorClosed;
    public bool IsTransparent => Type is not TileType.Wall and not TileType.DoorClosed;

    public Point OldPosition => Creature?.OldPosition ?? Position;
}
