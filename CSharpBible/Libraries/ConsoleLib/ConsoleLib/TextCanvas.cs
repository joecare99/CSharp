// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 04-19-2020
// ***********************************************************************
// <copyright file="TextCanvas.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;

namespace ConsoleLib
{
    /// <summary>
    /// Class TextCanvas.
    /// </summary>
    public class TextCanvas
    {
        /// <summary>
        /// The Dimension
        /// </summary>
        internal Rectangle _dimension;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextCanvas"/> class.
        /// </summary>
        /// <param name="dimension">The Dimension.</param>
        public TextCanvas(Rectangle dimension)
        {
            _dimension = dimension;
        }

        /// <summary>
        /// Gets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public ConsoleColor BackgroundColor { get; internal set; }
        /// <summary>
        /// Gets the clip rect.
        /// </summary>
        /// <value>The clip rect.</value>
        public Rectangle ClipRect { get =>_dimension; }
        /// <summary>
        /// Gets the color of the foreground.
        /// </summary>
        /// <value>The color of the foreground.</value>
        public ConsoleColor ForegroundColor { get; internal set; }


        /// <summary>
        /// Fills the rect.
        /// </summary>
        /// <param name="dimension">The Dimension.</param>
        /// <param name="frcolor">The frcolor.</param>
        /// <param name="bkcolor">The bkcolor.</param>
        /// <param name="c">The c.</param>
        public void FillRect(Rectangle dimension,ConsoleColor frcolor, ConsoleColor bkcolor, Char c)
        {
            lock (this)
            {
                Console.BackgroundColor = bkcolor;
                Console.ForegroundColor = frcolor;
                if (_dimension.Contains(dimension.Location))
                {
                    // Build String
                    string sLine = "";
                    for (int j = dimension.X; j < dimension.Right; j++)
                    {
                        sLine += c;
                    }
                    for (int i = dimension.Y; i < dimension.Bottom; i++)
                    {
                        _OutTextXY(dimension.X, i, sLine);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the rect.
        /// </summary>
        /// <param name="dimension">The Dimension.</param>
        /// <param name="frcolor">The frcolor.</param>
        /// <param name="bkcolor">The bkcolor.</param>
        /// <param name="boarder">The boarder.</param>
        public void DrawRect(Rectangle dimension, ConsoleColor frcolor, ConsoleColor bkcolor, char[] boarder)
        {
            lock (this)
            {
                if (dimension.Width == 0 || dimension.Height == 0) return;
                if (dimension.Width == 1)
                {
                    for (int i = dimension.Y; i < dimension.Bottom; i++)
                    {
                        _OutTextXY(dimension.Left, i, boarder[1]);
                    }
                    return;
                }
                if (dimension.Height == 1)
                {
                    for (int j = dimension.X; j < dimension.Right; j++)
                    {
                        _OutTextXY(j, dimension.Top, boarder[0]);
                    }
                    return;
                }
                Console.BackgroundColor = bkcolor;
                Console.ForegroundColor = frcolor;
                if (_dimension.Contains(dimension.Location))
                    for (int i = dimension.Y + 1; i < dimension.Bottom - 1; i++)
                    {
                        _OutTextXY(dimension.Left, i, boarder[1]);
                        _OutTextXY(dimension.Right - 1, i, boarder[1]);
                    }

                string sLine = "";
                for (int j = dimension.X; j < dimension.Right - 2; j++)
                {
                    sLine += boarder[0];
                }

                _OutTextXY(dimension.X + 1, dimension.Top, sLine);
                _OutTextXY(dimension.X + 1, dimension.Bottom - 1, sLine);

                _OutTextXY(dimension.Location, boarder[2]);
                _OutTextXY(dimension.Right - 1, dimension.Top, boarder[3]);
                _OutTextXY(dimension.Left, dimension.Bottom - 1, boarder[4]);
                _OutTextXY(dimension.Right - 1, dimension.Bottom - 1, boarder[5]);
            }
        }

        /// <summary>
        /// Outs the text xy.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="s">The s.</param>
        public void OutTextXY(Point place, string s)
        {
            lock (this)
                _OutTextXY(place.X, place.Y, s);
        }
        /// <summary>
        /// Outs the text xy.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="c">The c.</param>
        public void OutTextXY(Point place, char c)
        {
            lock (this)
                _OutTextXY(place.X, place.Y, c);
        }

        /// <summary>
        /// Outs the text xy.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="c">The c.</param>
        public void _OutTextXY(Point place, char c) => _OutTextXY(place.X, place.Y, c);

        /// <summary>
        /// Outs the text xy.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="s">The s.</param>
        public void OutTextXY(int x,int y, string s)
        {
            lock (this)
                _OutTextXY(x, y, s);
        }

        /// <summary>
        /// Outs the text xy.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="s">The s.</param>
        private void _OutTextXY(int x,int y, string s)
        {
            Console.SetCursorPosition(x + _dimension.X, y + _dimension.Y);
            Console.Write(s);
        }

        /// <summary>
        /// Outs the text xy.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="c">The c.</param>
        public void OutTextXY(int x, int y, char c)
        {
            lock (this)
                _OutTextXY(x, y, c);
        }

        /// <summary>
        /// Outs the text xy.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="c">The c.</param>
        private void _OutTextXY(int x, int y, char c)
        {
            Console.SetCursorPosition(x + _dimension.X, y + _dimension.Y);
            Console.Write(c);
        }
        /// <summary>
        /// Outs the text xy.
        /// </summary>
        /// <param name="x">The x-koordinate</param>
        /// <param name="y">The y-koordinate</param>
        /// <param name="c">The Char</param>
        /// <param name="f">The Foreground-color</param>
        /// <param name="b">The Background-color</param>
        public void OutTextXY(int x, int y, char c,ConsoleColor f,ConsoleColor b)
        {
            lock (this)
            {
                Console.ForegroundColor = f;
                Console.BackgroundColor = b;
                Console.SetCursorPosition(x + _dimension.X, y + _dimension.Y);
                Console.Write(c);
            }
        }
    }

}
