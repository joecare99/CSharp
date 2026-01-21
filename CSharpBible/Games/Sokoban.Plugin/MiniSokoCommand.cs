using BaseLib.Interfaces;
using Microsoft.Extensions.Logging;
using PluginBase.Interfaces;
using Sokoban.Plugin;
using Sokoban.Properties;
using System;
using System.Collections.Generic;
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

    public string Name => "sokoban-mini";

    public string Description => Resources.cmdDescription2;

    public int Execute(object? param = null)
    {
        _logger?.LogDebug($"{nameof(SokoCommand)}.{nameof(Execute)}");
        if (_console != null)
        {
            _console.Clear();
            _display = 
            var game = new MiniSokoGame(_env, _console);
            game.Run();
        }
        else
        {
            _logger?.LogError("Console service is not available.");
        }
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
