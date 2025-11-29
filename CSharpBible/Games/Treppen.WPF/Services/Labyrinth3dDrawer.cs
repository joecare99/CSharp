using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Treppen.Base;

namespace Treppen.WPF.Services;

public class Labyrinth3dDrawer : ILabyrinth3dDrawer
{
    public void DrawLabyrinth(Canvas canvas, IHeightLabyrinth labyrinth)
    {
        canvas.Children.Clear();

        double tileWidth = 20;
        double tileHeight = 10;
        double stepHeight = 2;

        for (int y = 0; y < labyrinth.Dimension.Height; y++)
        {
            for (int x = 0; x < labyrinth.Dimension.Width; x++)
            {
                int h = labyrinth[x, y];
                double screenX = (x - y) * tileWidth / 2 + canvas.ActualWidth / 2;
                double screenY = (x + y) * tileHeight / 2;

                for (int i = 0; i < h; i++)
                {
                    DrawCube(canvas, screenX, screenY - i * stepHeight, tileWidth, tileHeight, stepHeight);
                }
            }
        }
    }

    private void DrawCube(Canvas canvas, double x, double y, double width, double height, double stepHeight)
    {
        // Top
        var top = new Polygon
        {
            Points = new PointCollection
            {
                new(x, y),
                new(x + width / 2, y + height / 2),
                new(x, y + height),
                new(x - width / 2, y + height / 2)
            },
            Fill = Brushes.White,
            Stroke = Brushes.Black,
            StrokeThickness = 0.5
        };
        canvas.Children.Add(top);

        // Left
        var left = new Polygon
        {
            Points = new PointCollection
            {
                new(x - width / 2, y + height / 2),
                new(x, y + height),
                new(x, y + height + stepHeight),
                new(x - width / 2, y + height / 2 + stepHeight)
            },
            Fill = Brushes.LightGray,
            Stroke = Brushes.Black,
            StrokeThickness = 0.5
        };
        canvas.Children.Add(left);

        // Right
        var right = new Polygon
        {
            Points = new PointCollection
            {
                new(x + width / 2, y + height / 2),
                new(x, y + height),
                new(x, y + height + stepHeight),
                new(x + width / 2, y + height / 2 + stepHeight)
            },
            Fill = Brushes.Gray,
            Stroke = Brushes.Black,
            StrokeThickness = 0.5
        };
        canvas.Children.Add(right);
    }
}
