using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AsteroidsModern.UI;

public sealed class GameCanvasBehavior : Behavior<System.Windows.Controls.Canvas>
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(MainViewModel), typeof(GameCanvasBehavior), new PropertyMetadata(null));

    public MainViewModel? ViewModel
    {
        get => (MainViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private readonly WpfInput _input = new();

    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject == null) return;

        AssociatedObject.Loaded += OnLoaded;
        AssociatedObject.Unloaded += OnUnloaded;
        AssociatedObject.SizeChanged += OnSizeChanged;
        AssociatedObject.KeyDown += OnKeyDown;
        AssociatedObject.KeyUp += OnKeyUp;
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.Loaded -= OnLoaded;
            AssociatedObject.Unloaded -= OnUnloaded;
            AssociatedObject.SizeChanged -= OnSizeChanged;
            AssociatedObject.KeyDown -= OnKeyDown;
            AssociatedObject.KeyUp -= OnKeyUp;
        }
        CompositionTarget.Rendering -= OnRendering;
        base.OnDetaching();
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        AssociatedObject.Focus();
        CompositionTarget.Rendering += OnRendering;
        UpdateSize();
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        CompositionTarget.Rendering -= OnRendering;
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e) => UpdateSize();

    private void UpdateSize()
    {
        if (ViewModel != null && AssociatedObject != null)
            ViewModel.SetSize((float)AssociatedObject.ActualWidth, (float)AssociatedObject.ActualHeight);
    }

    private void OnKeyDown(object? sender, KeyEventArgs e) => _input.OnKeyDown(e.Key);

    private void OnKeyUp(object? sender, KeyEventArgs e) => _input.OnKeyUp(e.Key);

    private void OnRendering(object? sender, EventArgs e)
    {
        if (ViewModel == null || AssociatedObject == null) return;

        ViewModel.Tick(_input);

        var dv = new DrawingVisual();
        using (var dc = dv.RenderOpen())
        {
            var ctx = new WpfRenderContext(dc);
            ViewModel.Render(ctx);
        }
        int width = (int)Math.Max(1, AssociatedObject.ActualWidth);
        int height = (int)Math.Max(1, AssociatedObject.ActualHeight);
        var bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        bmp.Render(dv);

        AssociatedObject.Children.Clear();
        var img = new System.Windows.Controls.Image { Source = bmp };
        System.Windows.Controls.Canvas.SetLeft(img, 0);
        System.Windows.Controls.Canvas.SetTop(img, 0);
        AssociatedObject.Children.Add(img);
    }
}
