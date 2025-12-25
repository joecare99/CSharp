using System;
using SharpHack.Base.Interfaces;
using SharpHack.Base.Model;

namespace SharpHack.AI;

public class SimpleEnemyAI : IEnemyAI
{
    public Point GetNextMove(Creature enemy, Creature target, Map map)
    {
        int dx = target.Position.X - enemy.Position.X;
        int dy = target.Position.Y - enemy.Position.Y;

        int stepX = dx == 0 ? 0 : dx / Math.Abs(dx);
        int stepY = dy == 0 ? 0 : dy / Math.Abs(dy);

        // Try moving diagonally first if needed
        if (stepX != 0 && stepY != 0)
        {
            var diagPos = new Point(enemy.Position.X + stepX, enemy.Position.Y + stepY);
            if (IsWalkable(map, diagPos)) return diagPos;
        }

        // Try moving X
        if (stepX != 0)
        {
            var xPos = new Point(enemy.Position.X + stepX, enemy.Position.Y);
            if (IsWalkable(map, xPos)) return xPos;
        }

        // Try moving Y
        if (stepY != 0)
        {
            var yPos = new Point(enemy.Position.X, enemy.Position.Y + stepY);
            if (IsWalkable(map, yPos)) return yPos;
        }

        return enemy.Position;
    }

    private bool IsWalkable(Map map, Point p)
    {
        return map.IsValid(p) && map[p].IsWalkable && map[p].Creature == null;
    }
}
