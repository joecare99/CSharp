using System.Collections.Generic;

namespace SharpHack.Base.Model;

public enum TileType
{
    Empty,
    Floor,
    Wall,
    DoorClosed,
    DoorOpen,
    StairsUp,
    StairsDown
}

public class Tile
{
    public Point Position { get; set  { OldPosition = field; field = value; } }
    public TileType Type { get; set; }
    public List<Item> Items { get; } = new();
    public Creature? Creature { get; set; }
    
    public bool IsVisible { get; set; }
    public bool IsExplored { get; set; }

    public bool IsWalkable => Type != TileType.Wall && Type != TileType.Empty && (Type != TileType.DoorClosed);
    public bool IsTransparent => Type != TileType.Wall && Type != TileType.DoorClosed;

    public Point OldPosition { get; internal set; }
}
