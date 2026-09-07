using System;
using System.Drawing;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Rendering;

/// <summary>Maintains a synchronous canonical frame for one attached control tree.</summary>
public sealed class AttachedRenderService : IDisposable
{
    private readonly IControlFrameRenderer _renderer;
    private IControl? _root;
    private TerminalCell[,]? _cells;
    private Size _size;
    private long _revision;
    private bool _disposed;
    private bool _synchronizingSize;

    public AttachedRenderService(IControlFrameRenderer? renderer = null)
    {
        _renderer = renderer ?? new ControlFrameRenderer();
    }

    public event EventHandler? FrameChanged;

    public bool IsAttached => _root is not null;
    public Size Size => _size;
    public long Revision => _revision;

    public void Attach(IControl root, Size size)
    {
        if (root is null)
            throw new ArgumentNullException(nameof(root));
        if (size.Width <= 0 || size.Height <= 0)
            throw new ArgumentOutOfRangeException(nameof(size));

        Detach();
        _root = root;
        _size = size;
        SubscribeTree(root);
        RenderCore();
    }

    public IRenderFrameSnapshot GetSnapshot()
    {
        ThrowIfDisposed();
        if (_cells is null)
            throw new InvalidOperationException("No control tree is attached.");
        return new RenderFrameSnapshot(_size, _revision, new Rectangle(Point.Empty, _size), _cells);
    }

    public void Render()
    {
        ThrowIfDisposed();
        if (_root is null)
            throw new InvalidOperationException("No control tree is attached.");
        RenderCore();
    }

    public void Resize(Size size)
    {
        if (size.Width <= 0 || size.Height <= 0)
            throw new ArgumentOutOfRangeException(nameof(size));
        _size = size;
        if (_root is not null)
        {
            _synchronizingSize = true;
            _root.size = size;
            _synchronizingSize = false;
            RenderCore();
        }
    }

    /// <summary>Refreshes event subscriptions after controls are added or removed.</summary>
    public void RefreshTree()
    {
        ThrowIfDisposed();
        if (_root is null)
            throw new InvalidOperationException("No control tree is attached.");
        UnsubscribeTree(_root);
        SubscribeTree(_root);
        RenderCore();
    }

    public void Detach()
    {
        if (_root is not null)
            UnsubscribeTree(_root);
        _root = null;
        _cells = null;
        _size = Size.Empty;
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        Detach();
        _disposed = true;
    }

    private void RenderCore()
    {
        var next = new TerminalCell[_size.Width, _size.Height];
        _renderer.Render(_root!, next, _size);
        _cells = next;
        _revision++;
        FrameChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SubscribeTree(IControl control)
    {
        control.OnChange += ControlChanged;
        control.OnMove += ControlChanged;
        control.OnResize += ControlChanged;
        foreach (var child in control.Children)
            SubscribeTree(child);
    }

    private void UnsubscribeTree(IControl control)
    {
        control.OnChange -= ControlChanged;
        control.OnMove -= ControlChanged;
        control.OnResize -= ControlChanged;
        foreach (var child in control.Children)
            UnsubscribeTree(child);
    }

    private void ControlChanged(object? sender, EventArgs e)
    {
        if (!_synchronizingSize)
            RenderCore();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(AttachedRenderService));
    }
}
