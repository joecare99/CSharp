using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using AA15_Labyrinth.Model;
using System.ComponentModel;
using Avalonia.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AA15_Labyrinth.ViewModels;

public partial class LabyrinthViewModel : BaseViewModelCT, ILabyrinthViewModel
{
    private readonly ILabyrinthGenerator _generator;
    private readonly Random _rnd = new();
    private Size _last;
    private Geometry? _maze;
    private Geometry? _sol;
    private double _progress;
    private volatile bool _isGenerating;

    public LabyrinthViewModel(ILabyrinthGenerator generator)
    { _generator = generator; }

    [ObservableProperty]
    public partial int Seed { get; set; } = 1;
    [ObservableProperty]
    public partial double CellSize { get; set; } = 12.0;
    [ObservableProperty]
    public partial double LineThickness { get; set; } = 1.8;
    [ObservableProperty]
    public partial Thickness Padding { get; set; } = new Thickness(12);
    [ObservableProperty]
    public partial bool DrawSolution { get; set; } = false;

    public Geometry? MazeGeometry => _maze;
    public Geometry? SolutionGeometry => _sol;

    public double Progress => _progress;
    public bool IsGenerating => _isGenerating;

    public void Regenerate() { _maze = null; _sol = null; }
    public void Randomize() { Seed = _rnd.Next(); Regenerate(); }

    public void Build(Size bounds)
    {
        // if already generated for this size, nothing to do
        if (_maze != null && _last == bounds) return;
        // if generation already in progress for this size, do nothing
        if (_isGenerating && _last == bounds) return;

        var pad = Padding; var usableW = Math.Max(0, bounds.Width - pad.Left - pad.Right); var usableH = Math.Max(0, bounds.Height - pad.Top - pad.Bottom);
        int cols = Math.Max(6, (int)Math.Floor(usableW / CellSize));
        int rows = Math.Max(6, (int)Math.Floor(usableH / CellSize));
        if (cols < 2 || rows < 2) { _maze = new StreamGeometry(); _sol = null; _last = bounds; return; }

        _last = bounds;
        _maze = null; _sol = null;
        _progress = 0.0; _isGenerating = true;

        double step = Math.Min(usableW / cols, usableH / rows);
        double ox = pad.Left + (usableW - cols * step) / 2.0;
        double oy = pad.Top + (usableH - rows * step) / 2.0;
        Point P(int x, int y) => new(ox + x * step, oy + y * step);
        int Id(int x, int y) => x + y * cols;
        (int x, int y) FromId(int id) => (id % cols, id / cols);

        // run generation on background thread; update progress via IProgress
        var progressReporter = new Progress<double>(p => { _progress = p; });
        Task.Run(() =>
        {
            var lab = _generator.Generate(cols, rows, Seed, progressReporter);

            // convert to adjacency and prepare geometries on UI thread
            Dispatcher.UIThread.Post(() =>
     {
            try
            {
                var adj = new Dictionary<int, List<int>>();
                for (int id = 0; id < lab.Parent.Length; id++)
                {
                    int p = lab.Parent[id];
                    if (p < 0 || p == id) continue;
                    if (!adj.TryGetValue(id, out var l1)) adj[id] = l1 = new();
                    if (!adj.TryGetValue(p, out var l2)) adj[p] = l2 = new();
                    l1.Add(p); l2.Add(id);
                }

             // draw smoothed polylines along chains of degree==2
                var mazeGeom = new StreamGeometry();
                using (var mgc = mazeGeom.Open())
                {
                    var used = new HashSet<(int, int)>();
                    int Deg(int v) => adj.TryGetValue(v, out var ls) ? ls.Count : 0;
                    foreach (var node in adj.Keys.ToList())
                    {
                        if (Deg(node) == 0 || Deg(node) == 2) continue;
                        foreach (var nb in adj[node])
                        {
                            if (!used.Add((node, nb))) continue;
                            var chain = new List<int> { node, nb };
                            int prev = node, cur = nb;
                            while (Deg(cur) == 2)
                            {
                                var next = adj[cur][0] == prev ? adj[cur][1] : adj[cur][0];
                                if (!used.Add((cur, next))) break;
                                chain.Add(next);
                                prev = cur; cur = next;
                            }
                            DrawSmoothed(mgc, chain.Select(id => { var (x, y) = FromId(id); return P(x, y); }).ToList(), CellSize);
                        }
                    }
                }
                _maze = mazeGeom;

             // solution path (BFS) from right/middle to left/middle
                int start = Id(cols - 1, rows / 2);
                int goal = Id(0, rows / 2);
                var solIds = SolvePath(adj, start, goal);
                if (solIds.Count > 0)
                {
                    var sol = new StreamGeometry();
                    using (var sgc = sol.Open())
                    {
                        DrawSmoothed(sgc, solIds.Select(id => { var (x, y) = FromId(id); return P(x, y); }).ToList(), CellSize);
                    }
                    _sol = sol;
                }
                else _sol = null;
            }
            finally
            {
                _progress = 1.0; _isGenerating = false;
            }
        });
        });
    }

    private static List<int> SolvePath(Dictionary<int, List<int>> adj, int start, int goal)
    {
        var prev = new Dictionary<int, int>(); var q = new Queue<int>(); q.Enqueue(start); prev[start] = start;
        while (q.Count > 0) { var v = q.Dequeue(); if (v == goal) break; if (!adj.TryGetValue(v, out var ns)) continue; foreach (var n in ns) { if (prev.ContainsKey(n)) continue; prev[n] = v; q.Enqueue(n); } }
        if (!prev.ContainsKey(goal)) return new();
        var path = new List<int>(); for (int v = goal; ; v = prev[v]) { path.Add(v); if (v == start) break; }
        path.Reverse(); return path;
    }

    private static void DrawSmoothed(StreamGeometryContext gc, IList<Point> list, double radius)
    {
        if (list.Count == 0) return; if (list.Count == 1) { gc.BeginFigure(list[0], false); gc.EndFigure(false); return; }
        gc.BeginFigure(list[0], false);
        for (int i = 1; i < list.Count; i++)
        {
            if (i < list.Count - 1)
            {
                var prev = list[i - 1]; var curr = list[i]; var next = list[i + 1];
                var v1 = new Vector(curr.X - prev.X, curr.Y - prev.Y); var v2 = new Vector(next.X - curr.X, next.Y - curr.Y);
                if (v1.Length <= double.Epsilon || v2.Length <= double.Epsilon) { gc.LineTo(curr); continue; }
                var u1 = v1 / v1.Length; var u2 = v2 / v2.Length; var r = Math.Min(radius, Math.Min(v1.Length / 2, v2.Length / 2));
                var pA = new Point(curr.X - u1.X * r, curr.Y - u1.Y * r); var pB = new Point(curr.X + u2.X * r, curr.Y + u2.Y * r);
                gc.LineTo(pA); gc.QuadraticBezierTo(curr, pB);
            }
            else { gc.LineTo(list[i]); }
        }
        gc.EndFigure(false);
    }
}
