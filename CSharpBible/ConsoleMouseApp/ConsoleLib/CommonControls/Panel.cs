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
            ConsoleFramework.Canvas.FillRect(realDim,ForeColor, BackColor, ConsoleFramework.chars[3]);
            if (Boarder != null && Boarder.Length > 5)
                ConsoleFramework.Canvas.DrawRect(realDim, BoarderColor, BackColor, Boarder);
            foreach( Control c in children) if (c.visible)
                {
                if (c.shaddow)
                {
                    var sdim = c.dimension;
                    sdim.Offset(1, 1);
                    sdim.Offset(position);
                    ConsoleFramework.Canvas.FillRect(realDimOf(sdim), ConsoleColor.DarkGray, ConsoleColor.Black, ConsoleFramework.chars[4]);
                }
                c.Draw();

            }
        }

        public override void ReDraw(Rectangle dimension)
        {
            if (dimension.IsEmpty) return;
            Rectangle innerRect = _dimension;
            innerRect.Inflate(-1, -1);
            var icl = dimension;
            icl.Intersect(innerRect);
            ConsoleFramework.Canvas.FillRect(realDimOf(icl),ForeColor, BackColor, ConsoleFramework.chars[3]);
            // ToDo: Boarder
            if (Boarder != null && Boarder.Length > 5 && _dimension.IntersectsWith(dimension) &&
                !( innerRect.Contains(dimension.Location) && innerRect.Contains(Point.Subtract(Point.Add(dimension.Location,dimension.Size),new Size(1,1)) )
                ))
                            ConsoleFramework.Canvas.DrawRect(realDim,BoarderColor, BackColor, Boarder);
            foreach (Control c in children)
                if (c.visible)
                {
                    if (c.shaddow)
                    {
                        var sdim = c.dimension;
                        sdim.Offset(1, 1);
                        sdim.Offset(position);
                        sdim.Intersect(dimension);
                        ConsoleFramework.Canvas.FillRect(realDimOf(sdim), ConsoleColor.DarkGray, ConsoleColor.Black, ConsoleFramework.chars[4]);
                    }
                    var CClip = dimension;
                    CClip.Location = Point.Subtract(dimension.Location,(Size)_dimension.Location);
                    c.ReDraw(CClip);
                }
        }
    }

}
