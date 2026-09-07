namespace ConsoleLib.Showcase.Services;

/// <summary>
/// Testable window state for a desktop window (no UI dependency).
/// </summary>
public sealed class DesktopWindow
{
    /// <summary>Stable unique identifier of the desktop app (e.g., "calendar").</summary>
    public string Id { get; }

    /// <summary>Display title shown in the title bar and taskbar.</summary>
    public string Title { get; }

    /// <summary>True while the window is open (running or minimized).</summary>
    public bool IsOpen { get; internal set; }

    /// <summary>True while the window is minimized to the taskbar.</summary>
    public bool IsMinimized { get; internal set; }

    /// <summary>Higher value means closer to the front.</summary>
    public int ZIndex { get; internal set; }

    public DesktopWindow(string id, string title)
    {
        Id = id;
        Title = title;
    }
}
