using SharpHack.Base.Model;

namespace SharpHack.Base.Interfaces;

public interface IEnemyAI
{
    Point GetNextMove(ICreature enemy, ICreature target, IMap map);
}
