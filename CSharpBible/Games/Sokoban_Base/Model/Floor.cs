// ***********************************************************************
// Assembly         : Sokoban_Base
// Author           : Mir
// Created          : 07-09-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Floor.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Sokoban.Model.Interfaces;
using System;
using System.Drawing;

namespace Sokoban.Model;

/// <summary>
/// Class Floor.
/// Implements the <see cref="Field" />
/// </summary>
/// <seealso cref="Field" />
public class Floor: Field
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Floor" /> class.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="parentPlayObject">The parent play object.</param>
    public Floor(Point position, IPlayfield? parentPlayObject) : base(position, parentPlayObject)
    {
    }

    /// <summary>
    /// Gets the field definition.
    /// </summary>
    /// <returns>FieldDef.</returns>
    /// <exception cref="ArgumentException">Illegal Item</exception>
    protected override FieldDef GetFieldDef() => Item switch
    {
        Stone => FieldDef.Stone,
        Player => FieldDef.Player,
        { } => throw new ArgumentException("Illegal Item"),
        null => FieldDef.Floor
    };

    /// <summary>
    /// Sets the item.
    /// </summary>
    /// <param name="value">The value.</param>
    protected override void SetItem(IPlayObject? value)
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
