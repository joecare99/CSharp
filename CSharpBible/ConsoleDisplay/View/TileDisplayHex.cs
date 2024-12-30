using System;
using System.Collections.Generic;
using System.Drawing;

namespace ConsoleDisplay.View
{
    public static class HexMath {
		public static PointF HexPointF(this (float X, float Y) f, bool v) =>
			v ?
			new PointF(f.X, f.Y + ZigZag(f.X ) * 0.5f) :
			new PointF(f.X + ZigZag(f.Y) * 0.5f, f.Y);

		public static Point HexKPoint(this (float X, float Y) f, bool v) =>
			v ?
			new Point((int)Math.Round(f.X), (int)Math.Round(f.Y + ZigZag(f.X) * 0.5f)) :
			new Point((int)Math.Round(f.X + ZigZag(f.Y) * 0.5f), (int)Math.Round(f.Y));

		public static float ZigZag(this float x) =>
			(float)Math.Abs(x - Math.Floor((x + 1.0) * 0.5) * 2.0);
	}
	/// <summary>
	/// Class TileDisplay Hex
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TileDisplayHex<T> 
	{
		#region Properties
		#region static Properties
		/// <summary>
		/// The default tile
		/// </summary>
		public static T? defaultTile { get; set; } = default!;
		/// <summary>
		/// The default tile definition
		/// </summary>
		public static TileDefBase? tileDef { get; set; }
		#endregion

		/// <summary>The Display is vertical
		/// interleaved</summary>
		/// <example>
		///   <para>Horizontal:</para>
		///   <para>
		///     <pre>1 2 3 4<br /> 5 6 7<br />8 9 A B</pre>
		///   <para>Vertical:</para>
		///   <para><pre>  2   4<br />1   3   5<br />  7   9<br />6   8   A</pre>
		///   </para>
		/// </example>
		public bool Vertical { get => _vertical; set => _vertical = value; }

		/// <summary>
		/// Gets or sets the <see cref="T"/> with the specified index.
		/// </summary>
		/// <param name="Idx">The index.</param>
		/// <returns>T.</returns>
		public T? this[Point Idx] { get => GetTile(Idx); set => SetTile(Idx, value); }
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
		public static MyConsoleBase myConsole { get; set; } = new MyConsole();
		/// <summary>
		/// Gets or sets the tile definition.
		/// it returns the default-tileDef when the local tileDef isn't set.
		/// </summary>
		/// <value>The tile definition.</value>
		public TileDefBase? TileDef { get => _tileDef ?? tileDef; set => _tileDef = value; }

		public Point DispOffset { get; set; } = Point.Empty;
		public Func<Point, T>? FncGetTile;
		public Func<Point, Point>? FncOldPos;
		#region Private Properties and Fields

		/// <summary>The Display is vertical
		/// interleaved</summary>
		/// <example>
		///   <para>Horizontal:</para>
		///   <para>
		///     <pre>1 2 3 4<br /> 5 6 7<br />8 9 A B</pre>
		///   <para>Vertical:</para>
		///   <para><pre>  2   4<br />1   3   5<br />  7   9<br />6   8   A</pre>
		///   </para>
		/// </example>
		private bool _vertical = false;

		/// <summary>
		/// The tiles
		/// </summary>
		private readonly Dictionary<Point, T> _tiles = new();
		/// <summary>
		/// The rect
		/// </summary>
		private readonly Rectangle _rect = new();
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
		private bool _changed=false;
		/// <summary>
		/// The (local) tile-definition
		/// </summary>
		private TileDefBase? _tileDef;
		#endregion
		#endregion

		#region Methods
		#region Static Methods
		/// <summary>
		/// Initializes static members of the <see cref="TileDisplay{T}"/> class.
		/// </summary>
		static TileDisplayHex() {
			defaultTile = default;
		}

		/// <summary>
		/// Writes the tile.
		/// </summary>
		/// <param name="Offset">The offset.</param>
		/// <param name="p">The p.</param>
		/// <param name="tile">The tile.</param>
		public static void WriteTile(Point Offset, PointF p, T tile) => WriteTile(Offset, p, tile, Size.Empty);

		/// <summary>
		/// Writes the tile.
		/// </summary>
		/// <param name="Offset">The offset.</param>
		/// <param name="p">The p.</param>
		/// <param name="tile">The tile.</param>
		/// <param name="ts">The ts.</param>
		public static void WriteTile(Point Offset, PointF p, T tile, Size ts) {
			var def = tileDef?.GetTileDef(tile as Enum) ?? default;
			Size s = (ts == Size.Empty) ? new Size(def.lines[0].Length, def.lines.Length) : ts;
			Point pc = new();
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
		private static void WriteTileChunk(Point Offset, PointF p, (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) def, Size s, Point pc) {
			myConsole.ForegroundColor = def.colors[pc.X + pc.Y * s.Width].fgr;
			myConsole.BackgroundColor = def.colors[pc.X + pc.Y * s.Width].bgr;
			myConsole.SetCursorPosition(Offset.X + (int)(p.X * s.Width) + pc.X, Offset.Y + (int)(p.Y * s.Height) + pc.Y);
			myConsole.Write(def.lines[pc.Y][pc.X]);
		}
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="TileDisplay{T}"/> class.
		/// </summary>
		public TileDisplayHex() : this(Point.Empty, Size.Empty,false) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="TileDisplay{T}"/> class.
		/// </summary>
		/// <param name="position">The position on the Screen.</param>
		/// <param name="size">The size of the display.</param>
		public TileDisplayHex(Point position, Size size, bool vertical=false) : this(position, size, tileDef?.TileSize ?? new Size(4, 2),vertical) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="TileDisplay{T}"/> class.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="size">The size.</param>
		/// <param name="aTileDef">The tile-definition.</param>
		public TileDisplayHex(Point position, Size size, TileDefBase aTileDef, bool vertical = false) : this(position, size, aTileDef.TileSize,vertical) {
			TileDef = aTileDef;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TileDisplay{T}"/> class.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="size">The size.</param>
		/// <param name="tileSize">Size of the tile.</param>
		public TileDisplayHex(Point position, Size size, Size tileSize, bool vertical = false) {
			_vertical= vertical;
			_rect.Location = position;
			_tileSize = tileSize != Size.Empty ? tileSize : new Size(4, 2);
			if (size == Size.Empty)
				_size = new Size(myConsole.WindowWidth / _tileSize.Width, myConsole.WindowHeight / _tileSize.Height);
			else
				_size = size;
			_rect.Size = new Size(_size.Width * _tileSize.Width, _size.Height * _tileSize.Height);
		}

		/// <summary>
		/// Writes the tile.
		/// </summary>
		/// <param name="p">The p.</param>
		/// <param name="tile">The tile.</param>
		public void WriteTile(PointF p, T tile) {
			var def = TileDef?.GetTileDef(tile as Enum) ?? default;
			Size s = TileSize;
			Point pc = new((int)p.X * s.Width, (int)p.Y * s.Height);
			var _rect2 = new Rectangle(Point.Empty, _rect.Size);
			_rect2.Inflate(s);
			if (_rect2.Contains(pc)) {
				_rect2.Size = _rect.Size;
				_rect2.Location = Point.Empty;
				_rect2.Offset(new Point((int)(-p.X * s.Width), (int)(-p.Y * s.Height)));
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
		/// <returns>T.</returns>
		private T? GetTile(Point Idx) {
			if (Idx.X < 0 || Idx.X >= _size.Width || Idx.Y < 0 || Idx.Y >= _size.Height) return defaultTile;
			if (_tiles.ContainsKey(Idx)) return _tiles[Idx];
			return defaultTile;
		}

		/// <summary>
		/// Sets the tile.
		/// </summary>
		/// <param name="Idx">The index.</param>
		/// <param name="value">The value.</param>
		private void SetTile(Point Idx, T? value) {
			if (Idx.X < 0 || Idx.X >= _size.Width || Idx.Y < 0 || Idx.Y >= _size.Height) return;
			if (_tiles.TryGetValue(Idx,out T? e ) && e!.Equals(value)) return;
			if (value != null)
				_tiles[Idx] = value;
			else
				_tiles.Remove(Idx);	
			_changed = true;
		}

		public void Update(bool e) {
			var diffFields = new List<(PointF, T, PointF)>();
			var p = new Point();
			Point p3 = new();
			if (FncGetTile == null) return;

			for (p.Y = 0; p.Y < DispSize.Height; p.Y++)
				for (p.X = 0; p.X < DispSize.Width; p.X++) {
					p3.X = p.X + DispOffset.X;
					p3.Y = p.Y + DispOffset.Y;
					object td = FncGetTile(p3)!;
					object? ot = GetTile(p);
					var po = FncOldPos?.Invoke(p3);
					if (!td.Equals(ot)
						|| ((po ?? p3) != p3)) {
						var pp = Point.Subtract(po ?? p3, (Size)DispOffset);				
						diffFields.Add(( HexMath.HexPointF((p.X, p.Y), _vertical), (T)td, HexMath.HexPointF((pp.X, pp.Y), _vertical)));
						if (!e)
							_tiles[p] = (T)td;
					}

				}
			if (e) {
				foreach (var f in diffFields)
					if (f.Item1 == f.Item3) {
						WriteTile(f.Item1, f.Item2);
					}
				foreach (var f in diffFields)
					if (f.Item1 != f.Item3 && Math.Abs(f.Item1.X - f.Item3.X) < 2) {
						
						var	zPos = new PointF((f.Item1.X + f.Item3.X) * 0.5f,
										(f.Item1.Y + f.Item3.Y) * 0.5f);

						WriteTile(zPos, f.Item2);
					}
			}
			else
				foreach (var f in diffFields) {
					if ((f.Item1.X != f.Item3.X) || (f.Item1.Y != f.Item3.Y)) {
						var zPos = new PointF((f.Item1.X + f.Item3.X) * 0.5f,
										(f.Item1.Y + f.Item3.Y) * 0.5f);
						
						var p1 = HexMath.HexKPoint((zPos.X+0.4f, zPos.Y+0.4f),_vertical);
						WriteTile(HexMath.HexPointF((p1.X,p1.Y), _vertical), FncGetTile(Point.Add(p1, (Size)DispOffset)));
						p1 = HexMath.HexKPoint((zPos.X - 0.4f, zPos.Y - 0.4f), _vertical);
						WriteTile(HexMath.HexPointF((p1.X, p1.Y), _vertical), FncGetTile(Point.Add(p1, (Size)DispOffset)));
						p1 = HexMath.HexKPoint((zPos.X - 0.4f, zPos.Y + 0.4f), _vertical);
						WriteTile(HexMath.HexPointF((p1.X, p1.Y), _vertical), FncGetTile(Point.Add(p1, (Size)DispOffset)));
						p1 = HexMath.HexKPoint((zPos.X + 0.4f, zPos.Y - 0.4f), _vertical);
						WriteTile(HexMath.HexPointF((p1.X, p1.Y), _vertical), FncGetTile(Point.Add(p1, (Size)DispOffset)));
					}
					else
					WriteTile(HexMath.HexPointF((f.Item1.X, f.Item1.Y),_vertical) , f.Item2);
				}
            _changed = false;
        }

        public void FullRedraw() {
			if (FncGetTile == null) return;
			// Draw playfield
			Point p = new();
			Point p2 = new();
            for (p.Y = 0; p.Y < DispSize.Height; p.Y++)
				for (p.X = 0; p.X < DispSize.Width; p.X++) {
					p2.X = p.X + DispOffset.X;
					p2.Y = p.Y + DispOffset.Y;
                    PointF p3 = HexMath.HexPointF((p.X, p.Y), _vertical);
                    WriteTile(p3, _tiles[p] = FncGetTile(p2));
				}
			_changed = false;
		}
		#endregion
		#endregion
	}

	public class TileDisplayHex : TileDisplayHex<Enum> {
		/// <summary>
		/// Initializes a new instance of the <see cref="TileDisplay{T}"/> class.
		/// </summary>
		public TileDisplayHex() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="TileDisplay{T}"/> class.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="size">The size.</param>
		public TileDisplayHex(Point position, Size size, bool vertical = false) : base(position, size,vertical) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="TileDisplay{T}"/> class.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="size">The size.</param>
		/// <param name="aTileDef">The tile-definition.</param>
		public TileDisplayHex(Point position, Size size, TileDefBase aTileDef, bool vertical = false) : base(position, size, aTileDef, vertical) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="TileDisplay{T}"/> class.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="size">The size.</param>
		/// <param name="tileSize">Size of the tile.</param>
		public TileDisplayHex(Point position, Size size, Size tileSize, bool vertical = false) : base(position, size, tileSize, vertical) { }
	}
}

