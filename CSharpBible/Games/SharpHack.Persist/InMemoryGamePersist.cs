using SharpHack.Base.Interfaces;
using SharpHack.Base.Model;
using System.Collections.Generic;

namespace SharpHack.Persist;

public class InMemoryGamePersist : IGamePersist
{
    private Dictionary<int,(IMap, ICreature, IList<ICreature>)> _memory = new();

    public bool LoadLevel(int level, out IMap? map, out IList<ICreature>? enemies)
    {
        var result= _memory.TryGetValue(level, out var m);
        if (result)
        {
            map = m.Item1;
            enemies = [];
            foreach (var tile in map)
            {
                if (tile is Tile t && t.Creature is not null)
                    enemies.Add(t.Creature);
            }
        }
        else
        {
            map = null;
            enemies = null;
        }
        return result;
    }

    public void SaveLevel(int level, IMap map, ICreature player, IList<ICreature> enemies)
    {
        _memory[level] = (map, player, enemies);
    }
}