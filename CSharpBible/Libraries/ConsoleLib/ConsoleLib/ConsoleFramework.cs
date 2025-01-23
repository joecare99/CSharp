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
using ConsoleDisplay.View;
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
        public static readonly char[] singleBoarder = { '─', '│', '┌', '┐', '└', '┘', '├', '┤', '┬', '┴', '┼' };
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

        /// <summary>
        /// Gets a value indicating whether [mouse button left].
        /// </summary>
        /// <value><c>true</c> if [mouse button left]; otherwise, <c>false</c>.</value>
        public static bool MouseButtonLeft => System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left;
        /// <summary>
        /// Gets a value indicating whether [mouse button right].
        /// </summary>
        /// <value><c>true</c> if [mouse button right]; otherwise, <c>false</c>.</value>
        public static bool MouseButtonRight => System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Right;
        /// <summary>
        /// Gets a value indicating whether [mouse button middle].
        /// </summary>
        /// <value><c>true</c> if [mouse button middle]; otherwise, <c>false</c>.</value>
        public static bool MouseButtonMiddle => System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Middle;

        /// <summary>
        /// The canvas
        /// </summary>
        public static TextCanvas Canvas = new TextCanvas(new Rectangle(0, 0, Console.BufferWidth, Math.Min(50,Console.LargestWindowHeight)));

        /// <summary>
        /// The canvas
        /// </summary>
        public static IExtendedConsole console;

        /// <summary>
        /// Initializes static members of the <see cref="ConsoleFramework"/> class.
        /// </summary>
        static ConsoleFramework()
        {
            ExtendedConsole.MouseEvent += OnMouseEvent;
            ExtendedConsole.WindowBufferSizeEvent += OnWindowSizeEvent;
            console = new ExtendedConsole();
        }

        /// <summary>
        /// Called when [window Size event].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private static void OnWindowSizeEvent(object? sender, NativeMethods.WINDOW_BUFFER_SIZE_RECORD e)
        {
            (Canvas._dimension.Width, Canvas._dimension.Height) = (e.dwSize.X, e.dwSize.Y);
        }

        /// <summary>
        /// Called when [mouse event].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private static void OnMouseEvent(object? sender, NativeMethods.MOUSE_EVENT_RECORD e)
        {
            MousePos = e.dwMousePosition.AsPoint;
        }

        /// <summary>
        /// Sets the pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="color">The color.</param>
        static public void SetPixel(int x, int y, ConsoleColor color)
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = color;
            Console.Write(" ");
            Console.BackgroundColor = ConsoleColor.Black;
        }

 
    }

}
