using ConsoleLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ConsoleLib.CommonControls;

public enum Dock { Left, Top, Right, Bottom }

public class DockPanel : Control, IGroupControl
{
    private readonly Dictionary<IControl, Dock> _docks = new Dictionary<IControl, Dock>();
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<IControl, PendingDock> Pending = new();
    private bool _lastChildFill = true;
    public bool LastChildFill { get => _lastChildFill; set { _lastChildFill = value; Arrange(); } }
    public DockPanel() { OnResize += (_, __) => Arrange(); }
    public override IControl Add(IControl control) { var r = base.Add(control); if (Pending.TryGetValue(control, out var d)) { _docks[control] = d.Value; Pending.Remove(control); } control.OnResize += Changed; control.OnMove += Changed; Arrange(); return r; }
    public override IControl Remove(IControl control) { control.OnResize -= Changed; control.OnMove -= Changed; var r = base.Remove(control); Arrange(); return r; }
    public static void SetDock(IControl control, Dock dock) {     if (control.Parent is DockPanel p) { p._docks[control] = dock; p.Arrange(); } else
    {
        Pending.Remove(control);
        Pending.Add(control, new PendingDock(dock));
    }
    }
    public static Dock GetDock(IControl control) { return control.Parent is DockPanel p && p._docks.TryGetValue(control, out var d) ? d : Pending.TryGetValue(control, out var pending) ? pending.Value : Dock.Left; }
    private void Changed(object sender, EventArgs e) { Arrange(); }
    private void Arrange()
    {
        var area = new Rectangle(0, 0, Dimension.Width, Dimension.Height);
        for (var i = 0; i < Children.Count; i++)
        {
            var child = Children[i]; var fill = LastChildFill && i == Children.Count - 1;
            var d = GetDock(child); var s = child.size;
            Rectangle slot;
            if (fill) slot = area;
            else if (d == Dock.Left) slot = new Rectangle(area.X, area.Y, Math.Min(s.Width, area.Width), area.Height);
            else if (d == Dock.Right) slot = new Rectangle(area.Right - Math.Min(s.Width, area.Width), area.Y, Math.Min(s.Width, area.Width), area.Height);
            else if (d == Dock.Top) slot = new Rectangle(area.X, area.Y, area.Width, Math.Min(s.Height, area.Height));
            else slot = new Rectangle(area.X, area.Bottom - Math.Min(s.Height, area.Height), area.Width, Math.Min(s.Height, area.Height));
            child.Dimension = slot;
            if (!fill) { if (d == Dock.Left) area.X += slot.Width; else if (d == Dock.Right) area.Width -= slot.Width; else if (d == Dock.Top) area.Y += slot.Height; else area.Height -= slot.Height; }
        }
    }
    void IGroupControl.BringToFront(IControl control) { if (Children.Contains(control)) { Children.Remove(control); Children.Insert(0, control); } }

    private sealed class PendingDock
    {
        public PendingDock(Dock value) => Value = value;
        public Dock Value { get; }
    }
}
