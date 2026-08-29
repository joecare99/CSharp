using ConsoleLib.Interfaces;
using System;
using System.Drawing;

namespace ConsoleLib.CommonControls;

public enum Orientation { Horizontal, Vertical }

public class StackPanel : Control, IGroupControl
{
    private Orientation _orientation = Orientation.Vertical;
    private int _spacing;
    private HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Stretch;
    private VerticalAlignment _verticalAlignment = VerticalAlignment.Stretch;
    public Orientation Orientation { get => _orientation; set { _orientation = value; Arrange(); } }
    public int Spacing { get => _spacing; set { _spacing = Math.Max(0, value); Arrange(); } }
    public HorizontalAlignment HorizontalContentAlignment { get => _horizontalAlignment; set { _horizontalAlignment = value; Arrange(); } }
    public VerticalAlignment VerticalContentAlignment { get => _verticalAlignment; set { _verticalAlignment = value; Arrange(); } }
    public StackPanel() { OnResize += (_, __) => Arrange(); }
    public override IControl Add(IControl control) { var r = base.Add(control); control.OnResize += Changed; control.OnMove += Changed; Arrange(); return r; }
    public override IControl Remove(IControl control) { control.OnResize -= Changed; control.OnMove -= Changed; var r = base.Remove(control); Arrange(); return r; }
    private void Changed(object sender, EventArgs e) { Arrange(); }
    private void Arrange()
    {
        var offset = 0;
        foreach (var child in Children)
        {
            var s = child.size;
            var slot = Orientation == Orientation.Vertical
                ? new Rectangle(0, offset, Dimension.Width, Math.Min(s.Height, Math.Max(0, Dimension.Height - offset)))
                : new Rectangle(offset, 0, Math.Min(s.Width, Math.Max(0, Dimension.Width - offset)), Dimension.Height);
            child.Dimension = Align(child, slot);
            offset += (Orientation == Orientation.Vertical ? slot.Height : slot.Width) + Spacing;
        }
    }
    private Rectangle Align(IControl child, Rectangle slot)
    {
        var s = child.size;
        var w = HorizontalContentAlignment == HorizontalAlignment.Stretch ? slot.Width : Math.Min(slot.Width, s.Width);
        var h = VerticalContentAlignment == VerticalAlignment.Stretch ? slot.Height : Math.Min(slot.Height, s.Height);
        var x = HorizontalContentAlignment == HorizontalAlignment.Right ? slot.Right - w : HorizontalContentAlignment == HorizontalAlignment.Center ? slot.X + (slot.Width - w) / 2 : slot.X;
        var y = VerticalContentAlignment == VerticalAlignment.Bottom ? slot.Bottom - h : VerticalContentAlignment == VerticalAlignment.Center ? slot.Y + (slot.Height - h) / 2 : slot.Y;
        return new Rectangle(x, y, w, h);
    }
    void IGroupControl.BringToFront(IControl control) { if (Children.Contains(control)) { Children.Remove(control); Children.Insert(0, control); } }
}
