using System;
using System.Diagnostics;
using System.Windows;
using Asteroids.Model.Interfaces;

namespace Asteroids.Model;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Game: IGame
{
    public Game()
    {
    }

    public Point ScreenFactor => throw new NotImplementedException();

    public Rect Screen => throw new NotImplementedException();

    private string GetDebuggerDisplay()
    {
        return ToString()??"";
    }
}
