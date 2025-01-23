// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 04-19-2020
// ***********************************************************************
// <copyright file="Pixel.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
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
            Size = new Size(1, 1);
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
            Position = new Point(X,Y);
        }

        /// <summary>
        /// Sets the specified Position.
        /// </summary>
        /// <param name="position">The Position.</param>
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
            if (Parent != null && !Parent.Dimension.Contains(Point.Add(Position,(Size)Parent.Position))) return;
        }

    }

}
