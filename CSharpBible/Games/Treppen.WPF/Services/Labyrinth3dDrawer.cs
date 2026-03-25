using System.Windows;
using System.Windows.Media;
using Treppen.Base;
using Treppen.Export.Services.Interfaces;

namespace Treppen.WPF.Services;

public class Labyrinth3dDrawer : ILabyrinth3dDrawer
{

    private double ViewZ = 5000d;
    private double tileWidth;
    private double tileHeight;
    private double stepHeight;
    private double offsetX;
    private IDrawCommandFactory _factory;

    public double offsetY { get; private set; }

    public IReadOnlyList<IDrawCommand> Build(IHeightLabyrinth labyrinth, Size printableSize, IDrawCommandFactory dcFactory)
    {
        var result = new List<IDrawCommand>();
        _factory = dcFactory;
        double labHW = labyrinth.Dimension.Width + labyrinth.Dimension.Height;
        if (printableSize.Width < double.Epsilon) return result;

        ViewZ = -printableSize.Width * 2d;

        tileWidth = printableSize.Width / (labHW + 4) * 1.4;
        tileHeight = tileWidth;
        stepHeight = tileHeight / 5d;

        offsetX = printableSize.Width / 2;
        offsetY = printableSize.Height / 2.5;

        for (int i = 0; i < labHW + 4; i++)
        {
            for (int y = labyrinth.Dimension.Height - 1; y >= 0; y--)
            {
                for (int x = labyrinth.Dimension.Width - 1; x >= 0; x--)
                {
                    if (x == 0 || y == 0 || i + 6 > labyrinth.BaseLevel(x, y))
                    {
                        int h = labyrinth[x, y];
                        if (h < 0 || i > h)
                        {
                            continue;
                        }

                        bool drawTop = i == h;
                        bool drawLeft = y == 0 || labyrinth[x, y - 1] < i;
                        bool drawRight = x == 0 || labyrinth[x - 1, y] < i;

                        int hDiag = (x < labyrinth.Dimension.Width && y > 0) ? labyrinth[x + 1, y - 1] : -1;
                        int hLeft = (x < labyrinth.Dimension.Width) ? labyrinth[x + 1, y] : -1;

                        BuildCube(result, -x + labyrinth.Dimension.Width / 2d, y - labyrinth.Dimension.Height / 2d, -i + (labHW + 4) / 2d,
                            drawTop, drawLeft, drawRight, i, hDiag, hLeft);
                    }
                }
            }
        }
        return result;
    }

    private Point ToScreen(double x, double y, double z)
    {
        (x, y) = RotateTransform(x, y, -Math.PI * 0.22);
        (y, z) = RotateTransform(y, z, Math.PI / 3);
        return z > ViewZ ? new Point(offsetX + x * ViewZ / (ViewZ - z), offsetY - y * ViewZ / (ViewZ - z)) : new Point();
    }

    private (double x, double y) RotateTransform(double x, double y, double v)
    {
        return (x * Math.Cos(v) - y * Math.Sin(v), x * Math.Sin(v) + y * Math.Cos(v));
    }

    private void BuildCube(List<IDrawCommand> list,
        double x,
        double y,
        double z,
        bool drawTop,
        bool drawLeft,
        bool drawRight,
        int currentHeight,
        int hDiag,
        int hLeft)
    {
        if (drawLeft)
        {
            list.Add(_factory.newPolygonCommand(
            [
                ToScreen((x - 0.5d) * tileWidth, (y - 0.5) * tileHeight, (z - 0.5) * stepHeight),
                ToScreen((x + 0.5d) * tileWidth, (y - 0.5) * tileHeight, (z - 0.5) * stepHeight),
                ToScreen((x + 0.5d) * tileWidth, (y - 0.5) * tileHeight, (z + 0.5) * stepHeight),
                ToScreen((x - 0.5d) * tileWidth, (y - 0.5) * tileHeight, (z + 0.5) * stepHeight)
            ], Colors.LightGray, Colors.White, 0.5));

            // Ramp diagonally (top-left to right) if higher/equal
            if (hDiag >= currentHeight)
            {
                double s = (hDiag - currentHeight) * 0.1d;
                double s2 = (hDiag - currentHeight + 1) * 0.1d;
                var rampPoints = new[]
                {
                    ToScreen((x - 0.5d + s) * tileWidth, (y - 0.5) * tileHeight, (z - 0.5) * stepHeight),
                    ToScreen((x - 0.5d) * tileWidth, (y - 0.5) * tileHeight, (z - 0.5) * stepHeight),
                    ToScreen((x - 0.5d) * tileWidth, (y - 0.5) * tileHeight, (z + 0.5) * stepHeight),
                    ToScreen((x - 0.5d + s2) * tileWidth, (y - 0.5) * tileHeight, (z + 0.5) * stepHeight)
                };
                list.Add(_factory.newPolygonCommand(rampPoints, Colors.DarkGray, null, 0));
                list.Add(_factory.newPolyLineCommand(rampPoints, Colors.Gray, 0.5, true));
                list.Add(_factory.newPolyLineCommand(
                [
                    ToScreen((x - 0.5d + s) * tileWidth, (y - 0.5) * tileHeight, (z - 0.5) * stepHeight),
                    ToScreen((x - 0.5d + s2) * tileWidth, (y - 0.5) * tileHeight, (z + 0.5) * stepHeight)
                ], Colors.LightGray, 0.5, true));
            }
        }

        if (drawRight)
        {
            list.Add(_factory.newPolygonCommand(
            [
                ToScreen((x + 0.5d) * tileWidth, (y + 0.5) * tileHeight, (z - 0.5) * stepHeight),
                ToScreen((x + 0.5d) * tileWidth, (y - 0.5) * tileHeight, (z - 0.5) * stepHeight),
                ToScreen((x + 0.5d) * tileWidth, (y - 0.5) * tileHeight, (z + 0.5) * stepHeight),
                ToScreen((x + 0.5d) * tileWidth, (y + 0.5) * tileHeight, (z + 0.5) * stepHeight)
            ], Colors.DarkGray, Colors.Gray, 0.5));
        }

        if (drawTop)
        {
            list.Add(_factory.newPolygonCommand(
            [
                ToScreen((x - 0.5d) * tileWidth, (y - 0.5) * tileHeight, (z - 0.5) * stepHeight),
                ToScreen((x + 0.5d) * tileWidth, (y - 0.5) * tileHeight, (z - 0.5) * stepHeight),
                ToScreen((x + 0.5d) * tileWidth, (y + 0.5) * tileHeight, (z - 0.5) * stepHeight),
                ToScreen((x - 0.5d) * tileWidth, (y + 0.5) * tileHeight, (z - 0.5) * stepHeight)
            ], Colors.White, Colors.LightGray, 0.5));

            // Schräge Fläche zur höheren Zelle rechts (Logik x-1 -> hLeft)
            if (hLeft > currentHeight)
            {
                double s = (hLeft - currentHeight) * 0.1d;
                list.Add(_factory.newPolygonCommand(
                [
                    ToScreen((x - 0.5d) * tileWidth, (y - 0.5) * tileHeight, (z - 0.5) * stepHeight),
                    ToScreen((x - 0.5d) * tileWidth, (y + 0.5) * tileHeight, (z - 0.5) * stepHeight),
                    ToScreen((x - 0.5d + s) * tileWidth, (y + 0.5) * tileHeight, (z - 0.5) * stepHeight),
                    ToScreen((x - 0.5d + s) * tileWidth, (y - 0.5) * tileHeight, (z - 0.5) * stepHeight)
                ], Colors.DarkGray, Colors.LightGray, 0.5));
            }
        }

    }
}
