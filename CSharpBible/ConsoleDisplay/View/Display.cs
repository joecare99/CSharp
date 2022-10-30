// ***********************************************************************
// Assembly         : ConsoleDisplay
// Author           : Mir
// Created          : 07-16-2022
//
// Last Modified By : Mir
// Last Modified On : 07-18-2022
// ***********************************************************************
// <copyright file="Display.cs" company="ConsoleDisplay">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;

namespace ConsoleDisplay.View
{

    /// <summary>
    /// Class Display.
    /// </summary>
    public class Display
    {
        /// <summary>
        /// The color map
        /// </summary>
        static ConsoleColor[]? colorMap = null;
        /// <summary>
        /// My console
        /// </summary>
        static public MyConsoleBase myConsole = new MyConsole();

        /// <summary>
        /// The h block
        /// </summary>
        const char hBlock = '▄';
        /// <summary>
        /// Initializes a new instance of the <see cref="Display"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Display(int x, int y, int width, int height)
        {
            Origin = new Point(x, y);
            dSize = new Size(width, height);
            ScreenBuffer = new ConsoleColor[dSize.Width * dSize.Height];
            OutBuffer = new ConsoleColor[dSize.Width * dSize.Height];

            if (colorMap == null)
            {
                colorMap = new ConsoleColor[64];

                for (var i = 63; i > 0; i--)
                {
                    var cdm = 0;
                    for (var j = 0; j < 16; j++)
                    {
                        var cd = Math.Abs(i % 4 - ((j % 8) / 4) * ((j / 8) * 2 + 1))
                        + Math.Abs((i / 4) % 4 - (j % 4) / 2 * ((j / 8) * 2 + 1))
                        + Math.Abs((i / 16) - (j % 2) * ((j / 8) * 2 + 1));
                        if (j == 0 || cd <= cdm)
                        {
                            colorMap[i] = (ConsoleColor)j;
                            cdm = cd;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Gets the origin.
        /// </summary>
        /// <value>The origin.</value>
        public Point Origin { get; private set; }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            for (var i = 0; i < ScreenBuffer.Length; i++)
                ScreenBuffer[i] = ConsoleColor.Black;
        }

        /// <summary>
        /// Gets the size of the d.
        /// </summary>
        /// <value>The size of the d.</value>
        public Size dSize { get; private set; }

        /// <summary>
        /// Gets the screen buffer.
        /// </summary>
        /// <value>The screen buffer.</value>
        public ConsoleColor[] ScreenBuffer { get; private set; }
        /// <summary>
        /// Gets or sets the out buffer.
        /// </summary>
        /// <value>The out buffer.</value>
        private ConsoleColor[] OutBuffer { get; set; }
        /// <summary>
        /// Gets or sets the default color of the outside.
        /// </summary>
        /// <value>The default color of the outside.</value>
        public ConsoleColor DefaultOutsideColor { get; set; } = ConsoleColor.Black;

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
        {
            var _fgr = myConsole.ForegroundColor;
            var _bgr = myConsole.BackgroundColor;
            for (int y = 0; y < dSize.Height/2*2; y += 2)
                for (int x = 0; x < dSize.Width; x++)
                    if (ScreenBuffer[y * dSize.Width + x] != OutBuffer[y * dSize.Width + x] ||
                        ScreenBuffer[(y + 1) * dSize.Width + x] != OutBuffer[(y + 1) * dSize.Width + x])
                    {
                        myConsole.BackgroundColor= ScreenBuffer[(y + 0) * dSize.Width + x];
                        myConsole.ForegroundColor = ScreenBuffer[(y + 1) * dSize.Width + x];
                        myConsole.SetCursorPosition(Origin.X + x, Origin.Y + y / 2);
                        myConsole.Write(hBlock);
                        OutBuffer[y * dSize.Width + x] = ScreenBuffer[y * dSize.Width + x];
                        OutBuffer[(y + 1) * dSize.Width + x] = ScreenBuffer[(y + 1) * dSize.Width + x];
                    }
            if (dSize.Height % 2 == 1) 
            {
                var y = (dSize.Height - 1); 
                for (int x = 0; x < dSize.Width; x++)
                    if (ScreenBuffer[y * dSize.Width + x] != OutBuffer[y * dSize.Width + x])
                    {
                        myConsole.BackgroundColor = ScreenBuffer[(y + 0) * dSize.Width + x];
                        myConsole.ForegroundColor = ConsoleColor.Black;
                        myConsole.SetCursorPosition(Origin.X + x, Origin.Y + y / 2);
                        myConsole.Write(hBlock);
                        OutBuffer[y * dSize.Width + x] = ScreenBuffer[y * dSize.Width + x];
                    }
            }
            myConsole.ForegroundColor=_fgr;
            myConsole.BackgroundColor=_bgr;
        }

        /// <summary>
        /// Puts the pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        public void PutPixel(int x, int y, byte r, byte g, byte b)
        {
            if (x >= 0 && x < dSize.Width && y >= 0 && y < dSize.Height)
                ScreenBuffer[x + y * dSize.Width] = colorMap?[r / 64 + (g / 64) * 4 + (b / 64) * 16] ?? ConsoleColor.Black;
        }

        /// <summary>
        /// Puts the pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="c">The c.</param>
        public void PutPixel(int x, int y, ConsoleColor c)
        {
            if (x >= 0 && x < dSize.Width && y >= 0 && y < dSize.Height)
                ScreenBuffer[x + y * dSize.Width] = c;
        }

        /// <summary>
        /// Puts the line.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="c">The c.</param>
        public void PutLine(int x1, int y1, int x2, int y2, ConsoleColor c)
        {
            var mx = Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2));
            for (var i = 0; i <= mx; i++)
                PutPixel(x1 + (x2 - x1) * i / mx, y1 + (y2 - y1) * i / mx, c);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            string result=this.GetType().ToString();
            result += ".";
            result += $"({dSize.Width};{dSize.Height}),";
            for (var i = 0; i < ScreenBuffer.Length; i++)
                result += $"{(byte)ScreenBuffer[i]:X}";
            return result;
        }

        /// <summary>
        /// Gets the pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>ConsoleColor.</returns>
        public ConsoleColor GetPixel(int x, int y)
        {
            if (x >= 0 && x < dSize.Width && y >= 0 && y < dSize.Height)
                return ScreenBuffer[x + y * dSize.Width];
            else
                return DefaultOutsideColor;
        }
    }

}
