using BaseLib.Helper;
using BaseLib.Interfaces;
using ConsoleDisplay.View;
using Microsoft.Extensions.DependencyInjection;
using System;
using VTileEdit.ViewModels;
using VTileEdit.Views;

namespace VTileEdit
{
    /// <summary>
    /// Class Program.
    /// </summary>
    public class Program
    {
        private void OnStartUp()
        {
            var sc = new ServiceCollection()
                .AddTransient<IVTEViewModel, VTEViewModel>()
      //          .AddTransient<IRandom, CRandom>()
//                .AddSingleton<ISnakeGame, SnakeGame>()
  //              .AddSingleton<IPlayfield2D<ISnakeGameObject>, Playfield2D<ISnakeGameObject>>()
                .AddSingleton<IVisual, VTEVisual>()
                .AddTransient<ITileDisplay<Enum>, TileDisplay<Enum>>()
           //     .AddTransient<ITileDef, TileDef>()
                .AddSingleton<IConsole, MyConsole>();
            var sp = sc.BuildServiceProvider();

            IoC.Configure(sp);
        }


        /// <summary>Defines the entry point of the application.</summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            var _program = new Program();
            _program.Init(args);
            _program.Run();
        }

        private void Run()
        {
            throw new NotImplementedException();
        }

        private void Init(string[] args)
        {
            OnStartUp();
        }
    }
}