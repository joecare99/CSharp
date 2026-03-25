// ***********************************************************************
// Assembly         : Sokoban
// Author           : Mir
// Created          : 07-09-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Playfield.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Sokoban.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Sokoban.Models;

/// <summary>
/// Struct MoveField
/// </summary>
public struct MoveField
{
    /// <summary>
    /// The p
    /// </summary>
    public Point p;
    /// <summary>
    /// The n definition
    /// </summary>
    public FieldDef nDef;
}

/// <summary>
/// Struct Move
/// </summary>
public struct Move
{
    /// <summary>
    /// The d
    /// </summary>
    public Direction d;
    /// <summary>
    /// The mf
    /// </summary>
    public MoveField[] mf;
}

/// <summary>
/// Class Playfield.
/// </summary>
public class Playfield : IPlayfield
{
    /// <summary>
    /// Gets or sets the <see cref="System.Nullable{Field}" /> with the specified p.
    /// </summary>
    /// <param name="p">The p.</param>
    /// <returns>System.Nullable&lt;Field&gt;.</returns>
    public IField? this[Point p]
    {
        get => fields.TryGetValue((p.X, p.Y), out var v) ? v : null; set
        {
            if (value != null) fields[(p.X, p.Y)] = value;
        }
    }

    /// <summary>
    /// vs the field.
    /// </summary>
    /// <param name="p">The p.</param>
    /// <param name="moves">The moves.</param>
    /// <returns>FieldDef.</returns>
    public FieldDef VField(Point p, IList<Move> moves)
    {
        for (var i = moves.Count - 1; i >= 0; i--)
            foreach (var m in moves[i].mf)
                if (m.p == p) return m.nDef;
        return this[p]?.fieldDef ?? FieldDef.Empty;
    }

    /// <summary>
    /// The fields
    /// </summary>
    private Dictionary<(int, int), IField> fields = [];
    /// <summary>
    /// The field size
    /// </summary>
    public Size fieldSize { get; private set; } = Size.Empty;
    /// <summary>
    /// The stones
    /// </summary>
    public IList<IPlayObject> Stones { get; private set; } = [];
    /// <summary>
    /// The fields
    /// </summary>
    private List<IField> _fields = [];
    /// <summary>
    /// The player
    /// </summary>
    public IPlayer? player { get; private set; }

    /// <summary>
    /// Gets the stones in dest.
    /// </summary>
    /// <value>The stones in dest.</value>
    public int StonesInDest { get => Stones.Where((s) => s.field is Destination).Count(); }
    /// <summary>
    /// Gets a value indicating whether [game solved].
    /// </summary>
    /// <value><c>true</c> if [game solved]; otherwise, <c>false</c>.</value>
    public bool GameSolved { get => Stones.Count == StonesInDest; }

    /// <summary>
    /// Setups the specified sf definition.
    /// </summary>
    /// <param name="SFDef">The sf definition.</param>
    public void Setup(string[] SFDef)
    {
        Clear();
        var fs = fieldSize;
        fs.Height = SFDef.Length;     
        var lnr = 0;
        foreach (var line in SFDef)
        {
            fs.Width = Math.Max(fs.Width, line.Length);
            for (int x = 0; x < line.Length; x++)
            {
                CreateField(FieldDefs.SDef[line[x]], x, lnr);
            }
            lnr++;
        }
        fieldSize = fs;
    }

    /// <summary>
    /// Setups the specified sf definition.
    /// </summary>
    /// <param name="SFDef">The sf definition.</param>
    public void Setup((FieldDef[], Size) SFDef)
    {
        Clear();
        fieldSize = SFDef.Item2;
        var lnr = 0;
        foreach (var fd in SFDef.Item1)
        {
            CreateField(fd, lnr % fieldSize.Width, lnr / fieldSize.Width);
            lnr++;
        }
    }

    /// <summary>
    /// Creates the field.
    /// </summary>
    /// <param name="fieldDef">The field definition.</param>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns>System.Nullable&lt;Field&gt;.</returns>
    private Field? CreateField(FieldDef fieldDef, int x, int y)
    {
        Field? result = null;
        switch (fieldDef)
        {
            case FieldDef.Empty:
                break;
            case FieldDef.Wall:
                result = new Wall(new Point(x, y), this);
                break;
            case FieldDef.Floor:
                if ((x > 0 || y > 0) && this[Offsets.DirOffset(Direction.North, new Point(x, y))] != null && this[Offsets.DirOffset(Direction.West, new Point(x, y))] != null)
                    result = new Floor(new Point(x, y), this);
                break;
            case FieldDef.Player:
            case FieldDef.Stone:
                result = new Floor(new Point(x, y), this);
                break;
            case FieldDef.Destination:
            case FieldDef.PlayerOverDest:
            case FieldDef.StoneInDest:
                result = new Destination(new Point(x, y), this);
                break;
        }
        if (result != null)
        {
            _fields.Add(result);
            fields[(x, y)] = result;
            switch (fieldDef)
            {
                case FieldDef.Player:
                case FieldDef.PlayerOverDest:
                    result.Item = CreatePLayer();
                    break;

                case FieldDef.Stone:
                case FieldDef.StoneInDest:
                    result.Item = CreateStone();
                    break;
            }
        }
        return result;
    }

    /// <summary>
    /// Creates the stone.
    /// </summary>
    /// <returns>Stone.</returns>
    private IPlayObject CreateStone()
    {
        Stone result = new Stone(null);
        Stones.Add(result);
        return result;
    }

    /// <summary>
    /// Creates the p layer.
    /// </summary>
    /// <returns>Player.</returns>
    private IPlayObject CreatePLayer()
    {
        if (player != null) return player;
        player = new Player(null);
        return player;
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear()
    {
        fields.Clear();
        foreach (var f in _fields)
        {
            f.Item = null;
        }
        ;
        _fields.Clear();
        Stones.Clear();
        player = null;
        fieldSize = Size.Empty;
    }
}
