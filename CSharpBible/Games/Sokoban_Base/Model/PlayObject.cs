// ***********************************************************************
// Assembly         : Sokoban_Base
// Author           : Mir
// Created          : 07-09-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="PlayObject.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban_Base.Model
{
    /// <summary>
    /// Class PlayObject.
    /// </summary>
    public abstract class PlayObject
    {

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position of the player on the playfield</value>
        public Point Position { get; set; }
        /// <summary>
        /// Gets or sets the old position.
        /// </summary>
        /// <value>The old position.</value>
        public Point OldPosition { get; set; }

        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>The field as reference.</value>
        public Field? field { get; set; }

        /// <summary>
        /// Tests if the object can move in the given direction.
        /// </summary>
        /// <param name="dir">The directon to test</param>
        /// <returns>true: if the object can move in the direction</returns>
        public abstract bool TestMove(Direction dir);

        /// <summary>
        /// Tries to move the object in the given direction.
        /// </summary>
        /// <param name="dir">The directon to move</param>
        /// <returns>true: if the object has moveed in the direction</returns>
        public abstract bool TryMove(Direction dir);

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayObject" /> class.
        /// </summary>
        /// <param name="aField">a field.</param>
        public PlayObject(Field? aField) { field = aField; }
    }
}
