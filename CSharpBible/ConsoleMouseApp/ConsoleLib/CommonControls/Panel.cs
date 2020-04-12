using System;
using System.Drawing;

namespace ConsoleLib.CommonControls
{
    public class Panel : Control
    {
        public char[] Boarder;
        public ConsoleColor BoarderColor;
        
        public override void Draw()
        {
            ConsoleFramework.Canvas.FillRect(_dimension,ForeColor, BackColor, ConsoleFramework.chars[3]);
            if (Boarder != null && Boarder.Length > 5)
                ConsoleFramework.Canvas.DrawRect(_dimension, BoarderColor, BackColor, Boarder);
            foreach( Control c in children) if (c.visible)
                {
                if (c.shaddow)
                {
                    var sdim = c.dimension;
                    sdim.Offset(1, 1);
                    ConsoleFramework.Canvas.FillRect(sdim, ConsoleColor.DarkGray, ConsoleColor.Black, ConsoleFramework.chars[4]);
                }
                c.Draw();

            }
        }

        public override void ReDraw(Rectangle dimension)
        {
            if (dimension.Width == 0 || dimension.Height == 0) return;
            Rectangle innerRect = _dimension;
            innerRect.Inflate(-1, -1);
            dimension.Intersect(innerRect);
            ConsoleFramework.Canvas.FillRect(dimension,ForeColor, BackColor, ConsoleFramework.chars[3]);
            // ToDo: Boarder
            //            if (Boarder != null && Boarder.Length > 5)
            //                ConsoleFramework.Canvas.DrawRect(_dimension,BoarderColor, BackColor, Boarder);
            foreach (Control c in children)
                if (c.visible)
                {
                    if (c.shaddow)
                    {
                        var sdim = c.dimension;
                        sdim.Offset(1, 1);
                        sdim.Intersect(dimension);
                        ConsoleFramework.Canvas.FillRect(sdim, ConsoleColor.DarkGray, ConsoleColor.Black, ConsoleFramework.chars[3]);
                    }
                    var CClip = dimension;
                    dimension.Location = Point.Subtract(dimension.Location,(Size)c.dimension.Location);
                    c.ReDraw(dimension);
                }
        }
    }

}
