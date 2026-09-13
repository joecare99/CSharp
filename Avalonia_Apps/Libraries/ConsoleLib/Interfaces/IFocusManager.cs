using System;

namespace ConsoleLib.Interfaces;

/// <summary>Provides deterministic keyboard focus traversal for a control tree.</summary>
public interface IFocusManager
{
    IControl? FocusedControl { get; }
    bool Focus(IControl control);
    bool MoveNext();
    bool MovePrevious();
    void Clear();
    bool HandleKey(KeyInput input);
}
