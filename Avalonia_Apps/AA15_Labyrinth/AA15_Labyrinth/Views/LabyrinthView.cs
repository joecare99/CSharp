using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using AA15_Labyrinth.ViewModels;

namespace AA15_Labyrinth.Views;

public class LabyrinthView : Control
{
    private DispatcherTimer? _progressTimer;

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (DataContext is not ILabyrinthViewModel vm)
            return;

        vm.Build(Bounds.Size);

        context.FillRectangle(Brushes.White, Bounds);
        if (vm.MazeGeometry is { } maze)
        {
            var pen = new Pen(Brushes.Black, vm.LineThickness, lineCap: PenLineCap.Round);
            context.DrawGeometry(null, pen, maze);
        }
        if (vm.DrawSolution && vm.SolutionGeometry is { } sol)
        {
            var spen = new Pen(Brushes.Crimson, System.Math.Max(1.0, vm.LineThickness -0.6), lineCap: PenLineCap.Round);
            context.DrawGeometry(null, spen, sol);
        }

        // draw progress bar if generating
        if (vm.IsGenerating || vm.Progress <1.0)
        {
            // start timer to update while generating
            StartProgressTimer(vm);

            double barHeight =6.0;
            var r = new Rect(Bounds.X +8, Bounds.Bottom - barHeight -8, Bounds.Width -16, barHeight);
            context.FillRectangle(new SolidColorBrush(Color.FromRgb(230,230,230)), r);
            var filled = new Rect(r.X, r.Y, r.Width * System.Math.Max(0.0, System.Math.Min(1.0, vm.Progress)), r.Height);
            context.FillRectangle(new SolidColorBrush(Color.FromRgb(60,150,210)), filled);
        }
        else
        {
            StopProgressTimer();
        }
    }

    private void StartProgressTimer(ILabyrinthViewModel vm)
    {
        if (_progressTimer != null && _progressTimer.IsEnabled) return;
        _progressTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Background, (s, e) =>
        {
            // Invalidate to get updated progress rendering
            InvalidateVisual();
            if (!vm.IsGenerating && vm.Progress >=1.0) StopProgressTimer();
        });
        _progressTimer.Start();
    }

    private void StopProgressTimer()
    {
        if (_progressTimer == null) return;
        _progressTimer.Stop();
        _progressTimer = null;
    }
}
