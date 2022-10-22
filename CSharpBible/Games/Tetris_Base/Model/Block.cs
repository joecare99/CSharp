using System;
using System.Collections.Generic;
using System.Drawing;
using Tetris_Base.Helper;

namespace Tetris_Base.Model
{
    /// <summary>
    /// Class Block.
    /// </summary>
    public class Block
    {
        #region Constructors
        /// <summary>Initializes a new instance of the <see cref="Block" /> class.</summary>
        /// <param name="type">The type of the block.</param>
        /// <param name="angle">The angle of the block.</param>
        /// <seealso cref="BlockType" />
        /// <seealso cref="BlockAngle" />
        public Block(BlockType type, BlockAngle angle = BlockAngle.Degr0) : this(Point.Empty, type, angle){}

        /// <summary>Initializes a new instance of the <see cref="Block" /> class.</summary>
        /// <param name="position">The starting position of the block.</param>
        /// <param name="type">The type of the block.</param>
        /// <param name="angle">The angle of the block.</param>
        /// <seealso cref="Point" />
        /// <seealso cref="BlockType" />
        /// <seealso cref="BlockAngle" />
        public Block(Point position, BlockType type, BlockAngle angle = BlockAngle.Degr0)
        {
            Position = position;
            ActBlockType = type;
            ActBlockAngle = angle;
        }
        #endregion

        #region Properties

        #region Actions
        /// <summary>The predicate to test pixels</summary>
        /// <seealso cref="Point" />
        /// <seealso cref="CollisionTest" />
        public Predicate<Point>? prTestPixel = null;
        /// <summary>The action to paint a pixel</summary>
        /// <seealso cref="Point" />
        /// <seealso cref="ConsoleColor" />
        /// <seealso cref="Show" />
        /// <seealso cref="Hide" />
        /// <seealso cref="DoMove" />
        public Action<Point,ConsoleColor>? acPaint = null;
        #endregion

        /// <summary>Gets or sets the position of the block.</summary>
        /// <value>The (center-) position.</value>
        /// <seealso cref="Point" />
        public Point Position { get => _position;
			set => PropertyClass.SetPropertyP(ref _position, value, (o, n) => {
				if (Visible) DoMove(new Point(n.X - o.X, n.Y - o.Y), _blockAngle);
			});
        }

        /// <summary>Gets or sets the actual type of the block.</summary>
        /// <value>The actual type of the block.</value>
        /// <seealso cref="BlockType" />
        public BlockType ActBlockType
        {
            get => _blockType;
			set => PropertyClass.SetPropertyP(ref _blockType, value);
        }

        /// <summary>Gets or sets the actual block angle.</summary>
        /// <value>The actual block angle.</value>
        /// <seealso cref="BlockAngle" />
        public BlockAngle ActBlockAngle
        {
            get => _blockAngle;
			set => PropertyClass.SetPropertyP(ref _blockAngle, value, (o, n) => {
				if (Visible) DoMove(Point.Empty, n);
			});
        }

        /// <summary>Gets a value indicating whether this <see cref="Block" /> is visible.</summary>
        /// <value>
        ///   <c>true</c> if visible; otherwise, <c>false</c>.</value>
        /// <seealso cref="Show" />
        /// <seealso cref="Hide" />
        public bool Visible { get; private set; }

        #endregion

        #region Methods
        /// <summary>Shows this instance.</summary>
        /// <seealso cref="Visible" />
        public void Show()
        {

            ForeachPixelOfBlock(ActBlockAngle,Position,DoPixel);
            Visible = true;  
            void DoPixel(Point p,ConsoleColor c)=> acPaint?.Invoke(p, c);
        }

        /// <summary>Hides this instance.</summary>
        /// <seealso cref="Visible" />
        public void Hide()
        {

            ForeachPixelOfBlock(ActBlockAngle, Position, DoPixel);
            Visible = false;
            void DoPixel(Point p, ConsoleColor c) => acPaint?.Invoke(p, ConsoleColor.Black);
        }

        /// <summary>Moves the block by specified offset, and to specified angle.</summary>
        /// <param name="Offset">The offset.</param>
        /// <param name="newAngle">The new angle.</param>
        /// <seealso cref="BlockAngle" />
        /// <seealso cref="Point" />
        public void Move(Point Offset, BlockAngle? newAngle=null)
        {
            if (Offset != Point.Empty)
            {
                Position = new Point(Position.X+Offset.X,Position.Y+Offset.Y);  
            }
            ActBlockAngle = newAngle ?? ActBlockAngle;
        }

        /// <summary>Tests the block for collision with specified offset and angle.</summary>
        /// <param name="Offset">The offset.</param>
        /// <param name="newAngle">The new angle.</param>
        /// <returns>
        ///   <c>true</c> if collision was detected; otherwise <c>false</c></returns>
        /// <seealso cref="Point" />
        /// <seealso cref="BlockAngle" />
        /// <seealso cref="prTestPixel" />
        public bool CollisionTest(Point Offset,BlockAngle newAngle) { 
            List<Point> points = new List<Point>();
            ForeachPixelOfBlock( newAngle, Offset, (p, c) => points.Add(p));
            ForeachPixelOfBlock(ActBlockAngle, Point.Empty, (p, c) => points.Remove(p));
            foreach (var p in points)
            {
                p.Offset(Position);
                if (prTestPixel?.Invoke(p) ?? true) return true;
            }
            return false;
        }
        #endregion

        #region private Stuff
        /// <summary>Does the action for every pixel of the Block with the specified offset and angle.</summary>
        /// <param name="angle">The angle.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="action">The action to perform.</param>
        /// <seealso cref="BlockAngle" />
        /// <seealso cref="Point" />
        private void ForeachPixelOfBlock(BlockAngle angle, Point offset, Action<Point,ConsoleColor> action)
        {
            var Idx = ((int)ActBlockType) * 4 + (int)angle;
            var def = Defines.BlockDefines[Idx];
            var p = new Point(0, 0);
            p.Offset(offset);
            action.Invoke(p, def.bColor);
            foreach (var b in def.bKoor)
            {
                (p.X, p.Y) = (b % 16 - 4, b / 16 - 4);
                p.Offset(offset);
                action.Invoke(p, def.bColor);
            }
        }

        /// <summary>Moves the block by specified offset, and to specified angle.</summary>
        /// <param name="Offset">The offset.</param>
        /// <param name="newAngle">The new angle.</param>
        /// <seealso cref="BlockAngle" />
        /// <seealso cref="Point" />
        private void DoMove(Point Offset, BlockAngle newAngle )
        {
            List<(Point, ConsoleColor)> points = new List<(Point, ConsoleColor)>();
            ForeachPixelOfBlock(newAngle, Offset, (p, c) => points.Add((p, c)));
            ForeachPixelOfBlock(ActBlockAngle, Point.Empty, (p, c) =>
            {
                if (points.Contains((p, c))) points.Remove((p, c)); else points.Add((p, ConsoleColor.Black));
            });
            foreach (var (p, c) in points)
            {
                p.Offset(Position);
                acPaint?.Invoke(p, c);
            }
        }

        /// <summary>The block type</summary>
        /// <seealso cref="BlockType" />
        /// <seealso cref="ActBlockType" />
        private BlockType _blockType;
        /// <summary>The block angle</summary>
        /// <seealso cref="BlockAngle" />
        /// <seealso cref="ActBlockAngle" />
        private BlockAngle _blockAngle;
        /// <summary>The position
        /// of the block</summary>
        /// <seealso cref="Point" />
        /// <seealso cref="Position" />
        private Point _position;
        #endregion
    }
}
