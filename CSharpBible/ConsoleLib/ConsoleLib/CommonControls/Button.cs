using System;
using System.Drawing;

namespace ConsoleLib.CommonControls
{
    public class Button : Control
    {
        private bool _WasPressed;


        public bool Pressed(Point M) => Over(M) && !_WasPressed & (_WasPressed=ConsoleFramework.MouseButtonLeft) ;

        public void CalculateClick(Point M)
        {
           
        }

        public void Set(int X, int Y, string text, ConsoleColor backColor)
        {
            BackColor = backColor;
            Text = text;
            size = new Size(text.Length + 2, 1);
            position = new Point(X, Y);
        }

        public override void MouseEnter(Point M)
        {
            base.MouseEnter(M);
            BackColor = ConsoleColor.Green;
            Invalidate();
        }
        public override void MouseLeave(Point M)
        {
            base.MouseLeave(M);
            BackColor = ConsoleColor.Gray;
            Invalidate();
        }

        public override void SetText(string value)
        {
            base.SetText(value);
            size = new Size(value.Length+2, 1);
        }
    }

}
