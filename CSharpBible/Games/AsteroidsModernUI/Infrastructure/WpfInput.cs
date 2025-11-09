using System.Collections.Generic;
using System.Windows.Input;
using AsteroidsModern.Engine.Abstractions;

namespace AsteroidsModern.UI;

public sealed class WpfInput : IGameInput
{
    private readonly HashSet<GameKey> _down = new();

    public void OnKeyDown(Key key)
    {
        if (key == Key.Left) _down.Add(GameKey.Left);
        if (key == Key.Right) _down.Add(GameKey.Right);
        if (key == Key.Up) _down.Add(GameKey.Thrust);
        if (key == Key.Space) _down.Add(GameKey.Fire);
        if (key is Key.Escape or Key.Q) _down.Add(GameKey.Quit);
        if (key is Key.Down or Key.H) _down.Add(GameKey.Hyperspace);
    }

    public void OnKeyUp(Key key)
    {
        if (key == Key.Left) _down.Remove(GameKey.Left);
        if (key == Key.Right) _down.Remove(GameKey.Right);
        if (key == Key.Up) _down.Remove(GameKey.Thrust);
        if (key == Key.Space) _down.Remove(GameKey.Fire);
        if (key is Key.Escape or Key.Q) _down.Remove(GameKey.Quit);
        if (key is Key.Down or Key.H) _down.Remove(GameKey.Hyperspace);
    }

    public bool IsDown(GameKey key) => _down.Contains(key);
}
