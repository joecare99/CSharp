// ***********************************************************************
// Assembly         : Sokoban_Base
// Author           : Mir
// Created          : 07-09-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Wall.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Sokoban.Model.Interfaces;
using System;
using System.Drawing;

namespace Sokoban.Model;

/// <summary>
/// Class Wall.
/// Implements the <see cref="Field" />
/// </summary>
/// <seealso cref="Field" />
public class Wall : Field
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Wall" /> class.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="parentPlayObject">The parent play object.</param>
    public Wall(Point position, Playfield? parentPlayObject) : base(position, parentPlayObject)
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
    /// <exception cref="ArgumentException"></exception>
    protected override void SetItem(IPlayObject? value)
    {
        // a Wall cannot contain an object
        if (value != null)
           throw new ArgumentException();
    }
}
