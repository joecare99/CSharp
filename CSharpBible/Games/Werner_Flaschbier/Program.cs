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

using BaseLib.Helper;
using ConsoleDisplay.View;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Authentication.ExtendedProtection;
using System.Threading;
using Werner_Flaschbier_Base.Model;
using Werner_Flaschbier_Base.View;
using Werner_Flaschbier_Base.ViewModels;

namespace Werner_Flaschbier_Base
{
    /// <summary>
    /// Class Programm.
    /// </summary>
    public class Programm
    {
        /// <summary>
        /// The game
        /// </summary>
        private IWernerGame? game;
        private IVisual? visual;

        private void OnStartUp()
        {
            var sc = new ServiceCollection()
                .AddSingleton<IWernerGame, WernerGame>()
                .AddTransient<IWernerViewModel, WernerViewModel>()
                .AddSingleton<ITileDef, VTileDef>()
                .AddSingleton<IVisual, Visual>()
                .AddSingleton<IConsole, MyConsole>();
            
            var sp = sc.BuildServiceProvider();

            IoC.Configure(sp);
        }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(params string[] args)
        {
            var program = new Programm();
            program.Initialize(args);
            program.Run();
          //  program.OnExit();
        }

        public void Run()
        {
            while (game!.isRunning)
            {
                visual?.CheckUserAction();
                var delay = game.GameStep();
                Thread.Sleep(delay);
            }
        }

        public void Initialize(string[] args)
        {
            OnStartUp();

            game = IoC.GetRequiredService<IWernerGame>();
            visual = IoC.GetRequiredService<IVisual>();
        }
    }

}
