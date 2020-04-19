using System;

namespace ConsoleLib.CommonControls
{
    public class Label : Control
    {
        public bool ParentBackground { get; set; }
        public override void Draw()
        {
            // Draw Background
            Console.ForegroundColor = ForeColor;
            if (ParentBackground && parent != null)
            {
                Console.BackgroundColor = parent.BackColor;
            }
            else
            {
                Console.BackgroundColor = BackColor;
            }
            ConsoleFramework.Canvas.OutTextXY(realDim.Location,(" "+Text+"           ").Substring(0, Math.Min(size.Width,Text.Length+14)));
            Console.BackgroundColor = ConsoleColor.Black;
        }

    }
}
