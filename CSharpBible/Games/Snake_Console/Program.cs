using ConsoleDisplay.View;
using Microsoft.Extensions.DependencyInjection;
using Snake_Base.Models;
using Snake_Base.Models.Data;
using Snake_Base.Models.Interfaces;
using Snake_Base.ViewModels;
using BaseLib.Interfaces;
using BaseLib.Helper;
using Snake_Console.View;
using System.Threading;
using Game_Base.Model;

namespace Snake_Console
{
    public static class Program
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        private static ISnakeGame _game;
        private static IVisual _visual;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
		private static int iDelay;

        private static void OnStartUp()
        {
            var sc = new ServiceCollection()
                .AddTransient<ISnakeViewModel, SnakeViewModel>()
                .AddTransient<IRandom, CRandom>()
                .AddSingleton<ISnakeGame, SnakeGame>()
                .AddSingleton<IPlayfield2D<ISnakeGameObject>, Playfield2D<ISnakeGameObject>>()
                .AddSingleton<IVisual, Visual>()
                .AddTransient<ITileDisplay<SnakeTiles>, TileDisplay<SnakeTiles>>()
                .AddTransient<ITileDef,TileDef>()
                .AddSingleton<IConsole, MyConsole>();
            var sp = sc.BuildServiceProvider();

            IoC.Configure(sp);
        }

        public static void Main(string[] args)
        {
            Init(args);
            Run();
    	}

        private static void Run()
        {
            while (_game.IsRunning)
            {
                iDelay = _game.GameStep();
                Thread.Sleep(iDelay);
                _visual.CheckUserAction();
            }
        }

        private static void Init(string[] args)
        {
            OnStartUp();

            _game = IoC.GetRequiredService<ISnakeGame>();
            _game.Setup(1);

            _visual = IoC.GetRequiredService<IVisual>();
            _visual.FullRedraw();
        }
    }
}
