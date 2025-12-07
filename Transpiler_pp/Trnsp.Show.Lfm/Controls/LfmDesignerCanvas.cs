using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Trnsp.Show.Lfm.Models.Components;
using Trnsp.Show.Lfm.Services;

namespace Trnsp.Show.Lfm.Controls;

/// <summary>
/// Canvas control for displaying LFM forms visually.
/// </summary>
public class LfmDesignerCanvas : Canvas
{
    private readonly IComponentRenderer _renderer;
    private LfmComponentBase? _rootComponent;
    private double _zoom = 1.0;

    public static readonly DependencyProperty RootComponentProperty =
        DependencyProperty.Register(
            nameof(RootComponent),
            typeof(LfmComponentBase),
            typeof(LfmDesignerCanvas),
            new PropertyMetadata(null, OnRootComponentChanged));

    public static readonly DependencyProperty ZoomProperty =
        DependencyProperty.Register(
            nameof(Zoom),
            typeof(double),
            typeof(LfmDesignerCanvas),
            new PropertyMetadata(1.0, OnZoomChanged));

    public static readonly DependencyProperty SelectedComponentProperty =
        DependencyProperty.Register(
            nameof(SelectedComponent),
            typeof(LfmComponentBase),
            typeof(LfmDesignerCanvas),
            new PropertyMetadata(null));

    public LfmComponentBase? RootComponent
    {
        get => (LfmComponentBase?)GetValue(RootComponentProperty);
        set => SetValue(RootComponentProperty, value);
    }

    public double Zoom
    {
        get => (double)GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }

    public LfmComponentBase? SelectedComponent
    {
        get => (LfmComponentBase?)GetValue(SelectedComponentProperty);
        set => SetValue(SelectedComponentProperty, value);
    }

    public LfmDesignerCanvas()
    {
        _renderer = new ComponentRenderer();
        Background = new DrawingBrush
        {
            TileMode = TileMode.Tile,
            Viewport = new Rect(0, 0, 10, 10),
            ViewportUnits = BrushMappingMode.Absolute,
            Drawing = new GeometryDrawing
            {
                Brush = Brushes.White,
                Pen = new Pen(Brushes.LightGray, 0.5),
                Geometry = new RectangleGeometry(new Rect(0, 0, 10, 10))
            }
        };

        ClipToBounds = true;
    }

    private static void OnRootComponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LfmDesignerCanvas canvas)
        {
            canvas._rootComponent = e.NewValue as LfmComponentBase;
            canvas.RenderComponent();
        }
    }

    private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LfmDesignerCanvas canvas)
        {
            canvas.ApplyZoom();
        }
    }

    private void RenderComponent()
    {
        Children.Clear();

        if (_rootComponent == null)
            return;

        var element = _renderer.Render(_rootComponent);
        
        if (element is FrameworkElement fe)
        {
            // Position the form in the canvas
            Canvas.SetLeft(fe, 20);
            Canvas.SetTop(fe, 20);
            
            // Add click handler for selection
            fe.MouseLeftButtonDown += OnElementClicked;
        }

        Children.Add(element);
        ApplyZoom();
    }

    private void ApplyZoom()
    {
        var transform = new ScaleTransform(_zoom, _zoom);
        RenderTransform = transform;
    }

    private void OnElementClicked(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.Tag is LfmComponentBase component)
        {
            // Deselect previous
            if (SelectedComponent != null)
            {
                SelectedComponent.IsSelected = false;
            }

            // Select new
            component.IsSelected = true;
            SelectedComponent = component;
            e.Handled = true;
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            var delta = e.Delta > 0 ? 0.1 : -0.1;
            Zoom = Math.Max(0.25, Math.Min(4.0, Zoom + delta));
            e.Handled = true;
        }
    }
}
