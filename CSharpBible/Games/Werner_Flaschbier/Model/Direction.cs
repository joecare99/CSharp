// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 07-31-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Direction.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Drawing;

namespace Werner_Flaschbier_Base.Model
{
    /// <summary>
    /// Enum Direction
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Up
        /// </summary>
        Up,
        /// <summary>
        /// The west
        /// </summary>
        West,
        /// <summary>
        /// Down
        /// </summary>
        Down,
        /// <summary>
        /// The east
        /// </summary>
        East,
        /// <summary>
        /// The west down
        /// </summary>
        WestDown,
        /// <summary>
        /// The east down
        /// </summary>
        EastDown,
    }

    /// <summary>
    /// Class Offsets.
    /// </summary>
    public static class Offsets
    {
        /// <summary>
        /// The dir offsets
        /// </summary>
        static Point[] DirOffsets = { new Point(0, -1), 
            new Point(-1, 0), 
            new Point(0, 1), 
            new Point(1, 0),
            new Point(-1, 1),new Point(1, 1) };
        /// <summary>
        /// Dirs the offset.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>Point.</returns>
        public static Point DirOffset(Direction d) => DirOffsets[(int)d];

        /// <summary>
        /// Dirs the offset.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="position">The position.</param>
        /// <returns>Point.</returns>
        public static Point DirOffset(Direction? dir, Point position)
        {
            var p = new Point(position.X,position.Y);
            if (dir.HasValue)
              p.Offset(DirOffset((Direction)dir));
            if ((p.X < 0) && (p.Y > 0)) { p.X = 19;p.Y--; }
            if ((p.X > 19) && (p.Y < 11)) { p.X = 0; p.Y++; }
            return p;
        }
    }
}