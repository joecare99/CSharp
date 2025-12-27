// ***********************************************************************
// Assembly         : Sokoban
// Author           : Mir
// Created          : 07-09-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Destination.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Sokoban.Models.Interfaces;
using System;
using System.Drawing;

namespace Sokoban.Models;

/// <summary>
/// Class Destination.
/// Implements the <see cref="Floor" />
/// </summary>
/// <seealso cref="Floor" />
public class Destination : Floor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Destination" /> class.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="parentPlayObject">The parent play object.</param>
    public Destination(Point position, IPlayfield parentPlayObject) : base(position, parentPlayObject)
    {
    }

    /// <summary>
    /// Gets the field definition.
    /// </summary>
    /// <returns>FieldDef.</returns>
    /// <exception cref="ArgumentException">Illegal Item</exception>
    protected override FieldDef GetFieldDef() => Item switch
    {
        Stone s => FieldDef.StoneInDest,
        Player s => FieldDef.PlayerOverDest,
        { } => throw new ArgumentException("Illegal Item"),
        null => FieldDef.Destination
    };

}
