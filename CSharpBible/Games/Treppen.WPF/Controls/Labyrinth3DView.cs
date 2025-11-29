using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Treppen.Base;

namespace Treppen.WPF.Controls;

public class Labyrinth3DView : Canvas
{
    public static readonly DependencyProperty LabyrinthProperty =
        DependencyProperty.Register(nameof(Labyrinth), typeof(IHeightLabyrinth), typeof(Labyrinth3DView),
            new PropertyMetadata(null, OnLabyrinthChanged));

    public IHeightLabyrinth Labyrinth
    {
        get { return (IHeightLabyrinth)GetValue(LabyrinthProperty); }
        set { SetValue(LabyrinthProperty, value); }
    }

    private static void OnLabyrinthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Labyrinth3DView view && e.NewValue is IHeightLabyrinth labyrinth)
        {
            view.DrawLabyrinth(labyrinth);
        }
    }

    private void DrawLabyrinth(IHeightLabyrinth labyrinth)
    {
        Children.Clear();
        if (labyrinth == null || labyrinth.Dimension.Width == 0 || labyrinth.Dimension.Height == 0)
            return;

        double labHW = (labyrinth.Dimension.Width + labyrinth.Dimension.Height);
        double tileWidth = ActualWidth/(labHW+2)*2d;
        double tileHeight = tileWidth/2.2d;
        double stepHeight = tileHeight/5d;

        double totalWidth = (labyrinth.Dimension.Width + labyrinth.Dimension.Height) * tileWidth /2;
        double totalHeight = (labyrinth.Dimension.Width + labyrinth.Dimension.Height) * (tileHeight/2 + stepHeight);
        double offsetX = (ActualWidth - totalWidth) / 2 + (labyrinth.Dimension.Height * tileWidth / 2);
        double offsetY = (ActualHeight) / 2  ;

        for (int i = 0; i < labyrinth.Dimension.Height + labyrinth.Dimension.Width + 4; i++)
        {
            for (int y = labyrinth.Dimension.Height - 1; y >= 0; y--)
            {
                for (int x = labyrinth.Dimension.Width - 1; x >= 0; x--)
                if (x ==0 || y==0 ||i+6 > labyrinth.BaseLevel(x,y))
                {
                    int h = labyrinth[x, y];
                    if (h < 0 || i>h) continue;

                    double screenX = (-x + y) * tileWidth / 2 + offsetX;
                    double screenY = (labHW/2 - x - y) * tileHeight / 2 + (labHW/3 - i) * stepHeight + offsetY;

                    bool drawTop = (i == h);
                    bool drawLeft = (y == 0) || (labyrinth[x, y - 1] < i);
                    bool drawRight = (x == 0) || (labyrinth[x - 1, y] < i);

                    int h_diag = (x < labyrinth.Dimension.Width && y > 0) ? labyrinth[x + 1, y - 1] : -1;
                    int h_left = (x < labyrinth.Dimension.Width) ? labyrinth[x + 1, y] : -1;

                    DrawCube(screenX, screenY, tileWidth, tileHeight, stepHeight, drawTop, drawLeft, drawRight, i, h_diag, h_left);
                }
            }
        }
    }

    private void DrawCube(
        double x, double y, double width, double height, double stepHeight, 
        bool drawTop, bool drawLeft, bool drawRight, 
        int currentHeight, int h_diag, int h_left )
    {
        if (drawTop)
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
                Stroke = Brushes.LightGray,
                StrokeThickness = 0.5
            };
            Children.Add(top);

            // Schräge Fläche zur höheren Zelle rechts (in der Logik x-1)
            if (h_left > currentHeight)
            {
                double s = (h_left - currentHeight) * stepHeight*0.5d;
                var ramp = new Polygon
                {
                    Points = new PointCollection
                    {
                        new(x, y),
                        new(x - width / 2, y + height / 2),
                        new(x - width / 2 + s, y + height / 2 + height /width  * s),
                        new(x+s, y + height /width  * s)
                    },
                    Fill = Brushes.DarkGray,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 0.5
                };
                Children.Add(ramp);
            }
        }

        if (drawLeft)
        {
            // Left
            var left = new Polygon
            {
                Points = new PointCollection
                {
                    new(x - width / 2, y + height / 2),
                    new(x - width / 2, y + height / 2 + stepHeight),
                    new(x, y + height + stepHeight),
                    new(x, y + height),
                },
                Fill = Brushes.LightGray,
                Stroke = Brushes.White,
                StrokeThickness = 0.5
            };
            Children.Add(left);

            if (h_diag >= currentHeight)
            {
                double s = (h_diag - currentHeight) * stepHeight * 0.5d;
                double s2 = (h_diag - currentHeight+1) * stepHeight * 0.5d;
                var ramp = new Polygon
                {
                    Points = new PointCollection
                    {
                       new(x - width / 2, y + height / 2),
                       new(x - width / 2, y + height / 2 + stepHeight),
                       new(x - width / 2 + s2, y + height / 2 + height /width  * s2 + stepHeight),
                        new(x- width / 2 + s, y + height / 2 + height /width  * s)
                    },
                    Fill = Brushes.DarkGray,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 0.5
                };
                Children.Add(ramp);

            }
        }

        if (drawRight)
        {
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
                Fill = Brushes.DarkGray,
                Stroke = Brushes.Gray,
                StrokeThickness = 0.5
            };
            Children.Add(right);

        }
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        if (Labyrinth != null)
        {
            DrawLabyrinth(Labyrinth);
        }
    }
}
