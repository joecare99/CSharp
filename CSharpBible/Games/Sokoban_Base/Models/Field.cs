// ***********************************************************************
// Assembly         : Sokoban
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
using Sokoban.Models.Interfaces;
using System.Drawing;

namespace Sokoban.Models;

/// <summary>
/// Class Field.
/// </summary>
public abstract class Field : IField
{
    /// <summary>
    /// The item
    /// </summary>
    protected IPlayObject? _item = null;

    /// <summary>
    /// Gets the position.
    /// </summary>
    /// <value>The position.</value>
    public Point Position { get; private set; }
    /// <summary>
    /// Gets or sets the item.
    /// </summary>
    /// <value>The item.</value>
    public IPlayObject? Item { get => _item; set => SetItem(value); }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <value>The parent.</value>
    public IPlayfield? Parent { get; private set; }
    /// <summary>
    /// Gets the field definition.
    /// </summary>
    /// <value>The field definition.</value>
    public FieldDef fieldDef { get => GetFieldDef(); }

    /// <summary>
    /// Gets the field definition.
    /// </summary>
    /// <returns>FieldDef.</returns>
    protected abstract FieldDef GetFieldDef();
    /// <summary>
    /// Sets the item.
    /// </summary>
    /// <param name="value">The value.</param>
    protected abstract void SetItem(IPlayObject? value);

    /// <summary>
    /// Initializes a new instance of the <see cref="Field" /> class.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="parentPlayObject">The parent play object.</param>
    public Field(Point position, IPlayfield? parentPlayObject)
    {
        Position = position;
        Parent = parentPlayObject;
    }
}
