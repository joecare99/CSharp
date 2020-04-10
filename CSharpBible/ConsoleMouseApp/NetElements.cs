using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Traingames.NetElements
{
    public static class ConsoleFramework
    {
        public static readonly char[] chars = { '█', '▓', '▒', '░',' ' };
        public static readonly char[] singleBoarder = { '─', '│', '┌', '┐', '└', '┘', '├', '┤', '┬', '┴', '┼' };
        public static readonly char[] doubleBoarder = { '═', '║', '╔', '╗', '╚', '╝', '╠', '╣', '╦', '╩', '╬' };
        public static readonly char[] simpleBoarder = { '-', '|', '+', '+', '+', '+', '+', '+', '+', '+', '+' };

        public static Point MousePos =>
            new Point((System.Windows.Forms.Control.MousePosition.X / (Console.LargestWindowWidth / 24)) - 100, System.Windows.Forms.Control.MousePosition.Y / (Console.LargestWindowHeight / 7));

        public static bool MouseButtonLeft => System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left;
        public static bool MouseButtonRight => System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Right;
        public static bool MouseButtonMiddle => System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Middle;

        static ConsoleFramework()
        {
            IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);
            GetConsoleMode(inHandle, out ConsoleMode mode);
            mode &= ~ConsoleMode.ENABLE_QUICK_EDIT_MODE; //disable
            mode |= ConsoleMode.ENABLE_WINDOW_INPUT; //enable (if you want)
            mode |= ConsoleMode.ENABLE_MOUSE_INPUT; //enable
            SetConsoleMode(inHandle, mode);
        }

        static public void SetPixel(int x, int y, ConsoleColor color)
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

    public class Pixel : Control
    {
        public void Set(int X, int Y, string text)
        {
            ConsoleColor backColor = ConsoleColor.Black;
            BackColor = backColor;
            Text = text;
            position = new Point(X,Y);
        }
    }

    public class Control 
    {
        private Point _position;
        private Size _size;
        private bool _active;
        private bool _shaddow;
        private bool _visible;

        public Point position { get => _position; 
            set 
            { 
                if (_position == value) return;
                _position = value;
                OnMove?.Invoke(this, null);
            }
        }
        public Size dimension
        {
            get => _size; set
            {
                if (_size == value) return;
                _size = value;
                OnResize?.Invoke(this, null);
            }
        }

        public bool active
        {
            get => _active; set
            {
                if (_active == value) return;
                _active = value;
                OnChange?.Invoke(this, null);
                Invalidate();
            }
        }

        public event EventHandler OnMove;
        public event EventHandler OnResize;
        public event EventHandler OnChange;

        public ConsoleColor BackColor;
        public string Text;

        public List<Control> children = new List<Control>();
        public Control parent;


        public Control Add(Control control)
        {
            children.Add(control);
            control.parent = this;
            return control;
        }
        public virtual void Draw()
        {
            Console.SetCursorPosition(_position.X, _position.Y);
            Console.BackgroundColor = BackColor;
            Console.Write($"[{Text}]");
            Console.BackgroundColor = ConsoleColor.Black;
            Point M = ConsoleTools.NET.Program.MousePos;
        }

    }

    public class Button : Control
    {
        private bool _WasPressed;

        public bool Over(Point M) => new Rectangle(position, dimension).Contains(M);

        public bool Pressed(Point M) => Over(M) && !_WasPressed & (_WasPressed=ConsoleFramework.MouseButtonLeft) ;

        public void CalculateClick(Point M)
        {
           
        }

        public void Set(int X, int Y, string text, ConsoleColor backColor)
        {
            BackColor = backColor;
            Text = text;
            position = new Point(X, Y);
        }
    }


}
