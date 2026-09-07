using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleLib.Showcase.Services;

/// <summary>
/// Owns the desktop window lifecycle (open/close/toggle/minimize/restore),
/// z-order, and the active window. Pure logic without UI dependencies.
/// </summary>
public sealed class DesktopWindowManager
{
    private readonly List<DesktopWindow> _windows = new();
    private int _zCounter;

    /// <summary>Raised after every state change; the shell resynchronizes its UI.</summary>
    public event Action? Changed;

    /// <summary>Raised after a window is fully closed; the shell disposes its content.</summary>
    public event Action<DesktopWindow>? WindowClosed;

    /// <summary>Optional gate: return false to veto opening a new window (e.g., single-instance resources).</summary>
    public Func<string, bool>? CanOpen { get; set; }

    /// <summary>Hook called whenever a window becomes open and needs app content.</summary>
    public Action<DesktopWindow>? OnOpened { get; set; }

    /// <summary>All known windows in open order (open and closed).</summary>
    public IReadOnlyList<DesktopWindow> Windows => _windows;

    /// <summary>The topmost non-minimized open window, or null if none.</summary>
    public DesktopWindow? Active =>
        _windows.Where(w => w.IsOpen && !w.IsMinimized).OrderByDescending(w => w.ZIndex).FirstOrDefault();

    public bool IsOpen(string id) => _windows.Any(w => w.Id == id && w.IsOpen);

    public DesktopWindow? Find(string id) => _windows.FirstOrDefault(w => w.Id == id);

    /// <summary>
    /// Opens a window, or activates an already open one. Returns false when the
    /// CanOpen gate vetoed a new window.
    /// </summary>
    public bool Open(string id)
    {
        var existing = Find(id);
        if (existing is not null)
        {
            if (!existing.IsOpen)
            {
                existing.IsOpen = true;
                existing.IsMinimized = false;
                OnOpened?.Invoke(existing);
            }

            Activate(id);
            return true;
        }

        if (CanOpen is not null && !CanOpen(id))
        {
            return false;
        }

        var window = new DesktopWindow(id, id);
        window.IsOpen = true;
        window.ZIndex = NextZ();
        _windows.Add(window);
        OnOpened?.Invoke(window);
        RaiseChanged();
        return true;
    }

    /// <summary>Closes an open window. Returns false when no such open window exists.</summary>
    public bool Close(string id)
    {
        var window = Find(id);
        if (window is null || !window.IsOpen)
        {
            return false;
        }

        window.IsOpen = false;
        RaiseChanged();
        WindowClosed?.Invoke(window);
        return true;
    }

    /// <summary>
    /// Taskbar-style toggle: closed → open, minimized → restored, visible → minimized.
    /// </summary>
    public void Toggle(string id)
    {
        var window = Find(id);
        if (window is null || !window.IsOpen)
        {
            Open(id);
        }
        else if (window.IsMinimized)
        {
            window.IsMinimized = false;
            window.ZIndex = NextZ();
            RaiseChanged();
        }
        else
        {
            window.IsMinimized = true;
            RaiseChanged();
        }
    }

    public void Minimize(string id)
    {
        if (Find(id) is { IsOpen: true, IsMinimized: false } window)
        {
            window.IsMinimized = true;
            RaiseChanged();
        }
    }

    public void Restore(string id)
    {
        if (Find(id) is { IsOpen: true, IsMinimized: true } window)
        {
            window.IsMinimized = false;
            window.ZIndex = NextZ();
            RaiseChanged();
        }
    }

    /// <summary>Raises a visible open window to the front.</summary>
    public void BringToFront(string id)
    {
        if (Find(id) is { IsOpen: true, IsMinimized: false } window)
        {
            window.ZIndex = NextZ();
            RaiseChanged();
        }
    }

    /// <summary>Restores (if minimized) and brings a window to the front.</summary>
    public void Activate(string id)
    {
        if (Find(id) is { IsOpen: true } window)
        {
            if (window.IsMinimized)
            {
                window.IsMinimized = false;
            }

            window.ZIndex = NextZ();
            RaiseChanged();
        }
    }

    /// <summary>Closes the active window. Returns the closed window, or null.</summary>
    public DesktopWindow? CloseActive()
    {
        var active = Active;
        if (active is null)
        {
            return null;
        }

        Close(active.Id);
        return active;
    }

    /// <summary>Closes every open window, front to back.</summary>
    public void CloseAll()
    {
        foreach (var window in _windows.Where(w => w.IsOpen).OrderByDescending(w => w.ZIndex).ToList())
        {
            Close(window.Id);
        }
    }

    private int NextZ() => ++_zCounter;

    private void RaiseChanged() => Changed?.Invoke();
}
