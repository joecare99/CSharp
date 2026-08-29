using System;

namespace ConsoleLib.Interfaces;

/// <summary>
/// Mutable state owned by one input dispatch operation.
/// </summary>
public sealed class InputDispatchContext
{
    public bool Handled { get; set; }
    public bool StopPropagation { get; set; }
    public object? Target { get; internal set; }

    public void MarkHandled(bool stopPropagation = true)
    {
        Handled = true;
        StopPropagation = stopPropagation;
    }
}
