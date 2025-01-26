// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 07-15-2022
// ***********************************************************************
// <copyright file="ConsoleFramework.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Interfaces;
using BaseLib.Models;
using ConsoleLib.Interfaces;
using System;
using System.Drawing;

namespace ConsoleLib
{
    /// <summary>
    /// Class ConsoleFramework.
    /// </summary>
    public static class ConsoleFramework
    {
        /// <summary>
        /// The chars
        /// </summary>
        public static readonly char[] chars = { '█', '▓', '▒', '░',' ' };
        /// <summary>
        /// The single boarder
        /// </summary>
        public static readonly char[] singleBorder = { '─', '│', '┌', '┐', '└', '┘', '├', '┤', '┬', '┴', '┼' };
        /// <summary>
        /// The double boarder
        /// </summary>
        public static readonly char[] doubleBoarder = { '═', '║', '╔', '╗', '╚', '╝', '╠', '╣', '╦', '╩', '╬' };
        /// <summary>
        /// The simple boarder
        /// </summary>
        public static readonly char[] simpleBoarder = { '-', '|', ',', ',', '\'', '\'', '+', '+', '+', '+', '+' };

        /// <summary>
        /// Gets the mouse Position.
        /// </summary>
        /// <value>The mouse Position.</value>
        public static Point MousePos { get; private set; }

        public static IConsole console { get; set; } = new ConsoleProxy();

        public static IExtendedConsole? ExtendedConsole
        {
            get => extendedConsole; set
            {
                if (extendedConsole != null)
                {
                    extendedConsole.MouseEvent -= OnMouseEvent;
                    extendedConsole.WindowBufferSizeEvent -= OnWindowSizeEvent;
                }
                extendedConsole = value;
                if (extendedConsole != null)
                {
                    extendedConsole.MouseEvent += OnMouseEvent;
                    extendedConsole.WindowBufferSizeEvent += OnWindowSizeEvent;
                }
            }
        }
        /// <summary>
        /// The canvas
        /// </summary>
        public static TextCanvas Canvas = new TextCanvas(new Rectangle(0, 0, Console.BufferWidth, Math.Min(50,Console.LargestWindowHeight)));
        private static IExtendedConsole? extendedConsole;

        /// <summary>
        /// Initializes static members of the <see cref="ConsoleFramework"/> class.
        /// </summary>
        static ConsoleFramework()
        {
        }

        /// <summary>
        /// Called when [window size event].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private static void OnWindowSizeEvent(object? sender, Point e)
        {
            (Canvas._dimension.Width, Canvas._dimension.Height) = (e.X, e.Y);
        }

        /// <summary>
        /// Called when [mouse event].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private static void OnMouseEvent(    object?            sender, IMouseEvent e)
        {
            MousePos = e.MousePos;
        }

        /// <summary>
        /// Sets the pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="color">The color.</param>
        static public void SetPixel(int x, int y, ConsoleColor color)
        {
            console.SetCursorPosition(x, y);
            console.BackgroundColor = color;
            console.Write(" ");
            console.BackgroundColor = ConsoleColor.Black;
        }

 
    }

}
