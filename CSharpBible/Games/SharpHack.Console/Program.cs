using System;
using SharpHack.Engine;
using SharpHack.LevelGen;
using SharpHack.LevelGen.BSP;
using SharpHack.Base.Model;
using SharpHack.Combat;
using SharpHack.AI;
using SharpHack.ViewModel;
using BaseLib.Models; // Add using

namespace SharpHack.Console;

public class Program
{
    private static GameViewModel _viewModel; // Use ViewModel instead of Session directly

    public static void Main(string[] args)
    {
        // Setup dependencies
        var random = new CRandom();
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
        var tiles = _viewModel.DisplayTiles;
        int viewWidth = _viewModel.ViewWidth;
        int viewHeight = _viewModel.ViewHeight;

        System.Console.SetCursorPosition(0, 0);
        int index = 0;
        for (int y = 0; y < viewHeight; y++)
        {
            for (int x = 0; x < viewWidth; x++)
            {
                var tile = tiles[index++];
                var glyph = GetGlyph(tile);
                System.Console.ForegroundColor = glyph.color;
                System.Console.Write(glyph.symbol);
            }
            System.Console.WriteLine();
        }

        System.Console.ForegroundColor = System.ConsoleColor.White;
        System.Console.SetCursorPosition(0, viewHeight + 1);
        System.Console.Write($"HP: {_viewModel.HP}/{_viewModel.MaxHP}  Lvl: {_viewModel.Level}  ");
        System.Console.Write(new string(' ', 50));
        System.Console.SetCursorPosition(0, viewHeight + 2);

        if (_viewModel.Messages.Count > 0)
        {
            var lastMessage = _viewModel.Messages[_viewModel.Messages.Count - 1];
            System.Console.Write(lastMessage.PadRight(79));
        }
        else
        {
            System.Console.Write(new string(' ', 79));
        }
    }

    private static (char symbol, ConsoleColor color) GetGlyph(DisplayTile tile)
    {
        return tile switch
        {
            DisplayTile.Archaeologist => ('@', ConsoleColor.Yellow),
            DisplayTile.Goblin => ('g', ConsoleColor.Green),
            DisplayTile.Wall_EW => ('#', ConsoleColor.Gray),
            DisplayTile.Floor_Lit => ('.', ConsoleColor.DarkGray),
            DisplayTile.Door_Closed => ('+', ConsoleColor.DarkYellow),
            DisplayTile.Door_Open => ('/', ConsoleColor.DarkYellow),
            DisplayTile.Stairs_Up => ('<', ConsoleColor.White),
            DisplayTile.Stairs_Down => ('>', ConsoleColor.White),
            DisplayTile.Sword => ('/', ConsoleColor.Cyan),
            DisplayTile.Armor => ('[', ConsoleColor.Cyan),
            _ => (' ', ConsoleColor.Black)
        };
    }
}
