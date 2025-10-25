namespace AA15_Labyrinth.Model;

//12-direction grid step (projected to integer lattice)
public readonly record struct Dir(int Dx, int Dy)
{
    public static readonly Dir[] All = BuildAll();
    private static Dir[] BuildAll()
    {
        var list = new System.Collections.Generic.HashSet<(int, int)>();
        var baseDirs = new (int dx, int dy)[] { (1, 0), (0, 1), (2, 1), (1, 2) };
        foreach (var (dx, dy) in baseDirs)
        {
            list.Add((dx, dy)); list.Add((dx, -dy));
            list.Add((-dx, dy)); list.Add((-dx, -dy));
        }
        var arr = new Dir[list.Count];
        int i = 0; foreach (var v in list) arr[i++] = new Dir(v.Item1, v.Item2);
        return arr;
    }
}

public interface ILabyrinthGenerator
{
    // progress: reports values in [0,1]
    Labyrinth Generate(int cols, int rows, int seed, System.IProgress<double>? progress = null);
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
    public Labyrinth Generate(int cols, int rows, int seed, System.IProgress<double>? progress = null)
    {
        cols = System.Math.Max(2, cols);
        rows = System.Math.Max(2, rows);
        int N = cols * rows;
        int Id(int x, int y) => x + y * cols;
        (int x, int y) FromId(int id) => (id % cols, id / cols);
        var rnd = new System.Random(seed);
        var visited = new bool[N];
        var parent = new int[N];
        System.Array.Fill(parent, -1);

        // Kanten-Speicher + räumlicher Index
        var edges = new System.Collections.Generic.List<(int a, int b)>();
        var edgeMinX = new System.Collections.Generic.List<int>();
        var edgeMaxX = new System.Collections.Generic.List<int>();
        var edgeMinY = new System.Collections.Generic.List<int>();
        var edgeMaxY = new System.Collections.Generic.List<int>();
        var buckets = new System.Collections.Generic.Dictionary<long, System.Collections.Generic.List<int>>();
        static long BucketKey(int x, int y) => ((long)x << 32) | (uint)y;
        void AddEdgeToBuckets(int edgeIndex, int minx, int maxx, int miny, int maxy)
        {
            for (int x = minx; x <= maxx; x++)
            {
                for (int y = miny; y <= maxy; y++)
                {
                    var key = BucketKey(x, y);
                    if (!buckets.TryGetValue(key, out var list))
                        buckets[key] = list = new System.Collections.Generic.List<int>(4);
                    list.Add(edgeIndex);
                }
            }
        }

        var stack = new System.Collections.Generic.Stack<int>();
        int start = Id(cols - 1, rows / 2);
        visited[start] = true; parent[start] = start; stack.Push(start);

        int visitedCount = 1;
        progress?.Report(0.0);

        bool WouldCross(int a, int b)
        {
          /*  var (ax, ay) = FromId(a); var (bx, by) = FromId(b);
            int minx = System.Math.Min(ax, bx), maxx = System.Math.Max(ax, bx);
            int miny = System.Math.Min(ay, by), maxy = System.Math.Max(ay, by);

            var seen = new System.Collections.Generic.HashSet<int>();
            for (int x = minx; x <= maxx; x++)
            {
                for (int y = miny; y <= maxy; y++)
                {
                    if (!buckets.TryGetValue(BucketKey(x, y), out var list)) continue;
                    foreach (var ei in list)
                    {
                        if (!seen.Add(ei)) continue; // avoid duplicates
                        var (ea, eb) = edges[ei];
                        if (ea == a || ea == b || eb == a || eb == b) continue;
                        var (x1, y1) = FromId(ea); var (x2, y2) = FromId(eb);
                        if (SegmentsIntersect(ax, ay, bx, by, x1, y1, x2, y2)) return true;
                    }
                }
            }*/
            return false;
        }

        while (stack.Count > 0)
        {
            int cur = stack.Peek();
            var (cx, cy) = FromId(cur);
            var neigh = new System.Collections.Generic.List<(int x, int y)>();
            foreach (var d in Dir.All) { int nx = cx + d.Dx, ny = cy + d.Dy; if (nx >= 0 && ny >= 0 && nx < cols && ny < rows && !visited[Id(nx, ny)]) neigh.Add((nx, ny)); }
            if (neigh.Count == 0) { stack.Pop(); continue; }
            while (neigh.Count > 0)
            {
                int k = rnd.Next(neigh.Count); var (nx, ny) = neigh[k]; neigh.RemoveAt(k); int nid = Id(nx, ny);
                if (!WouldCross(cur, nid)) { visited[nid] = true; parent[nid] = cur; edges.Add((cur, nid));
                    // add to buckets
                    var (ax, ay) = FromId(cur); var (bx, by) = FromId(nid);
                    int minx = System.Math.Min(ax, bx), maxx = System.Math.Max(ax, bx);
                    int miny = System.Math.Min(ay, by), maxy = System.Math.Max(ay, by);
                    edgeMinX.Add(minx); edgeMaxX.Add(maxx); edgeMinY.Add(miny); edgeMaxY.Add(maxy);
                    AddEdgeToBuckets(edges.Count - 1, minx, maxx, miny, maxy);

                    visitedCount++;
                    if (visitedCount % 16 == 0) // report every 16 visits
                        progress?.Report(System.Math.Min(1.0, visitedCount / (double)N));
                    stack.Push(nid); break; }
            }
        }

        progress?.Report(1.0);
        return new Labyrinth { Cols = cols, Rows = rows, Parent = parent };
    }

    private static bool SegmentsIntersect(int ax, int ay, int bx, int by, int cx, int cy, int dx, int dy)
    {
        static int Orient(long px, long py, long qx, long qy, long rx, long ry)
        { long v = (qx - px) * (ry - py) - (qy - py) * (rx - px); return v == 0 ? 0 : (v > 0 ? 1 : -1); }
        static bool OnSeg(long px, long py, long qx, long qy, long rx, long ry)
        => System.Math.Min(px, rx) <= qx && qx <= System.Math.Max(px, rx) && System.Math.Min(py, ry) <= qy && qy <= System.Math.Max(py, ry);
        int o1 = Orient(ax, ay, bx, by, cx, cy); int o2 = Orient(ax, ay, bx, by, dx, dy); int o3 = Orient(cx, cy, dx, dy, ax, ay); int o4 = Orient(cx, cy, dx, dy, bx, by);
        if (o1 == 0 && OnSeg(ax, ay, cx, cy, bx, by)) return true; if (o2 == 0 && OnSeg(ax, ay, dx, dy, bx, by)) return true; if (o3 == 0 && OnSeg(cx, cy, ax, ay, dx, dy)) return true; if (o4 == 0 && OnSeg(cx, cy, bx, by, dx, dy)) return true;
        return (o1 > 0 && o2 < 0 || o1 < 0 && o2 > 0) && (o3 > 0 && o4 < 0 || o3 < 0 && o4 > 0);
    }
}
