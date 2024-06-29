using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Asteroids.Model.Interfaces;

public interface IGame
{
    Point ScreenFactor { get; }
    Rect Screen { get; }
}
