// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 04-19-2020
// ***********************************************************************
// <copyright file="TextCanvas.cs" company="ConsoleLib">
//     Copyright (c) HP Inc.. All rights reserved.
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
        /// The dimension
        /// </summary>
        internal Rectangle _dimension;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextCanvas"/> class.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
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
        /// <param name="dimension">The dimension.</param>
        /// <param name="frcolor">The frcolor.</param>
        /// <param name="bkcolor">The bkcolor.</param>
        /// <param name="c">The c.</param>
        public void FillRect(Rectangle dimension,ConsoleColor frcolor, ConsoleColor bkcolor, Char c)
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
                    OutTextXY(dimension.X, i, sLine);
                }
            }
        }

        /// <summary>
        /// Draws the rect.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        /// <param name="frcolor">The frcolor.</param>
        /// <param name="bkcolor">The bkcolor.</param>
        /// <param name="boarder">The boarder.</param>
        public void DrawRect(Rectangle dimension, ConsoleColor frcolor, ConsoleColor bkcolor, char[] boarder)
        {
            if (dimension.Width==0 || dimension.Height==0)  return;
            if (dimension.Width == 1 ) 
            {
                for (int i = dimension.Y ; i < dimension.Bottom ; i++)
                {
                    OutTextXY(dimension.Left, i, boarder[1]);
                }
                return;
            }
            if ( dimension.Height == 1 )
            {
                for (int j = dimension.X ; j < dimension.Right ; j++)
                {
                    OutTextXY(j, dimension.Top, boarder[0]);
                }
                return;
            }
            Console.BackgroundColor = bkcolor;
            Console.ForegroundColor = frcolor;
            if (_dimension.Contains(dimension.Location))
                for (int i = dimension.Y + 1; i < dimension.Bottom - 1; i++)
                {
                    OutTextXY(dimension.Left, i, boarder[1]);
                    OutTextXY(dimension.Right-1, i, boarder[1]);
                }

            string sLine = "";
            for (int j = dimension.X; j < dimension.Right - 2; j++)
            {
                sLine += boarder[0];
            }

            OutTextXY(dimension.X+1, dimension.Top, sLine);
            OutTextXY(dimension.X+1, dimension.Bottom-1, sLine);
 
            OutTextXY(dimension.Location, boarder[2]);
            OutTextXY(dimension.Right-1,dimension.Top, boarder[3]);
            OutTextXY(dimension.Left, dimension.Bottom-1, boarder[4]);
            OutTextXY(dimension.Right-1,dimension.Bottom-1, boarder[5]);
        }

        /// <summary>
        /// Outs the text xy.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="s">The s.</param>
        public void OutTextXY(Point place, string s)
        {
            OutTextXY(place.X, place.Y, s);
        }
        /// <summary>
        /// Outs the text xy.
        /// </summary>
        /// <param name="place">The place.</param>
        /// <param name="c">The c.</param>
        public void OutTextXY(Point place, char c)
        {
            OutTextXY(place.X, place.Y, c);
        }

        /// <summary>
        /// Outs the text xy.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="s">The s.</param>
        public void OutTextXY(int x,int y, string s)
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
            Console.SetCursorPosition(x + _dimension.X, y + _dimension.Y);
            Console.Write(c);
        }
    }

}
