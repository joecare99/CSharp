using Asteroids.Model.Interfaces;
using System.Windows;
using System.Windows.Media;

namespace Asteroids.Model;

public class BgrStar(Point pos,Color? col = null) : IScreenObj
{
    public float fSize { get; set; } = 1;

    public Color color { get; set; } = col ?? Colors.Gray;

    public bool xWrap => false;

    public bool xOutline => true;

    public Point[] Points { get; } = [pos];
}
