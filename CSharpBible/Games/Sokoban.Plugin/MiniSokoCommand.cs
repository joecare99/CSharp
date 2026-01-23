using BaseLib.Interfaces;
using ConsoleDisplay.View;
using Microsoft.Extensions.Logging;
using PluginBase.Interfaces;
using Sokoban.Models;
using Sokoban.Plugin;
using Sokoban.Properties;
using Sokoban.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban;

public class MiniSokoCommand : ICommand
{
    private IEnvironment _env;
    private IRandom? _rnd;
    private ILogger? _logger;
    private ISysTime? _time;
    private IConsole? _console;
    private Display _display;
    private Game _game;

    public string Name => "sokoban-mini";

    public string Description => Resources.cmdDescription2;

    public int Execute(object? param = null)
    {
        _logger?.LogDebug($"{nameof(SokoCommand)}.{nameof(Execute)}");
        if (_console != null)
        {
            _console.Clear();
            _game = new Game();
            _game.Init();   
            _display = new Display(_console, 3, 3,_game.PFSize.Width,_game.PFSize.Height);
            _game.visSetMessage = (msg) => 
            {
                _console.SetCursorPosition(0, 15);
                _console.WriteLine(msg);
            };
            _game.visShow = (ua) =>
            {
                _display.Clear();
                for (int y = 0; y < _game.PFSize.Height; y++)
                {
                    for (int x = 0; x < _game.PFSize.Width; x++)
                    {
                        var tile = _game.GetTile(new Point(x, y));
                        ConsoleColor color = tile switch
                        {
                            TileDef.Wall => ConsoleColor.DarkGray,
                            TileDef.Floor => ConsoleColor.Black,
                            TileDef.Destination => ConsoleColor.Yellow,
                            _ => ConsoleColor.Black,
                        };
                        _display.PutPixel(x, y, color);
                    }
                }
                foreach (var stone in _game.Stones)
                {
                    _display.PutPixel(stone.Position.X, stone.Position.Y, ConsoleColor.Blue);
                }
                if (_game.player != null)
                {
                    _display.PutPixel(_game.player.Position.X, _game.player.Position.Y, ConsoleColor.Green);
                }
                _display.Update();
            };
            _game.visUpdate = () =>
            {
                // Optional: Implement if needed
            };
            _game.visGetUserAction = (ua) =>
            {
                _console.SetCursorPosition(0, 14);
                _console.WriteLine("Use W/A/S/D to move, Q to quit.");
                while (true)
                {
                    var keyInfo = _console.ReadKey();
                    if (keyInfo.HasValue)
                    {
                        var key = keyInfo.Value.Key;
                        return key switch
                        {
                            ConsoleKey.W => UserAction.GoNorth,
                            ConsoleKey.UpArrow => UserAction.GoNorth,
                            ConsoleKey.A => UserAction.GoWest,
                            ConsoleKey.LeftArrow => UserAction.GoWest,
                            ConsoleKey.S => UserAction.GoSouth,
                            ConsoleKey.DownArrow => UserAction.GoSouth,
                            ConsoleKey.D => UserAction.GoEast,
                            ConsoleKey.RightArrow => UserAction.GoEast,
                            ConsoleKey.Q => UserAction.Quit,
                            _ => null,
                        };
                    }
                }
            };

            while (LabDefs.SLevels.Length > _game.level)
            {
                var action = _game.Run();
                if (action == UserAction.Quit)
                {
                    break;
                }
            }

            _game.Cleanup();
        }
        else
        {
            _logger?.LogError("Console service is not available.");
        }
        return 0;
    }

    public void Initialize(IEnvironment env)
    {
        _env = env;
        _rnd = _env.GetService<IRandom>();
        _logger = _env.GetService<ILogger>();
        _time = _env.GetService<ISysTime>();
        _console = _env.GetService<IConsole>();
    }
}
