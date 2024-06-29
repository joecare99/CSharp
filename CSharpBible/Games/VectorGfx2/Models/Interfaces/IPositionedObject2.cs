using System.Collections.Generic;
using System.Windows;

namespace VectorGfx2.Models.Interfaces;

public interface IPositionedObject2 : IHasPoint
{
    int ZRot { get; set; }

    int X { get; set; }
    int Y { get; set; }
    List<Point> Pnts { get; set; }
}
