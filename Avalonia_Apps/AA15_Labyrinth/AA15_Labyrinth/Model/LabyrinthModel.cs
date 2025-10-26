using System.Data;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;

namespace AA15_Labyrinth.Model;

//12-direction grid step (projected to integer lattice)
public readonly record struct Dir(int Dx, int Dy)
{
    public static readonly Dir[] All = BuildAll();
    private static Dir[] BuildAll()
    {
        HashSet<(int, int)> list = new HashSet<(int, int)>();
        (int dx, int dy)[] baseDirs = new (int dx, int dy)[] { (2, 0), (2, 1), (1, 2) };
        foreach ((int dx, int dy) in baseDirs)
        {
            list.Add((dx, dy));
            list.Add((dy, -dx));
            list.Add((-dy, dx));
            list.Add((-dx, -dy));
        }
        Dir[] arr = new Dir[list.Count];
        int i = 0; foreach ((int, int) v in list) arr[i++] = new Dir(v.Item1, v.Item2);
        return arr;
    }
}

public interface ILabyrinthGenerator
{
    // progress: reports values in [0,1]
    Labyrinth Generate(int cols, int rows, int seed, IProgress<double>? progress = null);
}

public sealed class Labyrinth
{
    public required int Cols { get; init; }
    public required int Rows { get; init; }
    // parent tree: index -> parent index; root points to itself
    public required int[] Parent { get; init; }
}

public sealed class LabyrinthGenerator : ILabyrinthGenerator
{
    public Labyrinth Generate(int cols, int rows, int seed, IProgress<double>? progress = null)
    {
        cols = Math.Max(2, cols);
        rows = Math.Max(2, rows);
        int N = cols * rows;

        int Id(int x, int y)
            => x + y * cols;

        (int x, int y) FromId(int id)
            => (id % cols, id / cols);

        Random rnd = new Random(seed);
        bool[] visited = new bool[N];
        int[] parent = new int[N];
        Array.Fill(parent, -1);

        // Kanten-Speicher + räumlicher Index
        List<(int a, int b)> edges = new List<(int a, int b)>();

        Dictionary<long, List<int>> buckets = new Dictionary<long, List<int>>();

        Stack<int> stack = new Stack<int>();
        int start = Id(cols - 1, rows / 2);
        visited[start] = true; parent[start] = start; stack.Push(start);

        int visitedCount = 1;
        progress?.Report(0.0);

        bool WouldCross(int a, int b)
        {
            (int ax, int ay) = FromId(a);
            (int bx, int by) = FromId(b);
            int minx = Math.Min(ax, bx), maxx = Math.Max(ax, bx);
            int miny = Math.Min(ay, by), maxy = Math.Max(ay, by);

            HashSet<int> seen = new HashSet<int>();
            for (int x = minx - 1; x <= maxx + 1; x++)
            {
                for (int y = miny - 1; y <= maxy + 1; y++)
                {
                    //  if (Id(x, y) == a || Id(x, y) == b) continue;
                    if (!buckets.TryGetValue(BucketKey(x, y), out List<int>? list)) continue;
                    foreach (int ei in list)
                    {
                        if (!seen.Add(ei)) continue; // avoid duplicates
                        (int ea, int eb) = edges[ei];
                        if (ea == a || ea == b || eb == a || eb == b) continue;
                        (int x1, int y1) = FromId(ea);
                        (int x2, int y2) = FromId(eb);
                        if (SegmentsIntersect(ax, ay, bx, by, x1, y1, x2, y2))
                            return true;
                    }
                }
            }
            return false;
        }

        while (stack.Count > 0)
        {
            int cur = stack.Peek();
            (int cx, int cy) = FromId(cur);
            List<(int x, int y)> neigh = new List<(int x, int y)>();
            foreach (Dir d in Dir.All)
            {
                int nx = cx + d.Dx,
                    ny = cy + d.Dy;
                if (nx >= 0 && ny >= 0 && nx < cols && ny < rows
                    && !visited[Id(nx, ny)]
                    && !buckets.ContainsKey(BucketKey((cx + nx) / 2, (cy + ny) / 2))
                    && !buckets.ContainsKey(BucketKey(nx, ny))
                    && !WouldCross(cur, Id(nx, ny)))
                    neigh.Add((nx, ny));
            }
            if (neigh.Count == 0)
            {
                stack.Pop();
                continue;
            }
            while (neigh.Count > 0)
            {
                int k = rnd.Next(neigh.Count);
                (int nx, int ny) = neigh[k];
                neigh.RemoveAt(k);
                int nid = Id(nx, ny);
                if (true)
                {
                    visited[nid] = true;
                    parent[nid] = cur;
                    edges.Add((cur, nid));
                    // add to buckets
                    (int ax, int ay) = FromId(cur);
                    (int bx, int by) = FromId(nid);
                    AddEdgeToBuckets(edges.Count - 1, ax, ay, bx, by, buckets);

                    visitedCount++;
                    if (visitedCount % 16 == 0) // report every 16 visits
                        progress?.Report(Math.Min(1.0, visitedCount / (double)N));
                    stack.Push(nid); break;
                }
            }
        }

        progress?.Report(1.0);
        return new Labyrinth { Cols = cols, Rows = rows, Parent = parent };
    }

    private static long BucketKey(int x, int y) => ((long)x << 32) | (uint)y;

    public static void AddEdgeToBuckets(int edgeIndex, int ax, int ay, int bx, int by, Dictionary<long, List<int>> buckets)
    {
        for (int i = 0; i <= 3; i++)
        {
            (int x, int y) = i switch
            {
                0 => (ax, ay),
                1 => ((ax + bx) / 2, (ay + by) / 2),
                2 => ((ax + bx + 1) / 2, (ay + by + 1) / 2),
                _ => (bx, by)
            };
            long key = BucketKey(x, y);
            if (!buckets.TryGetValue(key, out List<int>? list))
                buckets[key] = list = new List<int>(4);
            if (!list.Contains(edgeIndex))
                list.Add(edgeIndex);

        }
    }

    public static bool SegmentsIntersect(int ax, int ay, int bx, int by, int cx, int cy, int dx, int dy)
    {
        static int Orient(long px, long py, long qx, long qy, long rx, long ry)
        {
            long v = (qx - px) * (ry - py) - (qy - py) * (rx - px);
            return v == 0 ? 0 : (v > 0 ? 1 : -1);
        }
        static bool OnSeg(long px, long py, long qx, long qy, long rx, long ry)
        => Math.Min(px, rx) <= qx && qx <= Math.Max(px, rx) && Math.Min(py, ry) <= qy && qy <= Math.Max(py, ry);
        int o1 = Orient(ax, ay, bx, by, cx, cy);
        int o2 = Orient(ax, ay, bx, by, dx, dy);
        int o3 = Orient(cx, cy, dx, dy, ax, ay);
        int o4 = Orient(cx, cy, dx, dy, bx, by);
        if (o1 == 0 && OnSeg(ax, ay, cx, cy, bx, by)) return true;
        if (o2 == 0 && OnSeg(ax, ay, dx, dy, bx, by)) return true;
        if (o3 == 0 && OnSeg(cx, cy, ax, ay, dx, dy)) return true;
        if (o4 == 0 && OnSeg(cx, cy, bx, by, dx, dy)) return true;
        return (o1 > 0 == o2 > 0) && (o3 > 0 == o4 > 0);
    }
}
