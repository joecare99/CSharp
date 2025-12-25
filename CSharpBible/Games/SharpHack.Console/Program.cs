using System;
using SharpHack.Base.Model;
using SharpHack.Engine;
using SharpHack.LevelGen;
using SharpHack.LevelGen.BSP;
using BaseLib.Models;
using SharpHack.Combat;
using SharpHack.AI; // Add using

namespace SharpHack.Console;

class Program
{
    private static readonly List<string> _messages = new();

    static void Main(string[] args)
    {
        var random = new CRandom();
        var generator = new BSPMapGenerator(random);
        var combatSystem = new SimpleCombatSystem();
        var enemyAI = new SimpleEnemyAI(); // Instantiate AI
        var session = new GameSession(generator, random, combatSystem, enemyAI); // Pass to GameSession

        session.OnMessage += (msg) => 
        {
            _messages.Add(msg);
            if (_messages.Count > 5) _messages.RemoveAt(0);
        };

        System.Console.CursorVisible = false;

        while (session.IsRunning)
        {
            Render(session);
            HandleInput(session);
        }
    }

    static void Render(GameSession session)
    {
        System.Console.SetCursorPosition(0, 0);
        var map = session.Map;
        var player = session.Player;

        // Simple viewport rendering (or full map if small enough)
        for (int y = 0; y < map.Height; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                var tile = map[x, y];
                
                if (!tile.IsExplored)
                {
                    System.Console.Write(' ');
                    continue;
                }

                if (x == player.Position.X && y == player.Position.Y)
                {
                    System.Console.ForegroundColor = player.Color;
                    System.Console.Write(player.Symbol);
                }
                else if (tile.IsVisible)
                {
                    if (tile.Creature != null)
                    {
                        System.Console.ForegroundColor = tile.Creature.Color;
                        System.Console.Write(tile.Creature.Symbol);
                    }
                    else
                    {
                        System.Console.ForegroundColor = GetTileColor(tile.Type);
                        System.Console.Write(GetTileSymbol(tile.Type));
                    }
                }
                else // Explored but not visible
                {
                    System.Console.ForegroundColor = ConsoleColor.DarkGray;
                    System.Console.Write(GetTileSymbol(tile.Type));
                }
            }
            System.Console.WriteLine();
        }

        // Render Messages
        System.Console.SetCursorPosition(0, map.Height + 1);
        System.Console.ForegroundColor = ConsoleColor.White;
        foreach (var msg in _messages)
        {
            System.Console.WriteLine(msg.PadRight(80));
        }
    }

    static void HandleInput(GameSession session)
    {
        var key = System.Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.UpArrow: session.MovePlayer(Direction.North); break;
            case ConsoleKey.DownArrow: session.MovePlayer(Direction.South); break;
            case ConsoleKey.LeftArrow: session.MovePlayer(Direction.West); break;
            case ConsoleKey.RightArrow: session.MovePlayer(Direction.East); break;
            case ConsoleKey.Escape: Environment.Exit(0); break;
        }
    }

    static char GetTileSymbol(TileType type) => type switch
    {
        TileType.Floor => '.',
        TileType.Wall => '#',
        TileType.DoorClosed => '+',
        TileType.DoorOpen => '/',
        TileType.StairsUp => '<',
        TileType.StairsDown => '>',
        _ => ' '
    };

    static ConsoleColor GetTileColor(TileType type) => type switch
    {
        TileType.Floor => ConsoleColor.Gray,
        TileType.Wall => ConsoleColor.DarkGray,
        TileType.DoorClosed => ConsoleColor.DarkYellow,
        TileType.DoorOpen => ConsoleColor.DarkYellow,
        _ => ConsoleColor.Black
    };
}
