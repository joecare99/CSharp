using System;
using System.Collections.Generic;
using SharpHack.Base.Interfaces;
using SharpHack.Base.Model;

namespace SharpHack.Engine.Pathfinding;

public static class AStarPathfinder
{
    public static List<Point>? FindPath(this IMap map, Point start, Point goal, Func<Point, bool>? canEnter = null, bool goDiagonal = true)
    {
        canEnter ??= (p => map[p].IsWalkable);
        if (!map.IsValid(start) || !map.IsValid(goal) || !canEnter(goal))
        {
            return null;
        }

        var open = new PriorityQueue<Point, int>();
        var cameFrom = new Dictionary<Point, Point>();
        var gScore = new Dictionary<Point, int>
        {
            [start] = 0
        };

        open.Enqueue(start, Heuristic(start, goal));

        while (open.Count > 0)
        {
            var current = open.Dequeue();

            if (current == goal)
            {
                return ReconstructPath(cameFrom, current);
            }

            foreach (var n in GetNeighbors8(current,goDiagonal))
            {
                if (!map.IsValid(n) || !canEnter(n))
                {
                    continue;
                }

                var tentative = gScore[current] + (map[n].IsWalkable?1:2);

                if (!gScore.TryGetValue(n, out var existing) || tentative < existing)
                {
                    cameFrom[n] = current;
                    gScore[n] = tentative;
                    var f = tentative + Heuristic(n, goal)*2;
                    open.Enqueue(n, f);
                }
            }
        }

        return null;
    }

    private static int Heuristic(Point a, Point b)
    {
        // Chebyshev distance for 8-direction movement.
        return Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
    }

    public static IEnumerable<Point> GetNeighbors8(this Point p,bool goDiagonal = true)
    {
        yield return new Point(p.X + 1, p.Y);
        if (goDiagonal) yield return new Point(p.X + 1, p.Y + 1);
        yield return new Point(p.X, p.Y + 1);
        if (goDiagonal) yield return new Point(p.X - 1, p.Y + 1);
        yield return new Point(p.X - 1, p.Y);
        if (goDiagonal) yield return new Point(p.X - 1, p.Y - 1);
        yield return new Point(p.X, p.Y - 1);
        if (goDiagonal) yield return new Point(p.X + 1, p.Y - 1);
    }

    private static List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
    {
        var path = new List<Point> { current };
        while (cameFrom.TryGetValue(current, out var prev))
        {
            current = prev;
            path.Add(current);
        }

        path.Reverse();
        return path;
    }
}
