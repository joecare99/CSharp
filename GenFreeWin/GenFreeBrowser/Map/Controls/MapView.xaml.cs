using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GenFreeBrowser.Map.Controls;

public partial class MapView : UserControl
{
    public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register(
        nameof(Viewport), typeof(ViewportState), typeof(MapView), new PropertyMetadata(null, OnViewportChanged));

    public static readonly DependencyProperty TileSourceProperty = DependencyProperty.Register(
        nameof(TileSource), typeof(ITileSource), typeof(MapView), new PropertyMetadata(null, (_,__) => { }));

    private readonly ConcurrentDictionary<TileId, Image> _tiles = new();
    private Point? _lastDrag;

    public ViewportState? Viewport
    {
        get => (ViewportState?)GetValue(ViewportProperty);
        set => SetValue(ViewportProperty, value);
    }

    public ITileSource? TileSource
    {
        get => (ITileSource?)GetValue(TileSourceProperty);
        set => SetValue(TileSourceProperty, value);
    }

    public MapView()
    {
        InitializeComponent();
        Loaded += (_, _) => { if (Viewport != null) { Viewport.PixelSize = new Size(ActualWidth, ActualHeight); RefreshAsync(); } };
        SizeChanged += (_, _) => { if (Viewport != null) { Viewport.PixelSize = new Size(ActualWidth, ActualHeight); RefreshAsync(); } };
    }

    private static void OnViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MapView mv)
        {
            if (e.OldValue is ViewportState oldVp)
                oldVp.PropertyChanged -= mv.ViewportPropertyChanged;
            if (e.NewValue is ViewportState newVp)
                newVp.PropertyChanged += mv.ViewportPropertyChanged;
            mv.RefreshAsync();
        }
    }

    private void ViewportPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        => RefreshAsync();

    private async void RefreshAsync()
    {
        if (Viewport == null || TileSource == null) return;
        PART_Canvas.Children.Clear();
        _tiles.Clear();

        var (cx, cy) = Viewport.CenterTileXY();
        int z = Viewport.Zoom;
        double tileOffsetX = cx - Math.Floor(cx);
        double tileOffsetY = cy - Math.Floor(cy);

        int tilesX = (int)Math.Ceiling(Viewport.PixelSize.Width / MapConstants.TileSize) + 2;
        int tilesY = (int)Math.Ceiling(Viewport.PixelSize.Height / MapConstants.TileSize) + 2;

        int centerTileX = (int)Math.Floor(cx)+1;
        int centerTileY = (int)Math.Floor(cy)+1;

        for (int dx = -tilesX/2; dx <= tilesX/2; dx++)
        {
            for (int dy = -tilesY/2; dy <= tilesY/2; dy++)
            {
                long tx = centerTileX + dx;
                long ty = centerTileY + dy;
                if (tx < 0 || ty < 0 || tx >= (1 << z) || ty >= (1 << z)) continue; // outside world
                var id = new TileId(tx, ty, z);
                double screenX = (tilesX/2 + dx - tileOffsetX) * MapConstants.TileSize;
                double screenY = (tilesY/2 + dy - tileOffsetY) * MapConstants.TileSize;
                var img = new Image { Width = MapConstants.TileSize, Height = MapConstants.TileSize };
                Canvas.SetLeft(img, screenX - MapConstants.TileSize/2);
                Canvas.SetTop(img, screenY - MapConstants.TileSize/2);
                PART_Canvas.Children.Add(img);
                _tiles[id] = img;
                _ = LoadTileAsync(id, img);
            }
        }
    }

    private async Task LoadTileAsync(TileId id, Image img)
    {
        if (TileSource == null) return;
        try
        {
            var data = await TileSource.GetTileAsync(id).ConfigureAwait(false);
            if (data == null) return;
            await Dispatcher.InvokeAsync(() =>
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = new System.IO.MemoryStream(data);
                bmp.EndInit();
                bmp.Freeze();
                img.Source = bmp;
            });
        }
        catch { /* TODO: log */ }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);
        if (Viewport == null) return;
        var delta = e.Delta > 0 ? 1 : -1;
        Viewport.Zoom += delta;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        _lastDrag = e.GetPosition(this);
        CaptureMouse();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_lastDrag != null && Viewport != null && e.LeftButton == MouseButtonState.Pressed)
        {
            var pos = e.GetPosition(this);
            var dx = pos.X - _lastDrag.Value.X;
            var dy = pos.Y - _lastDrag.Value.Y;
            _lastDrag = pos;
            // convert pixel shift to tile shift
            var (cx, cy) = Viewport.CenterTileXY();
            double tilesDx = dx / MapConstants.TileSize;
            double tilesDy = dy / MapConstants.TileSize;
            var newCenter = WebMercator.Unproject(cx - tilesDx, cy - tilesDy, Viewport.Zoom);
            Viewport.Center = newCenter;
        }
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        _lastDrag = null;
        ReleaseMouseCapture();
    }
}
