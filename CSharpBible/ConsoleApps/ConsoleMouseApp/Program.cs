// ***********************************************************************
// Assembly         : ConsoleMouseApp
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : AI Assistant
// Last Modified On : 09-26-2025
// ***********************************************************************
using System;
using System.Drawing;
using ConsoleLib.CommonControls;
using ConsoleLib;
using ConsoleLib.ExtCon;
using ConsoleMouseApp.Views;
using ConsoleLib.Interfaces;
using BaseLib.Interfaces;
using BaseLib.Models;
using Microsoft.Extensions.DependencyInjection;
using ConsoleMouseApp.ViewModels; // added
using BaseLib.Helper; // added

namespace ConsoleMouseApp
{
    /// <summary>
    /// Class Program.
    /// </summary>
    class Program
    {
        #region Properties
        /// <summary>
        /// The mouse
        /// </summary>
    //    private static Pixel Mouse = new Pixel();

        /// <summary>
        /// The application
        /// </summary>
        private static ConsoleLib.CommonControls.Application App;
        #endregion

        #region Methods
        /// <summary>
        /// Initializes static members of the <see cref="Program"/> class.
        /// </summary>
        static Program()
        { }
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] _)
        {
            Init();

            App?.Run();

            Console.Write("Programm end ...");
            ConsoleLib.ExtCon.ConsoleFramework.ExtendedConsole?.Stop();
        }

        private static void Init()
        {
            var sp = new ServiceCollection()
             .AddSingleton<IExtendedConsole, ExtendedConsole>()
             .AddTransient<IConsole, ConsoleProxy>()
             .AddTransient<IWidgetSet, ConsoleWidgetSet>()
             .AddSingleton<IConsoleMouseViewModel,ConsoleMouseViewModel>()
             .AddSingleton<Application,ConsoleMouseView>()
             .BuildServiceProvider();

            IoC.Configure(sp);

            App = IoC.GetRequiredService<Application>();
            App.Visible = true;
            App.Draw();
            App.OnCanvasResize += App_CanvasResize;

        }


        /// <summary>
        /// Applications the canvas resize.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private static void App_CanvasResize(object? sender, Point e)
        {
            var cl = App.WidgetSet.ClipRect;
            cl.Inflate(-3, -3);
            if (App != null)
                App.Dimension = cl;
        }

        #endregion
    }
}

