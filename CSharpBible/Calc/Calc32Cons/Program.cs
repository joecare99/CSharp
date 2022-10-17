﻿// ***********************************************************************
// Assembly         : Calc32Cons
// Author           : Mir
// Created          : 12-23-2021
//
// Last Modified By : Mir
// Last Modified On : 08-28-2022
// ***********************************************************************
// <copyright file="Program.cs" company="Hewlett-Packard Company">
//     Copyright © Hewlett-Packard Company 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;
using ConsoleLib;
using ConsoleLib.CommonControls;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc32Cons.Visual;

namespace Calc32Cons
{
    /// <summary>
    /// Class Program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The mouse
        /// </summary>
        private static Pixel Mouse = new Pixel();
        /// <summary>
        /// The application
        /// </summary>
        private static ConsoleLib.CommonControls.Application App;

        /// <summary>
        /// Initializes static members of the <see cref="Program"/> class.
        /// </summary>
        static Program() {
            var cl = ConsoleFramework.Canvas.ClipRect;
            cl.Inflate(-3, -3);
            Console.ForegroundColor = ConsoleColor.White;

            App = new ConsoleLib.CommonControls.Application
            {
                visible = false,
                Boarder = ConsoleFramework.singleBoarder,
                ForeColor = ConsoleColor.Gray,
                BackColor = ConsoleColor.DarkGray,
                BoarderColor = ConsoleColor.Green,
                dimension = cl
            };

            // t.Draw(10, 40, ConsoleColor.Gray);
            Mouse.parent = App;
            Mouse.Set(0, 0, " ");
            Mouse.BackColor = ConsoleColor.Red;

            var CalcView = new ConsoleCalcView(App);

            App.visible = true;
            App.Draw();
            App.OnMouseMove += App_MouseMove;
            App.OnCanvasResize += App_CanvasResize;
        }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {

            App.Run();

            Console.Write("Programm end ...");
            ExtendedConsole.Stop();
        }

        /// <summary>
        /// Applications the canvas resize.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private static void App_CanvasResize(object sender, Point e)
        {
            var cl = ConsoleFramework.Canvas.ClipRect;
            cl.Inflate(-3, -3);
            App.dimension = cl;
        }

        /// <summary>
        /// Handles the MouseMove event of the App control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private static void App_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Set(Point.Subtract(e.Location, (Size)Mouse.parent.position));
        }

    }
}

