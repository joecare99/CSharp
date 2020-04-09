using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Traingames.NetElements
{
    public class ConsoleFramework
    {
        public char[] chars = { '█', '▓', '▒', '░' };

        public Point MousePos =>
            new Point((System.Windows.Forms.Control.MousePosition.X / (Console.LargestWindowWidth / 24)) - 100, System.Windows.Forms.Control.MousePosition.Y / (Console.LargestWindowHeight / 7));
        
       public ConsoleFramework()
        {
            IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);
            GetConsoleMode(inHandle, out ConsoleMode mode);
            mode &= ~ConsoleMode.ENABLE_QUICK_EDIT_MODE; //disable
            mode |= ConsoleMode.ENABLE_WINDOW_INPUT; //enable (if you want)
            mode |= ConsoleMode.ENABLE_MOUSE_INPUT; //enable
            SetConsoleMode(inHandle, mode);
        }

        public void SetPixel(int x, int y, ConsoleColor color)
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = color;
            Console.Write(" ");
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public const uint STD_INPUT_HANDLE = unchecked((uint)-10),
            STD_OUTPUT_HANDLE = unchecked((uint)-11),
            STD_ERROR_HANDLE = unchecked((uint)-12);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(uint nStdHandle);

        [Flags]
        public enum ConsoleMode :uint
        {
            ENABLE_MOUSE_INPUT = 0x0010,
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            ENABLE_EXTENDED_FLAGS = 0x0080,
            ENABLE_ECHO_INPUT = 0x0004,
            ENABLE_WINDOW_INPUT = 0x0008
        }; //more

        [DllImport("kernel32.dll")]
        public static extern bool GetConsoleMode(IntPtr hConsoleInput, out ConsoleMode lpMode);

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleMode(IntPtr hConsoleInput, ConsoleMode dwMode);

    }

    public class Pixel : GUI
    {
        public void Set(int X, int Y, string text)
        {
            ConsoleColor backColor = ConsoleColor.Black;
            BackColor = backColor;
            int yyyyyy = (int)Math.Floor(Y / 1.5f);
            Text = text;
            y = Y;
            x = X;
        }
    }

    public class GUI
    {
        public int x, y;
        public static GUI[,] GraphicalUserInterfaces = new GUI[1000, 1000];
        public ConsoleColor BackColor;
        public string Text;

        public void Draw()
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = BackColor;
            Console.Write($"[{Text}]");
            Console.BackgroundColor = ConsoleColor.Black;
            Point M = ConsoleTools.NET.Program.MousePos;
        }

        static GUI Last;

        public static void Add(GUI gui)
        {
            GraphicalUserInterfaces[gui.x, gui.y] = gui;
        }

        public static void CalculateOnStart()
        {
            for (int x = 0; x < 1000; x++)
            {
                for (int y = 0; y < 1000; y++)
                {
                    if (GraphicalUserInterfaces[x, y] != null)
                    {

                        if (Last != null && y < Last.y)
                        {
                            GraphicalUserInterfaces[x, y].x = Last.x - GraphicalUserInterfaces[x, y].x;
                            GraphicalUserInterfaces[x, y].y = Last.y - GraphicalUserInterfaces[x, y].y;
                        }
                        GraphicalUserInterfaces[x, y].Draw();
                        GraphicalUserInterfaces[x, y].x = x;
                        GraphicalUserInterfaces[x, y].y = y;
                        Last = GraphicalUserInterfaces[x, y];
                    }

                }
            }
        }

    }

    public class Button : GUI
    {

        public bool Over(Point M)
        {
            int yy = ((y * 2) - y / 3) + 2;

            int xx = (x / (Console.LargestWindowWidth / 24)) + Text.Length;

            if (M.X >= xx && M.X <= (xx + Text.Length + 1) && M.Y >= yy && M.Y <= yy + 2)
                Console.BackgroundColor = ConsoleColor.DarkBlue;

            return M.X >= xx && M.X <= (xx + Text.Length + 1) && M.Y >= yy && M.Y <= yy + 2;
        }

        public bool Pressed(Point M)
        {
            int yy = ((y * 2) - y / 3) + 1;

            int xx = (x / (Console.LargestWindowWidth / 24));

            return M.X >= xx && M.X <= (xx + Text.Length * 1.5f) && M.Y >= yy && M.Y <= yy + 2 && System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left;
        }

        public void CalculateClick(Point M)
        {
            if (Pressed(M))
            {
                Console.Clear();
                Draw();
            }
        }

        public void Set(int X, int Y, string text, ConsoleColor backColor)
        {
            BackColor = backColor;
            int yyyyyy = (int)Math.Floor(Y / 1.5f);
            Text = text;
            y = Y;
            x = X;

            int xx = (x / (Console.LargestWindowWidth / 24)) + Text.Length;
        }
    }
}
