using SharpHack.Base.Data;
using SharpHack.Base.Model;

namespace SharpHack.Base.Interfaces;

public interface ITile
{
    ICreature? Creature { get; set; }
    bool IsExplored { get; set; }
    bool IsTransparent { get; }
    bool IsVisible { get; set; }
    bool IsWalkable { get; }
    IList<IItem> Items { get; }
    Point OldPosition { get; }
    Point Position { get; set; }
    TileType Type { get; set; }
}