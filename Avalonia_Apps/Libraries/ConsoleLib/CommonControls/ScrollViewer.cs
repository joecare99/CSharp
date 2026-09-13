using System;
using System.Drawing;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>Provides bounded scrolling offsets for a hosted content control.</summary>
public sealed class ScrollViewer : Panel
{
    private Point _offset;

    public IControl? Content { get; private set; }
    public override Point Offset => _offset;

    public void SetContent(IControl content)
    {
        if (Content is not null)
            Content.Parent = null;
        Content = content ?? throw new ArgumentNullException(nameof(content));
        AddChild(content);
        ClampOffset();
    }

    public void ScrollBy(int horizontal, int vertical)
    {
        _offset = new Point(_offset.X + horizontal, _offset.Y + vertical);
        ClampOffset();
        Invalidate();
    }

    private void ClampOffset()
    {
        if (Content is null)
            return;
        var maxX = Math.Max(0, Content.size.Width - size.Width);
        var maxY = Math.Max(0, Content.size.Height - size.Height);
        _offset = new Point(Math.Max(0, Math.Min(maxX, _offset.X)), Math.Max(0, Math.Min(maxY, _offset.Y)));
    }
}
