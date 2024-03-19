using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
