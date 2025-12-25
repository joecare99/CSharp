using System;
using SharpHack.Engine;
using SharpHack.LevelGen;
using SharpHack.LevelGen.BSP;
using SharpHack.Base.Model;
using SharpHack.Combat;
using SharpHack.AI;
using SharpHack.ViewModel; // Add using

namespace SharpHack.Console;

public class Program
{
    private static GameViewModel _viewModel; // Use ViewModel instead of Session directly

    public static void Main(string[] args)
    {
        // Setup dependencies
        var random = new BaseLib.Helper.RandomRng();
        var mapGenerator = new BSPMapGenerator(random);
        var combatSystem = new SimpleCombatSystem();
        var enemyAI = new SimpleEnemyAI();

        var session = new GameSession(mapGenerator, random, combatSystem, enemyAI);
        _viewModel = new GameViewModel(session); // Initialize ViewModel

        System.Console.CursorVisible = false;
        System.Console.Title = "SharpHack";

        bool running = true;
        while (running)
        {
            Render();
            var key = System.Console.ReadKey(true).Key;
            running = HandleInput(key);
        }
    }

    private static bool HandleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow: _viewModel.Move(Direction.North); break;
            case ConsoleKey.DownArrow: _viewModel.Move(Direction.South); break;
            case ConsoleKey.LeftArrow: _viewModel.Move(Direction.West); break;
            case ConsoleKey.RightArrow: _viewModel.Move(Direction.East); break;
            case ConsoleKey.NumPad7: _viewModel.Move(Direction.NorthWest); break;
            case ConsoleKey.NumPad9: _viewModel.Move(Direction.NorthEast); break;
            case ConsoleKey.NumPad1: _viewModel.Move(Direction.SouthWest); break;
            case ConsoleKey.NumPad3: _viewModel.Move(Direction.SouthEast); break;
            case ConsoleKey.NumPad5: _viewModel.Wait(); break; // Wait command
            case ConsoleKey.Escape: return false;
        }
        return true;
    }

    private static void Render()
    {
        // Render Map
        var map = _viewModel.Map;
        var player = _viewModel.Player;
        
        // ... (Rendering logic mostly stays the same, accessing data via _viewModel)
        // Optimization: Only redraw if changed? For now, redraw all.
        
        // Simple camera centering on player
        int viewWidth = 80;
        int viewHeight = 25;
        int offsetX = player.Position.X - viewWidth / 2;
        int offsetY = player.Position.Y - viewHeight / 2;

        // Clamp offset
        offsetX = Math.Max(0, Math.Min(offsetX, map.Width - viewWidth));
        offsetY = Math.Max(0, Math.Min(offsetY, map.Height - viewHeight));

        System.Console.SetCursorPosition(0, 0);
        for (int y = 0; y < viewHeight; y++)
        {
            for (int x = 0; x < viewWidth; x++)
            {
                int mapX = x + offsetX;
                int mapY = y + offsetY;

                if (mapX < 0 || mapX >= map.Width || mapY < 0 || mapY >= map.Height)
                {
                    System.Console.Write(' ');
                    continue;
                }

                var tile = map[mapX, mapY];
                
                if (tile.IsVisible)
                {
                    if (tile.Creature != null)
                    {
                        System.Console.ForegroundColor = tile.Creature.Color;
                        System.Console.Write(tile.Creature.Symbol);
                    }
                    else if (tile.Items.Count > 0)
                    {
                        var item = tile.Items[0];
                        System.Console.ForegroundColor = item.Color;
                        System.Console.Write(item.Symbol);
                    }
                    else
                    {
                        System.Console.ForegroundColor = GetTileColor(tile.Type);
                        System.Console.Write(GetTileSymbol(tile.Type));
                    }
                }
                else if (tile.IsExplored)
                {
                    System.Console.ForegroundColor = System.ConsoleColor.DarkGray;
                    System.Console.Write(GetTileSymbol(tile.Type));
                }
                else
                {
                    System.Console.Write(' ');
                }
            }
            System.Console.WriteLine();
        }

        // Render UI
        System.Console.ForegroundColor = System.ConsoleColor.White;
        System.Console.SetCursorPosition(0, 26);
        System.Console.Write($"HP: {_viewModel.HP}/{_viewModel.MaxHP}  Lvl: {_viewModel.Level}  ");
        
        // Clear previous messages
        System.Console.Write(new string(' ', 50)); 
        System.Console.SetCursorPosition(0, 27);
        
        // Show last message
        if (_viewModel.Messages.Count > 0)
        {
            System.Console.Write(_viewModel.Messages[_viewModel.Messages.Count - 1].PadRight(79));
        }
        else
        {
             System.Console.Write(new string(' ', 79));
        }
    }

    private static ConsoleColor GetTileColor(TileType type)
    {
        return type switch
        {
            TileType.Wall => System.ConsoleColor.Gray,
            TileType.Floor => System.ConsoleColor.DarkGray,
            TileType.DoorClosed => System.ConsoleColor.DarkYellow,
            TileType.DoorOpen => System.ConsoleColor.DarkYellow,
            TileType.StairsDown => System.ConsoleColor.White,
            TileType.StairsUp => System.ConsoleColor.White,
            _ => System.ConsoleColor.Black
        };
    }

    private static char GetTileSymbol(TileType type)
    {
        return type switch
        {
            TileType.Wall => '#',
            TileType.Floor => '.',
            TileType.DoorClosed => '+',
            TileType.DoorOpen => '/',
            TileType.StairsDown => '>',
            TileType.StairsUp => '<',
            _ => ' '
        };
    }
}
