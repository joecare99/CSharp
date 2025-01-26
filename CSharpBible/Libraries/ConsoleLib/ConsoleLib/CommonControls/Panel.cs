// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 07-21-2022
// ***********************************************************************
// <copyright file="Panel.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;

namespace ConsoleLib.CommonControls
{
    /// <summary>
    /// Class Panel.
    /// Implements the <see cref="ConsoleLib.Control" />
    /// </summary>
    /// <seealso cref="ConsoleLib.Control" />
    public class Panel : Control
    {
        /// <summary>
        /// The boarder
        /// </summary>
        public char[] Border = {' ', ' ', ' ', ' ', ' ', ' ' };
        /// <summary>
        /// The boarder color
        /// </summary>
        public ConsoleColor BoarderColor;

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public override void Draw()
        {
            ConsoleFramework.Canvas.FillRect(RealDim,ForeColor, BackColor, ConsoleFramework.chars[3]);
            if (Border != null && Border.Length > 5)
                ConsoleFramework.Canvas.DrawRect(RealDim, BoarderColor, BackColor, Border);
            foreach( Control c in Children) if (c.Visible)
                {
                if (c.Shadow)
                {
                    var sdim = c.Dimension;
                    sdim.Offset(1, 1);
                    sdim.Offset(Position);
                    ConsoleFramework.Canvas.FillRect(RealDimOf(sdim), ConsoleColor.DarkGray, ConsoleColor.Black, ConsoleFramework.chars[4]);
                }
                c.Draw();

            }
            Valid = true;
        }

        /// <summary>
        /// Res the draw.
        /// </summary>
        /// <param name="dimension">The Dimension.</param>
        public override void ReDraw(Rectangle dimension)
        {
            if (dimension.IsEmpty) return;
            Rectangle innerRect = _dimension;
            innerRect.Inflate(-1, -1);
            var icl = dimension;
            icl.Intersect(innerRect);
            try
            {
                ConsoleFramework.Canvas.FillRect(RealDimOf(icl), ForeColor, BackColor, ConsoleFramework.chars[3]);
                // ToDo: Border
                if (Border != null && Border.Length > 5 && _dimension.IntersectsWith(dimension) &&
                    !(innerRect.Contains(dimension.Location) && innerRect.Contains(Point.Subtract(Point.Add(dimension.Location, dimension.Size), new Size(1, 1)))
                    ))
                    ConsoleFramework.Canvas.DrawRect(RealDim, BoarderColor, BackColor, Border);
                foreach (Control c in Children)
                    if (c.Visible)
                    {
                        if (c.Shadow)
                        {
                            var sdim = c.Dimension;
                            sdim.Offset(1, 1);
                            sdim.Offset(Position);
                            sdim.Intersect(dimension);
                            ConsoleFramework.Canvas.FillRect(RealDimOf(sdim), ConsoleColor.DarkGray, ConsoleColor.Black, ConsoleFramework.chars[4]);
                        }
                        var CClip = dimension;
                        CClip.Location = Point.Subtract(dimension.Location, (Size)_dimension.Location);
                        c.ReDraw(CClip);
                    }
                Valid = true;
            }
            catch
            {

            }
        }
    }

}
