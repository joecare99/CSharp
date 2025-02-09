// ***********************************************************************
// Assembly         : ConsoleMouseApp
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 08-18-2022
// ***********************************************************************
// <copyright file="Program.cs" company="JC-Soft">
//     Copyright © 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;
using ConsoleLib.CommonControls;
using ConsoleLib;
using ConsoleMouseApp.View;
using ConsoleLib.Interfaces;
using BaseLib.Interfaces;
using BaseLib.Models;
using ConsoleLib.ConsoleLib.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.DependencyInjection;

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
            ConsoleFramework.ExtendedConsole?.Stop();
        }

        private static void Init()
        {
            var sp = new ServiceCollection()
             .AddSingleton<IExtendedConsole, ExtendedConsole>()
             .AddTransient<IConsole, ConsoleProxy>()
             .AddSingleton<Application,ConsoleMouseView>()
             //   .AddTransient<Views.LoadingDialog, Views.LoadingDialog>()
             .BuildServiceProvider();

            Ioc.Default.ConfigureServices(sp);

            App = Ioc.Default.GetRequiredService<Application>();
            App.Visible = true;
            App.Draw();
            App.OnMouseMove += App_MouseMove;
            App.OnCanvasResize += App_CanvasResize;

        }


        /// <summary>
        /// Applications the canvas resize.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private static void App_CanvasResize(object? sender, Point e)
        {
            var cl = ConsoleFramework.Canvas.ClipRect;
            cl.Inflate(-3, -3);
            if (App != null)
                App.Dimension = cl;
        }

        /// <summary>
        /// Handles the MouseMove event of the App control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private static void App_MouseMove(object? sender, IMouseEvent e)
        {
            //         Mouse.Set(Point.Subtract(e.MousePos, (Size?)Mouse.Parent?.Position??Size.Empty));
        }

        #endregion
    }
}

