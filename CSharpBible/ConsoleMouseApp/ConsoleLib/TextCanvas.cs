using System;
using System.Drawing;

namespace ConsoleLib
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

        public void DrawRect(Rectangle dimension, ConsoleColor color, char[] boarder)
        {
            if (dimension.Width==0 || dimension.Height==0)  return;
            if (dimension.Width == 1 ) 
            {
                for (int i = dimension.Y ; i < dimension.Bottom ; i++)
                {
                    OutTextXY(dimension.Left, i, boarder[1]);
                }
                return;
            }
            if ( dimension.Height == 1 )
            {
                for (int j = dimension.X ; j < dimension.Right ; j++)
                {
                    OutTextXY(j, dimension.Top, boarder[0]);
                }
                return;
            }
            Console.BackgroundColor = color;
            if (_dimension.Contains(dimension.Location))
                for (int i = dimension.Y + 1; i < dimension.Bottom - 1; i++)
                {
                    OutTextXY(dimension.Left, i, boarder[1]);
                    OutTextXY(dimension.Right-1, i, boarder[1]);
                }
            for (int j = dimension.X + 1; j < dimension.Right - 1; j++)
            {
                OutTextXY(j, dimension.Top, boarder[0]);
                OutTextXY(j, dimension.Bottom-1, boarder[0]);
            }
            OutTextXY(dimension.Location, boarder[2]);
            OutTextXY(dimension.Right-1,dimension.Top, boarder[3]);
            OutTextXY(dimension.Left, dimension.Bottom-1, boarder[4]);
            OutTextXY(dimension.Right-1,dimension.Bottom-1, boarder[5]);
        }

        public void OutTextXY(Point place, string s)
        {
            OutTextXY(place.X, place.Y, s);
        }
        public void OutTextXY(Point place, char c)
        {
            OutTextXY(place.X, place.Y, c);
        }

        public void OutTextXY(int x,int y, string s)
        {
            Console.SetCursorPosition(x + _dimension.X, y + _dimension.Y);
            Console.Write(s);
        }
        public void OutTextXY(int x, int y, char c)
        {
            Console.SetCursorPosition(x + _dimension.X, y + _dimension.Y);
            Console.Write(c);
        }
    }

}
