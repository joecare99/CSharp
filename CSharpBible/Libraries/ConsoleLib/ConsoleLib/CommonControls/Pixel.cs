// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 04-19-2020
// ***********************************************************************
// <copyright file="Pixel.cs" company="ConsoleLib">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;

namespace ConsoleLib.CommonControls
{
    /// <summary>
    /// Class Pixel.
    /// Implements the <see cref="ConsoleLib.Control" />
    /// </summary>
    /// <seealso cref="ConsoleLib.Control" />
    public class Pixel : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Pixel"/> class.
        /// </summary>
        public Pixel()
        {
            size = new Size(1, 1);
        }
        /// <summary>
        /// Sets the specified x.
        /// </summary>
        /// <param name="X">The x.</param>
        /// <param name="Y">The y.</param>
        /// <param name="text">The text.</param>
        public void Set(int X, int Y, string text="")
        {
            if (text != "")
            {
                Text = text; 
            }
            position = new Point(X,Y);
        }

        /// <summary>
        /// Sets the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="text">The text.</param>
        public void Set(Point position, string text= "")
        {
            Set(position.X, position.Y, text);
        }
        /// <summary>
        /// Draws this instance.
        /// </summary>
        public override void Draw()
        {
            if (parent != null && !parent.dimension.Contains(Point.Add(position,(Size)parent.position))) return;
            Console.BackgroundColor = BackColor;
            ConsoleFramework.Canvas.OutTextXY(realDim.Location,$"{Text}");
            Console.BackgroundColor = ConsoleColor.Black;
        }

    }

}
