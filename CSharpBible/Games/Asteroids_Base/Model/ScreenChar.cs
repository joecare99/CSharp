using Asteroids.Model.Interfaces;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Asteroids.Model;

public class ScreenChar(char ch, float size, Point pos,Color? col = null) : IScreenObj
{
    public float fSize { get; set; } = size;

    public Color color { get; set; } = col ?? Colors.White;

    public bool xWrap => false;

    public bool xOutline => true;

    public Point[] Points { get; } = Char2Points.GetPoints(ch).Select((p)=> new Point(p.X*size*0.6+pos.X,p.Y*size+pos.Y)).ToArray();
}
