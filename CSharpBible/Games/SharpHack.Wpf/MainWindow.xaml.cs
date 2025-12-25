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

namespace SharpHack.Wpf;

public partial class MainWindow : Window
{
    private readonly GameViewModel _viewModel;
    private const int TileSize = 16;

    public MainWindow(GameViewModel viewModel)
    {
        InitializeComponent();
        
        _viewModel = viewModel;
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

                Rectangle rect = new Rectangle
                {
                    Width = TileSize,
                    Height = TileSize,
                    Fill = GetTileBrush(tile.Type, tile.IsVisible)
                };

                Canvas.SetLeft(rect, x * TileSize);
                Canvas.SetTop(rect, y * TileSize);
                MapCanvas.Children.Add(rect);

                if (tile.IsVisible)
                {
                    if (x == player.Position.X && y == player.Position.Y)
                    {
                        DrawEntity("@", Brushes.Yellow, x, y);
                    }
                    else if (tile.Creature != null)
                    {
                        DrawEntity(tile.Creature.Symbol.ToString(), Brushes.Red, x, y);
                    }
                    else if (tile.Items.Count > 0)
                    {
                        DrawEntity(tile.Items[0].Symbol.ToString(), Brushes.Cyan, x, y);
                    }
                }
            }
        }
    }

    private void DrawEntity(string symbol, Brush color, int x, int y)
    {
        TextBlock text = new TextBlock
        {
            Text = symbol,
            Foreground = color,
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            TextAlignment = TextAlignment.Center,
            Width = TileSize,
            Height = TileSize
        };
        Canvas.SetLeft(text, x * TileSize);
        Canvas.SetTop(text, y * TileSize);
        MapCanvas.Children.Add(text);
    }

    private Brush GetTileBrush(TileType type, bool isVisible)
    {
        if (!isVisible) return Brushes.DarkSlateGray;

        return type switch
        {
            TileType.Wall => Brushes.Gray,
            TileType.Floor => Brushes.Black,
            TileType.DoorClosed => Brushes.Brown,
            TileType.DoorOpen => Brushes.SandyBrown,
            TileType.StairsDown => Brushes.White,
            TileType.StairsUp => Brushes.White,
            _ => Brushes.Black
        };
    }
}
