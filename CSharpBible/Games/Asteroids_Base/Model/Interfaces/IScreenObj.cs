using System.Windows;
using System.Windows.Media;

namespace Asteroids.Model.Interfaces;

public interface IScreenObj
{
    float fSize { get; }
    Color color { get; }
    bool xWrap { get; }
    bool xOutline { get; }
    Point[] Points { get; }
}
