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

        public void Set(Point mousePos, string text= "")
        {
            Set(mousePos.X, mousePos.Y, text);
        }
        public override void Draw()
        {
            Console.SetCursorPosition(_dimension.X, _dimension.Y);
            Console.BackgroundColor = BackColor;
            Console.Write($"{Text}");
            Console.BackgroundColor = ConsoleColor.Black;
        }

    }

}
