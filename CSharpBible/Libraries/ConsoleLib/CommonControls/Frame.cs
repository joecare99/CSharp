using System;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>Hosts one non-floating <see cref="Page"/> at a time.</summary>
public sealed class Frame : Control
{
    public Page? Content { get; private set; }

    public Frame() => OnResize += Frame_OnResize;

    public void SetContent(Page page)
    {
        if (page is null)
            throw new ArgumentNullException(nameof(page));

        if (ReferenceEquals(Content, page))
            return;

        if (Content is not null)
            Remove(Content);

        Content = page;
        Add(page);
        page.Position = System.Drawing.Point.Empty;
        page.size = size;
        Invalidate();
    }

    public void ClearContent()
    {
        if (Content is null)
            return;

        Remove(Content);
        Content = null;
        Invalidate();
    }

    public override void Draw() => WidgetSet?.DrawControl(this);

    private void Frame_OnResize(object? sender, EventArgs e)
    {
        if (Content is not null)
            Content.size = size;
    }
}
