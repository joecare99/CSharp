using System.Windows;
using System.Windows.Media;

namespace Treppen.Export.Services.Interfaces;

public interface IDrawCommandFactory
{
    IDrawCommand newPolygonCommand(Point[] points, Color fill, Color? outline, double v);
    IDrawCommand newPolyLineCommand(Point[] rampPoints, Color color, double v1, bool v2);
}