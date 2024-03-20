using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
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
