using System.Windows;

namespace Asteroids.Model.Interfaces;

public interface IGame
{
    Point ScreenFactor { get; }
    Rect Screen { get; }
}
