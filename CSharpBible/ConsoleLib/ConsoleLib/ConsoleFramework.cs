using ConsoleLib;
using System;
using System.Drawing;

namespace ConsoleLib
{
    public static class ConsoleFramework
    {
        public static readonly char[] chars = { '█', '▓', '▒', '░',' ' };
        public static readonly char[] singleBoarder = { '─', '│', '┌', '┐', '└', '┘', '├', '┤', '┬', '┴', '┼' };
        public static readonly char[] doubleBoarder = { '═', '║', '╔', '╗', '╚', '╝', '╠', '╣', '╦', '╩', '╬' };
        public static readonly char[] simpleBoarder = { '-', '|', ',', ',', '\'', '\'', '+', '+', '+', '+', '+' };

        public static Point MousePos { get; private set; }

        public static bool MouseButtonLeft => System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left;
        public static bool MouseButtonRight => System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Right;
        public static bool MouseButtonMiddle => System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Middle;

        public static TextCanvas Canvas = new TextCanvas(new Rectangle(0, 0, Console.BufferWidth, 50));
        static ConsoleFramework()
        {
            ExtendedConsole.MouseEvent += OnMouseEvent;
            ExtendedConsole.WindowBufferSizeEvent += OnWindowSizeEvent;
        }

        private static void OnWindowSizeEvent(object sender, ExtendedConsole.WINDOW_BUFFER_SIZE_RECORD e)
        {
            (Canvas._dimension.Width, Canvas._dimension.Height) = (e.dwSize.X, e.dwSize.Y);
        }

        private static void OnMouseEvent(object sender, ExtendedConsole.MOUSE_EVENT_RECORD e)
        {
            MousePos = e.dwMousePosition.AsPoint;
        }

        static public void SetPixel(int x, int y, ConsoleColor color)
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = color;
            Console.Write(" ");
            Console.BackgroundColor = ConsoleColor.Black;
        }

 
    }

}
