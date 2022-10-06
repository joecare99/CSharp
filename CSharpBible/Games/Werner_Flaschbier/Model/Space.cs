// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 07-31-2022
//
// Last Modified By : Mir
// Last Modified On : 08-08-2022
// ***********************************************************************
// <copyright file="Space.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Drawing;

namespace Werner_Flaschbier_Base.Model
{
    /// <summary>
    /// Class Space.
    /// Implements the <see cref="Werner_Flaschbier_Base.Model.Field" />
    /// </summary>
    /// <seealso cref="Werner_Flaschbier_Base.Model.Field" />
    public class Space: Field
    {
#if NET6_0_OR_GREATER
        /// <summary>
        /// Initializes a new instance of the <see cref="Space"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="parentPlayObject">The parent play object.</param>
        public Space(Point position, Playfield? parentPlayObject) : base(position, parentPlayObject)
#else
        public Floor(Point position, Playfield parentPlayObject) : base(position, parentPlayObject)
#endif
        {
        }
        /// <summary>
        /// Gets or sets the old item.
        /// </summary>
        /// <value>The old item.</value>
        public PlayObject OldItem { get; set; }

        /// <summary>
        /// Gets the field definition.
        /// </summary>
        /// <returns>FieldDef.</returns>
        protected override FieldDef GetFieldDef() => Item switch
        {
            Stone s => FieldDef.Stone,
            Player p => FieldDef.Player,
            Enemy e => FieldDef.Enemy,
            Dirt d => FieldDef.Dirt,
            { } => throw new ArgumentException("Illegal Item"),
            null => FieldDef.Empty
        };

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="value">The value.</param>
        protected override void SetItem(PlayObject? value)
        {
            if (value == Item) return;
            if (Item != null && Item is not Dirt && value !=null) return; // Cannot contain 2 Objects
            if (value == null)
            {
                if (Item != null)
                {
                    Item.OldPosition = Item.Position;
                    Item.Position = new Point(0, 0);
                    Item.field = null;
                    OldItem = Item;
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
                OldItem = Item;
                _item = value;
            }
                
        }
    }
}
