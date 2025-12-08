using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAnalysis.Core.Models;

namespace DataAnalysis.WPF.ViewModels;

public sealed partial class ClusterAggregationViewModel : AggregationItemViewModel
{
    public sealed class PointItem { public double X { get; init; } public double Y { get; init; } public int Count { get; init; } }
    public sealed class AxisTick { public double Value { get; init; } public string Label { get; init; } = string.Empty; }

    public ObservableCollection<PointItem> Points { get; } = new();
    public ObservableCollection<PointItem> RawPoints { get; } = new();
    public ObservableCollection<AxisTick> XTicks { get; } = new();
    public ObservableCollection<AxisTick> YTicks { get; } = new();

    [ObservableProperty] private double minX;
    [ObservableProperty] private double maxX;
    [ObservableProperty] private double minY;
    [ObservableProperty] private double maxY;
    [ObservableProperty] private int maxCount;
    [ObservableProperty] private bool hasZeroX;
    [ObservableProperty] private bool hasZeroY;

    public ClusterAggregationViewModel(AggregationResult agg) : base(agg.Title) { Rebuild(agg); }

    private void Rebuild(AggregationResult agg)
    {
        Points.Clear(); RawPoints.Clear(); XTicks.Clear(); YTicks.Clear();
        double minx = double.PositiveInfinity, maxx = double.NegativeInfinity;
        double miny = double.PositiveInfinity, maxy = double.NegativeInfinity; int maxc = 0;
        if (agg.Series is not null)
        {
            foreach (var kv in agg.Series)
            {
                if (TryGetXY(kv.Key, out var x, out var y))
                {
                    Points.Add(new PointItem { X = x, Y = y, Count = kv.Value });
                    if (x < minx) minx = x; if (x > maxx) maxx = x;
                    if (y < miny) miny = y; if (y > maxy) maxy = y;
                    if (kv.Value > maxc) maxc = kv.Value;
                }
            }
        }
        if (double.IsInfinity(minx) || double.IsInfinity(miny)) { minx = 0; maxx = 1; miny = 0; maxy = 1; maxc = 1; }
        MinX = minx; MaxX = maxx; MinY = miny; MaxY = maxy; MaxCount = maxc;
        HasZeroX = MinX <= 0 && MaxX >= 0; HasZeroY = MinY <= 0 && MaxY >= 0;
        BuildTicks(XTicks, MinX, MaxX, 6); BuildTicks(YTicks, MinY, MaxY, 6);
    }

    private static void BuildTicks(ObservableCollection<AxisTick> target, double min, double max, int desired)
    {
        target.Clear(); if (max <= min) { target.Add(new AxisTick { Value = min, Label = min.ToString("G4") }); return; }
        var range = max - min; var rawStep = range / desired; double magnitude = System.Math.Pow(10, System.Math.Floor(System.Math.Log10(rawStep)));
        double normalized = rawStep / magnitude; double step = normalized < 1.5 ? 1 * magnitude : normalized < 3 ? 2 * magnitude : normalized < 7 ? 5 * magnitude : 10 * magnitude;
        double start = System.Math.Ceiling(min / step) * step;
        for (double v = start; v <= max + step * 0.001; v += step) target.Add(new AxisTick { Value = v, Label = FormatTick(v, range) });
        if (min <= 0 && max >= 0 && !target.Any(t => System.Math.Abs(t.Value) < step * 0.001)) target.Add(new AxisTick { Value = 0, Label = FormatTick(0, range) });
        var ordered = target.OrderBy(t => t.Value).ToList(); if (ordered.Count != target.Count) { target.Clear(); foreach (var t in ordered) target.Add(t); }
    }

    private static string FormatTick(double value, double range)
    {
        if (range == 0) return value.ToString("G4"); var absRange = System.Math.Abs(range);
        if (absRange >= 100000 || absRange <= 0.001) return value.ToString("G3");
        if (absRange >= 1000) return value.ToString("G4"); 
        if (absRange >= 1) return value.ToString("G5"); 
        return value.ToString("G3");
    }

    private static bool TryGetXY(object key, out double x, out double y)
    {
        x = y = 0; if (key is null) return false; var t = key.GetType();
        var px = t.GetProperty("X"); var py = t.GetProperty("Y"); if (px is not null && py is not null)
        {
            var vx = px.GetValue(key); var vy = py.GetValue(key);
            if (TryToDouble(vx, out x) && TryToDouble(vy, out y)) return true;
        }
        var fx = t.GetField("X"); var fy = t.GetField("Y"); if (fx is not null && fy is not null)
        {
            var vx = fx.GetValue(key); var vy = fy.GetValue(key);
            if (TryToDouble(vx, out x) && TryToDouble(vy, out y)) return true;
        }
        if (t.IsGenericType && t.FullName!.StartsWith("System.ValueTuple`2"))
        {
            var f1 = t.GetField("Item1"); var f2 = t.GetField("Item2"); if (f1 is not null && f2 is not null)
            {
                var v1 = f1.GetValue(key); var v2 = f2.GetValue(key);
                if (TryToDouble(v1, out x) && TryToDouble(v2, out y)) return true;
            }
        }
        if (key is ITuple tuple && tuple.Length >= 2)
        {
            var v1 = tuple[0]; var v2 = tuple[1];
            if (TryToDouble(v1, out x) && TryToDouble(v2, out y)) return true;
        }
        if (key is System.Collections.IList list && list.Count >= 2)
        {
            var v1 = list[0]; var v2 = list[1];
            if (TryToDouble(v1, out x) && TryToDouble(v2, out y)) return true;
        }
        var s = key.ToString() ?? string.Empty; var m = System.Text.RegularExpressions.Regex.Match(s, @"\(?\s*(-?[0-9]+(?:\.[0-9]+)?)\s*,\s*(-?[0-9]+(?:\.[0-9]+)?)\s*\)?");
        if (m.Success)
        {
            if (double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out x) &&
                double.TryParse(m.Groups[2].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out y)) return true;
        }
        return false;
    }

    private static bool TryToDouble(object? v, out double d) { try { d = System.Convert.ToDouble(v, System.Globalization.CultureInfo.InvariantCulture); return true; } catch { d = 0; return false; } }
}
