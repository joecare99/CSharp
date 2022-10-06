// ***********************************************************************
// Assembly         : ConsoleMouseApp
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 08-18-2022
// ***********************************************************************
// <copyright file="Program.cs" company="HP Inc.">
//     Copyright © 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
//using System.Windows.Forms;
using System.Drawing;
using ConsoleLib.CommonControls;
using ConsoleLib;
using System.Windows.Forms;
using ConsoleMouseApp.View;

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
        private static Pixel Mouse = new Pixel();

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
        {
            Console.ForegroundColor = ConsoleColor.White;

            App = new ConsoleMouseView();

            // t.Draw(10, 40, ConsoleColor.Gray);
            Mouse.parent = App;
            Mouse.Set(0, 0, " ");
            Mouse.BackColor = ConsoleColor.Red;

            App.OnCanvasResize += App_CanvasResize;
            App.OnMouseMove += App_MouseMove;

        }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {

            App.Draw();
            App.Run();

            Console.Write("Programm end ...");
            ExtendedConsole.Stop();
        }

#if NET5_0_OR_GREATER
        private static void App_CanvasResize(object? sender, Point e)
#else
        /// <summary>
        /// Applications the canvas resize.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private static void App_CanvasResize(object sender, Point e)
#endif
        {
            if (App == null) return;
            var cl = ConsoleFramework.Canvas.ClipRect;
            cl.Inflate(-3, -3);
            App.dimension = cl;
        }

#if NET5_0_OR_GREATER
        private static void App_MouseMove(object? sender, MouseEventArgs e)
#else
        /// <summary>
        /// Handles the MouseMove event of the App control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private static void App_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
#endif
        {
            Mouse.Set(Point.Subtract(e.Location, (Size?)Mouse.parent?.position ?? Size.Empty));
        }

        #endregion
    }
}

