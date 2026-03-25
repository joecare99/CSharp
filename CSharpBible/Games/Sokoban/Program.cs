// ***********************************************************************
// Assembly         : Sokoban
// Author           : Mir
// Created          : 07-09-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Program.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Sokoban.Models;
using Sokoban.View;
using Sokoban.ViewModels;
using BaseLib.Models;

namespace Sokoban
{
    /// <summary>
    /// Class Program.
    /// </summary>
    public class Program
    {
        static IGame? _SokobanGame;
        static Visuals? _visuals;
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            Init();
            Run();
            Cleanup();
        }

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        public static void Cleanup()
        {
            _SokobanGame?.Cleanup();
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public static void Run()
        {
            UserAction? direction = null;
            while (direction!=UserAction.Quit && LabDefs.SLevels.Length > _SokobanGame?.level)
            {
                direction = _SokobanGame?.Run();
            }

        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Init()
        {
            // Setup Visuals
            _SokobanGame = new Game();
            _visuals = new Visuals(new ConsoleProxy(), _SokobanGame);
            _SokobanGame.Init();
            _SokobanGame.visSetMessage = (s) => _visuals.Message = s;
            _SokobanGame.visShow = _visuals.Show;
            _SokobanGame.visUpdate = _visuals.Update;
            _SokobanGame.visGetUserAction = _visuals.WaitforUser;
        }

    }
}
