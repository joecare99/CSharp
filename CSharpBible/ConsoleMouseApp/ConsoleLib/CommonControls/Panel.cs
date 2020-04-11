using System;
using System.Drawing;

namespace ConsoleLib.CommonControls
{
    public class Panel : Control
    {
        public char[] Boarder;
        
        public override void Draw()
        {
            ConsoleFramework.Canvas.FillRect(_dimension, BackColor, ConsoleFramework.chars[3]);
            if (Boarder != null && Boarder.Length > 5)
                ConsoleFramework.Canvas.DrawRect(_dimension, BackColor, Boarder);
            foreach( Control c in children)
            {
                if (c.shaddow)
                    ConsoleFramework.Canvas.FillRect(_dimension, ConsoleColor.Black, ConsoleFramework.chars[3]);
                c.Draw();

            }
        }

        public override void ReDraw(Rectangle dimension)
        {
            if (dimension.Width == 0 || dimension.Height == 0) return;
            Rectangle innerRect = _dimension;
            innerRect.Inflate(-1, -1);
            dimension.Intersect(innerRect);
            ConsoleFramework.Canvas.FillRect(dimension, BackColor, ConsoleFramework.chars[3]);
            // ToDo: Boarder
//            if (Boarder != null && Boarder.Length > 5)
  //              ConsoleFramework.Canvas.DrawRect(dimension, BackColor, Boarder);
            foreach (Control c in children) if (c.visible)
            {
                if (c.shaddow)
                    ConsoleFramework.Canvas.FillRect(_dimension, ConsoleColor.Black, ConsoleFramework.chars[3]);                 
                c.ReDraw(dimension);
            }
        }
    }

}
