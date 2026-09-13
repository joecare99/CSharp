using ConsoleLib.Interfaces;
using System;
using System.Drawing;

namespace ConsoleLib;

/// <summary>
/// Fallback extended console implementation used when no console event source is available.
/// </summary>
public sealed class NullExtendedConsole : IExtendedConsole
{
    public static NullExtendedConsole Instance { get; } = new();

    public event EventHandler<IMouseEvent>? MouseEvent
    {
        add { }
        remove { }
    }

    public event EventHandler<IKeyEvent>? KeyEvent
    {
        add { }
        remove { }
    }

    public event EventHandler<Point>? WindowBufferSizeEvent
    {
        add { }
        remove { }
    }

    public void Stop()
    {
    }
}
