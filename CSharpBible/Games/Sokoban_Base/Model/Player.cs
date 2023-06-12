// ***********************************************************************
// Assembly         : Sokoban_Base
// Author           : Mir
// Created          : 07-09-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Player.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace Sokoban_Base.Model
{
    /// <summary>
    /// Class Player.
    /// Implements the <see cref="Sokoban_Base.Model.PlayObject" />
    /// </summary>
    /// <seealso cref="Sokoban_Base.Model.PlayObject" />
    public class Player : PlayObject
    {
        /// <summary>
        /// Gets or sets the last dir.
        /// </summary>
        /// <value>The last dir.</value>
        public Direction? LastDir { get; set; } = Direction.East;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player" /> class.
        /// </summary>
        /// <param name="aField">a field.</param>
        public Player(Field? aField) : base(aField)
        {
        }

        /// <summary>
        /// Goes the specified a dir.
        /// </summary>
        /// <param name="aDir">a dir.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Go(Direction? aDir) {
            if (aDir == null ) return false;
            else
            return TryMove((Direction)aDir); 
        }

        /// <summary>
        /// Tests if the object can move in the given direction.
        /// </summary>
        /// <param name="dir">The directon to test</param>
        /// <returns>true: if the object can move in the direction</returns>
        public override bool TestMove(Direction dir)
        {
            var newfield = field?.Parent?[Offsets.DirOffset(dir,Position)];
            if (newfield == null || !(newfield is Floor f)) return false;else
            {
                if (f.Item == null ) return true;else
                    return f.Item is Stone && f.Item.TestMove(dir);
            }
        }

        /// <summary>
        /// Tries to move the object in the given direction.
        /// </summary>
        /// <param name="dir">The directon to move</param>
        /// <returns>true: if the object has moveed in the direction</returns>
        public override bool TryMove(Direction dir)
        {
            var newfield = field?.Parent?[Offsets.DirOffset(dir, Position)];
            if (newfield == null || !(newfield is Floor f)) return false;
            else
            {
                if (f.Item == null) { f.Item = this; LastDir = dir; return true; }
                else
                    if (f.Item.TryMove(dir)) { f.Item = this; LastDir = dir; return true; } else return false;   
            }
        }

        /// <summary>
        /// Moveables the dirs.
        /// </summary>
        /// <returns>IEnumerable&lt;Direction&gt;.</returns>
        public IEnumerable<Direction> MoveableDirs() {
            foreach (Direction dir in Enum.GetValues(typeof(Direction))) 
                if (TestMove(dir)) yield return dir;
            yield break;
        }
    }
}
