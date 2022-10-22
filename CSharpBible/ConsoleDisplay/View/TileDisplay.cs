// ***********************************************************************
// Assembly         : ConsoleDisplay
// Author           : Mir
// Created          : 08-19-2022
//
// Last Modified By : Mir
// Last Modified On : 08-27-2022
// ***********************************************************************
// <copyright file="TileDisplay.cs" company="ConsoleDisplay">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDisplay.View
{
    /// <summary>
    /// Class TileDef.
    /// </summary>
    /// <typeparam name="Enum">The type of the enum.</typeparam>
    public abstract class TileDef<Enum>
    {
        /// <summary>
        /// Gets the tile definition.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>The visual defintion of the tile</returns>
        public abstract (string[] lines, (ConsoleColor fgr,ConsoleColor bgr)[] colors) GetTileDef(Enum tile);

        /// <summary>
        /// Converts a Byte to 2  Console-colors (fore- and background).
        /// </summary>
        /// <param name="colDef">The (Console-)color definition.</param>
        /// <returns>The Tuple of Forground- and background-color</returns>
        protected static (ConsoleColor fgr, ConsoleColor bgr) ByteTo2ConsColor(byte colDef) => ((ConsoleColor)(colDef & 0xf), (ConsoleColor)(colDef >> 4));

        /// <summary>
        /// Gets the array element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="tile">The tile.</param>
        /// <returns>T.</returns>
        protected static T GetArrayElement<T>(T[] array, Enum tile) => Tile2Int(tile) < array.Length ? array[Tile2Int(tile)] : array[array.Length - 1];

        /// <summary>
        /// Tile2s the int.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>System.Int32.</returns>
        protected static int Tile2Int(Enum tile) { return ((int)((object)tile ?? 0)); }
    }

    /// <summary>
    /// Class TileDisplay.
    /// </summary>
    /// <typeparam name="Enum">The type of the enum.</typeparam>
    public class TileDisplay<Enum>
    {
        #region Properties
        #region static Properties
        /// <summary>
        /// The default tile
        /// </summary>
        public static Enum defaultTile;
        /// <summary>
        /// The tile definition
        /// </summary>
        public static TileDef<Enum> tileDef;
        #endregion

        /// <summary>
        /// Gets or sets the <see cref="Enum"/> with the specified index.
        /// </summary>
        /// <param name="Idx">The index.</param>
        /// <returns>Enum.</returns>
        public Enum this[Point Idx] { get => GetTile(Idx); set => SetTile(Idx,value); }
        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public Point Position => _rect.Location;
        /// <summary>
        /// Gets the size of the disp.
        /// </summary>
        /// <value>The size of the disp.</value>
        public Size DispSize => _size;
        /// <summary>
        /// Gets the size of the tile.
        /// </summary>
        /// <value>The size of the tile.</value>
        public Size TileSize => _tileSize;
        /// <summary>
        /// My console
        /// </summary>
        public static MyConsoleBase myConsole= new MyConsole();
        /// <summary>
        /// Gets or sets the tile definition.
        /// </summary>
        /// <value>The tile definition.</value>
        public TileDef<Enum> TileDef { get => _tileDef ?? tileDef; set => _tileDef = value; }

        #region Private Properties and Fields

        /// <summary>
        /// The tiles
        /// </summary>
        private Dictionary<Point, Enum> _tiles = new Dictionary<Point, Enum>();
        /// <summary>
        /// The rect
        /// </summary>
        private Rectangle _rect = new Rectangle();
        /// <summary>
        /// The size
        /// </summary>
        private Size _size;
        /// <summary>
        /// The tile size
        /// </summary>
        private Size _tileSize;
        /// <summary>
        /// The changed
        /// </summary>
        private bool _changed;
        /// <summary>
        /// The tile definition
        /// </summary>
        public TileDef<Enum> _tileDef;
        #endregion
        #endregion

        #region Methods
        #region Static Methods
        /// <summary>
        /// Initializes static members of the <see cref="TileDisplay{Enum}"/> class.
        /// </summary>
        static TileDisplay()  {
            defaultTile = ((Enum[])typeof(Enum).GetEnumValues())[0];
        }

        /// <summary>
        /// Writes the tile.
        /// </summary>
        /// <param name="Offset">The offset.</param>
        /// <param name="p">The p.</param>
        /// <param name="tile">The tile.</param>
        public static void WriteTile(Point Offset,PointF p, Enum tile)=>WriteTile(Offset,p,tile,Size.Empty);

        /// <summary>
        /// Writes the tile.
        /// </summary>
        /// <param name="Offset">The offset.</param>
        /// <param name="p">The p.</param>
        /// <param name="tile">The tile.</param>
        /// <param name="ts">The ts.</param>
        public static void WriteTile(Point Offset,PointF p, Enum tile, Size ts)
        {
            var def = tileDef?.GetTileDef(tile) ?? default;
            Size s = (ts == Size.Empty)?new Size(def.lines[0].Length, def.lines.Length):ts;
            Point pc = new Point();
            for (pc.Y = 0; pc.Y < def.lines.Length; pc.Y++)
                for (pc.X = 0; pc.X < def.lines[pc.Y].Length; pc.X++)
                    WriteTileChunk(Offset, p, def, s, pc);
        }

        /// <summary>
        /// Writes the tile chunk.
        /// </summary>
        /// <param name="Offset">The offset.</param>
        /// <param name="p">The p.</param>
        /// <param name="def">The definition.</param>
        /// <param name="s">The s.</param>
        /// <param name="pc">The pc.</param>
        private static void WriteTileChunk(Point Offset, PointF p, (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) def, Size s, Point pc)
        {
            myConsole.ForegroundColor = def.colors[pc.X + pc.Y * s.Width].fgr;
            myConsole.BackgroundColor = def.colors[pc.X + pc.Y * s.Width].bgr;
            myConsole.SetCursorPosition(Offset.X + (int)(p.X * s.Width) + pc.X, Offset.Y + (int)(p.Y * s.Height) + pc.Y);
            myConsole.Write(def.lines[pc.Y][pc.X]);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TileDisplay{Enum}"/> class.
        /// </summary>
        public TileDisplay() : this(Point.Empty,Size.Empty){}

        /// <summary>
        /// Initializes a new instance of the <see cref="TileDisplay{Enum}"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="size">The size.</param>
        public TileDisplay(Point position, Size size) : this(position, size,new Size(4,2)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TileDisplay{Enum}"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="size">The size.</param>
        /// <param name="tileSize">Size of the tile.</param>
        public TileDisplay(Point position,Size size, Size tileSize)
        {
            _rect.Location = position;
            _tileSize = tileSize != Size.Empty ? tileSize : new Size(4, 2);
            if (size == Size.Empty)
                _size = new Size(myConsole.WindowWidth / _tileSize.Width,myConsole.WindowHeight/_tileSize.Height);
            else
                _size = size;
            _rect.Size = new Size(_size.Width * _tileSize.Width,_size.Height * _tileSize.Height);
        }

        /// <summary>
        /// Writes the tile.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="tile">The tile.</param>
        public void WriteTile(PointF p, Enum tile)
        {
            var def = TileDef?.GetTileDef(tile) ?? default;
            Size s = TileSize;
            Point pc = new Point((int)p.X*s.Width,(int)p.Y*s.Height);
            var _rect2 = new Rectangle(Point.Empty, _rect.Size);
            _rect2.Inflate(s);
            if (_rect2.Contains(pc))
            {
                _rect2.Size = _rect.Size;
                _rect2.Location = Point.Empty;
                _rect2.Offset(new Point((int)(-p.X * s.Width),(int)(-p.Y * s.Height)));
                for (pc.Y = 0; pc.Y < def.lines.Length; pc.Y++)
                    for (pc.X = 0; pc.X < def.lines[pc.Y].Length; pc.X++)
                        if (_rect2.Contains(pc))
                                WriteTileChunk(Position, p, def, s, pc);
            }
        }

        #region private Methods

        /// <summary>
        /// Gets the tile.
        /// </summary>
        /// <param name="Idx">The index.</param>
        /// <returns>Enum.</returns>
        private Enum GetTile(Point Idx) {
            if (Idx.X < 0 || Idx.X >= _size.Width || Idx.Y < 0 || Idx.Y >= _size.Height) return defaultTile;
            if (_tiles.ContainsKey(Idx)) return _tiles[Idx];
            return defaultTile;
        }

        /// <summary>
        /// Sets the tile.
        /// </summary>
        /// <param name="Idx">The index.</param>
        /// <param name="value">The value.</param>
        private void SetTile(Point Idx, Enum value)
        {
            if (Idx.X < 0 || Idx.X >= _size.Width || Idx.Y < 0 || Idx.Y >= _size.Height) return;
            if (_tiles.ContainsKey(Idx) && (_tiles[Idx] is Enum e) && e.Equals(value) ) return;
            _tiles[Idx] = value;
            _changed = true;
        }
        #endregion
        #endregion
    }
}
