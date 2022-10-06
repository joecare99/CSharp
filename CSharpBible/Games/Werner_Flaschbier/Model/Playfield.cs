// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 08-02-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Playfield.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Drawing;

namespace Werner_Flaschbier_Base.Model
{
    /// <summary>
    /// Class Playfield.
    /// </summary>
    public class Playfield
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Nullable{Field}" /> with the specified p.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>System.Nullable&lt;Field&gt;.</returns>
        public Field? this[Point p]
        {
            get => fields.TryGetValue((p.X, p.Y), out var v) ? v : null; set
            {
                if (value != null) fields[(p.X, p.Y)] = value;
            }
        }

        /// <summary>
        /// The fields
        /// </summary>
        Dictionary<(int,int),Field> fields = new();
        /// <summary>
        /// The size
        /// </summary>
        public Size size;
        /// <summary>
        /// The stones
        /// </summary>
        public List<Stone> Stones=new() { };
        /// <summary>
        /// The enemies
        /// </summary>
        public List<Enemy> Enemies = new() { };
        /// <summary>
        /// The fields
        /// </summary>
        private List<Field> _fields = new() { };
        /// <summary>
        /// The player
        /// </summary>
        public Player? player;

        /// <summary>
        /// Gets a value indicating whether [game solved].
        /// </summary>
        /// <value><c>true</c> if [game solved]; otherwise, <c>false</c>.</value>
        public bool GameSolved { get => player?.field is Destination; }

        /// <summary>
        /// Setups the specified sf definition.
        /// </summary>
        /// <param name="SFDef">The sf definition.</param>
        public void Setup(string[] SFDef)
        {
            Clear();
            size.Height = SFDef.Length;
            var lnr = 0;
            foreach(var line in SFDef)
            {
                size.Width = Math.Max(size.Width, line.Length);
                for (int x = 0;x  < line.Length; x++)
                {
                    CreateField(FieldDefs.SDef[line[x]],x,lnr);   
                }
                lnr++;   
            }
        }

        /// <summary>
        /// Setups the specified sf definition.
        /// </summary>
        /// <param name="SFDef">The sf definition.</param>
        public void Setup(FieldDef[]? SFDef)
        {
            Clear();
            if (SFDef == null) return;
            size.Height = SFDef.Length/20;
            for (int y = 0; y < size.Height; y++)
            {
                size.Width = 20;
                for (int x = 0; x < size.Width; x++)
                {
                    CreateField(SFDef[x+y*size.Width], x, y);
                }
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
                case FieldDef.Wall:
                    result = new Wall(new Point(x, y),this) ;
                    break;
                case FieldDef.Empty:
                case FieldDef.Player:
                case FieldDef.Stone:
                case FieldDef.Enemy:
                case FieldDef.Dirt:
                    result = new Space(new Point(x, y), this) ;
                    break;
                case FieldDef.Destination:
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
                        result.Item = CreatePLayer();
                        break;

                    case FieldDef.Stone:
                        result.Item = CreateStone();
                        result.Item.OldPosition = result.Item.Position;
                        break;
                    case FieldDef.Enemy:
                        result.Item = CreateEnemy();
                        result.Item.OldPosition = result.Item.Position;
                        break;
                    case FieldDef.Dirt:
                        result.Item = new Dirt(null); ;
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// Creates the stone.
        /// </summary>
        /// <returns>Stone.</returns>
        private Stone CreateStone()
        {
            Stone result = new(null);
            Stones.Add(result);
            return result;
        }

        /// <summary>
        /// Creates the enemy.
        /// </summary>
        /// <returns>Enemy.</returns>
        private Enemy CreateEnemy()
        {
            Enemy result = new Enemy(null);
            Enemies.Add(result);
            return result;
        }

        /// <summary>
        /// Creates the p layer.
        /// </summary>
        /// <returns>Player.</returns>
        private Player CreatePLayer()
        {
           // if (player != null) return player;
            player = new Player(null);
            return player;
        }

        /// <summary>
        /// Gets the active objects.
        /// </summary>
        /// <value>The active objects.</value>
        public IEnumerable<PlayObject> ActiveObjects { get
            {
                foreach(Space s in Spaces )  if (s.Item != null && s.Item is not Dirt)
                        yield return s.Item; 
                yield break;
            } 
        }

        /// <summary>
        /// Gets the spaces.
        /// </summary>
        /// <value>The spaces.</value>
        public IEnumerable<Space> Spaces
        {
            get
            {
                for (var i = _fields.Count - 1; i >= 0; i--) if (_fields[i] is Space s)
                        yield return s;
                yield break;
            }
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
            };
            _fields.Clear();
            Stones.Clear();
            Enemies.Clear();
            player = null;
            (size.Width, size.Height) = (0, 0);
        }
    }
}
