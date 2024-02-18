// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 08-01-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Dirt.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Werner_Flaschbier_Base.Model
{
    /// <summary>
    /// Class Dirt.
    /// Implements the <see cref="Werner_Flaschbier_Base.Model.PlayObject" />
    /// </summary>
    /// <seealso cref="Werner_Flaschbier_Base.Model.PlayObject" />
    public class Dirt : PlayObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dirt" /> class.
        /// </summary>
        /// <param name="aField">a field.</param>
        public Dirt(Field? aField) : base(aField)
        {
        }

        /// <summary>
        /// Tests if the object can move in the given direction.
        /// </summary>
        /// <param name="dir">The directon to test</param>
        /// <returns>true: if the object can move in the direction</returns>
        public override bool TestMove(Direction? dir = null)
        {
            return false;
        }

        /// <summary>
        /// Tries to move the object in the given direction.
        /// </summary>
        /// <param name="dir">The directon to move</param>
        /// <returns>true: if the object has moveed in the direction</returns>
        public override bool TryMove(Direction? dir = null)
        {
            return false;
        }
    }
}
