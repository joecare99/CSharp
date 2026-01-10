using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SharpHack.Base.Model;
using SharpHack.ViewModel;
using SharpHack.Wpf.Services;
using System.Collections.Generic;

namespace SharpHack.Wpf;

public partial class MainWindow : Window
{
    private readonly GameViewModel _viewModel;
    private readonly ITileService _tileService;
    private const int TileSize = 32;

    private readonly List<Image> _tileImages = new();

    public List<DisplayTile> MapBuffer { get; }

    public MainWindow(GameViewModel viewModel, ITileService tileService)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _tileService = tileService;

        _tileService.LoadTileset("tiles.png", TileSize);

        DataContext = _viewModel;

        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(GameViewModel.Map))
            {
                EnsureRenderGrid();
                DrawMap();
            }
        };

        MapBuffer = new List<DisplayTile>(_viewModel.ViewWidth * _viewModel.ViewHeight);
        for (var i = 0; i < _viewModel.ViewWidth * _viewModel.ViewHeight; i++)
            MapBuffer.Add(DisplayTile.Empty);

        EnsureRenderGrid();
        DrawMap();

        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up: _viewModel.Move(Direction.North); break;
            case Key.Down: _viewModel.Move(Direction.South); break;
            case Key.Left: _viewModel.Move(Direction.West); break;
            case Key.Right: _viewModel.Move(Direction.East); break;
            case Key.NumPad7: _viewModel.Move(Direction.NorthWest); break;
            case Key.NumPad9: _viewModel.Move(Direction.NorthEast); break;
            case Key.NumPad1: _viewModel.Move(Direction.SouthWest); break;
            case Key.NumPad3: _viewModel.Move(Direction.SouthEast); break;
            case Key.NumPad5: _viewModel.Wait(); break;
        }
        DrawMap();
    }

    private void EnsureRenderGrid()
    {
        MapCanvas.Width = _viewModel.ViewWidth * TileSize;
        MapCanvas.Height = _viewModel.ViewHeight * TileSize;

        int required = _viewModel.ViewWidth * _viewModel.ViewHeight;
        if (_tileImages.Count == required)
        {
            return;
        }

        MapCanvas.Children.Clear();
        _tileImages.Clear();

        for (int y = 0; y < _viewModel.ViewHeight; y++)
        {
            for (int x = 0; x < _viewModel.ViewWidth; x++)
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
            }
        }

        MapBuffer.Clear();
        for (var i = 0; i < required; i++)
        {
            MapBuffer.Add(DisplayTile.Empty);
        }
    }

    private void DrawMap()
    {
        EnsureRenderGrid();

        for (int x = 0; x < _viewModel.ViewWidth; x++)
        {
            for (int y = 0; y < _viewModel.ViewHeight; y++)
            {
                var ix = y * _viewModel.ViewWidth + x;
                var tileType = _viewModel.DisplayTiles[ix];

                if (MapBuffer[ix] == tileType)
                {
                    continue;
                }

                MapBuffer[ix] = tileType;
                _tileImages[ix].Source = _tileService.GetTile(tileType);
            }
        }
    }
}
