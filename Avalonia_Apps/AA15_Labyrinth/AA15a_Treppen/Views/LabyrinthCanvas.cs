using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AA15a_Treppen.Views;

public class LabyrinthCanvas : Control
{
    public static readonly StyledProperty<int[, ]?> DataProperty =
        AvaloniaProperty.Register<LabyrinthCanvas, int[, ]?>(nameof(Data));

    public int[, ]? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var grid = Data;
        if (grid == null || grid.GetLength(0) ==0 || grid.GetLength(1) ==0)
        {
            return;
        }
        int w = grid.GetLength(0);
        int h = grid.GetLength(1);
        var pad =1.0;
        var cell = Math.Max(1.0, Math.Floor(Math.Min((Bounds.Width -2 * pad) / w, (Bounds.Height -2 * pad) / h)));
        var penWhite = new Pen(Brushes.White, Math.Max(1.0, cell /6));
        for (int x =0; x < w; x++)
        {
            for (int y =0; y < h; y++)
            {
                int val = grid[x, y];
                int denom = w *2 +5;
                var color = Color.FromRgb(
                    (byte)(val *196 / denom),
                    (byte)(val *196 / denom),
                    (byte)(val *255 / denom));
                var brush = new SolidColorBrush(color);
                int yy = h - y -1;
                int xx = w - x -1;
                var rect = new Rect(pad + xx * cell, pad + yy * cell, cell, cell);
                context.FillRectangle(brush, rect);
                if (x >=1 && Math.Abs(grid[x, y] - grid[x -1, y]) <2)
                {
                    var p1 = rect.Center ;
                    var p2 = p1 + new Vector(cell,0);
                    context.DrawLine(penWhite, p1, p2);
                }
                if (y >=1 && Math.Abs(grid[x, y] - grid[x, y -1]) <2)
                {
                    var p1 = rect.Center ;
                    var p2 = p1 + new Vector(0, cell);
                    context.DrawLine(penWhite, p1, p2);
                }
            }
        }
    }
}
