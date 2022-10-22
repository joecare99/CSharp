// ***********************************************************************
// Assembly         : Sokoban_Base
// Author           : Mir
// Created          : 07-09-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Direction.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Drawing;

namespace Sokoban_Base.Model
{
    /// <summary>
    /// Enum Direction
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// The north
        /// </summary>
        North,
        /// <summary>
        /// The west
        /// </summary>
        West,
        /// <summary>
        /// The south
        /// </summary>
        South,
        /// <summary>
        /// The east
        /// </summary>
        East
    }

    /// <summary>
    /// Class Offsets.
    /// </summary>
    public static class Offsets
    {
        /// <summary>
        /// The dir offsets
        /// </summary>
        static Point[] DirOffsets = { new Point(0, -1), new Point(-1, 0), new Point(0, 1), new Point(1, 0) };
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
            return p;
        }
    }
}