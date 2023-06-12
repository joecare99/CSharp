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
using Sokoban_Base.Model;
using Sokoban_Base.View;
using Sokoban_Base.ViewModel;

namespace Sokoban
{
    /// <summary>
    /// Class Program.
    /// </summary>
    public class Program
    {
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
            Game.Cleanup();
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public static void Run()
        {
            UserAction? direction = null;
            while (direction!=UserAction.Quit && LabDefs.SLevels.Length > Game.level)
            {
                direction = Game.Run();
            }

        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Init()
        {
            Game.Init();
            Game.visSetMessage = (s) => Visuals.Message = s;
            Game.visShow = Visuals.Show;
            Game.visUpdate = Visuals.Update;
            Game.visGetUserAction = Visuals.WaitforUser;
        }

    }
}
