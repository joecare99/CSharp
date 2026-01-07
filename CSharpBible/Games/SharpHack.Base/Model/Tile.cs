using SharpHack.Base.Data;
using SharpHack.Base.Interfaces;
using System.Collections.Generic;

namespace SharpHack.Base.Model;

public class Tile : ITile
{
    public Point Position { get; set; }
    public TileType Type { get; set; }
    public IList<Item> Items { get; } = [];
    public ICreature? Creature { get; set; }

    public bool IsVisible { get; set; }
    public bool IsExplored { get; set; }

    public bool IsWalkable => Type != TileType.Wall && Type != TileType.Empty && (Type != TileType.DoorClosed);
    public bool IsTransparent => Type != TileType.Wall && Type != TileType.DoorClosed;

    public Point OldPosition => Creature?.OldPosition ?? Position;
}
