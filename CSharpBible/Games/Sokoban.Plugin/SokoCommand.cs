using BaseLib.Interfaces;
using Microsoft.Extensions.Logging;
using PluginBase.Interfaces;
using Sokoban.Models;
using Sokoban.Properties;
using Sokoban.View;
using Sokoban.ViewModels;
using System.IO;
using System.Resources;

namespace Sokoban.Plugin
{
    public class SokoCommand : ICommand
    {
        private IEnvironment? _env;
        private IRandom? _rnd;
        private ILogger? _logger;
        private ISysTime? _time;
        private IConsole? _console;

        public string Name { get => "sokoban"; }
        public string Description { get => Resources.cmdDescription; }
        public void Initialize(IEnvironment env)
        {
            _env = env;
            _rnd = _env.GetService<IRandom>();
            _logger = _env.GetService<ILogger>();
            _time = _env.GetService<ISysTime>();
            _console = _env.GetService<IConsole>();
        }

        public int Execute(object? param = null)
        {
            _logger?.LogDebug($"{nameof(SokoCommand)}.{nameof(Execute)}");
            if (_env != null)
                _env.ui.Title = Resources.msgTitle;
            
            if (_console != null)
            {
                _console.Clear();
                var game = new Game();
                var visuals = new Visuals(_console, game);
                game.Init();
                game.visSetMessage = (s) => visuals.Message = s;
                game.visShow = visuals.Show;
                game.visUpdate = visuals.Update;
                game.visGetUserAction = visuals.WaitforUser;

                while (LabDefs.SLevels.Length > game.level)
                {
                    var action = game.Run();
                    if (action == UserAction.Quit)
                    {
                        break;
                    }
                }

                game.Cleanup();
            }

            return 0;
        }

    }
}
