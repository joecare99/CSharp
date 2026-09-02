using ConsoleLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ConsoleLib.CommonControls;

public enum GridUnitType { Auto, Pixel, Star }
public enum HorizontalAlignment { Left, Center, Right, Stretch }
public enum VerticalAlignment { Top, Center, Bottom, Stretch }

public struct GridLength
{
    public GridLength(double value, GridUnitType unitType = GridUnitType.Pixel)
    {
        Value = value < 0 ? 0 : value;
        GridUnitType = unitType;
    }
    public double Value { get; }
    public GridUnitType GridUnitType { get; }
    public static GridLength Auto => new GridLength(1, GridUnitType.Auto);
    public static GridLength Star => new GridLength(1, GridUnitType.Star);
}

public sealed class RowDefinition
{
    public GridLength Height { get; set; } = GridLength.Star;
}

public sealed class ColumnDefinition
{
    public GridLength Width { get; set; } = GridLength.Star;
}

public class Grid : Control, IGroupControl
{
    private readonly Dictionary<IControl, int[]> _attached = new Dictionary<IControl, int[]>();
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<IControl, AttachedValues> Pending = new();
    private bool _arranging;
    private bool _adding;
    public ObservableCollection<RowDefinition> RowDefinitions { get; } = new ObservableCollection<RowDefinition>();
    public ObservableCollection<ColumnDefinition> ColumnDefinitions { get; } = new ObservableCollection<ColumnDefinition>();
    public HorizontalAlignment HorizontalContentAlignment { get; set; } = HorizontalAlignment.Stretch;
    public VerticalAlignment VerticalContentAlignment { get; set; } = VerticalAlignment.Stretch;

    public Grid()
    {
        RowDefinitions.CollectionChanged += (_, __) => Arrange();
        ColumnDefinitions.CollectionChanged += (_, __) => Arrange();
        OnResize += (_, __) => Arrange();
    }

    public override IControl Add(IControl control)
    {
        if (_adding)
            return this;
        _adding = true;
        try
        {
        if (Pending.TryGetValue(control, out var pending))
        {
            _attached[control] = pending.Values;
            Pending.Remove(control);
        }
        var result = base.Add(control);
        control.OnResize += ChildChanged;
        control.OnMove += ChildChanged;
        Arrange();
        return result;
        }
        finally
        {
            _adding = false;
        }
    }

    public override IControl Remove(IControl control)
    {
        control.OnResize -= ChildChanged;
        control.OnMove -= ChildChanged;
        var result = base.Remove(control);
        Arrange();
        return result;
    }

    public static void SetRow(IControl control, int value) { Set(control, 0, value); }
    public static int GetRow(IControl control) { return Get(control, 0); }
    public static void SetColumn(IControl control, int value) { Set(control, 1, value); }
    public static int GetColumn(IControl control) { return Get(control, 1); }
    public static void SetRowSpan(IControl control, int value) { Set(control, 2, Math.Max(1, value)); }
    public static int GetRowSpan(IControl control) { return Math.Max(1, Get(control, 2, 1)); }
    public static void SetColumnSpan(IControl control, int value) { Set(control, 3, Math.Max(1, value)); }
    public static int GetColumnSpan(IControl control) { return Math.Max(1, Get(control, 3, 1)); }

    private static void Set(IControl control, int index, int value)
    {
        var grid = control.Parent as Grid;
        if (grid == null)
        {
            if (!Pending.TryGetValue(control, out var pending))
            {
                pending = new AttachedValues();
                Pending.Add(control, pending);
            }
            pending.Values[index] = Math.Max(0, value);
            return;
        }
        if (!grid._attached.TryGetValue(control, out var values))
            grid._attached[control] = values = new[] { 0, 0, 1, 1 };
        values[index] = Math.Max(0, value);
        grid.Arrange();
    }

    private static int Get(IControl control, int index, int fallback = 0)
    {
        if (Pending.TryGetValue(control, out var pending))
            return pending.Values[index];
        return control.Parent is Grid grid && grid._attached.TryGetValue(control, out var values)
            ? values[index] : fallback;
    }

    private void ChildChanged(object sender, EventArgs e) { Arrange(); }

    private int[] Resolve(IList<GridLength> definitions, int available)
    {
        var result = new int[definitions.Count];
        var remaining = Math.Max(0, available);
        double stars = 0;
        for (var i = 0; i < definitions.Count; i++)
        {
            var d = definitions[i];
            if (d.GridUnitType == GridUnitType.Pixel) result[i] = Math.Max(0, (int)Math.Round(d.Value));
            else if (d.GridUnitType == GridUnitType.Auto) result[i] = AutoSize(i);
            else stars += d.Value;
            remaining -= result[i];
        }
        remaining = Math.Max(0, available - Sum(result));
        for (var i = 0; i < definitions.Count; i++)
            if (definitions[i].GridUnitType == GridUnitType.Star && stars > 0)
                result[i] = (int)Math.Floor(remaining * definitions[i].Value / stars);
        return result;
    }

    private int AutoSize(int index)
    {
        var max = 0;
        foreach (var child in Children)
        {
            var pos = _attached.TryGetValue(child, out var v) ? v : new[] { 0, 0, 1, 1 };
            if (pos[0] == index) max = Math.Max(max, child.size.Height);
        }
        return max;
    }

    private void Arrange()
    {
        if (_arranging)
            return;
        if (Children.Count == 0) return;
        if (RowDefinitions.Count == 0) RowDefinitions.Add(new RowDefinition());
        if (ColumnDefinitions.Count == 0) ColumnDefinitions.Add(new ColumnDefinition());
        _arranging = true;
        try
        {
            {
                var rows = ResolveRows(Dimension.Height);
                var cols = ResolveColumns(Dimension.Width);
                foreach (var child in Children)
                {
                    if (!_attached.ContainsKey(child)) _attached[child] = new[] { 0, 0, 1, 1 };
                    var p = _attached[child];
                    var r = Math.Min(p[0], rows.Length - 1); var c = Math.Min(p[1], cols.Length - 1);
                    var rs = Math.Min(p[2], rows.Length - r); var cs = Math.Min(p[3], cols.Length - c);
                    var x = Sum(cols, 0, c); var y = Sum(rows, 0, r);
                    var w = Sum(cols, c, cs); var h = Sum(rows, r, rs);
                    child.Dimension = Align(child, new Rectangle(x, y, w, h), HorizontalContentAlignment, VerticalContentAlignment);
                }
            }
        }
        finally
        {
            _arranging = false;
        }
    }

    private int[] ResolveRows(int available) { var list = new List<GridLength>(); foreach (var x in RowDefinitions) list.Add(x.Height); return Resolve(list, available); }
    private int[] ResolveColumns(int available) { var list = new List<GridLength>(); foreach (var x in ColumnDefinitions) list.Add(x.Width); return Resolve(list, available); }
    private static int Sum(int[] values, int start = 0, int count = -1) { if (count < 0) count = values.Length - start; var n = 0; for (var i = start; i < start + count && i < values.Length; i++) n += values[i]; return n; }
    private static Rectangle Align(IControl child, Rectangle slot, HorizontalAlignment h, VerticalAlignment v)
    {
        var s = child.size; var w = h == HorizontalAlignment.Stretch ? slot.Width : Math.Min(slot.Width, s.Width); var ht = v == VerticalAlignment.Stretch ? slot.Height : Math.Min(slot.Height, s.Height);
        var x = h == HorizontalAlignment.Right ? slot.Right - w : h == HorizontalAlignment.Center ? slot.X + (slot.Width - w) / 2 : slot.X;
        var y = v == VerticalAlignment.Bottom ? slot.Bottom - ht : v == VerticalAlignment.Center ? slot.Y + (slot.Height - ht) / 2 : slot.Y;
        return new Rectangle(x, y, w, ht);
    }

    private void BringToFront(IControl control) { if (Children.Contains(control)) { Children.Remove(control); Children.Insert(0, control); } }
    void IGroupControl.BringToFront(IControl control) { BringToFront(control); }

    private sealed class AttachedValues
    {
        public int[] Values { get; } = { 0, 0, 1, 1 };
    }
}
