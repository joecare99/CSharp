﻿// ***********************************************************************
// Assembly         : Calc32Cons
// Author           : Mir
// Created          : 12-23-2021
//
// Last Modified By : Mir
// Last Modified On : 10-22-2022
// ***********************************************************************
// <copyright file="Program.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;
using ConsoleLib;
using ConsoleLib.CommonControls;
using System.Windows.Forms;
using Calc32Cons.Visual;

/// <summary>
/// The Calc32Cons namespace.
/// </summary>
/// <autogeneratedoc />
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
        private static readonly Pixel Mouse = new();
        /// <summary>
        /// The application
        /// </summary>
        private static readonly ConsoleLib.CommonControls.Application App;

        /// <summary>
        /// Initializes static members of the <see cref="Program" /> class.
        /// </summary>
        static Program() {
            var cl = ConsoleFramework.Canvas.ClipRect;
            cl.Inflate(-3, -3);
            Console.ForegroundColor = ConsoleColor.White;

            App = new ConsoleLib.CommonControls.Application
            {
                Visible = false,
                Boarder = ConsoleFramework.singleBoarder,
                ForeColor = ConsoleColor.Gray,
                BackColor = ConsoleColor.DarkGray,
                BoarderColor = ConsoleColor.Green,
                Dimension = cl
            };

            // t.Draw(10, 40, ConsoleColor.Gray);
            Mouse.Parent = App;
            Mouse.Set(0, 0, " ");
            Mouse.BackColor = ConsoleColor.Red;

            var CalcView = new ConsoleCalcView(App);

            App.Visible = true;
            App.Draw();
            App.OnMouseMove += App_MouseMove;
            App.OnCanvasResize += App_CanvasResize;
        }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] _)
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
        private static void App_CanvasResize(object? sender, Point e)
        {
            var cl = ConsoleFramework.Canvas.ClipRect;
            cl.Inflate(-3, -3);
            App.Dimension = cl;
        }

        /// <summary>
        /// Handles the MouseMove event of the App control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private static void App_MouseMove(object? sender, MouseEventArgs e)
        {
            Mouse.Set(Point.Subtract(e.Location, (Size?)Mouse.Parent?.Position??Size.Empty));
        }

    }
}

