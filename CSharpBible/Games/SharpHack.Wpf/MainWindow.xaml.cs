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
using SharpHack.Wpf.Services; // Add using

namespace SharpHack.Wpf;

public partial class MainWindow : Window
{
    private readonly GameViewModel _viewModel;
    private readonly ITileService _tileService; // Add field
    private const int TileSize = 32; // Update tile size to 32 for NetHack tiles

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
        MapCanvas.Children.Clear();
        var map = _viewModel.Map;
        var player = _viewModel.Player;

        // Adjust Canvas size
        MapCanvas.Width = map.Width * TileSize;
        MapCanvas.Height = map.Height * TileSize;

        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                var tile = map[x, y];
                if (!tile.IsExplored) continue;

                // Draw Floor/Wall
                var tileImg = _tileService.GetTile(tile.Type);
                if (tileImg != null)
                {
                    DrawImage(tileImg, x, y, tile.IsVisible ? 1.0 : 0.5); // Dim if not visible
                }

                if (tile.IsVisible)
                {
                    if (x == player.Position.X && y == player.Position.Y)
                    {
                        DrawImage(_tileService.GetPlayer(), x, y);
                    }
                    else if (tile.Creature != null)
                    {
                        DrawImage(_tileService.GetCreature(tile.Creature), x, y);
                    }
                    else if (tile.Items.Count > 0)
                    {
                        DrawImage(_tileService.GetItem(tile.Items[0]), x, y);
                    }
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
