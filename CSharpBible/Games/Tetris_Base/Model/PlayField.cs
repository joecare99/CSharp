using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Tetris_Base.Model
{
    /// <summary>
    /// Class PlayField.
    /// </summary>
    public class PlayField
    {
        private Block? _actualBlock;
        private Block? _nextBlock;

        /// <summary>
        /// Gets or sets the next block.
        /// </summary>
        /// <value>The next block.</value>
        public Block? NextBlock
        {
            get => _nextBlock;
            set
            {
                if (_nextBlock == value) return;
                _nextBlock?.Hide();
                _nextBlock = value;
                if (_nextBlock != null)
                {
                    _nextBlock.acPaint = this.PrevPaint;
                    _nextBlock.prTestPixel = ((o) => false);
                    _nextBlock.Show();
                }
            }
        }

        /// <summary>
        /// Gets or sets the actual block.
        /// </summary>
        /// <value>The actual block.</value>
        public Block? ActualBlock
        {
            get => _actualBlock;
            set
            {
                if (_actualBlock == value) return;
                if (value == NextBlock) NextBlock = null;
                // Alter Block wird nicht versteckt, bleibt stehen
                _actualBlock = value;
                if (_actualBlock != null)
                {
                _actualBlock.acPaint = this.PaintPxl;
                _actualBlock.prTestPixel = this.TestPxl;
                } 
            }
        }

        private bool TestPxl(Point pnt)
        {
            if (pnt.X >= 0 && pnt.Y >= 0 && pnt.X < FieldSize.Width && pnt.Y < FieldSize.Height)
                return playGround[pnt]!=ConsoleColor.Black;
                else
                    return (pnt.Y > 0);
        }

        private void PaintPxl(Point arg1, ConsoleColor arg2)
        {
            if (arg1.X >= 0 && arg1.Y >= 0 && arg1.X < FieldSize.Width && arg1.Y < FieldSize.Height)
                playGround[arg1] = arg2;

        }
        Size FieldSize => playGround.FieldSize;

        /// <summary>
        /// Gets or sets the play ground.
        /// </summary>
        /// <value>The play ground.</value>
        public PlayGround playGround
        {
            get;
            set;
        } = new PlayGround();

        /// <summary>
        /// Gets or sets the previous paint.
        /// </summary>
        /// <value>The previous paint.</value>
        public Action<Point, ConsoleColor>? PrevPaint { get; set; } = null;

        /// <summary>
        /// Tests the remove line.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int TestRemoveLine()
        {
            int result = 0;
            Point pnt=new Point();
            for (int y = FieldSize.Height-1; y >= 0; y--)
            {
                pnt.Y = y;
                bool removeline = true;
                for (int x = 0; x < FieldSize.Width && removeline; x++)
                {
                    pnt.X = x;
                    removeline = TestPxl(pnt); 
                }
                if (removeline)
                {
                    result++;
                    playGround.RemoveLine(y);
                    y++;
                }

            }
            return result;
                
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            ActualBlock = null;
            NextBlock = null;
            playGround.Clear();
        }

        
    }
}