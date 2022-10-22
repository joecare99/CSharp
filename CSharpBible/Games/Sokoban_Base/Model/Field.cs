// ***********************************************************************
// Assembly         : Sokoban_Base
// Author           : Mir
// Created          : 07-09-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Field.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Drawing;

namespace Sokoban_Base.Model
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
    /// Class Wall.
    /// Implements the <see cref="Sokoban_Base.Model.Field" />
    /// </summary>
    /// <seealso cref="Sokoban_Base.Model.Field" />
    public class Wall : Field
    {
#if NET6_0_OR_GREATER
        public Wall(Point position, Playfield? parentPlayObject) : base(position, parentPlayObject)
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Wall" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="parentPlayObject">The parent play object.</param>
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

    /// <summary>
    /// Class Floor.
    /// Implements the <see cref="Sokoban_Base.Model.Field" />
    /// </summary>
    /// <seealso cref="Sokoban_Base.Model.Field" />
    public class Floor: Field
    {
#if NET6_0_OR_GREATER
        public Floor(Point position, Playfield? parentPlayObject) : base(position, parentPlayObject)
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Floor" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="parentPlayObject">The parent play object.</param>
        public Floor(Point position, Playfield parentPlayObject) : base(position, parentPlayObject)
#endif
        {
        }

        /// <summary>
        /// Gets the field definition.
        /// </summary>
        /// <returns>FieldDef.</returns>
        /// <exception cref="System.ArgumentException">Illegal Item</exception>
        protected override FieldDef GetFieldDef() => Item switch
        {
            Stone s => FieldDef.Stone,
            Player s => FieldDef.Player,
            { } => throw new ArgumentException("Illegal Item"),
            null => FieldDef.Floor
        };

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="value">The value.</param>
        protected override void SetItem(PlayObject? value)
        {
            if (value == Item) return;
            if (Item != null && value !=null) return; // Cannot contain 2 Objects
            if (value == null)
            {
                if (Item != null)
                {
                    Item.OldPosition = Item.Position;
                    Item.Position = new Point(0, 0);
                    Item.field = null;
                    _item = null;
                }
            }
            else
            {
                if (value.field != this && value.field != null)
                {
                    var f = value.field;
                    f.Item = null;
                }
//                value.OldPosition = value.Position;
                value.Position = Position;
                value.field = this;
                _item = value;
            }
                
        }
    }

    /// <summary>
    /// Class Destination.
    /// Implements the <see cref="Sokoban_Base.Model.Floor" />
    /// </summary>
    /// <seealso cref="Sokoban_Base.Model.Floor" />
    public class Destination : Floor
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
            Stone s => FieldDef.StoneInDest,
            Player s => FieldDef.PlayerOverDest,
            { } => throw new ArgumentException("Illegal Item"),
            null => FieldDef.Destination
        };

    }
}
