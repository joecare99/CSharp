// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 07-31-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Program.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Threading;
using Werner_Flaschbier_Base.View;
using Werner_Flaschbier_Base.ViewModel;

namespace Werner_Flaschbier_Base
{
    /// <summary>
    /// Class Programm.
    /// </summary>
    public static class Programm
    {
        /// <summary>
        /// The game
        /// </summary>
        static Game game;

        /// <summary>
        /// Initializes static members of the <see cref="Programm" /> class.
        /// </summary>
        static Programm()
        {
            game = new Game();
            Visual.SetGame(game);
        }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(params string[] args)
        {
            while (game.isRunning)
            {
                UserAction action;
                Visual.CheckUserAction(out action);
                game.HandleUserAction(action);
                var delay=game.GameStep();
                Thread.Sleep(delay);
            }
        }
    }

}
