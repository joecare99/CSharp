using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace Traingames.NetElements
{
    public class TextCanvas
    {
        internal Rectangle _dimension;

        public TextCanvas(Rectangle dimension)
        {
            _dimension = dimension;
        }

        public ConsoleColor BackgroundColor { get; internal set; }
        public Rectangle ClipRect { get =>_dimension; }

        public void FillRect(Rectangle dimension,ConsoleColor color,Char c)
        {
            Console.BackgroundColor = color;
            if (_dimension.Contains(dimension.Location))
            {
                for (int i = dimension.Y; i < dimension.Bottom; i++)
                {
                    Console.SetCursorPosition(dimension.X + _dimension.X, i + _dimension.Y);
                    for (int j = dimension.X; j < dimension.Right; j++)
                        Console.Write(c);
                }
                                                                                                                                                                                                                                                                                      }
        }
    }
    public static class ConsoleFramework
    {
        public static readonly char[] chars = { '█', '▓', '▒', '░',' ' };
        public static readonly char[] singleBoarder = { '─', '│', '┌', '┐', '└', '┘', '├', '┤', '┬', '┴', '┼' };
        public static readonly char[] doubleBoarder = { '═', '║', '╔', '╗', '╚', '╝', '╠', '╣', '╦', '╩', '╬' };
        public static readonly char[] simpleBoarder = { '-', '|', '+', '+', '+', '+', '+', '+', '+', '+', '+' };

        public static Point MousePos { get; private set; }

        public static bool MouseButtonLeft => System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left;
        public static bool MouseButtonRight => System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Right;
        public static bool MouseButtonMiddle => System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Middle;

        public static TextCanvas Canvas = new TextCanvas(new Rectangle(0, 0, Console.BufferWidth, 50));
        static ConsoleFramework()
        {
            IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);
            GetConsoleMode(inHandle, out ConsoleMode mode);
            mode &= ~ConsoleMode.ENABLE_QUICK_EDIT_MODE; //disable
            mode |= ConsoleMode.ENABLE_WINDOW_INPUT; //enable (if you want)
            mode |= ConsoleMode.ENABLE_MOUSE_INPUT; //enable
            SetConsoleMode(inHandle, mode);
            if (!Run)
            {
                Run = true;
                IntPtr handleIn = GetStdHandle(STD_INPUT_HANDLE);
                new Thread(() =>
                {
                    while (true)
                    {
                        uint numRead = 0;
                        INPUT_RECORD[] record = new INPUT_RECORD[1];
                        record[0] = new INPUT_RECORD();
                        ReadConsoleInput(handleIn, record, 1, ref numRead);
                        if (Run)
                            switch (record[0].usEventType)
                            {
                                case EventType.MOUSE_EVENT:
                                    MouseEvent?.Invoke(null,record[0].MouseEvent);
                                    MousePos = record[0].MouseEvent.dwMousePosition.AsPoint;
                                    break;
                                case EventType.KEY_EVENT:
                                    KeyEvent?.Invoke(null, record[0].KeyEvent);
                                    break;
                                case EventType.WINDOW_BUFFER_SIZE_EVENT:
                                    WindowBufferSizeEvent?.Invoke(null, record[0].WindowBufferSizeEvent);
                                    Canvas._dimension.Width = Console.BufferWidth;
                                    break;
                            }
                        else
                        {
                            uint numWritten = 0;
                            WriteConsoleInput(handleIn, record, 1, ref numWritten);
                            return;
                        }
                    }
                }).Start();
            }
        }

        static public void SetPixel(int x, int y, ConsoleColor color)
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = color;
            Console.Write(" ");
            Console.BackgroundColor = ConsoleColor.Black;
        }

 
    }

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

    public class Control 
    {
        protected Rectangle _dimension;
        private bool _active;
        private bool _shaddow;
        private bool _visible = true;

        public Point position { get => _dimension.Location; 
            set 
            { 
                if (_dimension.Location == value) return;
                if (parent == null)
                {
                    ConsoleFramework.Canvas.FillRect(_dimension, ConsoleFramework.Canvas.BackgroundColor, ConsoleFramework.chars[4]);
                }
                else
                {
                    parent.ReDraw(_dimension);
                }
                _dimension.Location = value;
                if (_visible) 
                {
                    Draw();
                } 
                OnMove?.Invoke(this, null);
            }
        }

        private void ReDraw(Rectangle dimension)
        {

            if (_visible && dimension.IntersectsWith(_dimension))
                Draw();
        }

        public Size size
        {
            get => _dimension.Size; set
            {
                if (_dimension.Size == value) return;
                if (parent == null)
                {
                    ConsoleFramework.Canvas.FillRect(_dimension, ConsoleFramework.Canvas.BackgroundColor, ConsoleFramework.chars[4]);
                }
                else
                {
                    parent.ReDraw(_dimension);
                }
                _dimension.Size = value;
                if (_visible)
                {
                    Draw();
                }
                OnResize?.Invoke(this, null);
            }
        }
        public bool Over(Point M) => _dimension.Contains(M);

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

        private void Invalidate()
        {
            // Todo:
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
            Console.SetCursorPosition(_dimension.X, _dimension.Y);
            Console.BackgroundColor = BackColor;
            Console.Write($"[{Text}]");
            Console.BackgroundColor = ConsoleColor.Black;
        }

    }

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
    }


}
