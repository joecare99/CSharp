// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 07-31-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Wall.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Drawing;

namespace Werner_Flaschbier_Base.Model
{
    /// <summary>
    /// Class Wall.
    /// Implements the <see cref="Werner_Flaschbier_Base.Model.Field" />
    /// </summary>
    /// <seealso cref="Werner_Flaschbier_Base.Model.Field" />
    public class Wall : Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Wall" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="parentPlayObject">The parent play object.</param>
#if NET6_0_OR_GREATER
        public Wall(Point position, Playfield? parentPlayObject) : base(position, parentPlayObject)
#else
        public Wall(Point position, Playfield parentPlayObject) : base(position, parentPlayObject)
#endif
        {
        }

        /// <summary>
        /// Gets the field definition.
        /// </summary>
        /// <returns>FieldDef.</returns>
        protected override FieldDef GetFieldDef() => FieldDef.Wall;
        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentException"></exception>
        protected override void SetItem(PlayObject? value)
        {
            // a Wall cannot contain an object
            if (value != null)
               throw new ArgumentException();
        }
    }
}
