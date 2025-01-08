using ConsoleDisplay.View;
using Microsoft.Extensions.DependencyInjection;
using Snake_Base.Models;
using Snake_Base.Models.Data;
using Snake_Base.Models.Interfaces;
using Snake_Base.ViewModels;
using BaseLib.Interfaces;
using BaseLib.Helper;
using Snake_Console.View;
using System;
using System.Threading;

namespace Snake_Console
{
    public static class Program
    {
        private static ISnakeGame _game;
		private static UserAction action;
		private static int iDelay;
        private static IVisual _visual;

        private static void OnStartUp()
        {
            var sc = new ServiceCollection()
                .AddTransient<ISnakeViewModel, SnakeViewModel>()
                .AddTransient<IRandom, CRandom>()
                .AddSingleton<ISnakeGame, SnakeGame>()
                .AddSingleton<IVisual, Visual>()
                .AddSingleton<ITileDisplay<SnakeTiles>, TileDisplay<SnakeTiles>>()
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
