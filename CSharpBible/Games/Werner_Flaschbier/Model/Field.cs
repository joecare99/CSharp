// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 07-31-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Field.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Drawing;

namespace Werner_Flaschbier_Base.Model
{
    /// <summary>
    /// Class Field.
    /// </summary>
    public abstract class Field
    {
        /// <summary>
        /// The item
        /// </summary>
        protected PlayObject? _item=null;

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public Point Position { get;private set; }
        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>The item.</value>
        public PlayObject? Item { get=>_item; set =>SetItem(value); }

#if NET5_0_OR_GREATER
        public Playfield? Parent { get; private set; }
#else
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public Playfield Parent { get; private set; }
#endif        
        /// <summary>
        /// Gets the field definition.
        /// </summary>
        /// <value>The field definition.</value>
        public FieldDef fieldDef { get=>GetFieldDef();  }

        /// <summary>
        /// Gets the field definition.
        /// </summary>
        /// <returns>FieldDef.</returns>
        protected abstract FieldDef GetFieldDef();
        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="value">The value.</param>
        protected abstract void SetItem(PlayObject? value);

#if NET6_0_OR_GREATER
        public Field(Point position, Playfield? parentPlayObject )
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Field" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="parentPlayObject">The parent play object.</param>
        public Field(Point position, Playfield parentPlayObject )
#endif
        {
            Position = position;
            Parent = parentPlayObject;
        }
    }

    /// <summary>
    /// Class Destination.
    /// Implements the <see cref="Werner_Flaschbier_Base.Model.Space" />
    /// </summary>
    /// <seealso cref="Werner_Flaschbier_Base.Model.Space" />
    public class Destination : Space
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Destination" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="parentPlayObject">The parent play object.</param>
        public Destination(Point position, Playfield parentPlayObject) : base(position, parentPlayObject)
        {
        }

        /// <summary>
        /// Gets the field definition.
        /// </summary>
        /// <returns>FieldDef.</returns>
        /// <exception cref="System.ArgumentException">Illegal Item</exception>
        protected override FieldDef GetFieldDef() => Item switch
        {
            Player p => FieldDef.Destination,
            { } => throw new ArgumentException("Illegal Item"),
            null => FieldDef.Destination
        };

    }
}
