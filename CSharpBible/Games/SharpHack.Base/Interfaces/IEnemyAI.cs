using SharpHack.Base.Model;

namespace SharpHack.Base.Interfaces;

public interface IEnemyAI
{
    Point GetNextMove(Creature enemy, Creature target, Map map);
}
