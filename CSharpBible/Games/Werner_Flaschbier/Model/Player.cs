// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 08-01-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Player.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace Werner_Flaschbier_Base.Model
{
    /// <summary>
    /// Class Player.
    /// Implements the <see cref="Werner_Flaschbier_Base.Model.PlayObject" />
    /// </summary>
    /// <seealso cref="Werner_Flaschbier_Base.Model.PlayObject" />
    public class Player : PlayObject
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is alive.
        /// </summary>
        /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
        public bool IsAlive { get; set; } = true;

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
        public override bool TestMove(Direction? dir)
        {
            var newfield = field?.Parent?[Offsets.DirOffset(dir,Position)];
            if (newfield == null || newfield is not Space s)
                return false;
            else
            {
                if (s.Item == null ) return true;else
                    return s.Item is Dirt;
            }
        }

        /// <summary>
        /// Tries to move the object in the given direction.
        /// </summary>
        /// <param name="dir">The directon to move</param>
        /// <returns>true: if the object has moveed in the direction</returns>
        public override bool TryMove(Direction? dir)
        {
            if (dir == null) return false;
            var newfield = field?.Parent?[Offsets.DirOffset(dir, Position)];
            if (newfield == null || newfield is not Space f )
            {
                if (newfield is Destination)
                { newfield.Item = this; return true; }
                else
                    return false;
            }
            else
            {
                f.Item?.TryMove();
                if (f.Item == null || f.Item is Dirt) 
                  { f.Item = this; return true; }
                else
                   return false;   
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
