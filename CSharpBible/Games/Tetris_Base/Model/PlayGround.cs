using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Tetris_Base.Model
{
    /// <summary>
    /// Class PlayGround.
    /// </summary>
    public class PlayGround
    {
        /// <summary>
        /// Gets the size of the field.
        /// </summary>
        /// <value>The size of the field.</value>
        public Size FieldSize { get; private set; } = new Size(12, 21);
        private ConsoleColor[] _buffer;
        private bool _dirty;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PlayGround"/> is dirty.
        /// </summary>
        /// <value><c>true</c> if dirty; otherwise, <c>false</c>.</value>
        public bool Dirty { get => _dirty; set => _dirty = value; }
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayGround"/> class.
        /// </summary>
        public PlayGround()
        {
            _buffer = new ConsoleColor[FieldSize.Width * FieldSize.Height];

        }

        /// <summary>
        /// Removes the line.
        /// </summary>
        /// <param name="Y">The y.</param>
        public void RemoveLine(int Y)
        {
            for (int i = Y * FieldSize.Width - 1; i > 0; i--)
                _buffer[i + FieldSize.Width] = _buffer[i];
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            _buffer = new ConsoleColor[FieldSize.Width * FieldSize.Height];
        }

        /// <summary>
        /// Gets or sets the <see cref="ConsoleColor"/> with the specified PNT.
        /// </summary>
        /// <param name="pnt">The PNT.</param>
        /// <returns>ConsoleColor.</returns>
        public ConsoleColor this[Point pnt] { 
            get =>(pnt.X >= 0 && pnt.Y >= 0 && pnt.X < FieldSize.Width && pnt.Y < FieldSize.Height) ? _buffer[pnt.Y*FieldSize.Width+pnt.X]:ConsoleColor.Black ;
            set { if (pnt.X >= 0 && pnt.Y >= 0 && pnt.X < FieldSize.Width && pnt.Y < FieldSize.Height)
                    if (_buffer[pnt.Y * FieldSize.Width + pnt.X] == value) return;
                    else
                    {
                        _buffer[pnt.Y * FieldSize.Width + pnt.X] = value;
                        _dirty = true;
                    }
            } 
        }
    }
}