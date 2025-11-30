using BaseLib.Helper;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Treppen.Base;
using Treppen.Export.Services.Interfaces;
using Treppen.Export.Services.Drawing;

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

    private IHeightLabyrinth? _currentLabyrinth; // track subscription

    private static void OnLabyrinthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Labyrinth3DView view)
        {
            if (e.NewValue is IHeightLabyrinth lab)
            {
                view._currentLabyrinth = lab;
                view.DrawLabyrinth(lab); // initial draw
            }
            else
            {
                view._currentLabyrinth = null;
                view.Children.Clear();
            }
        }
    }

    private void DrawLabyrinth(IHeightLabyrinth labyrinth)
    {
        Children.Clear();
        if (labyrinth == null || labyrinth.Dimension.Width == 0 || labyrinth.Dimension.Height == 0)
            return;

        // Einen gültigen DrawingContext über DrawingGroup.Open() erhalten
        var drawingGroup = new DrawingGroup();
        using (DrawingContext dc = drawingGroup.Open())
        {
            IoC.GetRequiredService<ILabyrinth3dDrawer>()
               .Build(labyrinth, new Size(ActualWidth, ActualHeight),IoC.GetKeyedRequiredService<IDrawCommandFactory>("dc")).Render(dc);
        }

        // Das Ergebnis als Bild auf dem Canvas anzeigen
        var img = new Image
        {
            Source = new DrawingImage(drawingGroup),
            Width = ActualWidth,
            Height = ActualHeight
        };
        Children.Add(img);
    }


    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        if (_currentLabyrinth != null)
        {
            DrawLabyrinth(_currentLabyrinth);
        }
    }
}
