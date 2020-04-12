using System;
using System.Drawing;

namespace ConsoleLib.CommonControls
{
    public class Pixel : Control
    {
        public Pixel()
        {
            size = new Size(1, 1);
        }
        public void Set(int X, int Y, string text="")
        {
            if (text != "")
            {
                Text = text; 
            }
            position = new Point(X,Y);
        }

        public void Set(Point position, string text= "")
        {
            Set(position.X, position.Y, text);
        }
        public override void Draw()
        {
            if (parent != null && !parent.dimension.Contains(Point.Add(position,(Size)parent.position))) return;
            Console.SetCursorPosition(realDim.X, realDim.Y);
            Console.BackgroundColor = BackColor;
            Console.Write($"{Text}");
            Console.BackgroundColor = ConsoleColor.Black;
        }

    }

}
