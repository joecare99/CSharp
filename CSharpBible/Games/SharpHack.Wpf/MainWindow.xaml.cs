using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SharpHack.ViewModel;
using SharpHack.Engine;
using SharpHack.LevelGen.BSP;
using SharpHack.Combat;
using SharpHack.AI;
using BaseLib.Models;
using SharpHack.Base.Model;
using SharpHack.Wpf.Services;
using System.Collections.Generic; // Add using

namespace SharpHack.Wpf;

public partial class MainWindow : Window
{
    private readonly GameViewModel _viewModel;
    private readonly ITileService _tileService; // Add field
    private const int TileSize = 32; // Update tile size to 32 for NetHack tiles

    public List<DisplayTile> MapBuffer { get; }

    public MainWindow(GameViewModel viewModel, ITileService tileService) // Inject TileService
    {
        InitializeComponent();
        
        _viewModel = viewModel;
        _tileService = tileService;
        
        // Load tileset (placeholder path, user should replace or we provide a default)
        // For now, we assume a file named "tiles.png" in the output directory or use fallback
        _tileService.LoadTileset("tiles.png", TileSize);

        DataContext = _viewModel;

        // Listen for property changes to redraw map (not ideal for MVVM but simple for Canvas rendering)
        _viewModel.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(GameViewModel.Map))
            {
                DrawMap();
            }
        };
        MapBuffer = new List<DisplayTile>(_viewModel.ViewWidth * _viewModel.ViewHeight);
        for (var i = 0; i < _viewModel.ViewWidth * _viewModel.ViewHeight; i++)
            MapBuffer.Add(DisplayTile.Empty);
        // Initial Draw
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
        DrawMap(); // Redraw after move
    }


    private void DrawMap()
    {
        var map = _viewModel.Map;
        var player = _viewModel.Player;

        // Adjust Canvas size
        MapCanvas.Width = _viewModel.ViewWidth * TileSize;
        MapCanvas.Height = _viewModel.ViewHeight * TileSize;

        for (int x = 0; x < _viewModel.ViewWidth; x++)
        {
            for (int y = 0; y < _viewModel.ViewHeight; y++)
            {
                var ix = y*_viewModel.ViewWidth+x;
                if (MapBuffer[ix] != _viewModel.DisplayTiles[ix])
                {
                    MapBuffer[ix] = _viewModel.DisplayTiles[ix];
                    var tileType = _viewModel.DisplayTiles[ix];
                    var tileImage = _tileService.GetTile(tileType);
                    DrawImage(tileImage, x, y);
                }
            }
        }
    }

    private void DrawImage(ImageSource source, int x, int y, double opacity = 1.0)
    {
        Image img = new Image
        {
            Source = source,
            Width = TileSize,
            Height = TileSize,
            Opacity = opacity
        };
        Canvas.SetLeft(img, x * TileSize);
        Canvas.SetTop(img, y * TileSize);
        MapCanvas.Children.Add(img);
    }
}
