using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SharpHack.ViewModel;
using SharpHack.Wpf.Services;

namespace SharpHack.Wpf.Controls;

public partial class MapRendererControl : UserControl
{
    public static readonly DependencyProperty DisplayTilesProperty = DependencyProperty.Register(
        nameof(DisplayTiles),
        typeof(IReadOnlyList<DisplayTile>),
        typeof(MapRendererControl),
        new PropertyMetadata(null, OnRenderPropertyChanged));

    public static readonly DependencyProperty ViewWidthProperty = DependencyProperty.Register(
        nameof(ViewWidth),
        typeof(int),
        typeof(MapRendererControl),
        new PropertyMetadata(0, OnRenderPropertyChanged));

    public static readonly DependencyProperty ViewHeightProperty = DependencyProperty.Register(
        nameof(ViewHeight),
        typeof(int),
        typeof(MapRendererControl),
        new PropertyMetadata(0, OnRenderPropertyChanged));

    public static readonly DependencyProperty TileServiceProperty = DependencyProperty.Register(
        nameof(TileService),
        typeof(ITileService),
        typeof(MapRendererControl),
        new PropertyMetadata(null, OnRenderPropertyChanged));

    public static readonly DependencyProperty TileSizeProperty = DependencyProperty.Register(
        nameof(TileSize),
        typeof(int),
        typeof(MapRendererControl),
        new PropertyMetadata(32, OnRenderPropertyChanged));

    private readonly List<Image> _tileImages = new();
    private readonly List<DisplayTile> _buffer = new();

    public MapRendererControl()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += (_, _) => Render();
        Unloaded += (_, _) => UnhookFromDataContext();
    }

    public IReadOnlyList<DisplayTile>? DisplayTiles
    {
        get => (IReadOnlyList<DisplayTile>?)GetValue(DisplayTilesProperty);
        set => SetValue(DisplayTilesProperty, value);
    }

    public int ViewWidth
    {
        get => (int)GetValue(ViewWidthProperty);
        set => SetValue(ViewWidthProperty, value);
    }

    public int ViewHeight
    {
        get => (int)GetValue(ViewHeightProperty);
        set => SetValue(ViewHeightProperty, value);
    }

    public ITileService? TileService
    {
        get => (ITileService?)GetValue(TileServiceProperty);
        set => SetValue(TileServiceProperty, value);
    }

    public int TileSize
    {
        get => (int)GetValue(TileSizeProperty);
        set => SetValue(TileSizeProperty, value);
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        UnhookFromDataContext();

        if (e.NewValue is INotifyPropertyChanged inpc)
        {
            inpc.PropertyChanged += OnVmPropertyChanged;
        }

        Render();
    }

    private void UnhookFromDataContext()
    {
        if (DataContext is INotifyPropertyChanged inpc)
        {
            inpc.PropertyChanged -= OnVmPropertyChanged;
        }
    }

    private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GameViewModel.DisplayTiles)
            || e.PropertyName == nameof(GameViewModel.ViewWidth)
            || e.PropertyName == nameof(GameViewModel.ViewHeight))
        {
            Render();
        }
    }

    private static void OnRenderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((MapRendererControl)d).Render();
    }

    private void EnsureGrid()
    {
        if (ViewWidth <= 0 || ViewHeight <= 0)
        {
            return;
        }

        MapCanvas.Width = ViewWidth * TileSize;
        MapCanvas.Height = ViewHeight * TileSize;

        int required = ViewWidth * ViewHeight;
        if (_tileImages.Count == required)
        {
            return;
        }

        MapCanvas.Children.Clear();
        _tileImages.Clear();
        _buffer.Clear();

        for (int y = 0; y < ViewHeight; y++)
        {
            for (int x = 0; x < ViewWidth; x++)
            {
                var img = new Image
                {
                    Width = TileSize,
                    Height = TileSize,
                    SnapsToDevicePixels = true
                };

                Canvas.SetLeft(img, x * TileSize);
                Canvas.SetTop(img, y * TileSize);

                _tileImages.Add(img);
                MapCanvas.Children.Add(img);
                _buffer.Add(DisplayTile.Empty);
            }
        }
    }

    private void Render()
    {
        if (TileService is null || DisplayTiles is null)
        {
            return;
        }

        EnsureGrid();

        int required = ViewWidth * ViewHeight;
        if (required <= 0 || DisplayTiles.Count < required)
        {
            return;
        }

        for (int i = 0; i < required; i++)
        {
            var tile = DisplayTiles[i];
            if (_buffer[i] == tile)
            {
                continue;
            }

            _buffer[i] = tile;
            _tileImages[i].Source = TileService.GetTile(tile);
        }
    }
}
