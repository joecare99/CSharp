using SharpHack.Base.Model;

namespace SharpHack.Base.Interfaces;

public interface IGamePersist
{
    bool LoadLevel(int level, out IMap map, out IList<ICreature> enemies);
    void SaveLevel(int level, IMap map, ICreature player, IList<ICreature> enemies);
}