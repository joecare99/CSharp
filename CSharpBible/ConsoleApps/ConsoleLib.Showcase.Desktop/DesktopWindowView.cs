using System;
using System.Drawing;
using ConsoleLib.CommonControls;
using ConsoleLib.Data;
using ConsoleLib.Interfaces;
using ConsoleLib.Showcase.Services;

namespace ConsoleLib.Showcase.Desktop;

/// <summary>Renderer-neutral visual chrome for one application window.</summary>
public sealed class DesktopWindowView : Panel
{
    private readonly DesktopWindow _state;
    private readonly Label _title;
    private readonly IControl _content;
    private readonly Button _close;
    private readonly Button _minimize;
    private bool _isDragging;
    private Point? _dragOrigin;
    private Point _dragStartPosition;

    public DesktopWindowView(
        DesktopWindow state,
        IControl content,
        Action<DesktopWindow> close,
        Action<DesktopWindow> minimize,
        Action<DesktopWindow> activate)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        _content = content ?? throw new ArgumentNullException(nameof(content));
        if (close is null)
            throw new ArgumentNullException(nameof(close));
        if (minimize is null)
            throw new ArgumentNullException(nameof(minimize));
        if (activate is null)
            throw new ArgumentNullException(nameof(activate));

        BorderStyle = BorderStyle.Single;
        BorderColor = ConsoleColor.DarkCyan;
        ForeColor = ConsoleColor.Gray;
        BackColor = ConsoleColor.Black;
        size = new Size(40, 16);

        _title = new Label
        {
            Parent = this,
            Position = new Point(1, 0),
            size = new Size(30, 1),
            Text = state.Title,
            ForeColor = ConsoleColor.Yellow
        };
        _title.OnClick += (_, _) => activate(_state);
        _title.OnMouseMove += (_, mouseEvent) => HandleTitleDrag(mouseEvent);
        OnClick += (_, _) => activate(_state);

        _minimize = new Button
        {
            Parent = this,
            Position = new Point(31, 0),
            size = new Size(3, 1),
            Text = "_"
        };
        _minimize.OnClick += (_, _) => minimize(_state);

        _close = new Button
        {
            Parent = this,
            Position = new Point(35, 0),
            size = new Size(3, 1),
            Text = "X"
        };
        _close.OnClick += (_, _) => close(_state);

        _content.Parent = this;
        _content.Position = new Point(1, 1);
        _content.size = new Size(Math.Max(1, size.Width - 2), Math.Max(1, size.Height - 2));
        OnResize += WindowView_Resize;
    }

    /// <summary>Window state represented by this view.</summary>
    public DesktopWindow State => _state;

    /// <summary>Updates the visual state after manager changes.</summary>
    public void Synchronize()
    {
        Visible = _state.IsOpen && !_state.IsMinimized;
        _title.Text = _state.Title;
        _content.Visible = Visible;
        Invalidate();
    }

    private void HandleTitleDrag(IMouseEvent mouseEvent)
    {
        if (mouseEvent.MouseButtonLeft)
        {
            if (!_isDragging)
            {
                _isDragging = true;
                _dragOrigin = mouseEvent.MousePos;
                _dragStartPosition = Position;
            }

            if (_dragOrigin is not null)
            {
                var deltaX = mouseEvent.MousePos.X - _dragOrigin.Value.X;
                var deltaY = mouseEvent.MousePos.Y - _dragOrigin.Value.Y;
                var maxX = Math.Max(0, (Parent?.size.Width ?? size.Width) - size.Width);
                var maxY = Math.Max(0, (Parent?.size.Height ?? size.Height) - size.Height);
                Position = new Point(
                    Math.Clamp(_dragStartPosition.X + deltaX, 0, maxX),
                    Math.Clamp(_dragStartPosition.Y + deltaY, 0, maxY));
            }

            return;
        }

        _isDragging = false;
        _dragOrigin = null;
    }

    private void WindowView_Resize(object? sender, EventArgs e)
    {
        _content.size = new Size(Math.Max(1, size.Width - 2), Math.Max(1, size.Height - 2));
        _minimize.Position = new Point(Math.Max(1, size.Width - 9), 0);
        _close.Position = new Point(Math.Max(1, size.Width - 5), 0);
        _title.size = new Size(Math.Max(1, size.Width - 12), 1);
    }
}
